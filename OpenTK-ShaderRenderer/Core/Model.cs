using Assimp;
using Assimp.Configs;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public class Model : IDisposable
{
    public List<Mesh> Meshes = new ();

    public Model(string path)
    {
        // Load model
        try
        {
            using (var context = new AssimpContext())
            {
                context.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                
                var logstream = new LogStream(delegate(String msg, String userData) {
                    Console.WriteLine(msg);
                });
                logstream.Attach();

                //Import the model. All configs are set. The model
                //is imported, loaded into managed memory. Then the unmanaged memory is released, and everything is reset.
                var scene = context.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
                if (scene == null)
                {
                    Dispose();
                    throw new Exception($"Failed to read file : {path}");
                }
                
                //TODO: Load the model data into your own structures
                ProcessNode(scene.RootNode, scene);
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

        return new Mesh(vertice, indices, null);
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
