using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using VikingGame;

namespace VikingGame {

    public abstract class Packet {
        public byte id { get; private set; }
        public Packet() {
            id = StreamData.packetToId[this.GetType()];
        }
        internal abstract void read(NetworkStream stream);
        internal abstract void write(NetworkStream stream);
        public void writePacket(NetworkStream stream) {
            StreamData.writePacket(stream, this);
        }
    }

    public class PacketVerifyClient : Packet {
        public string username;
        public PacketVerifyClient() : this("") {}
        public PacketVerifyClient(String username) : base() {
            this.username = username;
        }
        internal override void read(NetworkStream stream) {
            username = StreamData.readSmallString(stream);
        }
        internal override void write(NetworkStream stream) {
            StreamData.writeSmallString(stream, username);
        }
    }

    public class PacketNewPlayer : Packet {

        public string username;
        public byte worldId;
        public float x;
        public float y;
        public int entityId = 0;

        public PacketNewPlayer() : this("", 0, 0, 0, 0) { }
        public PacketNewPlayer(String username, byte worldId, float x, float y, int entityId)
            : base() {

            this.worldId = worldId;
            this.x = x;
            this.y = y;
            this.username = username;
            this.entityId = entityId;
        }
        internal override void read(NetworkStream stream) {
            username = StreamData.readSmallString(stream);
            worldId = StreamData.readByte(stream);
            x = StreamData.readFloat(stream);
            y = StreamData.readFloat(stream);
            entityId = StreamData.readInt(stream);
        }
        internal override void write(NetworkStream stream) {
            StreamData.writeSmallString(stream, username);
            StreamData.writeByte(stream, worldId);
            StreamData.writeFloat(stream, x);
            StreamData.writeFloat(stream, y);
            StreamData.writeInt(stream, entityId);
        }
    }

    public class PacketNewWorld : Packet {
        public WorldData worldData;
        public byte[,] worldWallGrid;
        public PacketNewWorld() : this(new WorldData(0, 0, 0), new byte[0, 0]) { }
        public PacketNewWorld(WorldData worldData, byte[,] worldWalls) : base() {
                this.worldData = worldData;
                this.worldWallGrid = worldWalls;
        }
        internal override void read(NetworkStream stream) {
            worldData.worldId = StreamData.readByte(stream);
            worldData.x = StreamData.readInt(stream);
            worldData.y = StreamData.readInt(stream);
            worldWallGrid = new byte[worldData.x, worldData.y];

            for (int i = 0; i < worldData.x; i++) {
                for (int j = 0; j < worldData.y; j++) {
                    worldWallGrid[i, j] = (byte)stream.ReadByte();
                }
            }
        }
        internal override void write(NetworkStream stream) {
            StreamData.writeByte(stream, worldData.worldId);
            StreamData.writeInt(stream, worldData.x);
            StreamData.writeInt(stream, worldData.y);

            for (int i = 0; i < worldData.x; i++) {
                for (int j = 0; j < worldData.y; j++) {
                    stream.WriteByte(worldWallGrid[i, j]);
                }
            }

        }
    }

    public class PacketRemoveEntity : Packet {

        public byte worldId = 0;
        public int entityId = 0;

        public PacketRemoveEntity() : this(0, 0) { }
        public PacketRemoveEntity(byte worldId, int entityId) : base() {
            this.worldId = worldId;
            this.entityId = entityId;
        }

        internal override void read(NetworkStream stream) {
            worldId = StreamData.readByte(stream);
            entityId = StreamData.readInt(stream);
        }

        internal override void write(NetworkStream stream) {
            StreamData.writeByte(stream, worldId);
            StreamData.writeInt(stream, entityId);
        }
    }

    public class PacketHello : Packet {

        public PacketHello() : base() { }

        internal override void read(NetworkStream stream) {

        }

        internal override void write(NetworkStream stream) {

        }
    }

    public class PacketPlayerInput : Packet {

        public Vector2 move = new Vector2();
        public int entityId;
        public byte worldId;

        public PacketPlayerInput() : this(Vector2.Zero, 0, 0) { }
        public PacketPlayerInput(Vector2 move, int entityId, byte worldId) : base() {
            this.move = move;
            this.entityId = entityId;
            this.worldId = worldId;
        }

        internal override void read(NetworkStream stream) {
            move.X = StreamData.readFloat(stream);
            move.Y = StreamData.readFloat(stream);
            entityId = StreamData.readInt(stream);
            worldId = StreamData.readByte(stream);
        }

        internal override void write(NetworkStream stream) {
            StreamData.writeFloat(stream, move.X);
            StreamData.writeFloat(stream, move.Y);
            StreamData.writeInt(stream, entityId);
            StreamData.writeByte(stream, worldId);
        }
    }

    public class PacketNewEntity : Packet {

        public byte worldId;
        public byte entityTypeId;

        public Entity entity;

        public PacketNewEntity() : this(null, 0) { }
        public PacketNewEntity(Entity entity, byte worldId) {
            this.worldId = worldId;
            if (entity!=null) {
                this.entityTypeId = StreamData.entityTypeToId[entity.GetType()];
                this.entity = entity;
            }
        }

        internal override void read(NetworkStream stream) {
            worldId = StreamData.readByte(stream);
            entityTypeId = StreamData.readByte(stream);
            entity = (Entity)Activator.CreateInstance(StreamData.idToEntityType[entityTypeId]);
            entity.readMajor(stream);
        }

