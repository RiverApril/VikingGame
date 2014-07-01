using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public class Object3<T1, T2, T3> {
        public T1 x { get; set; }
        public T2 y { get; set; }
        public T3 z { get; set; }

        public Object3(T1 x, T2 y, T3 z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }


    }
}
