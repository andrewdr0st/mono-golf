using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGolf
{
    public class DrawableObject : DrawableGameComponent
    {
        Scene scene;
        ModelMesh mesh;
        Matrix worldMatrix;

        public Vector3 Pos { get; set; }
        public Vector3 Scale { get; set; }

        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }



        public DrawableObject(Game game, Scene scene, ModelMesh mesh) : base(game)
        {
            this.scene = scene;
            this.mesh = mesh;
            DiffuseColor = new Vector3(0.25f, 0.25f, 0.25f);
            SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            Pos = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
            UpdateWorldMatrix();
        }

        public DrawableObject(Game game, Scene scene, ModelMesh mesh, Vector3 pos, Vector3 scale) : base(game)
        {
            this.scene = scene;
            this.mesh = mesh;
            DiffuseColor = new Vector3(0.25f, 0.25f, 0.25f);
            SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            Pos = pos;
            Scale = scale;
            UpdateWorldMatrix();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.EnableDefaultLighting();
                effect.DiffuseColor = DiffuseColor;
                effect.SpecularColor = SpecularColor;
                effect.View = scene.Camera.ViewMatrix;
                effect.Projection = scene.Camera.Projection;
                effect.World = worldMatrix;
            }
            mesh.Draw();

            base.Draw(gameTime);
        }

        protected void UpdateWorldMatrix()
        {
            worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Pos);
        }
    }
}
