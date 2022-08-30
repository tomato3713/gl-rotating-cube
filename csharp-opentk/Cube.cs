using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CSharpOpentk
{
	class Cube : GameWindow
	{
		private bool disposed = false;
		private int vao;
		private int vbo;
        private int cbo;
		private int prog;

        private Matrix4 proj, view, model;

        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

		private float[] cube = {
            // x, y, z
            -0.5f, -0.5f, -0.5f, -0.5f, -0.5f,  0.5f, -0.5f,  0.5f, 0.5f,
            0.5f,   0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f,  0.5f, -0.5f,
            0.5f,  -0.5f, 0.5f,  -0.5f, -0.5f, -0.5f, 0.5f,  -0.5f, -0.5f,
            0.5f,   0.5f, -0.5f,  0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f, -0.5f, 0.5f,   0.5f, -0.5f,  0.5f, -0.5f,
            0.5f,  -0.5f, 0.5f,  -0.5f, -0.5f,  0.5f, -0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f, 0.5f,  -0.5f, -0.5f,  0.5f, 0.5f,  -0.5f, 0.5f,
            0.5f,   0.5f, 0.5f,   0.5f, -0.5f, -0.5f, 0.5f,   0.5f, -0.5f,
            0.5f,  -0.5f, -0.5f,  0.5f, 0.5f,   0.5f, 0.5f,  -0.5f, 0.5f,
            0.5f,   0.5f, 0.5f,   0.5f, 0.5f,  -0.5f, -0.5f,  0.5f, -0.5f,
            0.5f,   0.5f, 0.5f,  -0.5f, 0.5f,  -0.5f, -0.5f,  0.5f, 0.5f,
            0.5f,   0.5f, 0.5f,  -0.5f, 0.5f,   0.5f, 0.5f,  -0.5f, 0.5f
        };

        private float[] color = {
            // r, g, b
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
            1.0f,  0.0f,  0.0f,
            0.0f,  1.0f,  0.0f,
            0.0f,  0.0f,  1.0f,
        };


        public Cube(int width, int height, string title)
            : base(
                    new OpenTK.Windowing.Desktop.GameWindowSettings(),
                    new OpenTK.Windowing.Desktop.NativeWindowSettings()
                    {
                    APIVersion = new System.Version(4, 6),
                    API = ContextAPI.OpenGL,
                    Profile = ContextProfile.Compatability,
                    Size = new OpenTK.Mathematics.Vector2i(width, height),
                    Title = title
                    }
                  )
            {
            }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                // GL関連リソースを解放する．C#外のリソース管理されていないオブジェクトであるため．
                disposed = true;
                GL.DeleteProgram(prog);
                GL.DeleteBuffer(vbo);
                GL.DeleteVertexArray(vao);
            }
            base.Dispose(disposing);
        }

        protected override void OnResize(ResizeEventArgs args)
        {
            base.OnResize(args);
            GL.Viewport(0, 0, args.Width, args.Height);

            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)Size.X / (float)Size.Y, 0.1f, 100.0f);
            SetProj(proj);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            var vertShaderCode = System.IO.File.ReadAllText(@"..\Shader\shader.vert");
            var fragShaderCode = System.IO.File.ReadAllText(@"..\Shader\shader.frag");

            int vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, vertShaderCode);
            GL.CompileShader(vertShader);
            string vertInfoLog = GL.GetShaderInfoLog(vertShader);
            if(!string.IsNullOrEmpty(vertInfoLog))
            {
                Console.WriteLine("vertex shader has some errors.");
                Console.WriteLine(vertInfoLog);
                Close();
            }

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, fragShaderCode);
            GL.CompileShader(fragShader);
            string fragInfoLog = GL.GetShaderInfoLog(fragShader);
            if(!string.IsNullOrEmpty(fragInfoLog))
            {
                Console.WriteLine("fragment shader has some errors.");
                Console.WriteLine(fragInfoLog);
                Close();
            }
            prog = GL.CreateProgram();

            GL.AttachShader(prog, vertShader);
            GL.AttachShader(prog, fragShader);
            GL.LinkProgram(prog);
            string progInfoLog = GL.GetProgramInfoLog(prog);
            if(!string.IsNullOrEmpty(progInfoLog))
            {
                Console.WriteLine("program has some errors.");
                Console.WriteLine(progInfoLog);
                Close();
            }
            GL.DetachShader(prog, vertShader);
            GL.DetachShader(prog, fragShader);
            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);

            // Load uniform locations
            GL.GetProgram(prog, GetProgramParameterName.ActiveUniforms, out var n);
            for(var i = 0; i < n; i++) {
                var key = GL.GetActiveUniform(prog, i, out _, out _);
                var loc = GL.GetUniformLocation(prog, key);
                uniformLocations.Add(key, loc);
            }

            GL.UseProgram(prog);

            GL.Enable(EnableCap.DepthTest);

            // vertex array
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // vertex buffer
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            var posLoc = GL.GetAttribLocation(prog, "a_pos");
            GL.EnableVertexAttribArray(posLoc);
            GL.VertexAttribPointer(posLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BufferData(BufferTarget.ArrayBuffer, cube.Length * sizeof(float), cube, BufferUsageHint.StaticDraw);

            // color buffer
            cbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            var colorLoc = GL.GetAttribLocation(prog, "a_color");
            GL.EnableVertexAttribArray(colorLoc);
            GL.VertexAttribPointer(colorLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BufferData(BufferTarget.ArrayBuffer, color.Length * sizeof(float), color, BufferUsageHint.StaticDraw);

            model = Matrix4.CreateScale(1.0f);
            SetModel(model);

            view = Matrix4.CreateTranslation(0, 0, -5.0f);
            SetView(view);

            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)Size.X / (float)Size.Y, 0.1f, 100.0f);
            SetProj(proj);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            model *= Matrix4.CreateRotationY(0.01f) * Matrix4.CreateRotationX(0.01f);
            SetModel(model);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 12*3);

            Context.SwapBuffers();

            base.OnUpdateFrame(args);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Q:
                    Close();
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }

        private void SetModel(Matrix4 modelMat) {
            GL.UniformMatrix4(uniformLocations["model"], true, ref modelMat);
        }
        private void SetView(Matrix4 viewMat) {
            GL.UniformMatrix4(uniformLocations["view"], true, ref viewMat);
        }
        private void SetProj(Matrix4 projMat) {
            GL.UniformMatrix4(uniformLocations["proj"], true, ref projMat);
        }
    }
}

