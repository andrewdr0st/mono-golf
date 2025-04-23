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
        private const float minPhi = 0;
        private const float maxPhi = MathHelper.Pi * 0.475f;
        private const float sensitivity = 0.1f;
        private Vector3 target;
        private float zoom = 20f;
        private const float minZoom = 5f;
        private const float maxZoom = 50f;
        private const float fov = MathHelper.PiOver4;

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

        public void Pan(Vector2 v)
        {
            Vector3 right = Vector3.Cross(dir, Vector3.Up);
            right.Normalize();
            Vector3 forward = Vector3.Cross(Vector3.Up, right);
            Vector3 move = forward * -v.Y + right * v.X;
            target += move * zoom * 0.05f;
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
