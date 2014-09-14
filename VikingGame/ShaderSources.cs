using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class ShaderSources {

        public static string vertexShader120 = @"
#version 120

uniform mat4 modelMatrix;
uniform mat4 perspectiveMatrix;

attribute vec3 in_Position;
attribute vec4 in_Color;
attribute vec2 in_TextureCoord;
attribute vec3 in_Normal;

varying vec4 pass_Color;
varying vec2 pass_TextureCoord;
varying vec3 pass_Normal;

void main(void) {
    gl_Position = perspectiveMatrix * modelMatrix * vec4(in_Position, 1.0);
	
	pass_Color = in_Color;
	pass_TextureCoord = in_TextureCoord;
	pass_Normal = in_Normal;
}
"; 
        public static string fragmentShader120 = @"
#version 120

uniform sampler2D texture_diffuse;

varying vec4 pass_Color;
varying vec2 pass_TextureCoord;
varying vec3 pass_Normal;

void main(void) {
    gl_FragColor =  (texture2D(texture_diffuse, pass_TextureCoord) * pass_Color);
}
";





        public static string vertexShader330 = @"
#version 330

in vec3 in_Position;
in vec4 in_Color;
in vec2 in_TextureCoord;
in vec3 in_Normal;

uniform mat4 modelMatrix;
uniform mat4 perspectiveMatrix;

out vec4 pass_Color;
out vec2 pass_TextureCoord;
out vec3 pass_Normal;

void main(void) {
    gl_Position = perspectiveMatrix * modelMatrix * vec4(in_Position, 1.0);
	
	pass_Color = in_Color;
	pass_TextureCoord = in_TextureCoord;
	pass_Normal = in_Normal;
}
";
        public static string fragmentShader330 = @"
#version 330

uniform sampler2D texture_diffuse;

in vec4 pass_Color;
in vec2 pass_TextureCoord;
in vec3 pass_Normal;

out vec4 outColor;

void main(void) {
    outColor =  (texture(texture_diffuse, pass_TextureCoord) * pass_Color);
}
";



    }
}
