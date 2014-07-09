using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VikingGame {

    public class KeyBind {

        public Key kId;
        public MouseButton mbId;

        public bool down = false;
        public bool pressed = false;
        public bool released = false;

        public bool isKey;

        public KeyBind(InputControl ic, Key id) {
            this.kId = id;
            isKey = true;
            ic.Add(this);
        }

        public KeyBind(InputControl ic, MouseButton id) {
            this.mbId = id;
            isKey = false;
            ic.Add(this);
        }

    }

    public class InputControl {

        public List<KeyBind> keyBindList = new List<KeyBind>();

        public KeyBind keyMoveForward;
        public KeyBind keyMoveBackward;
        public KeyBind keyMoveLeft;
        public KeyBind keyMoveRight;

        public KeyBind keyRotateLeft;
        public KeyBind keyRotateRight;
        public KeyBind keyRotateUp;
        public KeyBind keyRotateDown;

        public KeyBind keyAttack;

        public KeyBind leftClick;
        public KeyBind rightClick;

        public KeyBind keyEscape;

        public Vector2 mousePosition;
        public Vector2 lastMousePosition;

        public KeyboardState keyboardState;
        public MouseState mouseState;


        public void init() {
            keyMoveForward = new KeyBind(this, Key.W);
            keyMoveBackward = new KeyBind(this, Key.S);
            keyMoveLeft = new KeyBind(this, Key.A);
            keyMoveRight = new KeyBind(this, Key.D);

            keyRotateLeft = new KeyBind(this, Key.Q);
            keyRotateRight = new KeyBind(this, Key.E);
            keyRotateUp = new KeyBind(this, Key.R);
            keyRotateDown = new KeyBind(this, Key.F);

            keyAttack = new KeyBind(this, Key.Space);

            leftClick = new KeyBind(this, MouseButton.Left);
            rightClick = new KeyBind(this, MouseButton.Right);

            keyEscape = new KeyBind(this, Key.Escape);
            
        }

        public void Add(KeyBind keyBind) {
            keyBindList.Add(keyBind);
        }

        public void update(Game game) {

            keyboardState = Keyboard.GetState();

            mouseState = Mouse.GetCursorState();

            lastMousePosition = mousePosition;

            mousePosition = MathCustom.toVector2(game.PointToClient(new System.Drawing.Point(mouseState.X, mouseState.Y)));


            foreach(KeyBind keyBind in keyBindList){
                keyBind.pressed = !keyBind.down && isKeyDown(keyBind, game);
                keyBind.released = keyBind.down && !isKeyDown(keyBind, game);

                keyBind.down = isKeyDown(keyBind, game);
            }
        }

        private bool isKeyDown(KeyBind keyBind, Game game) {
            return game.Focused ? (keyBind.isKey ? keyboardState.IsKeyDown(keyBind.kId) : mouseState.IsButtonDown(keyBind.mbId)) : false;
        }

        internal bool hasKeyEvent(Game game) {
            throw new NotImplementedException();
        }

        internal void takeKeyEvent() {
            throw new NotImplementedException();
        }
    }
}
