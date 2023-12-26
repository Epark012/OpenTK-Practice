using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace OpenTK_Renderer
{
    public class Shader : IDisposable
    {
        public int ID { get; internal set; }

        private Dictionary<string, int> _uniformLocation;
        private bool _isInitialized;
        
        private readonly string _vertexPath;
        private readonly string _fragmentPath;
        
        // TODO: Make exception for shader, or log library
        public Shader(string vertexPath, string fragPath)
        {
            _vertexPath = vertexPath;
            _fragmentPath = fragPath;
        }
        
        public void Initialize()
        {
            // Create id for shader program
            ID = GL.CreateProgram();

            // Vertex
            var source = LoadSource(_vertexPath);
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception($"Failed to load shader source : {_vertexPath}");
            }
            
            var vertex = CreateShader(source, ShaderType.VertexShader);
            GL.AttachShader(ID, vertex);

            // Fragment
            source = LoadSource(_fragmentPath);
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception($"Failed to load shader source : {_fragmentPath}");
            }
            
            var fragment = CreateShader(source, ShaderType.FragmentShader);
            GL.AttachShader(ID, fragment);

            // Link
            GL.LinkProgram(ID);
            GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out var success);
            if (success == -1)
            {
                GL.GetProgramInfoLog(ID, out var log);
                throw new Exception($"Failed to link shader : {log}");
            }

            _isInitialized = true;

            // Clear
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        private int CreateShader(string source, ShaderType shaderType)
        {
            var shaderId = GL.CreateShader(shaderType);

            GL.ShaderSource(shaderId, source);
            GL.CompileShader(shaderId);

            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var success);
            if (success == -1)
            {
                GL.GetShaderInfoLog(shaderId, out var log);
                Dispose();
                throw new InvalidDataException(log);
            }

            return shaderId;
        }

        private void GetUniforms()
        {
            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out var activeUniformCount);
            _uniformLocation = new Dictionary<string, int>();
            for (int i = 0; i < activeUniformCount; i++)
            {
                var key = GL.GetActiveUniform(ID, i, out _, out _);
                var location = GL.GetUniformLocation(ID, key);
                _uniformLocation.Add(key, location);
            }
        }

        public void Use()
        {
            if (!_isInitialized)
            {
                Console.WriteLine($"Shader {ID} is not initialized, wrong access to use this shader");
            }
            
            GL.UseProgram(ID);
        }

        /// <summary>
        /// Load shader source at path
        /// </summary>
        /// <param name="path">Shader path</param>
        /// <returns>Shader source</returns>
        private static string LoadSource(string path)
        {
            var source = String.Empty;

            try
            {
                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    source = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to load a shader source at {path}\n {e.Message}");
            }

            return source;
        }

        public void Dispose()
        {
            _isInitialized = false;
            GL.DeleteProgram(ID);
            GC.SuppressFinalize(this);
        }

        internal int GetAttribLocation(int id, string attribName)
        {
            return GL.GetAttribLocation(id, attribName);
        }

        internal int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(ID, attribName);
        }
    }
}
