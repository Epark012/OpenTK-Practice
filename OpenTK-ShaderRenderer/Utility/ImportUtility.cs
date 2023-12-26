using Assimp;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public static class ImportUtility
{
    public static Vector3 FromVector3dto3(Vector3D vector3d)
    {
        return new Vector3(vector3d.X, vector3d.Y, vector3d.Z);
    }

    public static Vector2 FromVector3dto2(Vector3D vector3D)
    {
        return new Vector2(vector3D.X, vector3D.Y);
    }

    public static Matrix4 FromMatrix4dto4(Matrix4x4 matrix4)
    {
        return new Matrix4(new Vector4(matrix4.A1, matrix4.A2, matrix4.A3, matrix4.A4),
            new Vector4(matrix4.B1, matrix4.B2, matrix4.B3, matrix4.B4),
            new Vector4(matrix4.C1, matrix4.C2, matrix4.C3, matrix4.C4),
            new Vector4(matrix4.D1, matrix4.D2, matrix4.D3, matrix4.D4));
    }
}
