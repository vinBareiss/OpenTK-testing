#version 450
in vec3 position;
in vec4 color;

uniform mat4 transform;

out vec4 Color;

void main(){ 
    gl_Position = transform * vec4(position, 1.0);
    Color = color;    
}
