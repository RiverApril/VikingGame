using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class Player : Entity {

        private float moveSpeed = .1f;

        private float swingAngle = 0;

        private float swingDistance = .5f;
        private bool swingVisable;

        public Player(Game game) : base(game){

        }

        protected override void init() {

            position = new Vector3(0, 0, 0);
            halfSize = new Vector3(.2f, .2f, .2f);

        }

        public override void addRenderable(ref List<Object3<Vector3, Renderable, int>> renderList) {

            renderList.Add(new Object3<Vector3, Renderable, int>(position, this, 0));

            if (swingVisable) {
                Vector3 s = new Vector3(
                 position.X + (float)(Math.Sin(MathCustom.r180 - swingAngle - game.camera.rotation.Y) * swingDistance),
                 position.Y,
                 position.Z + (float)(Math.Cos(MathCustom.r180 - swingAngle - game.camera.rotation.Y) * swingDistance));

                renderList.Add(new Object3<Vector3, Renderable, int>(s, this, 1));
            }

        }

        public override void render(Game game, World world, Camera camera, Vector3 position, int meta) {

            if (meta == 0) {
                world.prepareModelMatrix(camera, position);
                FrameManager.playerDefault.render(game);
            }
            if (meta == 1 && swingVisable) {
                world.prepareModelMatrix(camera, position, -swingAngle);
                FrameManager.swing1.render(game);
            }
        }

        public override void update(Game game, World world) {

            Vector2 dif = new Vector2();

            if (game.inputControl.keyMoveForward.down) {
                dif.X += (float)Math.Sin(-game.camera.rotation.Y + MathCustom.r180);
                dif.Y += (float)Math.Cos(-game.camera.rotation.Y + MathCustom.r180);
            }
            if (game.inputControl.keyMoveBackward.down) {
                dif.X += (float)Math.Sin(-game.camera.rotation.Y);
                dif.Y += (float)Math.Cos(-game.camera.rotation.Y);
            }
            if (game.inputControl.keyMoveLeft.down) {
                dif.X += (float)Math.Sin(-game.camera.rotation.Y + MathCustom.r270);
                dif.Y += (float)Math.Cos(-game.camera.rotation.Y + MathCustom.r270);
            }
            if (game.inputControl.keyMoveRight.down) {
                dif.X += (float)Math.Sin(-game.camera.rotation.Y + MathCustom.r90);
                dif.Y += (float)Math.Cos(-game.camera.rotation.Y + MathCustom.r90);
            }


            if(dif != Vector2.Zero){
                dif.Normalize();
                move(world, dif * moveSpeed);
            }

            if (game.inputControl.keyAttack.pressed) {
                //world.entityAddList.Add(new EntityShot(game, position, this, MathCustom.r180-game.camera.rotation.Y));
                swingAngle = -MathCustom.r45;
                swingVisable = true;
            }

            if (swingVisable) {
                swingAngle += MathCustom.r5;
                if (swingAngle > MathCustom.r45) {
                    swingVisable = false;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Key.T)) {
                world.entityAddList.Add(new EntityMonster(game, position));
            }

            game.camera.position = -position;
        }

    }
}
