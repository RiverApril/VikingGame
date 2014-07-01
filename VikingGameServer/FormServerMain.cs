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
    public partial class FormServerMain : Form {

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

        private FpsControl fpsControl;

        private Stopwatch stopwatch;

        private WorldManager worldManager;

        private long lastMs = 0;
        private long maxMs = 1000/60;

        public FormServerMain() {
            InitializeComponent();
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

                IPAddress localAddress = Dns.GetHostAddresses("127.0.0.1")[0];

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
        }

        private void stopServer() {
            println("Stopping Server...");

            try {
                serverRunning = false;
                tcpListener.Stop();
                tcpListener = null;
                threadNewClientListener = null;
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

            while (serverRunning) {
                try {
                    if (stopwatch.ElapsedMilliseconds - lastMs >= maxMs) {

                        lastMs = stopwatch.ElapsedMilliseconds;
                        fpsControl.updateFrame(stopwatch.ElapsedMilliseconds);
                        fpsLabelText = "FPS: " + fpsControl.fps;

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

            if (true) {

                TextBoxConsole.Text = consoleText;
                LabelFps.Text = fpsLabelText;

            }
        }

        internal void println(byte[] inputBuffer) {
            throw new NotImplementedException();
        }

        internal void removePending(ClientConnection clientConnection) {
            pendingClients.Remove(clientConnection);
            println("Client Failed Verification");
        }

        internal void removeClient(ClientConnection clientConnection, String reason) {
            connectedClients.Remove(clientConnection.uniqueName);
            println("Client Disconnected: " + reason);
        }

        internal bool clientExists(string u) {
            return connectedClients.ContainsKey(u);
        }

        internal void verifyClient(ClientConnection clientConnection) {

            if (clientExists(clientConnection.uniqueName)) {
                removePending(clientConnection);
                clientConnection.remove = true;
                println("Client " + clientConnection.uniqueName + " Already Connected");
            } else {
                println("Client Verified as " + clientConnection.uniqueName);
            }

        }
    }
}
