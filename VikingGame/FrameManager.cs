using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public class Frame {

        public RenderGroup renderGroup;
        public Vector3 halfRenderSize = new Vector3(.5f, .5f, .5f);
        public Vector3 renderOffset = new Vector3(0, 0, 0);
        public Vector2[] textureCoords;

        public Frame(Game game, Vector2[] textureCoords) {
            this.textureCoords = textureCoords;
            renderGroup = new RenderGroup(game);
            resetRenderGroup();
        }

        private void resetRenderGroup() {
            renderGroup.begin(BeginMode.Quads);

            renderGroup.addTop(-halfRenderSize + renderOffset, halfRenderSize + renderOffset, null, textureCoords);

            renderGroup.end();
        }


        public void render(Game game) {
            renderGroup.render(game);
        }
    }
    public class FrameManager {

        public static Frame noTexture;
        public static Frame playerDefault;
        public static Frame tree;
        public static Frame shot1;
        public static Frame swing1;


        public static void initFrames(Game game) {
            noTexture = new Frame(game, RenderGroup.makeTextureCoords(0, 0));
            playerDefault = new Frame(game, RenderGroup.makeTextureCoords(3, 0));
            tree = new Frame(game, RenderGroup.makeTextureCoords(5, 0));
            shot1 = new Frame(game, RenderGroup.makeTextureCoords(6, 0));
            swing1 = new Frame(game, RenderGroup.makeTextureCoords(7, 0));
        }
    }
}
