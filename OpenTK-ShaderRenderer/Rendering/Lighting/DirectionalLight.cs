﻿using OpenTK.Mathematics;

namespace OpenTK_Renderer.Rendering.Lighting;

public class DirectionalLight : Light
{
    public Vector3 Direction;
    public Vector3 Ambient;
    public Vector3 Diffuse;
    public Vector3 Specular;

    public DirectionalLight(Vector3 direction, Vector3 ambient, Vector3 diffuse, Vector3 specular)
    {
        Direction = direction;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
    }
    public override void Update()
    {
        // Update for dynamic lighting
    }
}