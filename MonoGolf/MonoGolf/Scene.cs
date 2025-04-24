using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGolf
{
    public class Scene
    {
        private Game game;

        public Camera Camera { get; protected set; }

        List<DrawableGameComponent> objects;
        
        protected Scene(Game game)
        {
            this.game = game;
            Camera = new Camera(0, MathHelper.Pi * 0.25f, Vector3.Zero, MathHelper.PiOver4);
            objects = new List<DrawableGameComponent>();
        }

        protected void AddGameComponent(DrawableGameComponent c)
        {
            game.Components.Add(c);
            objects.Add(c);
        }

        public virtual void Update()
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
        }
        
    }

    public class Hole1 : Scene
    {
        public Hole1(Game game) : base(game)
        {
            DrawableObject platform = new DrawableObject(game, this, Minigolf.MeshList[0], Vector3.Zero, new Vector3(10, 1, 10));
            AddGameComponent(platform);
        }
    }
}
