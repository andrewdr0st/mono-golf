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

namespace MonoGolf
{
    public class CourseObject : DrawablePhysicsObject
    {
        public CourseObject(Scene scene, ModelMesh mesh, ObjectMaterial mat, Entity entity, Vector3 pos, Vector3 scale, float r) : base(scene, mesh, mat, entity, pos, scale)
        {
            r = MathHelper.ToRadians(r);
            Entity.Orientation = BEPUutilities.Quaternion.CreateFromAxisAngle(BEPUutilities.Vector3.Up, r);
            Entity.Material.Bounciness = 0.4f;
            Matrix rotate = Matrix.CreateFromAxisAngle(Vector3.Up, r);
            worldMatrix = rotate * Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Pos);
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
    }

    public class FloorBox(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[0], new FloorMaterial(), MakeBox(pos, scale), pos, scale, r)
    {
    }

    public class WallBox(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[0], new WallMaterial(), MakeBox(pos, scale), pos, scale, r)
    {
    }

    public class FloorSlope(Scene scene, Vector3 pos, Vector3 scale, float r) : CourseObject(scene, Minigolf.MeshList[3], new FloorMaterial(), MakeSlope(pos, scale), pos, scale, r)
    {
    }

}
