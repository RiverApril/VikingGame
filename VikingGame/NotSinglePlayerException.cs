using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    class NotSinglePlayerException : Exception {
        private Entity e;

        public NotSinglePlayerException(Entity e) {
            this.e = e;
        }
    }
}
