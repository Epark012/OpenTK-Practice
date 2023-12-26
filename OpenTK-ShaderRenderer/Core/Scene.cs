using OpenTK_Renderer.Rendering;
using OpenTK_Renderer.Rendering.Lighting;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

/// <summary>
/// Scene to render in project
/// </summary>
public class Scene
{
    public Camera Camera { get; private set; }
    private readonly RenderSetting _renderSetting;
    private CubeMap? _cubeMap;
    private List<Object> _objects;

    private DirectionalLight _directionalLight;
    private PointLight[] _pointLights;
    
    private readonly Vector3[] _pointLightPositions =
    {
        new (0.7f, 0.2f, 2.0f),
        new (2.3f, -3.3f, -4.0f),
        new (-4.0f, 2.0f, -12.0f),
        new (0.0f, 0.0f, -3.0f)
    };
    
    #region Cache

    private Matrix4 _view = Matrix4.Identity;
    private Matrix4 _projection = Matrix4.Identity;
    private readonly Action<Scene> _onInitialized;

    #endregion
    
    public Scene(RenderSetting renderSetting, Camera camera, CubeMap? cubeMap, Action<Scene>? onInitialized = null, params Object[] models)
    {
        _renderSetting = renderSetting;
        Camera = camera;
        _cubeMap = cubeMap;
        _objects = models.ToList();
        _onInitialized = onInitialized;

        _pointLights = new PointLight[_pointLightPositions.Length];
    }

    /// <summary>
    /// Initialize this scene
    /// </summary>
    public void Initialize()
    {
        // Initialize directional lights
        _directionalLight = new DirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f),
            new Vector3(0.05f, 0.05f, 0.05f),
            new Vector3(0.4f, 0.4f, 0.4f),
            new Vector3(0.5f, 0.5f, 0.5f)
        );
        
        // Initialize point lights
        for (var i = 0; i < _pointLightPositions.Length; i++)
        {
            _pointLights[i] = new PointLight(_pointLightPositions[i],
                new Vector3(0.05f, 0.05f, 0.05f),
                new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(1.0f, 1.0f, 1.0f),
                1.0f,
                0.09f,
                0.032f);
        }
        
        // Initialize spot light
        var spotLight = new SpotLight(new Vector3(0.0f, 0.0f, 0.0f), 
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            1.0f,
            0.09f,
            0.032f,
            MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
            MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
        
        // Initialize objects
        foreach (var obj in _objects)
        {
            obj.Initialize( _directionalLight, spotLight, _pointLights);
        }

        // Initialize cube map
        if (_renderSetting.RenderSkybox)
        {
            _cubeMap ??= new CubeMap(_renderSetting.DefaultCubeMapPath);
        }
        else
        {
            GL.ClearColor(_renderSetting.DefaultBackGroundColor);
        }
        
        _onInitialized?.Invoke(this);
    }

    /// <summary>
    /// Update this scene
    /// </summary>
    public void Update()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

        foreach (var obj in _objects)
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

        foreach (var obj in _objects)
        {
            // Draw objects
            obj.Render(Camera, _view, _projection);
        }

        // Render skybox
        if (_renderSetting.RenderSkybox && _cubeMap != null)
        {
            _cubeMap.RenderSkybox(_view, _projection);            
        }
    }
}
