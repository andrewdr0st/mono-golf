using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using System.Diagnostics;

namespace MonoGolf
{
    public class Scene
    {
        private Game game;
        private Space space;
        protected Ball activeBall;
        public Camera Camera { get; protected set; }
        
        protected Scene(Game game)
        {
            this.game = game;
            space = new Space();
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -5f, 0);
            Camera = new Camera(0, MathHelper.Pi * 0.25f, Vector3.Zero, MathHelper.PiOver4);
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
            if (InputManager.LeftCLick())
            {
                if (BallRaycast())
                {
                    Debug.WriteLine("hit!");
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
            Camera.UpdateViewMatrix();
            space.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        private bool BallRaycast()
        {
            Vector3 near = new Vector3(InputManager.MouseCoords(), 0f);
            Vector3 far = new Vector3(InputManager.MouseCoords(), 1f);
            Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            Vector3 nearPoint = game.GraphicsDevice.Viewport.Unproject(near, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 farPoint = game.GraphicsDevice.Viewport.Unproject(far, Camera.Projection, Camera.ViewMatrix, world);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray ray = new Ray(nearPoint, direction);
            float? distance = ray.Intersects(activeBall.BoundingSphere);
            return distance.HasValue;
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
