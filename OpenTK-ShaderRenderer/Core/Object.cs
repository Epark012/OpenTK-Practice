using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Object
{
    private Model _model;
    private Shader _shader;

    public Object(Model model, Shader shader)
    {
        _model = model;
        _shader = shader;
    }

    public void Initialize(Vector3[] pointLights)
    {
            _shader.SetUniform("material.diffuse", 0);
            _shader.SetUniform("material.specular", 1);
            _shader.SetUniform("material.shininess", 32.0f);
                
            // Directional light
            _shader.SetUniform("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _shader.SetUniform("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _shader.SetUniform("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            _shader.SetUniform("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));
            
            // Point lights
            for (var i = 0; i < pointLights.Length; i++)
            {
                _shader.SetUniform($"pointLights[{i}].position", pointLights[i]);
                _shader.SetUniform($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                _shader.SetUniform($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                _shader.SetUniform($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                _shader.SetUniform($"pointLights[{i}].constant", 1.0f);
                _shader.SetUniform($"pointLights[{i}].linear", 0.09f);
                _shader.SetUniform($"pointLights[{i}].quadratic", 0.032f);
            }
            
            // Spot light
            _shader.SetUniform("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
            _shader.SetUniform("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetUniform("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetUniform("spotLight.constant", 1.0f);
            _shader.SetUniform("spotLight.linear", 0.09f);
            _shader.SetUniform("spotLight.quadratic", 0.032f);
            _shader.SetUniform("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            _shader.SetUniform("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
    }

    public void Render(Camera camera, Matrix4 view, Matrix4 projection)
    {
        // TODO Set runtime lighting
        _shader.SetUniform("spotLight.position", camera.Position);
        _shader.SetUniform("spotLight.direction", camera.Front);
        
        // Render model
        _shader.Use();
        _shader.SetUniform<Matrix4>("view", view);
        _shader.SetUniform<Matrix4>("projection", projection);

        _shader.SetUniform("viewPos", camera);
        
        _model.Draw(_shader);
    }
}
