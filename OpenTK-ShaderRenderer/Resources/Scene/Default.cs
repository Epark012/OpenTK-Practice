using OpenTK_Renderer.Rendering;
using OpenTK_Renderer.Rendering.Lighting;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Default : Scene
{
    private readonly Vector3[] _pointLightPositions =
    {
        new (0.7f, 0.2f, 2.0f),
        new (2.3f, -3.3f, -4.0f),
        new (-4.0f, 2.0f, -12.0f),
        new (0.0f, 0.0f, -3.0f)
    };
    
    public Default(RenderSetting renderSetting, Camera camera, CubeMap? cubeMap, Action<Scene>? onInitialized = null, params Object[] models) : base(renderSetting, camera, cubeMap, onInitialized, models)
    {
        RenderSetting = renderSetting;
        Camera = camera;
        CubeMap = cubeMap;
        Objects = models.ToList();
        
        OnInitialized = onInitialized;
           
        var directionalLight = new DirectionalLight(new Vector3(-0.2f, -1.0f, -0.3f),
            new Vector3(0.05f, 0.05f, 0.05f),
            new Vector3(0.4f, 0.4f, 0.4f),
            new Vector3(0.5f, 0.5f, 0.5f)
        );
        
        Lights.Add(directionalLight);
        
        // Initialize spot light
        var spotLight = new SpotLight(new Vector3(0.0f, 0.0f, 0.0f), 
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            1.0f,
            0.09f,
            0.032f,
            MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
            MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
        
        Lights.Add(spotLight);
        
        // Initialize point lights
        for (var i = 0; i < _pointLightPositions.Length; i++)
        {
            var pointLight = new PointLight(_pointLightPositions[i],
                new Vector3(0.05f, 0.05f, 0.05f),
                new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(1.0f, 1.0f, 1.0f),
                1.0f,
                0.09f,
                0.032f);
            
            Lights.Add(pointLight);
        }
        
        // Initialize fields
        Initialize();
    }
}
