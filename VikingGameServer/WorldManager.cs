using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VikingGame;

namespace VikingGameServer {

    public class ServerWorld : WorldInterface{

        public byte[,] wallGrid;
        public int width;
        public int height;

        private List<ServerEntity> entityAddList = new List<ServerEntity>();
        public Dictionary<int, ServerEntity> entityList = new Dictionary<int, ServerEntity>();
        private List<int> entityRemoveList = new List<int>();

        private Server server;

        public ServerWorld(int width, int height, Server server) {
            wallGrid = new byte[width, height];
            this.width = width;
            this.height = height;
            this.server = server;
        }


        internal void gen() {

            Random rand = new Random();

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (rand.Next() % 6 == 0) {
                        wallGrid[i, j] = Wall.basicWall.index;
                    } else if (rand.Next() % 10 == 0) {
                        wallGrid[i, j] = Wall.basicTree.index;
                    } else {
                        wallGrid[i, j] = Wall.basicFloor.index;
                    }
                }
            }

            wallGrid[0, 0] = Wall.basicFloor.index;

        }

        internal void update(Server server) {
            foreach (ServerEntity e in entityList.Values) {
                e.update(this);
            }

            while (entityAddList.Count > 0) {
                if (!entityList.ContainsKey(entityAddList[0].entityId)) {
                    entityList.Add(entityAddList[0].entityId, entityAddList[0]);
                } else {
                    server.println("Can't add: " + entityAddList[0].entityId);
                }
                entityAddList.RemoveAt(0);
            }
            while (entityRemoveList.Count > 0) {
                entityList.Remove(entityRemoveList[0]);
                entityRemoveList.RemoveAt(0);
            }
        }

        public override Wall getWall(int x, int y) {
            if (x >= 0 && y >= 0 && x < width && y < height) {
                return Wall.getWall(wallGrid[x, y]);
            }
            return Wall.air;
        }

        public override void removeEntity(int entityId) {
            entityRemoveList.Add(entityId);
            server.sendPacketToAll(new PacketRemoveEntity(worldId, entityId));
        }

        public void addNewEntity(ServerEntity entity) {
            entityAddList.Add(entity);
        }

        public override void updateEntity(System.Net.Sockets.NetworkStream stream, int entityId) {
            throw new NotImplementedException();
        }
    }

    public class WorldManager {

        public Dictionary<int, ServerWorld> worlds = new Dictionary<int, ServerWorld>();

        public WorldManager(Server server) {
            
        }


        internal void genWorld(int worldId, Server server) {
            if (worlds.ContainsKey(worldId)) {
                worlds.Remove(worldId);
            }
            worlds.Add(worldId, new ServerWorld(100, 100, server));
            worlds[worldId].gen();
        }

        internal void update(Server server) {
            foreach(ServerWorld w in worlds.Values){
                w.update(server);
            }
        }
    }
}
