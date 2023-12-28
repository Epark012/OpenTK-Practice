using OpenTK_Renderer.Rendering;
using OpenTK_Renderer.Rendering.Lighting;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

/// <summary>
/// Scene template
/// </summary>
public class Scene
{
    // Default 
    public Camera Camera { get; protected set; }
    protected RenderSetting RenderSetting;
    protected CubeMap? CubeMap;
    protected List<Object> Objects;

    // Light
    protected List<Light> Lights = new ();
    
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;
    protected Action<Scene>? OnInitialized;

    public Scene(RenderSetting renderSetting, Camera camera, CubeMap? cubeMap, Action<Scene>? onInitialized = null, params Object[] models)
    {
        RenderSetting = renderSetting;
        Camera = camera;
        CubeMap = cubeMap;
        Objects = models.ToList();
        
        OnInitialized = onInitialized;
           
        // Initialize fields
        Initialize();
    }

    protected void Initialize()
    {
        // Initialize objects
        foreach (var obj in Objects)
        {
            // obj.Initialize( _directionalLight, spotLight, _pointLights);
            obj.Initialize(Lights);
        }

        // Initialize cube map
        if (RenderSetting.RenderSkybox)
        {
            CubeMap ??= new CubeMap(RenderSetting.DefaultCubeMapPath);
        }
        else
        {
            GL.ClearColor(RenderSetting.DefaultBackGroundColor);
        }
        
        OnInitialized?.Invoke(this);
    }
    
    public void Update()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        GL.Enable(EnableCap.DepthTest);
        
        foreach (var obj in Objects)
        {
            obj.Update();
        }
    }
    
    /// <summary>
    /// Render this scene
    /// </summary>
    public void Render()
    {
        // Set uniform config
        _view = Camera.GetViewMatrix();
        _projection = Camera.GetProjectionMatrix();

        foreach (var obj in Objects)
        {
            // Draw objects
            obj.Render(Camera, _view, _projection);
        }

        // Render skybox
        if (RenderSetting.RenderSkybox && CubeMap != null)
        {
            CubeMap.RenderSkybox(_view, _projection);            
        }
    }
}
