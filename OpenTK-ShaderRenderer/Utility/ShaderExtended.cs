using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTK_Renderer
{ 
    public static class ShaderExtended
    {

        /// <summary>
        /// Extension method to set uniform
        /// </summary>
        /// <typeparam name="T">Type of uniform to set</typeparam>
        /// <param name="shader">Shader to set a uniform value</param>
        /// <param name="name">Uniform name</param>
        /// <param name="value">Value to set uniform</param>
        public static void SetUniform<T>(this Shader shader, string name, T value)
        {
            var id = shader.ID;
            GL.UseProgram(id);
            var location = GL.GetUniformLocation(id, name);

            switch (value)
            {
                case int _int:
                    GL.Uniform1(location, _int);
                    break;
                case float _float:
                    GL.Uniform1(location, _float);
                    break;
                case Vector3 vec3:
                    GL.Uniform3(location, ref vec3);
                    break;

                case Matrix3 mat3:
                    GL.UniformMatrix3(location, true, ref mat3);
                    break;

                case Matrix4 mat4:
                    GL.UniformMatrix4(location, true, ref mat4);
                    break;
            }
        }
    }
}
