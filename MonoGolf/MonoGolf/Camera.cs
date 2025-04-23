using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGolf
{
    public class Camera
    {
        private Vector3 dir;
        private float theta;
        private float phi;
        private float minPhi = 0;
        private float maxPhi = MathHelper.Pi * 0.475f;
        private float sensitivity = 0.025f;
        private Vector3 target;
        private float zoom = 20f;
        private float minZoom = 5f;
        private float maxZoom = 50f;
        private float fov = MathHelper.PiOver4;

        public Matrix ViewMatrix { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(float theta, float phi, Vector3 target, float aspectRatio)
        {
            this.theta = theta;
            this.phi = MathHelper.Clamp(phi, minPhi, maxPhi);
            UpdateDir();
            this.target = target;
            //ViewMatrix = 
            Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, 0.5f, 300f);
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.CreateLookAt(target + dir * zoom, target, Vector3.Up);
        }

        public void Rotate(Vector2 v)
        {
            theta += v.X * sensitivity;
            phi = MathHelper.Clamp(phi + v.Y * sensitivity, minPhi, maxPhi);
            UpdateDir();
        }

        private void UpdateDir()
        {
            float x = (float) (Math.Sin(theta) * Math.Cos(phi));
            float y = (float) (Math.Sin(phi));
            float z = (float) (Math.Cos(theta) * Math.Cos(phi));
            dir = new Vector3(x, y, z);
        }

        public void SetTarget(Vector3 t)
        {
            target = t;
        }

        public void Zoom(float amount)
        {
            zoom = MathHelper.Clamp(zoom + amount, minZoom, maxZoom);
        }

    }
}
