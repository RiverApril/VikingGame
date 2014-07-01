using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VikingGame {
    public class World {

        private Game game;

        private RenderGroup renderGroupXLow;
        private RenderGroup renderGroupXHigh;
        private RenderGroup renderGroupZLow;
        private RenderGroup renderGroupZHigh;
        private RenderGroup renderGroupTop;
        private RenderGroup renderGroupFloor;

        public List<Entity> entityList = new List<Entity>();
        public List<Entity> entityAddList = new List<Entity>();
        public List<Entity> entityRemoveList = new List<Entity>();

        public List<Object3<Vector3, Renderable, int>> renderList = new List<Object3<Vector3, Renderable, int>>();

        public int[,] wallGrid;
        private int width;
        private int height;

        private static int blockRenderDistance = 8;
        private int entityRenderDistanceSquared = 8*8;

        public World(Game game, int width = 100, int height = 100) {
            this.game = game;

            wallGrid = new int[width, height];
            this.width = width;
            this.height = height;

            Random rand = new Random();

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (rand.Next() % 6 == 0) {
                        wallGrid[i, j] = Wall.basicWall.index;
                    } else if (rand.Next() % 10 == 0) {
                        wallGrid[i, j] = Wall.basicTree.index;
                    } else {
                        wallGrid[i, j] = Wall.basicFloor.index;
                    }
                }
            }

            wallGrid[0, 0] = Wall.basicFloor.index;

            renderGroupXLow = new RenderGroup(game);
            renderGroupXHigh = new RenderGroup(game);
            renderGroupZLow = new RenderGroup(game);
            renderGroupZHigh = new RenderGroup(game);
            renderGroupTop = new RenderGroup(game);
            renderGroupFloor = new RenderGroup(game);
            resetRenderGroup();

            entityAddList.Add(new Player(game));
        }

        internal void resetRenderGroup() {
            renderGroupXLow.begin(BeginMode.Quads);
            renderGroupXHigh.begin(BeginMode.Quads);
            renderGroupZLow.begin(BeginMode.Quads);
            renderGroupZHigh.begin(BeginMode.Quads);
            renderGroupTop.begin(BeginMode.Quads);
            renderGroupFloor.begin(BeginMode.Quads);

            for (int i = 0; i < width;i++ ) {
                for (int j = 0; j < height; j++) {
                    addWall(i, j);
                }
            }


            renderGroupXLow.end();
            renderGroupXHigh.end();
            renderGroupZLow.end();
            renderGroupZHigh.end();
            renderGroupTop.end();
            renderGroupFloor.end();
        }

        private void addWall(int i, int j) {
            Wall w = Wall.getWall(wallGrid[i, j]);
            if (w.hasFlag(WallFlag.wall)) {

                addCube(new Vector3(-i - .5f, -.5f, -j - .5f), new Vector3(-i + .5f, .5f, -j + .5f), null, w.texCoordsSides, w.texCoordsTop, 
                    !getWall(i + 1, j).hasFlag(WallFlag.wall), 
                    !getWall(i - 1, j).hasFlag(WallFlag.wall), 
                    !getWall(i, j - 1).hasFlag(WallFlag.wall), 
                    !getWall(i, j + 1).hasFlag(WallFlag.wall));

            }
            if (w.hasFlag(WallFlag.floor)) {
                renderGroupFloor.addCube(new Vector3(-i - .5f, -.5f, -j - .5f), new Vector3(-i + .5f, -.5f, -j + .5f), Sides.yLow, null, w.texCoordsTop);
            }
        }

        private void addCube(Vector3 min, Vector3 max, Vector4[] color, Vector2[] texSide, Vector2[] texTop, bool xLow, bool xHigh, bool zLow, bool zHigh) {
            if (xLow) renderGroupXLow.addCube(min, max, Sides.xLow, color, texSide);
            if (xHigh) renderGroupXHigh.addCube(min, max, Sides.xHigh, color, texSide);
            if (zLow) renderGroupZLow.addCube(min, max, Sides.zLow, color, texSide);
            if (zHigh) renderGroupZHigh.addCube(min, max, Sides.zHigh, color, texSide);
            renderGroupTop.addCube(min, max, Sides.yLow, color, texTop);
        }

        internal void render(Game game, Camera camera) {

            GL.UseProgram(game.glProgram);
            GL.UniformMatrix4(game.modelMatrixLocation, 1, false, game.Matrix4ToArray(camera.getModelMatrix()));
            GL.UniformMatrix4(game.perspectiveMatrixLocation, 1, false, game.Matrix4ToArray(game.perspectiveMatrix));

            GL.BindTexture(TextureTarget.Texture2D, game.texturesId);

            if (camera.rotation.Y > MathCustom.r180 && camera.rotation.Y < MathCustom.r360) renderGroupXLow.render(game);

            if (camera.rotation.Y > 0 && camera.rotation.Y < MathCustom.r180) renderGroupXHigh.render(game);

            if (camera.rotation.Y > MathCustom.r90 && camera.rotation.Y < MathCustom.r270) renderGroupZLow.render(game);

            if ((camera.rotation.Y > MathCustom.r270 && camera.rotation.Y < MathCustom.r360) || (camera.rotation.Y > 0 && camera.rotation.Y < MathCustom.r90)) renderGroupZHigh.render(game);

            renderGroupTop.render(game);
            renderGroupFloor.render(game);

            Wall w;
            //Vector3 v;

            renderList.Clear();

            foreach (Entity e in entityList) {
                if ((e.position + camera.position).LengthSquared < entityRenderDistanceSquared) {
                    e.addRenderable(ref renderList);
                    //e.render(game, this, camera);
                }
            }

            int xMin = Math.Max((int)((-camera.position.X) - blockRenderDistance), 0);
            int yMin = Math.Max((int)((-camera.position.Z) - blockRenderDistance), 0);

            int xMax = Math.Min((int)((-camera.position.X) + blockRenderDistance), width);
            int yMax = Math.Min((int)((-camera.position.Z) + blockRenderDistance), height);

            for (int i = xMin; i < xMax; i++) {
                for (int j = yMin; j < yMax; j++) {
                    w = Wall.getWall(wallGrid[i, j]);
                    if (w.hasFlag(WallFlag.flat)) {
                        renderList.Add(new Object3<Vector3, Renderable, int>(new Vector3(i, 0, j), w, 0));
                        /*v = new Vector3(i, 0, j);
                        GL.UniformMatrix4(mm, 1, false, game.Matrix4ToArray(camera.getFlatMatrix(v)));
                        w.render(game, this, camera);*/
                    }
                }
            }

            float l = camera.rotation.Y - MathCustom.r180;

            Vector3 c = -camera.position + new Vector3(
                (float)(-Math.Sin(l) * 100), 
                MathCustom.sin45Times100,
                (float)(Math.Cos(l) * 100));

            renderList.Sort(
                delegate(
                Object3<Vector3, Renderable, int> first,
                Object3<Vector3, Renderable, int> second) {
                    return 
                        (first.x - c).LengthSquared.CompareTo(
                        (second.x - c).LengthSquared);
                }
            );

            //Console.WriteLine();

            foreach (Object3<Vector3, Renderable, int> element in renderList) {
                //GL.UniformMatrix4(mm, 1, false, game.Matrix4ToArray(camera.getFlatMatrix(element.Key)));
                //Console.WriteLine((element.Key - c).LengthSquared+": "+element.Value.GetType().Name);
                element.y.render(game, this, camera, element.x, element.z);
            }



        }

        internal void update(Game game) {

            foreach(Entity e in entityList){
                e.update(game, this);
            }

            while (entityAddList.Count > 0) {
                entityList.Add(entityAddList[0]);
                entityAddList.RemoveAt(0);
            }
            while (entityRemoveList.Count > 0) {
                entityList.Remove(entityRemoveList[0]);
                entityRemoveList.RemoveAt(0);
            }

        }

        internal Wall getWall(int x, int y) {
            if (x >= 0 && y >= 0 && x < width && y < height) {
                return Wall.getWall(wallGrid[x, y]);
            }
            return Wall.air;
        }

        internal void prepareModelMatrix(Camera camera, Vector3 position, float angle = 0) {
            GL.UniformMatrix4(game.modelMatrixLocation, 1, false, game.Matrix4ToArray(camera.getFlatMatrix(position, angle)));
        }
    }
}
