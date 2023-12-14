using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace OpenTK_Renderer
{
    public class Shader : IDisposable
    {
        public int ID { get; internal set; }

        private Dictionary<string, int> _uniformLocation;

        public Shader(string vertexPath, string fragPath)
        {
            // Create id for shader program
            ID = GL.CreateProgram();

            string source;

            // Vertex 
            source = LoadSource(vertexPath);
            var vertex = CreateShader(source, ShaderType.VertexShader);
            GL.AttachShader(ID, vertex);

            // Fragment
            source = LoadSource(fragPath);
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
            GL.UseProgram(ID);
        }

        /// <summary>
        /// Load shader source at path
        /// </summary>
        /// <param name="path">Shader path</param>
        /// <returns>Shader source</returns>
        public static string LoadSource(string path)
        {
            var source = String.Empty;

            try
            {
                using (var reader = new StreamReader(path, Encoding.UTF8))
                {
                    source = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                throw new Exception($"Failed to load a shader source at {path}");
            }

            return source;
        }

        public void Dispose()
        {
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
