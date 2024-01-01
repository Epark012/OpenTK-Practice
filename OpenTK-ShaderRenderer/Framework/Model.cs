using Assimp;
using Assimp.Configs;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Model : IDisposable
{
    private readonly List<Mesh> _meshes = new ();
    
    private readonly Assimp.Scene _raw;
    private List<TextureInfo> _texturesLoaded = new List<TextureInfo>();
    private readonly string _directory;
    public Matrix4 ModelMatrix = Matrix4.Identity;

    private string _defaultTexture;
    
    public Model(string path, string? customTexture = null)
    {
        _defaultTexture = string.IsNullOrEmpty(customTexture) ? "Resources/Image/container.png" : customTexture;
        
        // Load model
        try
        {
            using (var context = new AssimpContext())
            {
                context.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                _raw = context.ImportFile(path, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.Triangulate);
                if (_raw == null)
                {
                    Dispose();
                    throw new Exception($"Failed to read file : {path}");
                }
                
                var index = path.LastIndexOf("/");
                _directory = path.Substring(0, index);

                ProcessNode(_raw.RootNode, _raw);
            }
        }
        catch (AssimpException e)
        {
            throw new Exception("failed to read file: " + path + " (" + e.Message + ")");
        }

        ModelMatrix = ImportUtility.FromMatrix4dto4(_raw.RootNode.Transform);
    }

    private void ProcessNode(Node node, Assimp.Scene scene)
    {
        for (int i = 0; i < node.MeshCount; i++)
        {
            var mesh = scene.Meshes[i];
            _meshes.Add(ProcessMesh(mesh, scene));
        }

        for (int i = 0; i < node.ChildCount; i++)
        {
            ProcessNode(node.Children[i], scene);
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene)
    {
        var vertice = new List<Vertex>();
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            var vertex = new Vertex();
            
            // Position
            vertex.Position = ImportUtility.FromVector3dto3(mesh.Vertices[i]);
            
            // Normals
            vertex.Normal = ImportUtility.FromVector3dto3(mesh.Normals[i]);
            
            // TextCoords
            vertex.TexCoord = mesh.HasTextureCoords(0)
                ? ImportUtility.FromVector3dto2(mesh.TextureCoordinateChannels[0][i])
                : Vector2.Zero;
            
            // Tangents
            vertex.Tangent = ImportUtility.FromVector3dto3(mesh.Tangents[i]);
            
            // BiTangent
            vertex.Bitangent = ImportUtility.FromVector3dto3(mesh.BiTangents[i]);
            vertice.Add(vertex);
        }
        
        var indices = new List<uint>();
        for (var i = 0; i < mesh.FaceCount; i++)
        {
            var face = mesh.Faces[i];
            for (int j = 0; j < face.IndexCount; j++)
            {
                indices.Add((uint)face.Indices[j]);
            }
        }

        var textures = new List<TextureInfo>();
        var material = scene.Materials[mesh.MaterialIndex];
            
        // Diffuse maps
        var diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
        textures.AddRange(diffuseMaps);
            
        // specular maps
        var specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
        textures.AddRange(specularMaps);
            
        // normal maps
        var normalMaps = LoadMaterialTextures(material, TextureType.Height, "texture_normal");
        textures.AddRange(normalMaps);
            
        // height maps
        var heightMaps = LoadMaterialTextures(material, TextureType.Ambient, "texture_height");
        textures.AddRange(heightMaps);
        
        // Use default texture if texture is null
        if (_texturesLoaded.Count < 1)
        {
            var t = new Texture(_defaultTexture);
            TextureInfo texture;
            texture.Id = (uint)t.ID;
            texture.Type = "texture_diffuse";
            texture.Path = "Resources/Image/container.png";

            Console.WriteLine("Default texture loaded");
            textures.Add(texture);
        }
        
        return new Mesh(vertice.ToArray(), indices.ToArray(), textures.ToArray());
    }

    /// <summary>
    /// Draw this model at Model matrix;
    /// </summary>
    /// <param name="shader">Shader program for this model</param>
    public void Draw(Shader shader) => Draw(shader, ModelMatrix);

    /// <summary>
    /// Draw this model at 
    /// </summary>
    /// <param name="shader">Shader program for this model</param>
    /// <param name="model">Model Matrix to transform this model</param>
    public void Draw(Shader shader, Matrix4 model)
    {
        shader.SetUniform("model", model);
        
        foreach (var mesh in _meshes)
        {
            mesh.Draw(shader);
        }
    }
    
    private IEnumerable<TextureInfo> LoadMaterialTextures(Material mat, TextureType type, string typeName)
    {
        var textures = new List<TextureInfo>();
        for (var i = 0; i < mat.GetMaterialTextureCount(type); i++)
        {
            TextureSlot str;
            mat.GetMaterialTexture(type, i, out str);
            // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
            var skip = false;
            for (int j = 0; j < _texturesLoaded.Count; j++)
            {
                if (_texturesLoaded[j].Path == str.FilePath)
                {
                    textures.Add(_texturesLoaded[j]);
                    skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                    break;
                }
            }
            if (!skip)
            {   // if texture hasn't been loaded already, load it
                Console.WriteLine(str.TextureType + " -- " + str.FilePath + " is loaded");
                TextureInfo texture;
                texture.Id = TextureFromFile(str.FilePath, _directory);
                texture.Type = typeName;
                texture.Path = str.FilePath;
                textures.Add(texture);
                
                _texturesLoaded.Add(texture);
            }
        }

        return textures;
    }
    
    private uint TextureFromFile(string path, string directory)
    {
        var tPath = System.IO.Path.Combine(directory, path);
        var t = new Texture(tPath);

        return (uint)t.ID;
    }

    /// <summary>
    /// Scale model matrix 
    /// </summary>
    /// <param name="value">Scale value</param>
    public void Scale(float value)
    {
        ModelMatrix *= Matrix4.CreateScale(value);
    }

    /// <summary>
    /// Translate model matrix
    /// </summary>
    /// <param name="position">Translation value</param>
    public void Translate(Vector3 position)
    {
        ModelMatrix *= Matrix4.CreateTranslation(position);
    }

    /// <summary>
    /// Rotate model matrix
    /// </summary>
    /// <param name="axis">Axis to rotate</param>
    /// <param name="angle">Angle to rotate</param>
    public void Rotate(Vector3 axis, float angle)
    {
        ModelMatrix *= Matrix4.CreateFromAxisAngle(axis, angle);
    }

    public void Dispose()
    {
        // TODO need to dispose all datas
        _meshes.Clear();
        GC.SuppressFinalize(this);
    }
}
