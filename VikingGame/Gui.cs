using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {
    public abstract class Gui {

        protected bool dirty = false;

        public Gui parent { get; private set; }
        public List<Gui> childGuiList = new List<Gui>();

        private RenderGroup renderGroup;

        public Rectangle bounds;

        public Gui(Game game, Gui parent, Rectangle bounds) {
            this.bounds = bounds;
            init(game);
            this.parent = parent;
            if(parent == null){
                this.renderGroup = new RenderGroup(game);
            }
            dirty = true;
        }

        protected abstract void init(Game game);

        public void update(Game game) {
            updateGui(game);

            if (parent == null) {
                if(game.inputControl.leftClick.pressed){
                    distributeEvent(new GuiEvent(GuiEventType.mousePressed, game.inputControl.mousePosition));
                }
                if (game.inputControl.leftClick.released) {
                    distributeEvent(new GuiEvent(GuiEventType.mouseReleased, game.inputControl.mousePosition));
                }
                if (game.inputControl.mousePosition != game.inputControl.lastMousePosition) {
                    distributeEvent(new GuiEvent(GuiEventType.mouseMoved, game.inputControl.mousePosition));
                }
            }

            foreach (Gui g in childGuiList) {
                g.update(game);
                if(g.dirty){
                    dirty = true;
                    g.dirty = false;
                }
            }

            if (dirty && parent == null) {
                dirty = false;

                renderGroup.begin(BeginMode.Quads);
                foreach (Gui g in childGuiList) {
                    g.updateRenderGroup(game, renderGroup);
                }
                updateRenderGroup(game, renderGroup);
                renderGroup.end();

                Console.WriteLine("Render Group Updated");

            }
        }

        private GuiEvent distributeEvent(GuiEvent guiEvent) {

            foreach (Gui g in childGuiList) {
                if(guiEvent.type == GuiEventType.cancel){
                    return guiEvent;
                }

                guiEvent = g.distributeEvent(guiEvent);
            }

            processEvent(guiEvent, bounds.contains(guiEvent.mousePosition));

            return guiEvent;

        }

        public void render(Game game) {
            renderGroup.render(game);
        }

        protected void addGui(GuiButton guiButton) {
            childGuiList.Add(guiButton);
        }

        protected abstract void processEvent(GuiEvent guiEvent, bool inGui);

        protected abstract void updateGui(Game game);

        protected abstract void updateRenderGroup(Game game, RenderGroup renderGroup);
    }

    public class GuiMenu : Gui {

        public GuiMenu(Game game, Gui parent) 
            : base(game, parent, new Rectangle(0, 0, game.Width, game.Height)) { }

        protected override void init(Game game) {
            addGui(new GuiButton(game, this, new Rectangle(20, 20, 100, 20), "Singleplayer", processButton));
            addGui(new GuiButton(game, this, new Rectangle(20, 50, 100, 20), "Multiplayer", processButton));
            addGui(new GuiButton(game, this, new Rectangle(20, 80, 100, 20), "Quit", processButton));
        }

        private bool processButton(Gui gui) {
            Console.WriteLine("CLICK!");
            return true;
        }

        protected override void processEvent(GuiEvent guiEvent, bool inGui) {

        }

        protected override void updateGui(Game game) {

        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiRectangle(bounds, Vector4Color.black, RenderGroup.makeTextureCoords(0, 0));
        }
    }

    public class GuiButton : Gui {

        public String text = "";
        public Func<Gui, bool> function;

        private bool buttonDown = false;

        private Vector2 buttonOffset = new Vector2(2, 2);

        public GuiButton(Game game, Gui parent, Rectangle rectangle, String text, Func<Gui, bool> function) 
            : base(game, parent, rectangle) {
            this.text = text;
            this.function = function;
        }

        protected override void init(Game game) {

        }

        protected override void processEvent(GuiEvent guiEvent, bool inGui) {
            if(inGui && guiEvent.type == GuiEventType.mousePressed){
                if (function.Invoke(this)) {
                    buttonDown = true;
                    dirty = true;
                }
            } else if ((!inGui || guiEvent.type == GuiEventType.mouseReleased) && buttonDown) {
                buttonDown = false;
                dirty = true;
            }
        }

        protected override void updateGui(Game game) {

        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiText((buttonDown ? bounds.position : (bounds.position - buttonOffset)) + bounds.halfSize + new Vector2(-(renderGroup.stringSize(text)/2), -4), text, Vector4Color.blue);
            renderGroup.addGuiRectangle(buttonDown ? bounds : (bounds - buttonOffset), Vector4Color.gray(.3f));
            renderGroup.addGuiRectangle(bounds + buttonOffset, Vector4Color.gray(.2f));

        }
    }
}
