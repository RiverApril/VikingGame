using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public struct Rectangle {

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rectangle(float X, float Y, float Width, float Height) {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        public bool contains(Vector2 v) {
            return v.X >= X && v.Y >= Y && v.X <= (X+Width) && v.Y <= (Y+Height);
        }

        public override string ToString() {
            return "(" + X + ", " + Y + ", " + Width + ", " + Height + ")";
        }

        public static Rectangle operator - (Rectangle r, Vector2 v) {
            return new Rectangle(r.X - v.X, r.Y - v.Y, r.Width, r.Height);
        }

        public static Rectangle operator + (Rectangle r, Vector2 v){
            return new Rectangle(r.X + v.X, r.Y + v.Y, r.Width, r.Height);
        }

        public Vector2 position { get { return new Vector2(X, Y); } }
        public Vector2 halfSize { get { return new Vector2(Width/2, Height/2); } }
    }
}
