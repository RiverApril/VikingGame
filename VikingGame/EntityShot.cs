using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VikingGame {
    public class EntityShot : Entity{

        private float moveSpeed = .2f;

        private Vector2 moveTarget = new Vector2();

        private float angle;

        private byte life = 0;

        public EntityShot() : base(){

        }

        public EntityShot(Vector3 p, Entity owner, float angle, int entityId) : base(entityId) {
            position = p;
            moveTarget.X = (float)(Math.Sin(angle));
            moveTarget.Y = (float)(Math.Cos(angle));
            this.angle = angle;
        }

        protected override void init() {
            halfSize = new Vector3(.1f, .1f, .1f);
        }

        public override void update(WorldInterface world) {
            life++;
            if (move(world, moveTarget * moveSpeed)) {
                world.removeEntity(entityId);
            }

        }

        public override void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            if(life > 2){
                world.prepareModelMatrix(camera, position, camera.rotation.Y+angle);
                FrameManager.shot1.render(game);
            }
        }

        public override void readMajor(NetworkStream stream) {
            base.readMajor(stream);
            moveTarget.X = StreamData.readFloat(stream);
            moveTarget.Y = StreamData.readFloat(stream);
            angle = StreamData.readFloat(stream);
        }

        public override void writeMajor(NetworkStream stream) {
            base.writeMajor(stream);
            StreamData.writeFloat(stream, moveTarget.X);
            StreamData.writeFloat(stream, moveTarget.Y);
            StreamData.writeFloat(stream, angle);
        }

        public override void readMinor(NetworkStream stream) {
            base.readMinor(stream);
            life = StreamData.readByte(stream);
        }

        public override void writeMinor(NetworkStream stream) {
            base.writeMinor(stream);
            StreamData.writeByte(stream, life);
        }
    }
}
