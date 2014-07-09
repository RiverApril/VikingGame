using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VikingGame {
    public class Camera {

        public Vector3 position;
        public Vector3 rotation;

        public Matrix4 inverseRotation;

        public Matrix4 modelMatrix;
        private Matrix4 flatMatrix;

        public float scale = 50;

        private float yRotSpeed = MathCustom.r1 * 1.5f;

        public Camera() {
            reset();
        }

        public void reset() {
            position = new Vector3(0, -6, -6);
            rotation = new Vector3(MathCustom.r45, MathCustom.r45, -MathCustom.r45);
        }

        internal void update(Game game) {

            /*if(ks.IsKeyDown(Key.Space)){
                position.Y -= moveSpeed;
            }
            if (ks.IsKeyDown(Key.LShift)) {
                position.Y += moveSpeed;
            }*/
            if (game.inputControl.keyRotateLeft.down) {
                rotation.Y -= yRotSpeed;
                if (rotation.Y < 0) {
                    rotation.Y += MathCustom.r360;
                }
            }
            if (game.inputControl.keyRotateRight.down) {
                rotation.Y += yRotSpeed;
                if (rotation.Y > MathCustom.r360) {
                    rotation.Y -= MathCustom.r360;
                }
            }

            if (game.inputControl.keyRotateUp.down) {
                rotation.X += MathCustom.r1;
                if(rotation.X > MathCustom.r89){
                    rotation.X = MathCustom.r89;
                }
            }
            if (game.inputControl.keyRotateDown.down) {
                rotation.X -= MathCustom.r1;
                if (rotation.X < 0) {
                    rotation.X = 0;
                }
            }

            //Console.WriteLine(position);
            //Console.WriteLine(rotation);
        }

        internal Matrix4 getModelMatrix() {
            modelMatrix = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            modelMatrix *= Matrix4.CreateScale(scale);
            modelMatrix *= Matrix4.CreateTranslation(position * scale);
            modelMatrix *= Matrix4.CreateRotationY(rotation.Y);
            modelMatrix *= Matrix4.CreateRotationX(rotation.X);
            return modelMatrix;
        }

        internal Matrix4 getFlatMatrix() {
            return getFlatMatrix(Vector3.Zero);
        }

        internal Matrix4 getFlatMatrix(Vector3 offset, float angle = 0) {

            Vector3 p = position + offset;

            double l = p.Length;

            double a = Math.Atan2(p.Z, p.X);
            a += -rotation.Y - MathCustom.r90;

            p.X = -(float)(Math.Sin(a) * l);
            p.Z = (float)(Math.Cos(a) * l);

            flatMatrix = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            flatMatrix *= Matrix4.CreateRotationY(angle);
            flatMatrix *= Matrix4.CreateRotationX(MathCustom.r90-rotation.X);
            flatMatrix *= Matrix4.CreateScale(scale);
            flatMatrix *= Matrix4.CreateTranslation(p * scale);
            flatMatrix *= Matrix4.CreateRotationX(rotation.X);
            return flatMatrix;
        }
    }
}