        internal override void write(NetworkStream stream) {
            StreamData.writeByte(stream, worldId);
            StreamData.writeByte(stream, entityTypeId);
            entity.writeMajor(stream);
        }
    }

    public class PacketUpdateEntity : Packet {

        public int entityId;
        public byte worldId;
        public Entity entity;

        public PacketUpdateEntity() : this(0, 0, null) { }
        public PacketUpdateEntity(int entityId, byte worldId, Entity entity) {
            this.entityId = entityId;
            this.worldId = worldId;
            this.entity = entity;
        }

        internal override void read(NetworkStream stream) {
            entityId = StreamData.readInt(stream);
            worldId = StreamData.readByte(stream);
        }

        public void read(NetworkStream stream, WorldInterface world) {
            world.updateEntity(stream, entityId);
        }

        internal override void write(NetworkStream stream) {
            StreamData.writeInt(stream, entityId);
            StreamData.writeByte(stream, worldId);
            entity.writeMinor(stream);
        }
    }

    public class StreamData {

        public static Dictionary<byte, Type> idToPacket = new Dictionary<byte, Type>();
        public static Dictionary<Type, byte> packetToId = new Dictionary<Type, byte>();

        public static Dictionary<byte, Type> idToEntityType = new Dictionary<byte, Type>();
        public static Dictionary<Type, byte> entityTypeToId = new Dictionary<Type, byte>();

        public static void init() {

            idToPacket.Clear();
            packetToId.Clear();

            newPacketType(1, typeof(PacketVerifyClient));
            newPacketType(2, typeof(PacketNewPlayer));
            newPacketType(3, typeof(PacketNewWorld));
            newPacketType(4, typeof(PacketHello));
            newPacketType(5, typeof(PacketPlayerInput));
            newPacketType(6, typeof(PacketNewEntity));
            newPacketType(7, typeof(PacketUpdateEntity));
            newPacketType(8, typeof(PacketRemoveEntity));


            idToEntityType.Clear();
            entityTypeToId.Clear();

            newEntityType(1, typeof(EntityMonster));
            newEntityType(2, typeof(EntityShot));

        }

        private static void newPacketType(byte id, Type type) {
            idToPacket.Add(id, type);
            packetToId.Add(type, id);
        }

        private static void newEntityType(byte id, Type type) {
            idToEntityType.Add(id, type);
            entityTypeToId.Add(type, id);
        }

        private static Packet createPacket(byte id) {
            //Console.WriteLine("id = " + id + "   idToPacket[id] = " + idToPacket[id]);
            if (idToPacket.ContainsKey(id)) {
                return (Packet)Activator.CreateInstance(idToPacket[id]);
            } else {
                throw new NoSuchPacketException(id);
            }
        }

        public static Packet readPacket(NetworkStream stream, byte packetId) {
            try {
                Packet p = createPacket(packetId);
                p.read(stream);
                //Console.WriteLine("Read Packet: " + packetId);
                return p;
            } catch (NoSuchPacketException e) {
                Console.WriteLine("No Such Packet: "+e.packetId);
                while(stream.CanRead){
                    Console.Write(stream.ReadByte()+" ");
                }
                Console.WriteLine();
                throw e;
            }
        }

        public static Packet readPacket(NetworkStream stream) {
            byte packetId = (byte)stream.ReadByte();
            return readPacket(stream, packetId);
        }

        internal static void writePacket(NetworkStream stream, Packet packet) {
            lock (stream) {
                stream.WriteByte(packet.id);
                packet.write(stream);
            }
        }

        internal static byte readByte(NetworkStream stream) {
            return (byte)stream.ReadByte();
        }

        internal static int readInt(NetworkStream stream) {
            return (stream.ReadByte() << 24) | (stream.ReadByte() << 16) | (stream.ReadByte() << 8) | (stream.ReadByte());
        }

        internal static float readFloat(NetworkStream stream) {
            return BitConverter.ToSingle(new byte[] { (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte() }, 0);
        }

        internal static String readSmallString(NetworkStream stream) {
            byte size = (byte)stream.ReadByte();
            String s = "";
            for (int i = 0; i < size; i++ ) {
                s += Convert.ToChar((byte)stream.ReadByte());
            }
            return s;
        }

        internal static void writeByte(NetworkStream stream, byte i) {
            stream.WriteByte(i);
        }

        internal static void writeInt(NetworkStream stream, int i) {
            stream.WriteByte((byte)(i >> 24));
            stream.WriteByte((byte)(i >> 16));
            stream.WriteByte((byte)(i >> 8));
            stream.WriteByte((byte)(i));
        }

        internal static void writeFloat(NetworkStream stream, float i) {
            byte[] bytes = BitConverter.GetBytes(i);
            stream.WriteByte(bytes[0]);
            stream.WriteByte(bytes[1]);
            stream.WriteByte(bytes[2]);
            stream.WriteByte(bytes[3]);
        }

        internal static void writeSmallString(NetworkStream stream, string s) {
            if(s.Length > 255){
                s = s.Substring(0, 255);
            }
            stream.WriteByte((byte)s.Length);
            foreach(char c in s){
                stream.WriteByte(Convert.ToByte(c));
            }
        }
    }
}
