#version 330

struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float shininess;
};

struct Light
{
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 viewPos;

in vec3 Normal;
in vec3 FragPos;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    
    // Ambient
    vec3 ambient = material.ambient * light.ambient;
    
    // Diffuse
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = (light.diffuse * material.diffuse) * lightColor;
    
    // Specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = (spec * material.specular) * light.specular;
    
    // Mix with textures
    vec3 result = (ambient + diffuse + specular);
    vec4 texture = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.5);
    
    outputColor = mix(texture, vec4(result, 1.0), 0.5);
    return;
}