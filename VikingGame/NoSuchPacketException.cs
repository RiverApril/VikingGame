using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    class NoSuchPacketException : Exception {
        public byte packetId;

        public NoSuchPacketException(byte packetId) {
            this.packetId = packetId;
        }
    }
}
