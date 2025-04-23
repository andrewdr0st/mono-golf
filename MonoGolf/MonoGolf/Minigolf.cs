using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public class Minigolf : Game
    {
        private GraphicsDeviceManager _graphics;

        private Matrix worldMatrix;

        private ModelMesh cubeMesh;

        private Camera camera;

        public Minigolf()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            worldMatrix = Matrix.CreateScale(new Vector3(10, 1, 10));

            camera = new Camera(0, MathHelper.PiOver4, Vector3.Zero, GraphicsDevice.Viewport.AspectRatio);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            cubeMesh = Content.Load<Model>("testcube").Meshes[0];

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();

            if (InputManager.RightPressed())
            {
                camera.Rotate(InputManager.GetMoveAmount());
            }
            else if (InputManager.LeftPressed())
            {
                camera.Pan(InputManager.GetMoveAmount());
            }
            camera.Zoom(InputManager.GetScrollAmount());
            camera.UpdateViewMatrix();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Crimson);

            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState1;

            foreach (BasicEffect effect in cubeMesh.Effects)
            {
                effect.EnableDefaultLighting();
                effect.View = camera.ViewMatrix;
                effect.Projection = camera.Projection;
                effect.World = worldMatrix;
            }
            cubeMesh.Draw();

            base.Draw(gameTime);
        }
    }
}
