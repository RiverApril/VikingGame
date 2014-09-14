using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class MathCustom {

        public static float r360 = MathHelper.DegreesToRadians(360);
        public static float r270 = MathHelper.DegreesToRadians(270);
        public static float r180 = MathHelper.DegreesToRadians(180);
        public static float r90 = MathHelper.DegreesToRadians(90);
        public static float r89 = MathHelper.DegreesToRadians(89);
        public static float r45 = MathHelper.DegreesToRadians(45);
        public static float r5 = MathHelper.DegreesToRadians(5);
        public static float r1 = MathHelper.DegreesToRadians(1);

        public static float one64 = 1f/64f;
        public static float one512 = 1f / 512f;

        public static float sin45Times100 = (float)(Math.Sin(MathCustom.r45) * 100);

        public static Vector3 vect3PointFive = new Vector3(.5f, .5f, .5f);

        internal static T[] toArray<T>(T a) {
            return new T[] { a };
        }

        internal static double min(int a, int b) {
            return a < b ? a : b;
        }

        internal static double max(int a, int b) {
            return a > b ? a : b;
        }

        internal static Vector2 toVector2(System.Drawing.Point point) {
            return new Vector2(point.X, point.Y);
        }

        internal static float bound(float a, float minA, float maxA) {
            return a < minA ? minA : (a > maxA ? maxA : a);
        }
    }
}
