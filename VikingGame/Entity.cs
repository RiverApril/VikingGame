using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {
    public abstract class Entity : Renderable{



        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 halfSize = new Vector3(.5f, .5f, .5f);

        protected Game game;

        public Entity(Game game) {
            this.game = game;
            init();
        }

        protected abstract void init();


        public virtual void render(Game game, World world, Camera camera, Vector3 position, int meta) {
            world.prepareModelMatrix(camera, position);
            FrameManager.noTexture.render(game);
        }

        public abstract void update(Game game, World world);

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
        protected bool move(World world, Vector2 dif) {
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

        public virtual void addRenderable(ref List<Object3<Vector3, Renderable, int>> renderList) {
            renderList.Add(new Object3<Vector3, Renderable, int>(position, this, 0));
        }
    }
}
