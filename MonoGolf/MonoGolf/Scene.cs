using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGolf
{
    public class Scene
    {
        public Camera Camera { get; protected set; }

        List<DrawableObject> objects;
        
        protected Scene()
        {
            Camera = new Camera(0, MathHelper.Pi * 0.25f, Vector3.Zero, MathHelper.PiOver4);
            objects = new List<DrawableObject>();
        }
        
    }

    public class Hole1 : Scene
    {

    }
}
