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

    public class Ball : DrawablePhysicsObject
    {
        public BoundingSphere BoundingSphere { get; private set; }
        private BEPUutilities.Vector3 homePos;

        public Ball(Game game, Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity, Vector3 pos, float scale) : base(game, scene, mesh, mat, entity, pos, new Vector3(scale, scale, scale))
        {
            BoundingSphere = new BoundingSphere(pos, scale);
            entity.Tag = this;
            homePos = MathConverter.Convert(pos);
        }

        public override void Update(GameTime gameTime)
        {
            BoundingSphere = new BoundingSphere(Pos, BoundingSphere.Radius);
            base.Update(gameTime);
            
        }

        public void ResetPosition()
        {
            Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            Entity.AngularVelocity = BEPUutilities.Vector3.Zero;
            Entity.Position = homePos;
        }
    }
}
