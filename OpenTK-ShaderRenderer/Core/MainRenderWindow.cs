using OpenTK_Renderer.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

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
        private Scene _scene;
        
        // Models
        private Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private Model _lightModel;
        private Shader _lightShader;
        
        // Cube map
        private CubeMap _cubeMap;

        private Camera _camera;
        
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

            // Initialize scene
            _scene = new Scene( RenderSetting, 
                    new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y), 
                    null, 
            new Object(new Model("Resources/Model/Ship.fbx"), new Shader("Resources/Shader/Default.vert", "Resources/Shader/Default.frag")));

            _scene.Initialize();
            
            // Initialize fields
            _camera = _scene.Camera;
            CursorState = CursorState.Grabbed;

            {
                #region Lighting
                
                _lightShader = new Shader("Resources/Shader/Light.vert", "Resources/Shader/Light.frag");
                _lightModel = new Model("Resources/Model/Cube.fbx");
                
                #endregion
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
        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Title = $"Running - Vsync: {VSync}) FPS: {1f / args.Time:0}";
            GL.Viewport(0, 0, Size.X, Size.Y);

            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            {
                _scene.Render();
                
                // Light
                _lightShader.Use();

                var lightModel = Matrix4.Identity * Matrix4.CreateTranslation(lightPos);
                lightModel *= Matrix4.CreateScale(0.2f);
                
                _lightShader.SetUniform<Matrix4>("view", _camera.GetViewMatrix());
                _lightShader.SetUniform<Matrix4>("projection", _camera.GetProjectionMatrix());
                
                _lightModel.Draw(_lightShader, lightModel);
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
