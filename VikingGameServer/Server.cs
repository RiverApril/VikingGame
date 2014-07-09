using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VikingGame;

namespace VikingGameServer {
    public partial class Server : Form {

        private bool serverStarted = false;
        public volatile bool serverRunning = false;

        private TcpListener tcpListener;

        private int port = 7777;

        private Thread threadNewClientListener;
        private Thread threadGameLoop;

        private volatile List<ClientConnection> pendingClients = new List<ClientConnection>();
        private volatile Dictionary<string, ClientConnection> connectedClients = new Dictionary<string, ClientConnection>();

        private volatile string consoleText = "";
        private volatile string fpsLabelText = "";
        private bool needToUpdateConsoleText = false;
        private bool needToUpdateLabelText = false;

        private FpsControl fpsControl;

        private Stopwatch stopwatch;

        public WorldManager worldManager;

        private long lastMs = 0;
        private long maxMs = 1000 / 60;

        private int neid = 0;

        public int nextEntityId { get { neid++; return neid; } }

        private int tick = 0;

        public Server() {
            InitializeComponent();
            StreamData.init();
        }

        private void FormServerMain_Load(object sender, EventArgs e) {
            stopwatch = Stopwatch.StartNew();
        }

        private void FormServerMain_Closing(object sender, EventArgs e) {
            stopServer();
        }

        private void ButtonStartStop_Click(object sender, EventArgs e) {
            if(serverStarted){
                serverStarted = false;
                ButtonStartStop.Enabled = false;
                ButtonStartStop.Text = "Stopping...";
                stopServer();
            } else {
                serverStarted = true;
                ButtonStartStop.Text = "Stop";
                startServer();
            }
        }

        private void startServer() {
            try {

                IPAddress localAddress = IPAddress.Any;

                println("Starting Server: "+localAddress.ToString()+":"+port);

                tcpListener = new TcpListener(localAddress, port);

                serverRunning = true;

                threadNewClientListener = new Thread(new ThreadStart(listenFornewClients));
                threadNewClientListener.Start();

                threadGameLoop = new Thread(new ThreadStart(gameStart));
                threadGameLoop.Start();

            }catch(Exception e){
                println(e);
            }
        }

        public void println(object p) {
            print(p+Environment.NewLine);
        }

        public void print(object p) {
            consoleText += p;
            needToUpdateConsoleText = true;
        }

        private void stopServer() {
            println("Stopping Server...");

            try {
                serverRunning = false;
                tcpListener.Stop();
                tcpListener = null;
                threadNewClientListener.Interrupt();
                threadNewClientListener = null;
                foreach(ClientConnection cc in pendingClients){
                    cc.interuptThread();
                }
            } catch (Exception e) {
                println(e);
            }

            println("Server Stopped.");

            //After stop:
            ButtonStartStop.Enabled = true;
            ButtonStartStop.Text = "Start";
        }

        private void addPendingClientConnection(ClientConnection clientConnection) {
            println("Client Pending Verification...");
            lock (pendingClients) {
                pendingClients.Add(clientConnection);
            }
        }

        private void listenFornewClients() {
            tcpListener.Start();
            while(serverRunning){
                 try {
                     addPendingClientConnection(new ClientConnection(this, tcpListener.AcceptTcpClient()));
                 } catch (SocketException) {
                     
                 } catch (Exception e) {
                     println(e);
                 }
            }
        }

        private void gameStart() {

            fpsControl = new FpsControl();
            worldManager = new WorldManager(this);
            worldManager.genWorld(0, this);

            while (serverRunning) {
                try {
                    if (stopwatch.ElapsedMilliseconds - lastMs >= maxMs) {

                        lastMs = stopwatch.ElapsedMilliseconds;
                        fpsControl.updateFrame(stopwatch.ElapsedMilliseconds);
                        fpsLabelText = "FPS: " + fpsControl.fps;

                        if(tick%10==0){
                            sendPacketToAll(new PacketHello());
                        }

                        PacketUpdateEntity p = new PacketUpdateEntity();

                        foreach(ClientConnection cc in connectedClients.Values){
                            ServerWorld w = worldManager.worlds[cc.worldId];
                            foreach(ServerEntity e in w.entityList.Values){
                                p.entityId = e.entityId;
                                p.entity = e.entity;
                                p.worldId = cc.worldId;

                                //println("Send Entity("+e.entityId+") to client: "+cc.uniqueName);

                                sendPacket(cc, p);
                            }
                        }

                        worldManager.update(this);

                        tick++;

                    }
                } catch (Exception e) {
                    println(e);
                }
            }

            fpsControl = null;
            worldManager = null;

            pendingClients.Clear();
            connectedClients.Clear();

        }

