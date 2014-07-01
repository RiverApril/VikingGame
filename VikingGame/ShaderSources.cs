using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class ShaderSources {

        public static string vertexShader = @"
#version 130

precision highp float;

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
        public static string fragmentShader = @"
#version 130

precision highp float;

uniform sampler2D texture_diffuse;

in vec4 pass_Color;
in vec2 pass_TextureCoord;
in vec3 pass_Normal;

void main(void) {
    gl_FragColor =  (texture(texture_diffuse, pass_TextureCoord) * pass_Color);
}
";



    }
}
