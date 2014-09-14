using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace VikingGame {
    public class ServerConnection {
        private Game game;

        public volatile TcpClient tcpClient;
        public volatile NetworkStream stream;
        private Thread threadListener;
        
        public bool remove = false;

        public ServerConnection(Game game, TcpClient tcpClient) {
            this.game = game;
            this.tcpClient = tcpClient;

            stream = tcpClient.GetStream();

            threadListener = new Thread(new ThreadStart(listen));
            threadListener.Start();
        }

        private void listen() {
            stream.ReadTimeout = 100;
            int b;
            while (game.isRunning && !remove) {
                try {
                    b = stream.ReadByte();
                    if (b != -1) {
                        Packet p = StreamData.readPacket(stream, (byte)b);
                        clientRecived(p);
                    }

                } catch (IOException) {
                    remove = true;
                }
            }
            game.serverConnection = null;
        }

        private void clientRecived(Packet p) {

            if (p.id == StreamData.packetToId[typeof(PacketNewPlayer)]) {
                PacketNewPlayer pnp = ((PacketNewPlayer)p);
                string u = pnp.username;
                if (u == game.uniqueUsername) {
                    game.world.entityList.Add(pnp.entityId, new Player(game.uniqueUsername, pnp.entityId));
                    Console.WriteLine("New Player from Server (Me)");
                } else {
                    game.world.entityList.Add(pnp.entityId, new EntityMultiplayerPlayer(game.uniqueUsername, pnp.entityId));
                    Console.WriteLine("New Player from Server (Not Me: "+pnp.username+")");
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketNewWorld)]) {

                PacketNewWorld pnw = ((PacketNewWorld)p);

                byte id = pnw.worldData.worldId;
                int w = pnw.worldData.x;
                int h = pnw.worldData.y;

                game.world = new World(game, w, h, id);

                Console.WriteLine("Server sent "+pnw.worldData);

                for (int i = 0; i < w; i++) {
                    for (int j = 0; j < h; j++) {
                        game.world.wallGrid[i, j] = pnw.worldWallGrid[i, j];
                    }
                }

                game.world.markToResetRenderGroup = true;

                game.inGame = true;

            } else if (p.id == StreamData.packetToId[typeof(PacketNewEntity)]) {
                PacketNewEntity pep = (PacketNewEntity)p;
                //Console.WriteLine("New Entity from Server: " + pep.entity.entityId);
                if (!game.world.entityList.ContainsKey(pep.entity.entityId)) {
                    game.world.entityList.Add(pep.entity.entityId, pep.entity);
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketRemoveEntity)]) {
                PacketRemoveEntity pep = (PacketRemoveEntity)p;
                //Console.WriteLine("Remove Entity from Server: " + pep.entityId);
                if (game.world.entityList.ContainsKey(pep.entityId)) {
                    game.world.entityList.Remove(pep.entityId);
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketUpdateEntity)]) {
                PacketUpdateEntity pep = (PacketUpdateEntity)p;
                pep.read(stream, game.world);
            } else if (p.id == StreamData.packetToId[typeof(PacketClientDisconnect)]) {
                PacketClientDisconnect pep = (PacketClientDisconnect)p;
                Console.Write("Client Disconnected: "+pep.username);
            }
        }

        internal void interuptThread() {
            threadListener.Interrupt();
        }
    }
}
