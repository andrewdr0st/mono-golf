using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using System.Diagnostics;
using ConversionHelper;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Paths.PathFollowing;
using System;

namespace MonoGolf
{
    public class Scene
    {
        public Minigolf Game { get; private set; }
        protected Space space;
        protected Ball activeBall;
        protected Ball otherBall;
        protected Entity winPlane;
        protected Scene nextScene;
        private const float launchStrength = 0.6f;
        private const float minStrength = 0.75f;
        private const float maxStrength = 8f;
        private bool followingBall = false;
        private bool dragging = false;
        private DrawableObject[] aimIndicators;
        public Camera Camera { get; protected set; }
        public int StrokeCount { get; protected set; }
        public int StrokeCount2 { get; protected set; }
        private int strokeInc = 1;
        private int strokeInc2 = 1;
        private float jingleCooldown = 0;
        public bool TwoPlayer { get; set; }
        private int pTurn = 1;
        private bool p2Spawned = false;
        protected Vector3 spawnPoint;

        protected Scene(Minigolf game, Vector3 spawnPoint)
        {
            Game = game;
            this.spawnPoint = spawnPoint;
            space = new Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -7f, 0);
            Camera = new Camera(0, MathHelper.Pi * 0.25f, Vector3.Zero, MathHelper.PiOver4);
            StrokeCount = 1;
            StrokeCount2 = 1;
            TwoPlayer = game.TwoPlayer;
            Entity deathPlane = new Triangle(
                new BEPUutilities.Vector3(-10000f, -10f, -10000f),
                new BEPUutilities.Vector3(10000f, -10f, -10000f),
                new BEPUutilities.Vector3(0f, -10f, 10000f)
            );
            deathPlane.CollisionInformation.Events.InitialCollisionDetected += (EntityCollidable sender, Collidable other, CollidablePairHandler pair) =>
            {
                if (other is EntityCollidable otherEntityCollidable)
                {
                    if (otherEntityCollidable.Entity.Tag is Ball ball)
                    {
                        ball.ResetPosition();
                    }
                }
            };
            space.Add(deathPlane);
            aimIndicators = [
                new DrawableObject(this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new DrawableObject(this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new DrawableObject(this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f))
            ];
            foreach (DrawableObject a in aimIndicators)
            {
                a.Visible = false;
                AddGameComponent(a);
            }
            Ball ball = new Ball(this, spawnPoint, TwoPlayer ? 1 : 0);
            AddGameComponent(ball);
            activeBall = ball;
        }

        protected void AddGameComponent(DrawableGameComponent c)
        {
            Game.Components.Add(c);
        }

        protected void AddGameComponent(DrawablePhysicsObject o)
        {
            Game.Components.Add(o);
            space.Add(o.Entity);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (dragging)
            {
                LaunchBall();
            }
            else
            {
                if (InputManager.LeftClicked() && !followingBall)
                {
                    if (BallRaycast())
                    {
                        dragging = true;
                    }
                }
                else if (InputManager.RightPressed())
                {
                    Camera.Rotate(InputManager.GetMoveAmount());
                }
                if (followingBall)
                {
                    Camera.SetTarget(activeBall.Pos);
                    if (!activeBall.BallActive)
                    {
                        if (pTurn == 1)
                        {
                            StrokeCount += strokeInc;
                        }
                        else
                        {
                            StrokeCount2 += strokeInc2;
                        }
                        followingBall = false;
                        SwapBalls();
                    }
                }
                else
                {
                    Camera.Pan(InputManager.MoveVector);
                }
                 Camera.Zoom(InputManager.GetScrollAmount());
            }
            Camera.UpdateViewMatrix();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            jingleCooldown -= deltaTime;
            space.Update(deltaTime);
        }

        private bool BallRaycast()
        { 
            float? distance = MouseRay().Intersects(activeBall.BoundingSphere);
            return distance.HasValue;
        }

