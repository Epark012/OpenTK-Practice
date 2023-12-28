using System;
using OpenTK.Mathematics;

namespace OpenTK_Renderer;

public static class Random
{
    private static readonly System.Random Handler = new ();

    /// <summary>
    /// Create vector3 with random values in x, y, z
    /// </summary>
    /// <param name="min">Min value</param>
    /// <param name="max">Max value</param>
    /// <returns></returns>
    public static Vector3 GenerateRandomVector3(float min, float max)
    {
        var x = (float)Handler.NextDouble() * (max - min) + min;
        var y = (float)Handler.NextDouble() * (max - min) + min;
        var z = (float)Handler.NextDouble() * (max - min) + min;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Create vector3 with random values in x, y, z
    /// </summary>
    /// <param name="value">Value for min and max</param>
    /// <returns></returns>
    public static Vector3 GenerateRandomVector3(float value) => GenerateRandomVector3(-value, value);

    /// <summary>
    /// Create random value 
    /// </summary>
    /// <returns></returns>
    public static float GenerateRandomFloat(float min, float max)
    {
        return (float)Handler.NextDouble() * (max - min) + min;
    }

    /// <summary>
    /// Create random vector3 for axis
    /// </summary>
    /// <returns></returns>
    public static Vector3 GenerateRandomAxis()
    {
        return new Vector3(
            GenerateRandomFloat(0,1),
            GenerateRandomFloat(0,1),
            GenerateRandomFloat(0,1)
        );
    }
}
