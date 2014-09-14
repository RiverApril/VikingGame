using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public class EntityMultiplayerPlayer : Entity{

        private string username;

        public EntityMultiplayerPlayer(string username, int entityId) : base(entityId) {
            this.username = username;
        }

        protected override void init() {

        }

        public override void update(WorldInterface world) {

        }

        public override void clientUpdate(Game game, WorldInterface world) {

            if (speed != Vector2.Zero) {
                move(world, speed);
            }
        }

        public override void render(Game game, World world, Camera camera, Vector3 position, int meta) {

            world.prepareModelMatrix(camera, position);
            FrameManager.playerDefault.render(game);
        }
    }
}
