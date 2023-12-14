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
}
