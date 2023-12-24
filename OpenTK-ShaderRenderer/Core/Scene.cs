using OpenTK_Renderer.Rendering;
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
    private List<Object> _objects = new ();

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

    #endregion
    
    public Scene(RenderSetting renderSetting, Camera camera, CubeMap? cubeMap, params Object[] models)
    {
        _renderSetting = renderSetting;
        Camera = camera;
        _cubeMap = cubeMap;
        _objects = models.ToList();
    }

    /// <summary>
    /// Initialize this scene
    /// </summary>
    public void Initialize()
    {
        // Initialize cube map
        if (_renderSetting.RenderSkybox)
        {
            _cubeMap ??= new CubeMap(_renderSetting.DefaultCubeMapPath);
        }
        else
        {
            GL.ClearColor(_renderSetting.DefaultBackGroundColor);
        }
        
        // Initialize objects
        foreach (var obj in _objects)
        {
            obj.Initialize(_pointLightPositions);
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
            obj.Render(Camera, _view, _projection);
        }

        // Render skybox
        if (_renderSetting.RenderSkybox && _cubeMap != null)
        {
            _cubeMap.RenderSkybox(_view, _projection);            
        }
    }
}
