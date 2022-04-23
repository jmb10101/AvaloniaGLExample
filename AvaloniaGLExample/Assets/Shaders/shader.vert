#version 330 core

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inColor;
layout(location = 2) in vec2 inTexCoord;

out vec3 vertColor;
out vec2 texCoord;

void main(void)
{
    vertColor = inColor;
    texCoord = inTexCoord;
    gl_Position = vec4(inPosition, 1.0) * model * view * projection;
}