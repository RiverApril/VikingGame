using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {

    public enum GuiEventType {
        cancel,
        mousePressed,
        mouseReleased,
        mouseMoved
    }

    public class GuiEvent {

        public GuiEventType type;
        public Vector2 mousePosition;

        public GuiEvent(GuiEventType type, Vector2 mousePosition) {
            this.type = type;
            this.mousePosition = mousePosition;
        }
    }
}
