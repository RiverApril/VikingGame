using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGameServer {

    public class ServerWorld {

        public ServerWorld(){

        }

    }

    public class WorldManager {

        public Dictionary<int, ServerWorld> worlds = new Dictionary<int, ServerWorld>();

        public WorldManager(FormServerMain server) {
            
        }

    }
}
