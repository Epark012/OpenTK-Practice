using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK_Renderer
{
    public class MainWindow : GameWindow
    {
        public MainWindow(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Title = title, ClientSize = (width, height) }) { }
        private Vector4 _initialBackgroundColor = new Vector4(0.8f, 0.674f, 0.6313f, 1.0f);

        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        private int _vao, _vbo, _ebo;

        #region Test

        private Vector3[] _position = new Vector3[10];

        #endregion

        float[] _vertices = {
           // Position            Normal                  Texture coordinates  
           // Up
           0.5f,  0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       1.0f, 1.0f,
           0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       1.0f, 0.0f,
          -0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       0.0f, 0.0f,
          -0.5f,  0.5f, 0.5f,     0.0f, 0.0f, 1.0f,       0.0f, 1.0f,
                        
            // Down                 
           0.5f,  0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      1.0f, 1.0f,
           0.5f, -0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      0.0f, 0.0f,
          -0.5f,  0.5f, -0.5f,    0.0f, 0.0f, -1.0f,      0.0f, 1.0f,
                        
            // Foward               
           0.5f, -0.5f, 0.5f,     0.0f, -1.0f, 0.0f,      1.0f, 1.0f,
           0.5f, -0.5f, -0.5f,    0.0f, -1.0f, 0.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    0.0f, -1.0f, 0.0f,      0.0f, 0.0f,
          -0.5f, -0.5f,  0.5f,    0.0f, -1.0f, 0.0f,      0.0f, 1.0f,
                        
            // Back                 
           0.5f, 0.5f, 0.5f,      0.0f, 1.0f, 0.0f,       1.0f, 1.0f,
           0.5f, 0.5f, -0.5f,     0.0f, 1.0f, 0.0f,       1.0f, 0.0f,
          -0.5f, 0.5f, -0.5f,     0.0f, 1.0f, 0.0f,       0.0f, 0.0f,
          -0.5f, 0.5f,  0.5f,     0.0f, 1.0f, 0.0f,       0.0f, 1.0f,
                        
            // Right                
           0.5f,  0.5f, 0.5f,      1.0f, 0.0f, 0.0f,       1.0f, 1.0f,
           0.5f,  0.5f, -0.5f,     1.0f, 0.0f, 0.0f,       1.0f, 0.0f,
           0.5f, -0.5f, -0.5f,     1.0f, 0.0f, 0.0f,       0.0f, 0.0f,
           0.5f, -0.5f,  0.5f,     1.0f, 0.0f, 0.0f,       0.0f, 1.0f,
                        
            // Left                 
          -0.5f,  0.5f, 0.5f,     -1.0f, 0.0f, 0.0f,      1.0f, 1.0f,
          -0.5f,  0.5f, -0.5f,    -1.0f, 0.0f, 0.0f,      1.0f, 0.0f,
          -0.5f, -0.5f, -0.5f,    -1.0f, 0.0f, 0.0f,      0.0f, 0.0f,
          -0.5f, -0.5f,  0.5f,    -1.0f, 0.0f, 0.0f,      0.0f, 1.0f
        };

        uint[] indices = {  // note that we start from 0!
           // Up
            0, 3, 1,
            1, 3, 2,   
            
            // Down
            4, 7, 5,
            5, 7, 6,

            // Foward
            8, 11, 09,
            9, 11, 10,

            // Back
            12, 15, 13,
            13, 15, 14,

            // Right
            16, 17, 19,
            17, 18, 19,

            // Left
            20, 21, 23,
            21, 22, 23,
        };

        private Shader _shader;
        private Texture _texture;
        private Texture _texture2;

        protected override void OnLoad()
        {
            base.OnLoad();

            // Clear background 
            GL.ClearColor(_initialBackgroundColor.X,
                          _initialBackgroundColor.Y,
                          _initialBackgroundColor.Z,
                          _initialBackgroundColor.W);

            // Enable depth
            GL.Enable(EnableCap.DepthTest);

            // Vao
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            // Vbo
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            _shader = new Shader("Resources/Shader/Default.vert", "Resources/Shader/Default.frag");

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var normal = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            _texture = Texture.LoadFromFile("Resources/Image/container.png");
            _texture2 = Texture.LoadFromFile("Resources/Image/wall.jpg");

            // Set uniform for multiple textures
            _shader.SetUniform("texture0", 0);
            _shader.SetUniform("texture1", 1);

            // Camera
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;
            
            // Create random position
            var random = new Random();
            for (var i = 0; i < _position.Length; i++)
            {
                _position[i] = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(-5, 5));
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Process input
            ProcessKeyboardInput(args);

            // Get the mouse state
            ProcessMouseInput();
        }
        private double _time;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            {
                GL.BindVertexArray(_vao);

                _texture.Use(TextureUnit.Texture0);
                _texture2.Use(TextureUnit.Texture1);
                _shader.Use();

                _time += 40.0 * args.Time;
                foreach (var t in _position)
                {
                    var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
                    Matrix4.CreateTranslation(t, out var newPos);
                    model *= newPos;
                    
                    _shader.SetUniform<Matrix4>("model", model);
                    _shader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                    _shader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                
                    GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }

            SwapBuffers();
        }

        /// <summary>
        /// Process Mouse Input
        /// </summary>
        private void ProcessMouseInput()
        {
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
                return;
            }

            // Calculate the offset of the mouse position
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            const float sensitivity = 0.2f;

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
        }
        
        /// <summary>
        /// Process Keyboard Input
        /// </summary>
        private void ProcessKeyboardInput(FrameEventArgs e)
        {
            if (!KeyboardState.IsAnyKeyDown)
            {
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            var input = KeyboardState;

            const float cameraSpeed = 1.5f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
