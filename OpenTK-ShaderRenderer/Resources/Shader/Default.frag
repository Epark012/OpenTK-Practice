﻿#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    vec4 texture = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.5);
    outputColor = mix(texture, vec4(1.0, 1.0, 0.0, 1.0), 0.1);
    // outputColor = vec4(1.0, 1.0, 0.0, 1.0);
}