using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public class EntityShot : Entity{

        private float moveSpeed = .2f;

        private Vector2 moveTarget = new Vector2();

        private Random rand = new Random();

        private float angle;

        private int life = 0;

        public EntityShot(Game game, Vector3 p, Entity owner, float angle) : base(game) {
            position = p;
            moveTarget.X = (float)(Math.Sin(angle));
            moveTarget.Y = (float)(Math.Cos(angle));
            this.angle = angle;
        }

        protected override void init() {
            halfSize = new Vector3(.1f, .1f, .1f);
        }

        public override void update(Game game, World world) {
            life++;
            if (move(world, moveTarget * moveSpeed)) {
                world.entityRemoveList.Add(this);
            }

        }

        public override void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            if(life > 2){
                world.prepareModelMatrix(camera, position, camera.rotation.Y+angle);
                FrameManager.shot1.render(game);
            }
        }
    }
}
