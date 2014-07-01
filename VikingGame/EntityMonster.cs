using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class EntityMonster : Entity {

        private float moveSpeed = .05f;

        private Vector2 moveTarget = new Vector2();

        public EntityMonster(Game game, Vector3 p) : base(game){
            position = p;
        }

        protected override void init() {
            halfSize = new Vector3(.2f, .2f, .2f);
        }

        public override void update(Game game, World world) {

            if (game.rand.Next() % 30 == 0) {
                double r = game.rand.NextDouble() * MathCustom.r360;
                moveTarget = new Vector2((float)Math.Sin(r), (float)Math.Cos(r));
            }

            move(world, moveTarget * moveSpeed);
        }

        public override void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            world.prepareModelMatrix(camera, position);
            FrameManager.playerDefault.render(game);
        }

    }
}
