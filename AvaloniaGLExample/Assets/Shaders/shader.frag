#version 330

uniform sampler2D texture0;
uniform sampler2D texture1;

in vec3 vertColor;
in vec2 texCoord;

out vec4 fragColor;

void main()
{
    // texture0 is correct. texture1 is all black, but should be awesomeface.png
    fragColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.5);
}