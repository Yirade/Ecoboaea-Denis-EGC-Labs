using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTK_SimpleProject
{
    class SimpleWindow : GameWindow
    {
        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        public SimpleWindow() : base(800, 600)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.25f, 0.25f, 1.0f, 1.0f); // Blue color in RGB format
            GL.Enable(EnableCap.DepthTest); // Activate depth testing
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            double aspectRatio = Width / (double)Height;
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspectRatio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            KeyboardState keyboard = OpenTK.Input.Keyboard.GetState();
            MouseState mouse = OpenTK.Input.Mouse.GetState();

            if (keyboard[OpenTK.Input.Key.Escape])
            {
                Exit();
            }

            // Controlul obiectului prin taste
            if (keyboard[OpenTK.Input.Key.Left])
            {
                rotationY -= 1.0f;
            }
            if (keyboard[OpenTK.Input.Key.Right])
            {
                rotationY += 1.0f;
            }
            if (keyboard[OpenTK.Input.Key.Up])
            {
                rotationX -= 1.0f;
            }
            if (keyboard[OpenTK.Input.Key.Down])
            {
                rotationX += 1.0f;
            }

            // Controlul obiectului prin mouse
            float sensitivity = 0.01f;
            rotationY -= mouse.X * sensitivity;
            rotationX -= mouse.Y * sensitivity;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Activate depth buffer

            Matrix4 lookAt = Matrix4.LookAt(20, 50, 20, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            GL.Rotate(rotationX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(rotationY, 0.0f, 1.0f, 0.0f);

            DrawCube();

            SwapBuffers();
        }

        private void DrawCube()
        {
            float size = 5.0f; // Set the size of the cube
            float halfSize = size / 2;

            GL.Begin(PrimitiveType.Quads);
            // Front face
            GL.Color3(1.0f, 0.0f, 0.0f); // Red color in RGB format
            GL.Vertex3(-halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, halfSize);
            GL.Vertex3(-halfSize, halfSize, halfSize);

            // Back face
            GL.Color3(0.0f, 1.0f, 0.0f); // Green color in RGB format
            GL.Vertex3(-halfSize, -halfSize, -halfSize);
            GL.Vertex3(halfSize, -halfSize, -halfSize);
            GL.Vertex3(halfSize, halfSize, -halfSize);
            GL.Vertex3(-halfSize, halfSize, -halfSize);

            // Left face
            GL.Color3(0.0f, 0.0f, 1.0f); // Blue color in RGB format
            GL.Vertex3(-halfSize, -halfSize, halfSize);
            GL.Vertex3(-halfSize, halfSize, halfSize);
            GL.Vertex3(-halfSize, halfSize, -halfSize);
            GL.Vertex3(-halfSize, -halfSize, -halfSize);

            // Right face
            GL.Color3(1.0f, 1.0f, 0.0f); // Yellow color in RGB format
            GL.Vertex3(halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, -halfSize);
            GL.Vertex3(halfSize, -halfSize, -halfSize);

            // Top face
            GL.Color3(1.0f, 0.0f, 1.0f); // Magenta color in RGB format
            GL.Vertex3(-halfSize, halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, -halfSize);
            GL.Vertex3(-halfSize, halfSize, -halfSize);

            // Bottom face
            GL.Color3(0.0f, 1.0f, 1.0f); // Cyan color in RGB format
            GL.Vertex3(-halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, -halfSize, -halfSize);
            GL.Vertex3(-halfSize, -halfSize, -halfSize);

            GL.End();
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (SimpleWindow example = new SimpleWindow())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
