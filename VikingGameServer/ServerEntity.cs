using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VikingGame;

namespace VikingGameServer {
    public class ServerEntity {

        public int entityId { get { return entity.entityId; } set { entity.entityId = value; } }
        public Entity entity;

        public ServerEntity(int entityId, Entity entity) {
            this.entity = entity;
            this.entityId = entityId;
        }

        internal void setPosition(float x, float z) {
            entity.setPosition(x, z);
        }

        internal void setPosition(PacketPlayerInput pep) {
            entity.setPosition(pep);
        }

        public float X { get { return entity.X; } set { entity.X = value; } }
        public float Z { get { return entity.Z; } set { entity.Z = value; } }

        internal void update(ServerWorld w) {
            entity.update(w);
        }

        /*internal void move(ServerWorld w, PacketPlayerInput pep) {
            entity.moveViaPacket(w, pep);
        }*/
    }
}
