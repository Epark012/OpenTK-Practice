using OpenTK_Renderer.GUI;
using OpenTK_Renderer.Resources.Scene;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenTK_Renderer
{
    // TODO Resize frame buffer
    /// <summary>
    /// Partial class for rendering
    /// </summary>
    public partial class MainRenderWindow : RenderWindow
    {
        public MainRenderWindow(WindowState windowState, string title, int targetFrameRate = 60) : base(windowState, title, targetFrameRate) { }
        public MainRenderWindow(int width, int height, string title, int targetFrameRate = 60) : base(width, height, title, targetFrameRate) { }
        
        private Scene _gameViewScene;
        private Camera _camera;
        private FrameBuffer _gameViewFrameBuffer;

        private GUIController _controller;

        protected override void OnLoad()
        {
            base.OnLoad();
            
            // Initialize UI configs
            InitializeUIConfig();
            
            // Initialize interaction
            InitializeInteraction();
            
            // TODO setting by mask
            // Enable depth
            GL.Enable(EnableCap.DepthTest);
            
            // Default Depth Function
            GL.DepthFunc(DepthFunction.Less);
            
            // GL.Enable(EnableCap.CullFace);
            // GL.CullFace(CullFaceMode.Back);
            
            // Stencil Test
            // GL.Enable(EnableCap.StencilTest);

            // Turn off render Skybox
            RenderSetting.RenderSkybox = false;
            
            _gameViewScene = new Shadow(RenderSetting,
                new Camera(Vector3.UnitZ * 3, _gameViewSize.X / (float)_gameViewSize.Y));
            
            // Initialize fields
            _camera = _gameViewScene.Camera;
            
            _controller = new GUIController(ClientSize.X, ClientSize.Y);
            
            // TODO I don't know why the ratio is different when using 1200, 1000 stuff
            _gameViewFrameBuffer = new FrameBuffer(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Process input
            ProcessInput(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            Title = $"Running - Vsync: {VSync}) FPS: {1f / args.Time:0}";
            
            _controller.Update(this, (float)args.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw UI
            // ImGui.ShowDemoWindow();
            RenderImGuiLayer();

            // Render scene
            _gameViewFrameBuffer.Bind(true);
            
            {
                _gameViewScene.Update();
                _gameViewScene.Render();
            }
            
            _gameViewFrameBuffer.Bind(false);
            
            _controller.Render();
            GUIController.CheckGLError("End of frame");
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

            // _fbo.Resize();
            // _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _controller.MouseScroll(e.Offset);
        }
    }
}
