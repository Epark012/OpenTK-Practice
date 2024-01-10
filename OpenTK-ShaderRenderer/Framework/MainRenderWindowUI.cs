using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using Vector2 = System.Numerics.Vector2;

namespace OpenTK_Renderer;

public partial class MainRenderWindow
{
    private readonly Vector2 _min = new (0, 1);
    private readonly Vector2 _max = new (1, 0);

    #region Scene Navigation Window Config
    private static readonly float SceneWindowWidth = 300f;
    private static readonly Vector2 SceneWindowPosition = new(0, 19);
    private Vector2 _sceneWindowSize;
    #endregion

    #region Inspector Window Config

    private static readonly float InspectorWindowWidth = 300f;
    private static Vector2 _inspectorWindowPosition;
    private Vector2 _inspectorWindowSize;

    #endregion
    
    #region Game View Window Config

    private Vector2 _gameViewSize;
    private Vector2 _gameViewWindowPosition;
    private Vector2 _lastCachedMousePosition;
    
    #endregion

    private void InitializeUIConfig()
    {
        _sceneWindowSize = new Vector2(300, ClientSize.Y - 19);
        
        _inspectorWindowSize = new Vector2(InspectorWindowWidth, ClientSize.Y - 19);
        _inspectorWindowPosition = new Vector2(ClientSize.X - InspectorWindowWidth, 19);

        _gameViewSize = new Vector2(ClientSize.X - SceneWindowWidth - InspectorWindowWidth, ClientSize.Y);
        _gameViewWindowPosition = new Vector2(SceneWindowWidth, 19);

        _onGameViewActivated += activated =>
        {
            // TODO: Cache mouse position
        };
    }
    
    private void RenderImGuiLayer()
    {
        // Views
        DrawSceneNavWindow();
        DrawInspectorWindow();
        DrawGameView();

        // Main menu
        DrawMainMenu();
    }

    private void DrawSceneNavWindow()
    {
        ImGui.SetNextWindowSize(_sceneWindowSize);
        ImGui.SetNextWindowPos(SceneWindowPosition);
        ImGui.Begin("Scene Navigation", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize);
        
        ImGui.Text("Need to make tree view");
        
        ImGui.End();
    }

    private void DrawGameView()
    {
        ImGui.SetNextWindowSize(_gameViewSize);
        ImGui.SetNextWindowPos(_gameViewWindowPosition);
        ImGui.Begin("Window", ImGuiWindowFlags.NoCollapse);
        // var size = ImGui.GetContentRegionAvail();
        var size = _gameViewSize;
        // _gameViewFrameBuffer.Resize((int)size.X, (int)size.Y);
        GL.Viewport(0, 0, (int)ClientSize.X, (int)ClientSize.Y);

        var pos = ImGui.GetCursorScreenPos();
        ImGui.GetWindowDrawList().AddImage((IntPtr)_gameViewFrameBuffer.BufferTexture, 
            pos, 
            new Vector2(pos.X + size.X, pos.Y + size.Y), 
            _min, 
            _max);

        GameViewActive = ImGui.IsWindowFocused();
        ImGui.End();
    }

    private void DrawInspectorWindow()
    {
        ImGui.SetNextWindowSize(_inspectorWindowSize);
        ImGui.SetNextWindowPos(_inspectorWindowPosition);
        ImGui.Begin("Inspector");

        ImGui.End();
    }

    private void DrawMainMenu()
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
}
