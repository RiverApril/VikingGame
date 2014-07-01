using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VikingGame.StreamData {

    public abstract class Packet {
        public byte id { get; private set; }
        public Packet() : this(0) {}
        public Packet(byte id) { this.id = id; }
        internal abstract void read(NetworkStream stream);
        internal abstract void write(NetworkStream stream);
        public void writePacket(NetworkStream stream) {
            StreamData.writePacket(stream, this);
        }
    }

    public class PacketVerifyClient : Packet {
        public string username;
        public PacketVerifyClient() : this("") {}
        public PacketVerifyClient(String username) : base(1) {
            this.username = username;
        }
        internal override void read(NetworkStream stream) {
            username = StreamData.readSmallString(stream);
        }
        internal override void write(NetworkStream stream) {
            StreamData.writeSmallString(stream, username);
        }
    }

    public class StreamData {

        private static Type[] packets = new Type[] { typeof(Packet), typeof(PacketVerifyClient) };

        private static Packet createPacket(byte id) {
            Console.WriteLine(id);
            return (Packet)Activator.CreateInstance(packets[id]);
        }

        public static Packet readPacket(NetworkStream stream, byte packetId) {
            Packet p = createPacket(packetId);
            p.read(stream);
            return p;
        }

        public static Packet readPacket(NetworkStream stream) {
            byte packetId = (byte)stream.ReadByte();
            return readPacket(stream, packetId);
        }

        internal static void writePacket(NetworkStream stream, Packet packet) {
            stream.WriteByte(packet.id);
            packet.write(stream);
        }

        internal static int readInt(NetworkStream stream) {
            return (stream.ReadByte() << 24) | (stream.ReadByte() << 16) | (stream.ReadByte() << 8) | (stream.ReadByte());
        }

        internal static String readSmallString(NetworkStream stream) {
            byte size = (byte)stream.ReadByte();
            String s = "";
            for (int i = 0; i < size; i++ ) {
                s += Convert.ToChar((byte)stream.ReadByte());
            }
            return s;
        }

        internal static void writeInt(NetworkStream stream, int i) {
            stream.WriteByte((byte)(i >> 24));
            stream.WriteByte((byte)(i >> 16));
            stream.WriteByte((byte)(i >> 8));
            stream.WriteByte((byte)(i));
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
