using System.Diagnostics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGolf
{
    public class DrawablePhysicsObject : DrawableObject, IPhysicsComponent
    {
        public Entity Entity { get; set; }

        public DrawablePhysicsObject(Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity) : base(scene, mesh, mat)
        {
            Entity = entity;
        }

        public DrawablePhysicsObject(Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity, Vector3 pos, Vector3 scale) : base(scene, mesh, mat, pos, scale)
        {
            Entity = entity;
            Entity.Position = MathConverter.Convert(pos);
        }

        public override void Update(GameTime gameTime)
        {
            Pos = MathConverter.Convert(Entity.Position);
            base.Update(gameTime);
        }
    }

    public class Ball : DrawablePhysicsObject
    {
        public BoundingSphere BoundingSphere { get; private set; }
        private BEPUutilities.Vector3 homePos;
        public bool BallActive { get; set; }
        private const float ballScale = 0.75f;
        private const float velocityThreshold = 0.4f;

        public Ball(Scene scene, Vector3 pos) : base(scene, Minigolf.MeshList[1], new BallMaterial(), new Sphere(MathConverter.Convert(pos), 0.75f, 0.2f), pos, new Vector3(ballScale))
        {
            BoundingSphere = new BoundingSphere(pos, ballScale);
            Entity.Tag = this;
            Entity.AngularDamping = 0.5f;
            Entity.LinearDamping = 0.15f;
            Entity.Material.Bounciness = 0.9f;
            homePos = MathConverter.Convert(pos);
            BallActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            BoundingSphere = new BoundingSphere(Pos, BoundingSphere.Radius);
            if (BallActive && Entity.LinearVelocity.LengthSquared() < velocityThreshold && Entity.AngularVelocity.LengthSquared() < velocityThreshold)
            {
                Entity.AngularVelocity = BEPUutilities.Vector3.Zero;
                Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
                BallActive = false;
                if (Pos.Y > -5f)
                {
                    homePos = MathConverter.Convert(Pos);
                }
            }
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
