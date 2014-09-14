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
        private float accSpeed = .02f;

        private float swingAngle = 0;

        private float swingDistance = .5f;
        private bool swingVisable;

        private string username;

        public Player(string username, int entityId) : base(entityId){
            this.username = username;
        }

        protected override void init() {

            position = new Vector3(0, 0, 0);
            halfSize = new Vector3(.2f, .2f, .2f);

        }

        public override void addRenderable(ref List<Object3<Vector3, Renderable, int>> renderList, Camera camera) {

            renderList.Add(new Object3<Vector3, Renderable, int>(position, this, 0));

            if (swingVisable) {
                Vector3 s = new Vector3(
                 position.X + (float)(Math.Sin(MathCustom.r180 - swingAngle - camera.rotation.Y) * swingDistance),
                 position.Y,
                 position.Z + (float)(Math.Cos(MathCustom.r180 - swingAngle - camera.rotation.Y) * swingDistance));

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
        public override void update(WorldInterface world) {

        }

        public override void clientUpdate(Game game, WorldInterface world){

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

            if (dif != Vector2.Zero) {

                dif.Normalize();

                if (game.isMP) {
                    game.sendPacket(new PacketPlayerInput(position.Xz, speed, entityId, world.worldId));
                }

                speed += dif * accSpeed;

            }

            speed.X *= .8f;
            speed.Y *= .8f;

            speed.X = MathCustom.bound(speed.X, -moveSpeed, moveSpeed);
            speed.Y = MathCustom.bound(speed.Y, -moveSpeed, moveSpeed);

            if (speed != Vector2.Zero) {
                move(world, speed);
            }

            if (game.inputControl.keyAttack.pressed) {
                //game.sendNewEntity(new EntityShot(position, this, MathCustom.r180-game.camera.rotation.Y, 0), world.worldId);
                game.world.addNewEntity(new EntityShot(position, this, MathCustom.r180-game.camera.rotation.Y, 0));
                //swingAngle = -MathCustom.r45;
                //swingVisable = true;
            }

            if (swingVisable) {
                swingAngle += MathCustom.r5;
                if (swingAngle > MathCustom.r45) {
                    swingVisable = false;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Key.T)) {
                game.world.addNewEntity(new EntityMonster(position, 0));
            }

            game.camera.position = -position;
        }


        public override void readMinor(System.Net.Sockets.NetworkStream stream) {
            StreamData.readFloat(stream);
            StreamData.readFloat(stream);
            StreamData.readFloat(stream);
            StreamData.readFloat(stream);
        }

    }
}
