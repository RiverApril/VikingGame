using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {


    public class Vector4Color {

        public static Vector4 white = new Vector4(1, 1, 1, 1);
        public static Vector4 black = new Vector4(0, 0, 0, 1);
        public static Vector4 red = new Vector4(1, 0, 0, 1);
        public static Vector4 green = new Vector4(0, 1, 0, 1);
        public static Vector4 blue = new Vector4(0, 0, 1, 1);


        internal static Vector4 gray(float level) {
            return new Vector4(level, level, level, 1);
        }
    }
}