        private Vector3 DragRaycast()
        {
            Plane p = new Plane(activeBall.Pos, Vector3.Up);
            Ray mouseRay = MouseRay();
            float? distance = mouseRay.Intersects(p);
            if (distance.HasValue)
            {
                return mouseRay.Position + mouseRay.Direction * distance.Value;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        private Ray MouseRay()
        {
            Vector3 near = new Vector3(InputManager.MouseCoords(), 0f);
            Vector3 far = new Vector3(InputManager.MouseCoords(), 1f);
            Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            Vector3 nearPoint = Game.GraphicsDevice.Viewport.Unproject(near, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 farPoint = Game.GraphicsDevice.Viewport.Unproject(far, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        private void LaunchBall()
        {
            Vector3 mousePos = DragRaycast();
            Vector3 launchVector = activeBall.Pos - mousePos;
            float length = launchVector.Length();
            launchVector.Normalize();
            float strength = Math.Clamp((length - minStrength) * launchStrength, 0f, maxStrength);
            for (int i = 0; i < 3; i++)
            {
                DrawableObject a = aimIndicators[i];
                a.Visible = strength > 0f;
                a.Pos = activeBall.Pos + launchVector * (minStrength + strength) * ((i + 1) * 0.33f);
            }
            if (!InputManager.LeftPressed())
            {
                activeBall.Entity.ApplyImpulse(MathConverter.Convert(activeBall.Pos), MathConverter.Convert(launchVector * strength));
                activeBall.BallActive = true;
                dragging = false;
                if (strength > 0f)
                {
                    followingBall = true;
                    Minigolf.SoundEffects[0].Play();
                }
                foreach (DrawableObject a in aimIndicators)
                {
                    a.Visible = false;
                }
            }
        }

        public void InHole(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            if (jingleCooldown < 0f) {
                Minigolf.SoundEffects[2].Play();
                jingleCooldown = 1f;
            }
            if (pTurn == 1)
            {
                strokeInc = 0;
            }
            else if (pTurn == 2)
            {
                strokeInc2 = 0;
            }
            if (SwapBalls())
            {
                space.Remove(otherBall.Entity);
                otherBall.Visible = false;
                followingBall = false;
                return;
            }
            Game.HoleFinished();
        }

        private bool SwapBalls()
        {
            if (!TwoPlayer)
            {
                return false;
            }
            if (pTurn == 1 && strokeInc2 == 0)
            {
                return false;
            }
            if (pTurn == 2 && strokeInc == 0)
            {
                return false;
            }
            if (!p2Spawned)
            {
                Ball p2 = new Ball(this, spawnPoint, 2);
                AddGameComponent(p2);
                otherBall = p2;
                p2Spawned = true;
            }
            (otherBall, activeBall) = (activeBall, otherBall);
            Camera.SetTarget(activeBall.Pos);
            pTurn = pTurn == 1 ? 2 : 1;
            return true;
        }
    }

    public class Hole1 : Scene
    {
        public Hole1(Minigolf game) : base(game, new Vector3(-7f, 2f, 0))
        {
            if (TwoPlayer)
            {
                activeBall.homePos = new BEPUutilities.Vector3(-10f, 2f, 0);
            }
            Camera = new Camera(MathHelper.Pi * -0.1f, MathHelper.Pi * 0.25f, activeBall.Pos, MathHelper.PiOver4);
            AddGameComponent(new Tee(this, new Vector3(-7f, 1.1f, 0), 0));
            AddGameComponent(new FloorBox(this, Vector3.Zero, new Vector3(12f, 1f, 8f), 0));
            AddGameComponent(new WallBox(this, new Vector3(6f, 0.5f, 9f), new Vector3(18f, 1.5f, 1f), 0));
            AddGameComponent(new WallBox(this, new Vector3(-13f, 1.25f, -1f), new Vector3(1f, 2.25f, 11f), 0));
            AddGameComponent(new WallBox(this, new Vector3(11f, 0.5f, -19f), new Vector3(1f, 1.5f, 11f), 0));
            AddGameComponent(new FloorSlope(this, new Vector3(-1f, 1f, -10f), new Vector3(11f, 2f, 2f), 180));
            AddGameComponent(new FloorBox(this, new Vector3(18f, 0f, -17f), new Vector3(6f, 1f, 25f), 0));
            AddGameComponent(new WallBox(this, new Vector3(25f, 1.5f, -17f), new Vector3(1f, 2.5f, 27f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(10f, 0f, -37f), new Vector3(2f, 1f, 5f), 0));
            AddGameComponent(new WallBox(this, new Vector3(11f, 0.5f, -43f), new Vector3(13f, 1.5f, 1f), 0));
            AddGameComponent(new WallBox(this, new Vector3(-3f, 0.5f, -37f), new Vector3(1f, 1.5f, 7f), 0));
            AddGameComponent(new WallBox(this, new Vector3(5f, 0.5f, -31f), new Vector3(7f, 1.5f, 1f), 0));
            AddGameComponent(new HoleBox(this, new Vector3(3f, 0f, -37f), 0));
            winPlane = new Triangle(
                new BEPUutilities.Vector3(5f, 0.3f, -39f),
                new BEPUutilities.Vector3(5f, 0.3f, -35f),
                new BEPUutilities.Vector3(1f, 0.3f, -37f)
            );
            winPlane.CollisionInformation.Events.InitialCollisionDetected += InHole;
            space.Add(winPlane);
        }
    }

    public class Hole2 : Scene
    {
        public Hole2(Minigolf game) : base(game, new Vector3(-17f, 7f, -11f))
        {
            if (TwoPlayer)
            {
                activeBall.homePos = new BEPUutilities.Vector3(-17f, 7f, -14f);
            }
            Camera = new Camera(MathHelper.Pi * -0.5f, MathHelper.Pi * 0.25f, activeBall.Pos, MathHelper.PiOver4);
            AddGameComponent(new Tee(this, new Vector3(-17f, 6.1f, -11f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(-7.5f, 5f, -11f), new Vector3(12.5f, 1f, 5f), 0));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, 4f, -4.5f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, 3f, -3f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, 2f, -1.5f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, 1f, 0f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, 0f, 1.5f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, -1f, 3f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorSlope(this, new Vector3(0f, -2f, 4.5f), new Vector3(5f, 2f, 1.5f), 180));
            AddGameComponent(new FloorBox(this, new Vector3(6f, -3f, 11f), new Vector3(11f, 1f, 5f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(12f, -3f, -5f), new Vector3(5f, 1f, 11f), 0));
            AddGameComponent(new HoleBox(this, new Vector3(12f, -3f, -21f), 0));
            AddGameComponent(new WallBox(this, new Vector3(0f, 7f, -11f), new Vector3(1.5f, 2f, 1.5f), 45));
            AddGameComponent(new WallBox(this, new Vector3(0f, -2f, 11f), new Vector3(1.5f, 2f, 1.5f), 45));
            AddGameComponent(new WallBox(this, new Vector3(-6f, 1f, 6f), new Vector3(1f, 5f, 12f), 0));
            AddGameComponent(new WallBox(this, new Vector3(6f, 1f, -5f), new Vector3(1f, 5f, 11f), 0));
            AddGameComponent(new WallBox(this, new Vector3(-7.5f, 2f, -17f), new Vector3(14.5f, 6f, 1f), 0));
            AddGameComponent(new WallBox(this, new Vector3(-21f, 2f, -10f), new Vector3(1f, 6f, 6f), 0));
            AddGameComponent(new WallBox(this, new Vector3(-13.5f, 1f, -5f), new Vector3(6.5f, 5f, 1f), 0));
            AddGameComponent(new WallBox(this, new Vector3(6f, -2f, 17f), new Vector3(11f, 2f, 1f), 0));
            AddGameComponent(new WallBox(this, new Vector3(18f, -2f, 6f), new Vector3(1f, 2f, 12f), 0));
            AddGameComponent(new WallBox(this, new Vector3(18f, -2f, -21f), new Vector3(1f, 2f, 5f), 0));
            AddGameComponent(new WallBox(this, new Vector3(6f, -2f, -22f), new Vector3(1f, 2f, 4f), 0));
            AddGameComponent(new WallBox(this, new Vector3(12f, -2f, -27f), new Vector3(7f, 2f, 1f), 0));
            winPlane = new Triangle(
                new BEPUutilities.Vector3(10f, -2.7f, -20f),
                new BEPUutilities.Vector3(14f, -2.7f, -20f),
                new BEPUutilities.Vector3(12f, -2.7f, -23f)
            );
            winPlane.CollisionInformation.Events.InitialCollisionDetected += InHole;
            space.Add(winPlane);
        }
    }

    class Hole3 : Scene
    {
        public Hole3(Minigolf game) : base(game, new Vector3(-17.5f, 7f, 7.5f)) {
            Camera = new Camera(MathHelper.Pi * -0.25f, MathHelper.Pi * 0.25f, activeBall.Pos, MathHelper.PiOver4);
            AddGameComponent(new Tee(this, new Vector3(-17.5f, 6.1f, 7.5f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(0f, 5f, 0f), new Vector3(20f, 1f, 10f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(5f, 9f, -6f), new Vector3(5f, 5f, 12f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(-4f, 9f, 2f), new Vector3(4f, 5f, 4f), 0));
            AddGameComponent(new FloorBox(this, new Vector3(15f, 9f, -11f), new Vector3(5f, 5f, 7f), 0));
            AddGameComponent(new Slope2(this, new Vector3(-12f, 7f, 2f), new Vector3(1f, 1f, 1f), 0));
        }
    }
}
