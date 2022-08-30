#version 460 core
layout (location = 0) in vec3 a_pos;
layout (location = 1) in vec4 a_color;

out vec4 v_color;
  
uniform mat4 proj;
uniform mat4 model;
uniform mat4 view;

void main()
{
    v_color = a_color;
    gl_Position = vec4(a_pos, 1.0f) * model * view * proj;
}
