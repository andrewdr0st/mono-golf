using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGolf
{
    public class DrawableObject : DrawableGameComponent
    {
        Scene scene;
        ModelMesh mesh;
        protected Matrix worldMatrix;

        public Vector3 Pos { get; set; }
        public Vector3 Scale { get; set; }
        public ObjectMaterial Material { get; set; }

        public DrawableObject(Scene scene, ModelMesh mesh, ObjectMaterial mat) : base(scene.Game)
        {
            this.scene = scene;
            this.mesh = mesh;
            Material = mat;
            Pos = new Vector3(0, 0, 0);
            Scale = new Vector3(1, 1, 1);
            UpdateWorldMatrix();
            SetupEffects();
        }

        public DrawableObject(Scene scene, ModelMesh mesh, ObjectMaterial mat, Vector3 pos, Vector3 scale) : base(scene.Game)
        {
            this.scene = scene;
            this.mesh = mesh;
            Material= mat;
            Pos = pos;
            Scale = scale;
            UpdateWorldMatrix();
            SetupEffects();
        }

        private void SetupEffects()
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.LightingEnabled = true;
                effect.DirectionalLight0.Enabled = true;
                effect.DirectionalLight1.Enabled = false;
                effect.DirectionalLight2.Enabled = false;
                effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-0.25f, -1f, -0.1f));
                effect.DirectionalLight0.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                effect.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.View = scene.Camera.ViewMatrix;
                effect.Projection = scene.Camera.Projection;
                effect.World = worldMatrix;
                effect.AmbientLightColor = Material.AmbientColor;
                effect.DiffuseColor = Material.DiffuseColor;
                effect.SpecularColor = Material.SpecularColor;
            }
            mesh.Draw();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateWorldMatrix();
            base.Update(gameTime);
        }

        protected virtual void UpdateWorldMatrix()
        {
            worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Pos);
        }
    }


    public class ObjectMaterial
    {
        public Vector3 DiffuseColor { get; protected set; }
        public Vector3 SpecularColor { get; protected set; }
        public Vector3 AmbientColor { get; protected set; }
    }

    public class FloorMaterial : ObjectMaterial
    {
        public FloorMaterial()
        {
            DiffuseColor = new Vector3(0.2f, 0.2f, 0.25f);
            SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            AmbientColor = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    public class WallMaterial : ObjectMaterial
    {
        public WallMaterial()
        {
            DiffuseColor = new Vector3(0.25f, 0.25f, 0.6f);
            SpecularColor = new Vector3(0.35f, 0.35f, 0.35f);
            AmbientColor = new Vector3(0.1f, 0.1f, 0.25f);
        }
    }

    public class BallMaterial : ObjectMaterial
    {
        public BallMaterial()
        {
            DiffuseColor = new Vector3(0.75f, 0.75f, 0.75f);
            SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            AmbientColor = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public class IndicatorMaterial : ObjectMaterial
    {
        public IndicatorMaterial()
        {
            DiffuseColor = new Vector3(0.5f, 0.5f, 1f);
            SpecularColor = new Vector3(0f, 0f, 0f);
            AmbientColor = new Vector3(0.5f, 0.5f, 1f);
        }
    }

    public class TeeMaterial : ObjectMaterial
    {
        public TeeMaterial()
        {
            DiffuseColor = new Vector3(0.05f, 0.05f, 0.05f);
            SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            AmbientColor = new Vector3(0.05f, 0.05f, 0.05f);
        }
    }
}
