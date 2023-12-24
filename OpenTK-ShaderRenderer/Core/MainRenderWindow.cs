using OpenTK_Renderer.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK_Renderer
{
    // TODO: Create Transform and Scene to control. If have time, set the scene for shader map using Imgui.net
    
    /// <summary>
    /// Partial class for rendering
    /// </summary>
    public partial class MainRenderWindow : RenderWindow
    {
        public MainRenderWindow(int width, int height, string title, int targetFrame = 60) : base(width, height, title, targetFrame) { }
        
        private readonly Vector4 _initialBackgroundColor = new (0.2f, 0.3f, 0.3f, 1.0f);

        // Camera
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;
        
        // Models
        private Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private Model _lightModel;
        private Model _model;
        private Shader _shader;
        private Shader _lightShader;
        
        // Cube map
        private CubeMap _cubeMap;

        private readonly Vector3[] _pointLightPositions =
        {
            new (0.7f, 0.2f, 2.0f),
            new (2.3f, -3.3f, -4.0f),
            new (-4.0f, 2.0f, -12.0f),
            new (0.0f, 0.0f, -3.0f)
        };
        
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
            
            // Default Depth Function
            GL.DepthFunc(DepthFunction.Less);
            
            // Stencil Test
            // GL.Enable(EnableCap.StencilTest);

            _shader = new Shader("Resources/Shader/Default.vert", "Resources/Shader/Default.frag");

            // Camera
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            CursorState = CursorState.Grabbed;

            _model = new Model("Resources/Model/Ship.fbx");

            #region Lighting

            _lightShader = new Shader("Resources/Shader/Light.vert", "Resources/Shader/Light.frag");
            _lightModel = new Model("Resources/Model/Cube.fbx");
            
            #endregion

            #region CubeMap
            
            InitializeSkybox();

            #endregion
        }

        /// <summary>
        /// Initialize skybox
        /// </summary>
        private void InitializeSkybox()
        {
            if (RenderSetting.RenderSkybox)
                _cubeMap = new CubeMap(null);
            else
                Console.WriteLine("Skip initializing skybox following RenderSetting");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Process input
            ProcessKeyboardInput(args);

            // Get the mouse state
            ProcessMouseInput();
        }
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Title = $"Running - Vsync: {VSync}) FPS: {1f / args.Time:0}";
            GL.Viewport(0, 0, Size.X, Size.Y);

            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            {
                _shader.Use();
                
                _shader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                _shader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                
                _shader.SetUniform("viewPos", _camera.Position);

                _shader.SetUniform("material.diffuse", 0);
                _shader.SetUniform("material.specular", 1);
                _shader.SetUniform("material.shininess", 32.0f);
                    
                // Directional light
                _shader.SetUniform("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
                _shader.SetUniform("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _shader.SetUniform("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
                _shader.SetUniform("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));
                
                // Point lights
                for (int i = 0; i < _pointLightPositions.Length; i++)
                {
                    _shader.SetUniform($"pointLights[{i}].position", _pointLightPositions[i]);
                    _shader.SetUniform($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                    _shader.SetUniform($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                    _shader.SetUniform($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                    _shader.SetUniform($"pointLights[{i}].constant", 1.0f);
                    _shader.SetUniform($"pointLights[{i}].linear", 0.09f);
                    _shader.SetUniform($"pointLights[{i}].quadratic", 0.032f);
                }
                
                // Spot light
                _shader.SetUniform("spotLight.position", _camera.Position);
                _shader.SetUniform("spotLight.direction", _camera.Front);
                _shader.SetUniform("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
                _shader.SetUniform("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetUniform("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetUniform("spotLight.constant", 1.0f);
                _shader.SetUniform("spotLight.linear", 0.09f);
                _shader.SetUniform("spotLight.quadratic", 0.032f);
                _shader.SetUniform("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
                _shader.SetUniform("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
                
                var time = DateTime.Now.Second + DateTime.Now.Millisecond / 1000f;

                var model = Matrix4.Identity;
                model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f , 0.3f, 0.5f), 20f * (time / 100));
                model *= Matrix4.CreateTranslation(new  Vector3(0, 0, -10));
                
                _model.Draw(_shader, model);
                
                // Light
                _lightShader.Use();

                var lightModel = Matrix4.Identity * Matrix4.CreateTranslation(lightPos);
                lightModel *= Matrix4.CreateScale(0.2f);
                
                _lightShader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                _lightShader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                
                _lightModel.Draw(_lightShader, lightModel);
                
                // Cubemap
                if (RenderSetting.RenderSkybox)
                    _cubeMap.RenderSkybox(_camera.GetViewMatrix(), _camera.GetProjectionMatrix());
            }

            SwapBuffers();
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
