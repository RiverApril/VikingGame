using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Net;
using System.Net.Sockets;

namespace VikingGame {
    public class Game : GameWindow{

        public int glProgram;

        public int vertexShader;
        public int fragmentShader;

        public int perspectiveMatrixLocation;
        public int modelMatrixLocation;

        public int texturesId;

        public Matrix4 guiPerspectiveMatrix;
        public Matrix4 perspectiveMatrix;
        public Matrix4 emptyMatrix = Matrix4.CreateTranslation(0, 0, 0);
        private Matrix4 guiModelMatrix = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

        public InputControl inputControl = new InputControl();

        public FpsControl renderFpsControl = new FpsControl();
        public FpsControl updateFpsControl = new FpsControl();

        public Camera camera = new Camera();

        public World world;

        private float fov = MathHelper.DegreesToRadians(90);

        private bool loadDone = false;
        private float screenScale = 500;

        private System.Diagnostics.Stopwatch stopwatch;

        public Random rand = new Random();

        public bool inGame = false;

        private Gui cgui = null;

        public ServerConnection serverConnection;

        public bool isRunning = true;

        public string uniqueUsername = "Unamed";

        public static Game debugInstanceOnly;

        public bool isMP { get { return serverConnection != null; } }

        public Gui currentGui {
            get {
                return cgui;
            }
            set {
                if (value != null) {
                    value.loadFromOtherGui(this, cgui);
                }
                cgui = value;
            } 
        }



        public Game() : base(640, 480, GraphicsMode.Default, "Viking Game", GameWindowFlags.Default){
            debugInstanceOnly = this;
            Run(60);
        }

        public override void Exit() {
            isRunning = false;
            leaveServer();
            base.Exit();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();

            //Set Working Directory to Source Directory
            if (System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "textures.png").Length==0) {
                System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\");
            }
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory() + "");

            texturesId = loadTexture("textures.png");

            initShaders();

            modelMatrixLocation = GL.GetUniformLocation(glProgram, "modelMatrix");
            perspectiveMatrixLocation = GL.GetUniformLocation(glProgram, "perspectiveMatrix");

            //world = new World(this);
            //inGame = true;

            currentGui = new GuiMainMenu(this);

            inputControl.init();

            StreamData.init();

            Wall.init(this);
            FrameManager.initFrames(this);
            loadDone = true;

            uniqueUsername += "-"+(rand.Next() % 100);

        }

        private void initShaders() {

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, ShaderSources.vertexShader);
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, ShaderSources.fragmentShader);
            GL.CompileShader(fragmentShader);

            glProgram = GL.CreateProgram();
            GL.AttachShader(glProgram, vertexShader);
            GL.AttachShader(glProgram, fragmentShader);

            GL.LinkProgram(glProgram);
            GL.ValidateProgram(glProgram);
            GL.UseProgram(glProgram);

            

            Console.WriteLine("GL Error:\n"+GL.GetError()+"\n");
            Console.WriteLine("Vertex Shader Info Log:\n" + GL.GetShaderInfoLog(vertexShader)+"\n");
            Console.WriteLine("Fragment Shader Info Log:\n" + GL.GetShaderInfoLog(fragmentShader)+"\n");
            Console.WriteLine("Program Info Log:\n" + GL.GetProgramInfoLog(glProgram)+ "\n");


            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
        }

        private int loadTexture(string filename) {
            if(System.IO.File.Exists(filename)){
                int id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);

                Bitmap bmp = new Bitmap(filename);
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                return id;
            }
            return -1;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            GL.Viewport(ClientRectangle);

            //perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(fov, Width/(float)Height, .01f, 100);
            bool a = Width < Height;
            double aspectRatio = (double)(a?Width:Height) / (double)(a?Height:Width);
            float w = (float)((a ? aspectRatio : 1) * screenScale);
            float h = (float)((a ? 1 : aspectRatio) * screenScale);
            perspectiveMatrix = Matrix4.CreateOrthographic(
                w,
                h,
                -400, 400);

            guiPerspectiveMatrix = Matrix4.CreateOrthographicOffCenter(
                0,
                Width,
                -Height,
                0,
                -100, 100);

            currentGui.resize(this);

        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            if (!loadDone) return;
            base.OnRenderFrame(e);

            renderFpsControl.updateFrame(stopwatch.ElapsedMilliseconds);

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.LoadIdentity();

            if (inGame) {
                world.render(this, camera);
            }
            
            if (currentGui != null) {

                GL.UseProgram(glProgram);
                GL.UniformMatrix4(modelMatrixLocation, 1, false, Matrix4ToArray(guiModelMatrix));
                GL.UniformMatrix4(perspectiveMatrixLocation, 1, false, Matrix4ToArray(guiPerspectiveMatrix));

                GL.BindTexture(TextureTarget.Texture2D, texturesId);

                currentGui.render(this);
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            updateFpsControl.updateFrame(stopwatch.ElapsedMilliseconds);

            inputControl.update(this);

            if (inputControl.keyEscape.pressed && currentGui == null) {
                currentGui = new GuiPauseMenu(this);
            }

            if(currentGui != null){
                currentGui.update(this);
            }else if(inGame){
                camera.update(this);
                world.update(this);
            }

            Title = "ups: "+updateFpsControl.fps+"  fps: "+renderFpsControl.fps+(world==null?"":"  e: "+world.entityCount);

        }

        public float[] Matrix4ToArray(Matrix4 m) {
            return new float[]{
                m.Row0.X,
                m.Row0.Y,
                m.Row0.Z,
                m.Row0.W,
                m.Row1.X,
                m.Row1.Y,
                m.Row1.Z,
                m.Row1.W,
                m.Row2.X,
                m.Row2.Y,
                m.Row2.Z,
                m.Row2.W,
                m.Row3.X,
                m.Row3.Y,
                m.Row3.Z,
                m.Row3.W
            };
        }


        [STAThread]
        static void Main(string[] args) {
            new Game();
        }

        internal void startSinglePlayer() {
            world = new World(this);
            world.gen();
            inGame = true;
            world.addNewEntity(new Player(uniqueUsername, world.nextEntityId));
            camera.reset();
        }

        public void joinServer(IPAddress[] ip, int port) {

            TcpClient tcpClient = new TcpClient();
            try {
                Console.WriteLine("before tcpClient.connect");
                tcpClient.Connect(ip, port);
                Console.WriteLine("after tcpClient.connect");
                serverConnection = new ServerConnection(this, tcpClient);

                PacketVerifyClient p = new PacketVerifyClient(uniqueUsername);
                p.writePacket(serverConnection.stream);

            }catch(SocketException){
                if (serverConnection != null) serverConnection = null;
                currentGui = new GuiMainMenu(this);
            }
            camera.reset();

        }

        internal void leaveServer() {
            if (serverConnection != null) {
                serverConnection.remove = true;
                serverConnection.interuptThread();
            }
            serverConnection = null;
        }

        internal void sendPacket(Packet packet) {
            packet.writePacket(serverConnection.stream);
        }
    }
}
