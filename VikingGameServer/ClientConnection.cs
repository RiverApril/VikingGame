using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VikingGame;
using VikingGame.StreamData;

namespace VikingGameServer {
    class ClientConnection {

        private Thread threadListener;

        private volatile TcpClient tcpClient;

        private volatile FormServerMain server;

        private volatile NetworkStream stream;

        public volatile string uniqueName = null;

        public bool remove = false;

        public ClientConnection(FormServerMain server, TcpClient tcpClient) {
            this.tcpClient = tcpClient;
            this.server = server;

            stream = tcpClient.GetStream();

            threadListener = new Thread(new ThreadStart(listen));
            threadListener.Start();
        }

        private void listen() {
            int b;
            while (server.serverRunning && !remove) {
                try {

                    b = stream.ReadByte();
                    if(b != -1){
                        if (uniqueName == null) {
                            Packet p = StreamData.readPacket(stream, (byte)b);
                            if (p.GetType() == typeof(PacketVerifyClient)) {
                                uniqueName = ((PacketVerifyClient)p).username;
                                server.verifyClient(this);
                            } else {
                                server.removePending(this);
                                remove = true;
                            }
                        } else {

                        }
                    }

                }catch(IOException){
                    if(uniqueName==null){
                        server.removePending(this);
                    } else {
                        server.removeClient(this, "IOExcpetion");
                    }
                    remove = true;
                }
            }
        }
    }
}
