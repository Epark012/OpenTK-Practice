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

    protected Action<Scene>? OnInitialized;
    protected Action<Scene>? OnUpdate; 
    protected Action<Scene>? OnRender; 

    // Light
    protected readonly List<Light> Lights = new ();
    
    public List<DirectionalLight> DirectionalLight = new();
    
    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;

    protected Scene(RenderSetting renderSetting, Camera camera, CubeMap? cubeMap = null, Action<Scene>? onInitialized = null, params Object[] models)
    {
        RenderSetting = renderSetting;
        Camera = camera;
        CubeMap = cubeMap;
        Objects = models.ToList();
        
        OnInitialized = onInitialized;
    }

    protected virtual void Initialize()
    {
        if (Lights.Count < 1)
        {
            Console.WriteLine("No lights applied in the scene");
        }
        
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

        foreach (var light in Lights)
        {
            if (light is not DirectionalLight directionalLight)
            {
                continue;
            }
            
            DirectionalLight.Add(directionalLight);
        }
        
        OnInitialized?.Invoke(this);
    }
    
    public virtual void Update()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        GL.Enable(EnableCap.DepthTest);
        
        foreach (var obj in Objects)
        {
            obj.Update();
        }
        
        OnUpdate?.Invoke(this);
    }
    
    /// <summary>
    /// Render this scene
    /// </summary>
    public virtual void Render()
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
        
        OnRender?.Invoke(this);
    }
}
