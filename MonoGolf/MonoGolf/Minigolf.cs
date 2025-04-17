using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGolf
{
    public class Minigolf : Game
    {
        private GraphicsDeviceManager _graphics;

        //Matrices for 3D perspective
        private Matrix worldMatrix, viewMatrix, projectionMatrix;

        // Vertex data for rendering
        private VertexPositionColor[] triangleVertices;

        // A Vertex format structure that contains position, normal data, and one set of texture coordinates
        private BasicEffect basicEffect;

        private Model cube;

        public Minigolf()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 300.0f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            basicEffect = new BasicEffect(_graphics.GraphicsDevice);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            // primitive color
            basicEffect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            basicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            basicEffect.SpecularPower = 5.0f;
            basicEffect.Alpha = 1.0f;
            // The following MUST be enabled if you want to color your vertices
            basicEffect.VertexColorEnabled = true;

            // Use the built in 3 lighting mode provided with BasicEffect            
            //basicEffect.EnableDefaultLighting();

            triangleVertices = new VertexPositionColor[3];

            triangleVertices[0].Position = new Vector3(0f, 0f, 0f);
            triangleVertices[0].Color = Color.Violet;
            triangleVertices[1].Position = new Vector3(10f, 10f, 0f);
            triangleVertices[1].Color = Color.GhostWhite;
            triangleVertices[2].Position = new Vector3(10f, 0f, -5f);
            triangleVertices[2].Color = Color.Indigo;


            cube = Content.Load<Model>("testcube");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Crimson);

            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState1;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    triangleVertices,
                    0,
                    1,
                    VertexPositionColor.VertexDeclaration
                );
            }

            base.Draw(gameTime);
        }
    }
}
