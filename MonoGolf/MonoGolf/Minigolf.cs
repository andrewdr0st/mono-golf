using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public class Minigolf : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        public Scene Scene { get; set; }
        public bool TwoPlayer { get; set; }
        public static List<ModelMesh> MeshList { get; private set; }
        private int currentScene = -1;
        private float transitionTimer = 2f;
        private bool transitioning = false;
        public static List<SoundEffect> SoundEffects { get; private set; }

        public Minigolf()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MeshList = [];
            SoundEffects = [];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            MeshList.Add(Content.Load<Model>("testcube").Meshes[0]);
            MeshList.Add(Content.Load<Model>("ball").Meshes[0]);
            MeshList.Add(Content.Load<Model>("diamond").Meshes[0]);
            MeshList.Add(Content.Load<Model>("slope").Meshes[0]);
            MeshList.Add(Content.Load<Model>("hole").Meshes[0]);
            MeshList.Add(Content.Load<Model>("slope2").Meshes[0]);

            font = Content.Load<SpriteFont>("font");

            SoundEffects.Add(Content.Load<SoundEffect>("Launch"));
            SoundEffects.Add(Content.Load<SoundEffect>("wall-hit"));
            SoundEffects.Add(Content.Load<SoundEffect>("yay"));

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
            if (currentScene >= 0)
            {
                Scene.Update(gameTime);
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1) || Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                {
                    TwoPlayer = false;
                    currentScene = 0;
                    Scene = new Hole1(this);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D2) || Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                {
                    TwoPlayer = true;
                    currentScene = 0;
                    Scene = new Hole1(this);
                }
                    
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = rasterizerState1;

            base.Draw(gameTime);

            spriteBatch.Begin();
            if (currentScene == -1)
            {
                spriteBatch.DrawString(font, "COSMO GOLF", new Vector2(425, 120), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Press 1 for single player", new Vector2(432, 300), Color.White);
                spriteBatch.DrawString(font, "Press 2 for two player", new Vector2(450, 400), Color.White);
            }
            else
            {
                if (TwoPlayer)
                {
                    spriteBatch.DrawString(font, "P1 " + Scene.StrokeCount, new Vector2(10, 10), Color.Red);
                    spriteBatch.DrawString(font, "P2 " + Scene.StrokeCount2, new Vector2(10, 50), Color.LimeGreen);
                }
                else
                {
                    spriteBatch.DrawString(font, "Stroke " + Scene.StrokeCount, new Vector2(10, 10), Color.White);
                }
            }
            spriteBatch.End();
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
