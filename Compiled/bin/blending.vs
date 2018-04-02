#version 110

varying vec3 v_OrigitnalPosition;

void main(void) 
{
	gl_Position = vec4( gl_Vertex.x * 2.0 - 1.0, 1.0 - gl_Vertex.y * 2.0, 0.0, 1.0);

	v_OrigitnalPosition = gl_Normal.xyz;

	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_Vertex;//vec4(gl_Vertex.xy, gl_MultiTexCoord0.x, 0.0);
}