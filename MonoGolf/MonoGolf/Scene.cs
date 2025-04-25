using System.Collections.Generic;
using Microsoft.Xna.Framework;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using System;

namespace MonoGolf
{
    public class Scene
    {
        private Game game;
        private Space space;

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
            if (InputManager.RightPressed())
            {
                Camera.Rotate(InputManager.GetMoveAmount());
            }
            else if (InputManager.LeftPressed())
            {
                Camera.Pan(InputManager.GetMoveAmount());
            }
            Camera.Zoom(InputManager.GetScrollAmount());
            Camera.UpdateViewMatrix();
            space.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
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
            DrawablePhysicsObject ball = new DrawablePhysicsObject(game, this, Minigolf.MeshList[1], new BallMaterial(), b, new Vector3(0, 10, 0), new Vector3(0.75f, 0.75f, 0.75f));
            AddGameComponent(ball);
        }
    }
}
