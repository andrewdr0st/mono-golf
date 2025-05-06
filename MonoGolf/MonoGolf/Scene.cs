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
        private Game game;
        private Space space;
        protected Ball activeBall;
        private const float launchStrength = 0.4f;
        private const float minStrength = 0.75f;
        private const float maxStrength = 6f;
        private bool dragging = false;
        private DrawableObject[] aimIndicators;
        public Camera Camera { get; protected set; }

        protected Scene(Game game)
        {
            this.game = game;
            space = new Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -5f, 0);
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
                new DrawableObject(game, this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new DrawableObject(game, this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f)),
                new DrawableObject(game, this, Minigolf.MeshList[2], new IndicatorMaterial(), Vector3.Zero, new Vector3(0.2f, 0.2f, 0.2f))
            ];
            foreach (DrawableObject a in aimIndicators)
            {
                a.Visible = false;
                AddGameComponent(a);
            }
        }

        protected void AddGameComponent(DrawableGameComponent c)
        {
            game.Components.Add(c);
        }

        protected void AddGameComponent(DrawablePhysicsObject o)
        {
            game.Components.Add(o);
            space.Add(o.Entity);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (dragging)
            {
                Vector3 mousePos = DragRaycast();
                Vector3 launchVector = activeBall.Pos - mousePos;
                float length = launchVector.Length();
                launchVector.Normalize();
                float strength = Math.Clamp((length - minStrength) * launchStrength , 0f, maxStrength);
                for (int i = 0; i < 3; i++)
                {
                    DrawableObject a = aimIndicators[i];
                    a.Visible = strength > 0f;
                    a.Pos = activeBall.Pos + launchVector * (minStrength + strength) * ((i + 1) * 0.33f);
                }
                if (!InputManager.LeftPressed())
                {
                    activeBall.Entity.ApplyImpulse(MathConverter.Convert(activeBall.Pos), MathConverter.Convert(launchVector * strength));
                    dragging = false;
                    foreach (DrawableObject a in aimIndicators)
                    {
                        a.Visible = false;
                    }
                }
            }
            else
            {
                if (InputManager.LeftClicked())
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
                else if (InputManager.MiddlePressed())
                {
                    Camera.Pan(InputManager.GetMoveAmount());
                }
                Camera.Pan(InputManager.MoveVector);
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
            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(near, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(far, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

    }

    public class Hole1 : Scene
    {
        public Hole1(Game game) : base(game)
        {
            Entity e = new Box(new BEPUutilities.Vector3(0, 0, 0), 20, 2, 20);
            e.Material.Bounciness = 0.4f;
            DrawablePhysicsObject platform = new DrawablePhysicsObject(game, this, Minigolf.MeshList[0], new FloorMaterial(), e, Vector3.Zero, new Vector3(10, 1, 10));
            AddGameComponent(platform);
            Entity b = new Sphere(new BEPUutilities.Vector3(0, 10, 0), 0.75f, 0.1f);
            b.Material.Bounciness = 0.9f;
            Ball ball = new Ball(game, this, Minigolf.MeshList[1], new BallMaterial(), b, new Vector3(0, 10, 0), 0.75f);
            AddGameComponent(ball);
            activeBall = ball;
        }
    }
}
