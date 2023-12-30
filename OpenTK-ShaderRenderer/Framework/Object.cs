using OpenTK_Renderer.Rendering;
using OpenTK_Renderer.Rendering.Lighting;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Object
{
    public Model Model;
    public Shader Shader;
    private Material _material;
    private readonly Action<Object>? _onInitialized;
    private List<Light> _appliedLights = new ();

    public Object(Model model, Shader shader, Action<Object>? onInitialized = null)
    {
        Model = model;
        Shader = shader;

        _onInitialized = onInitialized;
    }
    // public void Initialize(DirectionalLight dirLight, SpotLight spotLight, PointLight[] pointLights)
    public void Initialize(List<Light> lights)
    {
            // Create shaders
            Shader.Initialize();

            // Set material
            _material = new Material(0, 1, 32.0f);
            SetMaterial(_material);

            var pointLights = new List<PointLight>();
            foreach (var light in lights)
            {
                switch (light)
                {
                    case DirectionalLight directionalLight:
                        SetDirLight(directionalLight);
                        break;
                    case SpotLight spotLight:
                        SetSpotLight(spotLight);
                        break;
                    case PointLight pointLight:
                        pointLights.Add(pointLight);            
                        break;
                }
            }
            
            // Point lights
            if (pointLights.Count > 0)
                SetPointLights(pointLights);

            _onInitialized?.Invoke(this);
    }

    private void SetMaterial(Material material)
    {
        Shader.SetUniform("material.diffuse", material.Diffuse);
        Shader.SetUniform("material.specular", material.Specular);
        Shader.SetUniform("material.shininess", material.Shininess);    
    }

    private void SetDirLight(DirectionalLight light)
    {
        Shader.SetUniform("dirLight.direction", light.Direction);
        Shader.SetUniform("dirLight.ambient", light.Ambient);
        Shader.SetUniform("dirLight.diffuse", light.Diffuse);
        Shader.SetUniform("dirLight.specular",light.Specular);
        
        _appliedLights.Add(light);
    }
    
    private void SetPointLights(List<PointLight> pointLights)
    {
        for (var i = 0; i < pointLights.Count; i++)
        {
            Shader.SetUniform($"pointLights[{i}].position", pointLights[i]);
            Shader.SetUniform($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            Shader.SetUniform($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            Shader.SetUniform($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
            Shader.SetUniform($"pointLights[{i}].constant", 1.0f);
            Shader.SetUniform($"pointLights[{i}].linear", 0.09f);
            Shader.SetUniform($"pointLights[{i}].quadratic", 0.032f);
            
            _appliedLights.Add(pointLights[i]);
        }
    }

    private void SetSpotLight(SpotLight spotLight)
    {
        Shader.SetUniform("spotLight.ambient",  spotLight.Ambient);
        Shader.SetUniform("spotLight.diffuse",  spotLight.Diffuse);
        Shader.SetUniform("spotLight.specular", spotLight.Specular);
        Shader.SetUniform("spotLight.constant", spotLight.Constant);
        Shader.SetUniform("spotLight.linear", spotLight.Linear);
        Shader.SetUniform("spotLight.quadratic", spotLight.Quadratic);
        Shader.SetUniform("spotLight.cutOff", spotLight.CutOff);
        Shader.SetUniform("spotLight.outerCutOff", spotLight.OuterCutOff);
        
        _appliedLights.Add(spotLight);
    }

    public void Update()
    {
        // Set runtime light uniforms
        foreach (var appliedLight in _appliedLights)
        {
            appliedLight.Update();
        }
    }
    
    public void Render(Camera camera, Matrix4 view, Matrix4 projection)
    {
        // TODO Set runtime lighting
        Shader.SetUniform("spotLight.position", camera.Position);
        Shader.SetUniform("spotLight.direction", camera.Front);
        
        // Render model
        Shader.Use();
        Shader.SetUniform<Matrix4>("view", view);
        Shader.SetUniform<Matrix4>("projection", projection);

        Shader.SetUniform("viewPos", camera);
        
        Model.Draw(Shader);
    }
}
