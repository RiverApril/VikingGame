using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public abstract class Entity : Renderable {

        public static Random rand = new Random();


        public int entityId { get; set; }


        public float X { get { return position.X; } set { position.X = value; } }
        public float Z { get { return position.Z; } set { position.Z = value; } }

        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 halfSize = new Vector3(.5f, .5f, .5f);

        public Entity() : this(0) {

        }

        public Entity(int entityId) {
            this.entityId = entityId;
            init();
        }

        protected abstract void init();


        public virtual void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            world.prepareModelMatrix(camera, position);
            FrameManager.noTexture.render(game);
        }

        public virtual void clientUpdate(Game game, WorldInterface world) {

        }

        public abstract void update(WorldInterface world);

        public bool aabbXZ(Vector3 dif, Vector3 pb, Vector3 rb) {
            return 
                !(
                position.X + halfSize.X + dif.X < pb.X - rb.X || 
                position.X - halfSize.X + dif.X > pb.X + rb.X ||
                position.Z + halfSize.Z + dif.Z < pb.Z - rb.Z ||
                position.Z - halfSize.Z + dif.Z > pb.Z + rb.Z
                );
        }

        //Returns true if it hits a wall
        public bool move(WorldInterface world, Vector2 dif) {
            bool cx = false;
            bool cz = false;

            bool exit = false;

            Vector3 a = new Vector3();

            Wall w;

            for (int i = -1; i <= 1 && !exit; i++) {
                for (int j = -1; j <= 1 && !exit; j++) {
                    int x = i + (int)position.X;
                    int z = j + (int)position.Z;
                    a.X = x;
                    a.Z = z;
                    w = world.getWall(x, z);
                    if (w.hasFlag(WallFlag.solid)) {
                        if (aabbXZ(new Vector3(dif.X, 0, 0), a, w.halfSize)) {
                            cx = true;
                        }
                        if (aabbXZ(new Vector3(0, 0, dif.Y), a, w.halfSize)) {
                            cz = true;
                        }
                        if (cx && cz) {
                            exit = true;
                        }
                    }
                }
            }

            position.X += cx ? 0 : dif.X;
            position.Z += cz ? 0 : dif.Y;
            return cx || cz;
        }

        public virtual void addRenderable(ref List<Object3<Vector3, Renderable, int>> renderList, Camera camera) {
            renderList.Add(new Object3<Vector3, Renderable, int>(position, this, 0));
        }

        public void setPosition(float x, float z) {
            position.X = x;
            position.Z = z;
        }

        public void moveViaPacket(WorldInterface world, PacketPlayerInput pep) {
            move(world, pep.move);
        }

        public virtual void readMajor(NetworkStream stream) {
            entityId = StreamData.readInt(stream);
            readMinor(stream);
        }

        public virtual void writeMajor(NetworkStream stream) {
            StreamData.writeInt(stream, entityId);
            writeMinor(stream);
        }

        public virtual void readMinor(NetworkStream stream) {
            position.X = StreamData.readFloat(stream);
            position.Z = StreamData.readFloat(stream);
        }

        public virtual void writeMinor(NetworkStream stream) {
            StreamData.writeFloat(stream, position.X);
            StreamData.writeFloat(stream, position.Z);
        }
    }
}
