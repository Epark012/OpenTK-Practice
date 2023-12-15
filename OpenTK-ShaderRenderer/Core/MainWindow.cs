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
        
        private Mesh _mesh;
        
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

        uint[] indices = 
        {
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
        private Shader _lightShader;

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

            _shader = new Shader("Resources/Shader/Default.vert", "Resources/Shader/Default.frag");

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

            var vertices = new List<Vertex>();
            for (int i = 0; i < _vertices.Length / 8; i++)
            {
                var cycle = i * 8;
                var pos = new Vector3(_vertices[0 + cycle], _vertices[1 + cycle], _vertices[2 + cycle]);
                var normals = new Vector3(_vertices[3 + cycle], _vertices[4 + cycle], _vertices[5 + cycle]);
                var tex = new Vector2(_vertices[6 + cycle], _vertices[7 + cycle]);

                vertices.Add(new Vertex(pos, normals, tex));
            }

            var texs = new List<Texture>();
            texs.Add(_texture);
            texs.Add(_texture2);
            
            _mesh = new Mesh(vertices, indices.ToList(), texs);

            // _model = new Model("Resources/Model/Sphere.obj");

            #region Lighting

            _lightShader = new Shader("Resources/Shader/Light.vert", "Resources/Shader/Light.frag");
            
            _lightMesh = new Mesh(vertices, indices.ToList(), texs);
            
            #endregion
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Process input
            ProcessKeyboardInput(args);

            // Get the mouse state
            ProcessMouseInput();
        }

        private Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private Vector3 lightColor = Vector3.One;
        private Mesh _lightMesh;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            {
                
                _shader.Use();
                
                for (var i = 0; i < _position.Length; i++)
                {
                    var t = _position[i];
                    var model = Matrix4.Identity;
                    Matrix4.CreateTranslation(t, out var newPos);
                    model *= newPos;

                    _shader.SetUniform<Matrix4>("model", model);
                    _shader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                    _shader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                    
                    _shader.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
                    _shader.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
                    _shader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                    _shader.SetUniform("material.shininess", 32.0f);
                    
                    // The ambient light is less intensive than the diffuse light in order to make it less dominant
                    var ambientColor = lightColor * new Vector3(0.2f);
                    var diffuseColor = lightColor * new Vector3(1f);

                    _shader.SetUniform("light.position", lightPos);
                    _shader.SetUniform("light.ambient", ambientColor);
                    _shader.SetUniform("light.diffuse", diffuseColor);
                    _shader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

                    // _model.Draw(_shader);
                    _mesh.Draw(_shader);
                }
                
                // Light
                _lightShader.Use();

                var lightModel = Matrix4.Identity * Matrix4.CreateTranslation(lightPos);
                lightModel *= Matrix4.CreateScale(0.2f);
                
                _lightShader.SetUniform<Matrix4>("model", lightModel);
                _lightShader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                _lightShader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                
                _lightMesh.Draw(_lightShader);
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
        
        /// <summary>
        /// Callback for resizing window
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
