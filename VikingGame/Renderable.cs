using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public interface Renderable {

        void render(Game game, World world, Camera camera, Vector3 position, int meta);

    }
}
