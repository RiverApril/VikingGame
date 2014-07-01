using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public class FpsControl {

        long lastTime = 0;
        public int frames = 0;
        public int fps{private set;get;}


        public void updateFrame(long time) {
            frames += 1;
            if(time - lastTime > 1000){
                lastTime = time;
                fps = frames;
                frames = 0;
            }
        }

    }
}
