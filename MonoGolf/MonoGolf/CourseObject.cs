using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace MonoGolf
{
    public class CourseObject : DrawablePhysicsObject
    {
        public CourseObject(Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity, Vector3 pos, Vector3 scale, float r, bool isWall) : base(scene, mesh, mat, entity, pos, scale)
        {
            r = MathHelper.ToRadians(r);
            Entity.Orientation = BEPUutilities.Quaternion.CreateFromAxisAngle(BEPUutilities.Vector3.Up, r);
            Entity.Material.Bounciness = isWall ? 0.75f : 0.5f;
            Matrix rotate = Matrix.CreateFromAxisAngle(Vector3.Up, r);
            worldMatrix = rotate * Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Pos);
            if (isWall) {
                Entity.CollisionInformation.Events.InitialCollisionDetected += (EntityCollidable sender, Collidable other, CollidablePairHandler pair) =>
                {
                    if (other is EntityCollidable otherEntityCollidable) {
                        if (otherEntityCollidable.Entity.Tag is Ball ball) {
                            Minigolf.SoundEffects[1].Play();
                        }
                    }
                };
            }
        }

        protected override void UpdateWorldMatrix()
        {
        }

        public static Entity MakeBox(Vector3 pos, Vector3 scale)
        {
            return new Box(MathConverter.Convert(pos), scale.X * 2, scale.Y * 2, scale.Z * 2);
        }

        public static Entity MakeSlope(Vector3 pos, Vector3 scale)
        {
            List<BEPUutilities.Vector3> verts = [];
            foreach (ModelMeshPart part in Minigolf.MeshList[3].MeshParts)
            {
                VertexBuffer vertexBuffer = part.VertexBuffer;
                int vertexStride = vertexBuffer.VertexDeclaration.VertexStride;
                int vertexCount = vertexBuffer.VertexCount;
                float[] vertexData = new float[vertexCount * vertexStride / sizeof(float)];
                vertexBuffer.GetData(vertexData);
                for (int i = 0; i < vertexData.Length; i += vertexStride / sizeof(float))
                {
                    BEPUutilities.Vector3 vertexPosition = new BEPUutilities.Vector3(vertexData[i] * scale.X, vertexData[i + 1] * scale.Y, vertexData[i + 2] * scale.Z);
                    verts.Add(vertexPosition);
                }
            }
            return new ConvexHull(MathConverter.Convert(pos), verts);
        }

        public static Entity MakeHole(Vector3 pos)
        {
            List<CompoundShapeEntry> entries = [];
            entries.Add(new CompoundShapeEntry(new BoxShape(4, 2, 10), MathConverter.Convert(pos + new Vector3(-3, 0, 0))));
            entries.Add(new CompoundShapeEntry(new BoxShape(4, 2, 10), MathConverter.Convert(pos + new Vector3(3, 0, 0))));
            entries.Add(new CompoundShapeEntry(new BoxShape(2, 2, 4), MathConverter.Convert(pos + new Vector3(0, 0, -3))));
            entries.Add(new CompoundShapeEntry(new BoxShape(2, 2, 4), MathConverter.Convert(pos + new Vector3(0, 0, 3))));
            entries.Add(new CompoundShapeEntry(new BoxShape(2, 1, 2), MathConverter.Convert(pos + new Vector3(0, -0.5f, 0))));
            return new CompoundBody(entries);
        }
    }

    public class FloorBox(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[0], new FloorMaterial(), MakeBox(pos, scale), pos, scale, r, false)
    {
    }

    public class WallBox(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[0], new WallMaterial(), MakeBox(pos, scale), pos, scale, r, true)
    {
    }

    public class FloorSlope(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[3], new FloorMaterial(), MakeSlope(pos, scale), pos, scale, r, false)
    {
    }

    public class WallSlope(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[3], new WallMaterial(), MakeSlope(pos, scale), pos, scale, r, true)
    {
    }

    public class HoleBox(Scene scene, Vector3 pos, float r) : CourseObject(scene, Minigolf.MeshList[4], new FloorMaterial(), MakeHole(pos), pos, new Vector3(5f, 1f, 5f), r, false)
    {
    }

    public class Tee(Scene scene, Vector3 pos, float r) : CourseObject(scene, Minigolf.MeshList[0], new TeeMaterial(), MakeBox(pos, new Vector3(2f, 0.1f, 2f)), pos, new Vector3(2f, 0.1f, 2f), r, false)
    {
    }

}