        public void update(object sender, EventArgs e) {

            if (needToUpdateConsoleText) {
                needToUpdateConsoleText = false;
                TextBoxConsole.Text = consoleText;
            }
            if (needToUpdateLabelText) {
                needToUpdateLabelText = false;
                LabelFps.Text = fpsLabelText;
            }
        }

        public void println(byte[] inputBuffer) {
            throw new NotImplementedException();
        }

        public void removePending(ClientConnection clientConnection) {
            pendingClients.Remove(clientConnection);
            println("Client Failed Verification");
        }

        public void removeClient(ClientConnection clientConnection, String reason) {
            connectedClients.Remove(clientConnection.uniqueName);
            println("Client Disconnected: " + reason);
        }

        public bool clientExists(string u) {
            return connectedClients.ContainsKey(u);
        }

        public void verifyClient(ClientConnection clientConnection) {

            if (clientExists(clientConnection.uniqueName)) {
                removePending(clientConnection);
                clientConnection.remove = true;
                println("Client \"" + clientConnection.uniqueName + "\" Already Connected, Denying New Client");
            } else {
                println("Client Verified as \"" + clientConnection.uniqueName + "\"");

                pendingClients.Remove(clientConnection);

                println("Sending World to Client...");

                clientConnection.entityId = nextEntityId;
                ServerEntity e = new ServerEntity(clientConnection.entityId, new Player(clientConnection.uniqueName, clientConnection.entityId));
                worldManager.worlds[clientConnection.worldId].addNewEntity(e);


                ServerWorld w = worldManager.worlds[clientConnection.worldId];
                PacketNewWorld pnw = new PacketNewWorld(new WorldData(clientConnection.worldId, w.width, w.height), w.wallGrid);
                sendPacket(clientConnection, pnw);

                PacketNewPlayer pnp = new PacketNewPlayer(clientConnection.uniqueName, clientConnection.worldId, e.X, e.Z, clientConnection.entityId);
                sendPacketToAll(pnp);
                sendPacket(clientConnection, pnp);

                foreach (ClientConnection cc in connectedClients.Values) {
                    if (cc.entityId != clientConnection.entityId) {
                        ServerEntity ee = worldManager.worlds[clientConnection.worldId].entityList[cc.entityId];
                        PacketNewPlayer p = new PacketNewPlayer(cc.uniqueName, cc.worldId, ee.X, ee.Z, cc.entityId);
                        sendPacket(clientConnection, p);
                    }
                }

                println("World and Player Sent to Client");
                connectedClients.Add(clientConnection.uniqueName, clientConnection);
            }

        }

        public void sendPacketToAll(Packet p) {
            foreach(ClientConnection cc in connectedClients.Values){
                sendPacket(cc, p);
            }
        }

        public void sendPacket(ClientConnection clientConnection, Packet p) {
            //println("Sending packet: " + p + "  " + ((p.id == StreamData.packetToId[typeof(PacketNewEntity)]) ? (((PacketNewEntity)p).entity.entityId+"") : ""));
            lock (clientConnection.stream) {
                p.writePacket(clientConnection.stream);
            }
        }

        internal void sendPacketToAllExcept(ClientConnection clientConnection, Packet p) {
            foreach (ClientConnection cc in connectedClients.Values) {
                if(cc.entityId != clientConnection.entityId){
                    sendPacket(cc, p);
                }
            }
        }


    }
}
