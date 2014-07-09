using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VikingGame {
    public abstract class WorldInterface { // Interfaces are weird in c# so I'm just using a class instead

        public abstract Wall getWall(int x, int z);

        public abstract void removeEntity(int entityId);

        //public abstract void addNewEntity(Entity entity);

        public byte worldId { get; set; }

        public abstract void updateEntity(NetworkStream stream, int entityId);
    }
}
