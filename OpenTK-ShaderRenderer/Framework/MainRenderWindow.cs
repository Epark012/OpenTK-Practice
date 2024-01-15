using OpenTK_Renderer.GUI;
using OpenTK_Renderer.Resources.Scene;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
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
        private int _depthmap;
        private int _depthmapFbo;
        private Shader _depthShader;

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

            _depthShader = new Shader("Resources/Shader/Depth.vert", "Resources/Shader/Depth.frag");

            _depthmapFbo = GL.GenFramebuffer();
            _depthmap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _depthmap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, 1024, 1024, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthmapFbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthmap, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

            // Render depth of scene from light's perspective
            var lightProjection = Matrix4.CreateOrthographic(20, 20, 1, 7.5f);
            var lightView = Matrix4.LookAt(_gameViewScene.DirectionalLight[0].Direction, Vector3.Zero, Vector3.Zero);
            var lightSpaceMatrix = lightView * lightProjection;
            
            _depthShader.Use(true);
            _depthShader.SetUniform("lightSpaceMatrix", lightSpaceMatrix);
            
            GL.Viewport(0, 0, 1024, 1024);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthmapFbo);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.ActiveTexture(TextureUnit.Texture0);
                _gameViewScene.Render();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
            GL.Viewport(0,0, ClientSize.X, ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                
            // Render game view
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
