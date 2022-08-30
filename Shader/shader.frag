#version 460 core

out vec4 frag_color;
in  vec4 v_color;
      
void main()
{
    frag_color = v_color; 
}
