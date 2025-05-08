using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public class Minigolf : Game
    {
        private GraphicsDeviceManager _graphics;
        public Scene Scene { get; set; }
        public static List<ModelMesh> MeshList { get; private set; }
        private int currentScene = 0;
        private float transitionTimer = 2f;
        private bool transitioning = false;

        public Minigolf()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            MeshList = new List<ModelMesh>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            MeshList.Add(Content.Load<Model>("testcube").Meshes[0]);
            MeshList.Add(Content.Load<Model>("ball").Meshes[0]);
            MeshList.Add(Content.Load<Model>("diamond").Meshes[0]);
            MeshList.Add(Content.Load<Model>("slope").Meshes[0]);
            MeshList.Add(Content.Load<Model>("hole").Meshes[0]);

            Scene = new Hole1(this);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (transitioning)
            {
                transitionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (transitionTimer < 0)
                {
                    transitioning = false;
                    transitionTimer = 2f;
                    NextScene();
                }
            }

            InputManager.Update();

            Scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            //rasterizerState1.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rasterizerState1;

            base.Draw(gameTime);
        }

        public void NextScene()
        {
            Components.Clear();
            currentScene = (currentScene + 1) % 2;
            if (currentScene == 0)
            {
                Scene = new Hole1(this);
            } else if (currentScene == 1)
            {
                Scene = new Hole2(this);
            }
        }

        public void HoleFinished()
        {
            transitioning = true;
        }
    }
}
