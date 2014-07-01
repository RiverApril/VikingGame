using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {

    [Flags]
    public enum WallFlag {
        none = 0,
        wall = 1 << 0,
        floor = 1 << 1,
        flat = 1 << 2,
        solid = 1 << 3,
        deafultWall = solid | wall,
    }

    public class Wall : Renderable{

        public static Wall[] walls = new Wall[10];

        public static Wall air = new Wall(0, WallFlag.none);
        public static Wall basicWall = new Wall(1, WallFlag.deafultWall, 2,0, 1,0, 0,0);
        public static Wall basicFloor = new Wall(2, WallFlag.floor, 0,0, 4,0, 0,0);
        public static Wall basicTree = new Wall(3, WallFlag.flat | WallFlag.floor | WallFlag.solid, 0,0, 4,0, 5,0).setHalfSize(.1f, .1f, .1f);

        public WallFlag flags = WallFlag.deafultWall;

        public Vector3 halfSize;
        public Vector2[] texCoordsSides;
        public Vector2[] texCoordsTop;
        public Vector2[] texCoordsFlat;

        public int index;

        private RenderGroup renderGroup = null;

        public Wall(int index, WallFlag flags, int sidesX, int sidesY, int topX, int topY, int flatX, int flatY)
            : this(index, flags, RenderGroup.makeTextureCoords(sidesX, sidesY), RenderGroup.makeTextureCoords(topX, topY), RenderGroup.makeTextureCoords(flatX, flatY)) {

        }

        public Wall(int index, WallFlag flags)
            : this(index, flags, MathCustom.toArray(Vector2.Zero), MathCustom.toArray(Vector2.Zero), MathCustom.toArray(Vector2.Zero)) {
            
        }

        public Wall(int index, WallFlag flags, Vector2[] sides, Vector2[] top, Vector2[] flat) {
            walls[index] = this;
            this.index = index;
            this.flags = flags;
            halfSize = new Vector3(.5f, .5f, .5f);
            texCoordsSides = sides;
            texCoordsTop = top;
            texCoordsFlat = flat;
        }

        private Wall setHalfSize(float x, float y, float z) {
            halfSize = new Vector3(x, y, z);
            return this;
        }

        public static void init(Game game) {
            for (int i = 0; i < walls.Length;i++ ) {
                if (walls[i] != null) {
                    walls[i].initWall(game);
                }
            }
        }

        private void initWall(Game game) {
            if (flags.HasFlag(WallFlag.flat)) {
                renderGroup = new RenderGroup(game);
                renderGroup.begin(BeginMode.Quads);
                renderGroup.addTop(-MathCustom.vect3PointFive, MathCustom.vect3PointFive, null, texCoordsFlat);
                renderGroup.end();
            }
        }

        internal static Wall getWall(int index) {
            return walls[index];
        }

        internal bool hasFlag(WallFlag find) {
            return flags.HasFlag(find);
        }


        internal RenderGroup getRenderGroup() {
            return renderGroup;
        }

        public void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            world.prepareModelMatrix(camera, position);
            renderGroup.render(game);
        }
    }
}
