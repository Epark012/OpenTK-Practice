using System.Numerics;
using ImGuiNET;
using OpenTK_Renderer.GUI;
using OpenTK_Renderer.Resources.Scene;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenTK_Renderer
{
    // TODO Resize frame buffer
    // TODO Make doc 
    /// <summary>
    /// Partial class for rendering
    /// </summary>
    public partial class MainRenderWindow : RenderWindow
    {
        public MainRenderWindow(int width, int height, string title, int targetFrame = 60) : base(width, height, title, targetFrame) { }
        
        private Scene _scene;
        private Camera _camera;
        private FrameBuffer _fbo;

        private GUIController _controller;

        private readonly Vector2 _min = new (0, 1);
        private readonly Vector2 _max = new (1, 0);
        
        protected override void OnLoad()
        {
            base.OnLoad();

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
            
            _scene = new Shadow(RenderSetting,
                new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y));
            
            // Initialize fields
            _camera = _scene.Camera;
            // CursorState = CursorState.Grabbed;
            
            _controller = new GUIController(ClientSize.X, ClientSize.Y);

            _fbo = new FrameBuffer(ClientSize.X, ClientSize.Y);
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

            _controller.Update(this, (float)args.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            ImGui.Begin("Test");

            var pos = ImGui.GetCursorScreenPos();
            ImGui.GetWindowDrawList().AddImage((IntPtr)_fbo.BufferTexture, 
                pos, 
                new Vector2(pos.X + ClientSize.X, pos.Y + ClientSize.Y), 
                _min, 
                _max);
            
            ImGui.End();
            
            DrawMenuUi();
            
            // Render scene
            _fbo.Bind(true);
            GL.Viewport(0, 0, (int)ClientSize.X, (int)ClientSize.Y);
            {
                _scene.Update();
                _scene.Render();
            }
            
            _fbo.Bind(false);
            
            // Enable Docking
            // ImGui.DockSpaceOverViewport();
            // ImGui.ShowDemoWindow();

            _controller.Render();
            GUIController.CheckGLError("End of frame");
            SwapBuffers();
        }
        

        private static void DrawMenuUi()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Add"))
                    {
                        // Add model
                    }
                    
                    ImGui.Separator();
                    if (ImGui.MenuItem("Close"))
                    {
                        // Close application
                        Environment.Exit(1);
                    }
                    
                    ImGui.EndMenu();
                }
                
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("#######"))
                    {
                        // Add function
                    }
                    
                    ImGui.EndMenu();
                }
                
                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Open Github"))
                    {
                        
                    }
                    
                    ImGui.EndMenu();
                }
                
                ImGui.EndMainMenuBar();
            }
        }

        /// <summary>
        /// Callback for resizing window
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // _fbo?.Initialize(Size.X, Size.Y);

            // TODO _fbo.Resize();
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
