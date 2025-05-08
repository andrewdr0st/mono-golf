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
        public Game Game { get; private set; }
        private Space space;
        protected Ball activeBall;
        private const float launchStrength = 0.6f;
        private const float minStrength = 0.75f;
        private const float maxStrength = 8f;
        private bool followingBall = false;
        private bool dragging = false;
        private DrawableObject[] aimIndicators;
        public Camera Camera { get; protected set; }

        protected Scene(Game game)
        {
            Game = game;
            space = new Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -6f, 0);
            Camera = new Camera(0, MathHelper.Pi * 0.25f, Vector3.Zero, MathHelper.PiOver4);
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
                        followingBall = false;
                    }
                }
                else
                {
                    Camera.Pan(InputManager.MoveVector);
                }
                 Camera.Zoom(InputManager.GetScrollAmount());
            }
            Camera.UpdateViewMatrix();
            space.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
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
                followingBall = true;
                foreach (DrawableObject a in aimIndicators)
                {
                    a.Visible = false;
                }
            }
        }

    }

    public class Hole1 : Scene
    {
        public Hole1(Game game) : base(game)
        {
            Ball ball = new Ball(this, new Vector3(-7f, 2f, 0));
            AddGameComponent(ball);
            activeBall = ball;
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
        }
    }
}
