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

        public Gui holder { get; private set; }
        public List<Gui> heldGuiList = new List<Gui>();

        private RenderGroup renderGroup;

        public Rectangle bounds;
        public Gui focus = null;

        public Gui(Game game, Gui holder, Rectangle bounds) {
            this.bounds = bounds;
            init(game);
            this.holder = holder;
            if(holder == null){
                this.renderGroup = new RenderGroup(game);
            }
            dirty = true;
        }

        protected abstract void init(Game game);

        public void update(Game game) {
            updateGui(game);

            if (holder == null) {
                if(game.inputControl.leftClick.pressed){
                    distributeEvent(new GuiEvent(GuiEventType.mousePressed, game.inputControl.mousePosition), game);
                }
                if (game.inputControl.leftClick.released) {
                    distributeEvent(new GuiEvent(GuiEventType.mouseReleased, game.inputControl.mousePosition), game);
                }
                if (game.inputControl.mousePosition != game.inputControl.lastMousePosition) {
                    distributeEvent(new GuiEvent(GuiEventType.mouseMoved, game.inputControl.mousePosition), game);
                }
                /*while(game.inputControl.hasKeyEvent()){
                    distributeEvent(new GuiKeyEvent(GuiEventType.keyTyped, game.inputControl.takeKeyEvent()), game);
                }*/
            }

            foreach (Gui g in heldGuiList) {
                g.update(game);
                if(g.dirty){
                    dirty = true;
                    g.dirty = false;
                }
            }

            if (dirty && holder == null) {
                dirty = false;

                renderGroup.begin(BeginMode.Quads);
                foreach (Gui g in heldGuiList) {
                    g.updateRenderGroup(game, renderGroup);
                }
                updateRenderGroup(game, renderGroup);
                renderGroup.end();

                Console.WriteLine("Render Group Updated");

            }
        }

        private GuiEvent distributeEvent(GuiEvent guiEvent, Game game) {

            foreach (Gui g in heldGuiList) {
                if(guiEvent.type == GuiEventType.cancel){
                    return guiEvent;
                }

                guiEvent = g.distributeEvent(guiEvent, game);
            }

            processEvent(guiEvent, game, bounds.contains(guiEvent.mousePosition));

            return guiEvent;

        }

        public void render(Game game) {
            renderGroup.render(game);
        }

        protected void addGui(GuiButton guiButton) {
            heldGuiList.Add(guiButton);
        }

        protected abstract void processEvent(GuiEvent guiEvent, Game game, bool inGui);

        protected abstract void updateGui(Game game);

        protected abstract void updateRenderGroup(Game game, RenderGroup renderGroup);

        public virtual void loadFromOtherGui(Game game, Gui other) {
            if (holder == null) {
                distributeEvent(new GuiEvent(GuiEventType.mouseMoved, game.inputControl.mousePosition), game);
            }
        }

        public virtual void resize(Game game) {

        }
    }

    public class GuiButton : Gui {

        public String text = "";
        public Func<Gui, Game, bool> function;

        private bool buttonDown = false;

        private Vector2 buttonOffset = new Vector2(2, 2);

        public GuiButton(Game game, Gui holder, Rectangle rectangle, String text, Func<Gui, Game, bool> function) 
            : base(game, holder, rectangle) {
            this.text = text;
            this.function = function;
        }

        protected override void init(Game game) {

        }

        protected override void processEvent(GuiEvent guiEvent, Game game, bool inGui) {
            if(guiEvent.type == GuiEventType.mousePressed){
                if(inGui){
                    function.Invoke(this, game);
                }
            } else if (guiEvent.type == GuiEventType.mouseMoved) {
                if (inGui && !buttonDown) {
                    buttonDown = true;
                    dirty = true;
                } else if (!inGui && buttonDown) {
                    buttonDown = false;
                    dirty = true;
                }
            }
        }

        protected override void updateGui(Game game) {

        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiRectangle(buttonDown ? bounds : (bounds - buttonOffset), ColorMaker.gray(.3f));
            renderGroup.addGuiRectangle(bounds + buttonOffset, ColorMaker.gray(.2f));
            renderGroup.addGuiText((buttonDown ? bounds.position : (bounds.position - buttonOffset)) + bounds.halfSize + new Vector2(-(renderGroup.stringSize(text) / 2), -4), text, ColorMaker.white);

        }
    }

    public class GuiTextInput : Gui {

        public String text = "";

        private bool buttonDown = false;

        private Vector2 buttonOffset = new Vector2(2, 2);

        public GuiTextInput(Game game, Gui holder, Rectangle rectangle, String text)
            : base(game, holder, rectangle) {
            this.text = text;
        }

        protected override void init(Game game) {

        }

        protected override void processEvent(GuiEvent guiEvent, Game game, bool inGui) {
            if (guiEvent.type == GuiEventType.mousePressed) {
                if (inGui) {
                    this.holder.focus = this;
                }
            } else if (guiEvent.type == GuiEventType.keyTyped) {
                if(inGui){
                    if (this.holder.focus == this) {

                    }
                }
            }
        }

        protected override void updateGui(Game game) {

        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiRectangle(buttonDown ? bounds : (bounds - buttonOffset), ColorMaker.gray(.3f));
            renderGroup.addGuiRectangle(bounds + buttonOffset, ColorMaker.gray(.2f));
            renderGroup.addGuiText((buttonDown ? bounds.position : (bounds.position - buttonOffset)) + bounds.halfSize + new Vector2(-(renderGroup.stringSize(text) / 2), -4), text, ColorMaker.white);

        }
    }
}
