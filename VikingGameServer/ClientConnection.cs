using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VikingGame;

namespace VikingGameServer {
    public class ClientConnection {

        private Thread threadListener;
        private Thread threadTeller;

        private volatile TcpClient tcpClient;

        private volatile Server server;

        public volatile NetworkStream stream;

        public volatile string uniqueName = null;

        public volatile int entityId = 0;
        public volatile byte worldId = 0;

        public bool remove = false;

        public bool verified = false;

        private PacketUpdateEntity p = new PacketUpdateEntity();

        public bool tellClient = false;

        public ClientConnection(Server server, TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            this.server = server;

            stream = tcpClient.GetStream();

            threadListener = new Thread(new ThreadStart(listen));
            threadListener.Start();

            threadTeller = new Thread(new ThreadStart(tell));
            threadTeller.Start();
        }

        private void tell() {

            while (server.serverRunning && !remove) {
                if (verified && tellClient) {

                    ServerWorld w = server.worldManager.worlds[worldId];

                    foreach (ServerEntity e in w.entityArray) {
                        p.entityId = e.entityId;
                        p.entity = e.entity;

                        if(!remove){
                            server.sendPacket(this, p);
                        }
                    }

                    tellClient = false;

                }
            }
        }

        private void listen() {
            int b;
            while (server.serverRunning && !remove) {
                try {

                    b = stream.ReadByte();
                    if(b != -1){
                        Packet p = StreamData.readPacket(stream, (byte)b);
                        serverRecived(p);
                    }

                } catch (System.IO.IOException) {
                    remove = true;
                    if(uniqueName==null){
                        server.removePending(this);
                    } else {
                        server.removeClient(this, "IOExcpetion");
                    }
                }
            }
        }

        private void serverRecived(Packet p) {

            if (uniqueName == null) {
                if (p.id == StreamData.packetToId[typeof(PacketVerifyClient)]) {
                    uniqueName = ((PacketVerifyClient)p).username;
                    server.verifyClient(this);
                } else {
                    server.removePending(this);
                    remove = true;
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketNewEntity)]) {
                PacketNewEntity pep = (PacketNewEntity)p;
                if (server.worldManager.worlds.ContainsKey(pep.worldId)) {

                    pep.entity.entityId = server.nextEntityId;

                    if (!server.worldManager.worlds[pep.worldId].entityList.ContainsKey(pep.entity.entityId)) {
                        server.worldManager.worlds[pep.worldId].addNewEntity(new ServerEntity(pep.entity.entityId, pep.entity));
                        server.println("New Entity with id: " + pep.entity.entityId);
                        server.sendPacketToAll(pep);
                    } else {
                        server.println("Entity with id already exists: " + pep.entity.entityId);
                    }
                } else {
                    server.println("No such world with id: " + pep.worldId);
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketUpdateEntity)]) {
                //Server Shouldent Recive
            } else if (p.id == StreamData.packetToId[typeof(PacketRemoveEntity)]) {
                PacketRemoveEntity pep = (PacketRemoveEntity)p;
                if (server.worldManager.worlds.ContainsKey(pep.worldId)) {
                    if (server.worldManager.worlds[pep.worldId].entityList.ContainsKey(pep.entityId)) {
                        server.worldManager.worlds[pep.worldId].removeEntity(pep.entityId);
                        server.println("Removed Entity with id: " + pep.entityId);
                        server.sendPacketToAll(pep);
                    } else {
                        server.println("Entity with id does not exist: " + pep.entityId);
                    }
                } else {
                    server.println("No such world with id: " + pep.worldId);
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketPlayerInput)]) {
                PacketPlayerInput pep = (PacketPlayerInput)p;
                if (server.worldManager.worlds.ContainsKey(pep.worldId)) {
                    ServerWorld w = server.worldManager.worlds[pep.worldId];
                    if (w.entityList.ContainsKey(pep.entityId)) {
                        w.entityList[pep.entityId].setPosition(pep);

                        //server.worldManager.worlds[pep.worldId].entityList[pep.entityId].X += pep.moveX;
                        //server.worldManager.worlds[pep.worldId].entityList[pep.entityId].Z += pep.moveZ;

                        //server.println("Update Player From Client Input  id:" + pep.entityId);
                    } else {
                        server.println("Entity with id does not exist: " + pep.entityId);
                    }
                } else {
                    server.println("No such world with id: " + pep.worldId);
                }
            } else if (p.id == StreamData.packetToId[typeof(PacketClientDisconnect)]) {
                PacketClientDisconnect pep = (PacketClientDisconnect)p;
                server.removeClient(this, "Disconnect");
                server.worldManager.worlds[worldId].removeEntity(entityId);
                server.sendPacketToAllExcept(this, new PacketRemoveEntity(worldId, entityId));
                remove = true;
            }

        }

        internal void interuptThread() {
            threadListener.Interrupt();
        }
    }
}
