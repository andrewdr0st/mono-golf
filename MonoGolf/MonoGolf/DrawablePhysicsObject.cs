using System.Diagnostics;
using BEPUphysics.Entities;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGolf
{
    public class DrawablePhysicsObject : DrawableObject, IPhysicsComponent
    {
        public Entity Entity { get; set; }


        public DrawablePhysicsObject(Game game, Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity) : base(game, scene, mesh, mat)
        {
            Entity = entity;
        }

        public DrawablePhysicsObject(Game game, Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity, Vector3 pos, Vector3 scale) : base(game, scene, mesh, mat, pos, scale)
        {
            Entity = entity;
            Entity.Position = MathConverter.Convert(pos);
        }

        public override void Update(GameTime gameTime)
        {
            Pos = MathConverter.Convert(Entity.Position);
            UpdateWorldMatrix();
            base.Update(gameTime);
        }
    }
}
