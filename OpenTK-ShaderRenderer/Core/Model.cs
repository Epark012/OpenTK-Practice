using Assimp;
using Assimp.Configs;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Model : IDisposable
{
    public List<Mesh> Meshes = new ();
    private readonly Scene _raw;

    public Model(string path)
    {
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
                
                ProcessNode(_raw.RootNode, _raw);
            }
        }
        catch (AssimpException e)
        {
            throw new Exception("failed to read file: " + path + " (" + e.Message + ")");
        }
    }

    private void ProcessNode(Node node, Scene scene)
    {
        for (int i = 0; i < node.MeshCount; i++)
        {
            var mesh = scene.Meshes[i];
            Meshes.Add(ProcessMesh(mesh, scene));
        }

        for (int i = 0; i < node.ChildCount; i++)
        {
            ProcessNode(node.Children[i], scene);
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
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
            
            vertice.Add(vertex);
        }
        
        var indices = new List<uint>();
        for (int i = 0; i < mesh.FaceCount; i++)
        {
            var face = mesh.Faces[i];
            for (int j = 0; j < face.IndexCount; j++)
            {
                indices.Add((uint)face.Indices[j]);
            }
        }

        var result = new Mesh(vertice.ToArray(), indices.ToArray(), Array.Empty<Texture>()); 
        return result;
    }

    public void Draw(Shader shader)
    {
        foreach (var mesh in Meshes)
        {
            mesh.Draw(shader);
        }
    }

    public void Dispose()
    {
        // TODO need to dispose all datas
        Meshes.Clear();
        GC.SuppressFinalize(this);
    }
}
