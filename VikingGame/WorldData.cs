using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public class WorldData {
        public byte worldId;
        public int x;
        public int y;

        public WorldData(byte worldId, int x, int y) {
            this.worldId = worldId;
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return "World: " + worldId + "  " + x + ", " + y;
        }
    }
}
