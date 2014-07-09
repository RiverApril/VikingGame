using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class ShaderSources {

        public static string vertexShader = @"
#version 120

//precision highp float;

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
        public static string fragmentShader = @"
#version 120

//precision highp float;

uniform sampler2D texture_diffuse;

varying vec4 pass_Color;
varying vec2 pass_TextureCoord;
varying vec3 pass_Normal;

//out vec4 gl_FragColor;

void main(void) {
    gl_FragColor =  (texture2D(texture_diffuse, pass_TextureCoord) * pass_Color);
}
";



    }
}
