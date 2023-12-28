using OpenTK_Renderer.Resources.Scene;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace OpenTK_Renderer
{
    // TODO Create light classes and use ImguiNet    
    /// <summary>
    /// Partial class for rendering
    /// </summary>
    public partial class MainRenderWindow : RenderWindow
    {
        public MainRenderWindow(int width, int height, string title, int targetFrame = 60) : base(width, height, title, targetFrame) { }
        
        private Scene _scene;
        private Camera _camera;
        private FrameBuffer _fbo;

        protected override void OnLoad()
        {
            base.OnLoad();

            // TODO setting by mask
            
            // Enable depth
            GL.Enable(EnableCap.DepthTest);
            
            // Default Depth Function
            GL.DepthFunc(DepthFunction.Less);
            
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            
            // Stencil Test
            // GL.Enable(EnableCap.StencilTest);

            // Turn off render Skybox
            RenderSetting.RenderSkybox = false;
            
            _scene = new Shadow(RenderSetting,
                new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y));
            
            // Initialize fields
            _camera = _scene.Camera;
            CursorState = CursorState.Grabbed;

            _fbo = new FrameBuffer(Size.X, Size.Y);
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
            base.OnRenderFrame(args);
            
            Title = $"Running - Vsync: {VSync}) FPS: {1f / args.Time:0}";

            // Render scene
            _fbo.Bind();
            
            {
                _scene.Update();
                _scene.Render();
            }
            
            // Second pass
            _fbo.Process();
            
            SwapBuffers();
        }

        /// <summary>
        /// Callback for resizing window
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            // _fbo?.Initialize(Size.X, Size.Y);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
