using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VikingGame {


    public abstract class GuiMenu : Gui{

        public GuiMenu previous;

        public GuiMenu(Game game, GuiMenu previous = null) : base(game, null, new Rectangle(0, 0, game.Width, game.Height)){
            this.previous = previous;
        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiRectangle(bounds, ColorMaker.black, RenderGroup.makeTextureCoords(0, 0));
        }

        public override void resize(Game game) {
            bounds = new Rectangle(0, 0, game.Width, game.Height);
        }

    }

    public class GuiMainMenu : GuiMenu {

        public GuiMainMenu(Game game, GuiMenu previous = null)
            : base(game, previous) {

        }

        protected override void init(Game game) {
            addGui(new GuiButton(game, this, new Rectangle(20, 20, 100, 20), "Singleplayer", buttonSP));
            addGui(new GuiButton(game, this, new Rectangle(20, 50, 100, 20), "Multiplayer", buttonMP));
            addGui(new GuiButton(game, this, new Rectangle(20, 80, 100, 20), "Quit", buttonQT));
        }

        private bool buttonSP(Gui arg, Game game) {
            game.currentGui = null;
            game.startSinglePlayer();
            return true;
        }

        private bool buttonMP(Gui arg, Game game) {
            game.currentGui = new GuiMPMenu(game, this);
            return true;
        }

        private bool buttonQT(Gui arg, Game game) {
            game.currentGui = null;
            game.Exit();
            return true;
        }

        protected override void processEvent(GuiEvent guiEvent, Game game, bool inGui) {

        }

        protected override void updateGui(Game game) {

        }
    }

    public class GuiMPMenu : GuiMenu {

        public GuiMPMenu(Game game, GuiMenu previous = null)
            : base(game, previous) {

        }

        protected override void init(Game game) {
            addGui(new GuiButton(game, this, new Rectangle(20, 20, 100, 20), "Join Server", buttonJoin));

            addGui(new GuiButton(game, this, new Rectangle(20, 80, 100, 20), "Back", buttonBack));
        }

        private bool buttonJoin(Gui arg1, Game game) {
            game.currentGui = null;
            game.joinServer(Dns.GetHostAddresses("72.208.52.99"), 7777);
            return true;
        }

        private bool buttonBack(Gui arg1, Game game) {
            game.currentGui = previous;
            return true;
        }

        protected override void processEvent(GuiEvent guiEvent, Game game, bool inGui) {

        }

        protected override void updateGui(Game game) {

        }
    }

    public class GuiPauseMenu : GuiMenu {

        public GuiPauseMenu(Game game, GuiMenu previous = null)
            : base(game, previous) {

        }

        protected override void init(Game game) {

            game.inputControl.keyEscape.pressed = false;
            addGui(new GuiButton(game, this, new Rectangle(20, 20, 100, 20), "Resume", buttonR));

            if (game.serverConnection != null) {
                addGui(new GuiButton(game, this, new Rectangle(20, 80, 100, 20), "Disconnect", buttonMM));
            } else {
                addGui(new GuiButton(game, this, new Rectangle(20, 80, 100, 20), "Main Menu", buttonMM));
            }
        }

        private bool buttonR(Gui arg1, Game game) {
            game.currentGui = null;
            return true;
        }

        private bool buttonMM(Gui arg1, Game game) {
            game.currentGui = new GuiMainMenu(game, null);
            game.leaveServer();
            return true;
        }

        protected override void processEvent(GuiEvent guiEvent, Game game, bool inGui) {

        }

        protected override void updateGui(Game game) {
            if (game.inputControl.keyEscape.pressed) {
                game.currentGui = null;
            }
        }

        protected override void updateRenderGroup(Game game, RenderGroup renderGroup) {
            renderGroup.addGuiRectangle(bounds, ColorMaker.transparent(.5f), RenderGroup.makeTextureCoords(0, 0));
        }
    }
}
