using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public abstract class InputComponent<T> : MonoBehaviour where T: InputComponent<T> {

        protected static T _instance;

        public static T Instance {
            get {
                return _instance;
            }
        }

        protected void Awake() {
            if (_instance == null) {
                _instance = this as T;
            } else {
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + _instance.name + " and " + name + ".");
            }
        }

        /*        private void OnEnable() {
            if (Instance == null) {
                Instance = this;
            } else if (s_Instance != this) {
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
            }

            //PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable() {
            //PersistentDataManager.UnregisterPersister(this);

            s_Instance = null;
        }*/

        public enum InputType {
            MouseAndKeyboard,
            Controller,
        }


        public enum XboxControllerButtons {
            None,
            A,
            B,
            X,
            Y,
            Leftstick,
            Rightstick,
            View,
            Menu,
            LeftBumper,
            RightBumper,
        }


        public enum XboxControllerAxes {
            None,
            LeftstickHorizontal,
            LeftstickVertical,
            DpadHorizontal,
            DpadVertical,
            RightstickHorizontal,
            RightstickVertical,
            LeftTrigger,
            RightTrigger,
        }


        [Serializable]
        public class InputButton {
            public KeyCode key;
            public XboxControllerButtons controllerButton;
            // 按下
            public bool Down { get; protected set; }
            // 按住
            public bool Held { get; protected set; }
            // 松开
            public bool Up { get; protected set; }
            public bool Enabled {
                get { return enabled; }
            }

            [SerializeField]
            protected bool enabled = true;
            protected bool gettingInput = true;

            //This is used to change the state of a button (Down, Up) only if at least a FixedUpdate happened between the previous Frame
            //and this one. Since movement are made in FixedUpdate, without that an input could be missed it get press/release between fixedupdate
            bool afterFixedUpdateDown;
            bool afterFixedUpdateHeld;
            bool afterFixedUpdateUp;

            protected static readonly Dictionary<int, string> buttonsToName = new Dictionary<int, string>
            {
                {(int)XboxControllerButtons.A, "A"},
                {(int)XboxControllerButtons.B, "B"},
                {(int)XboxControllerButtons.X, "X"},
                {(int)XboxControllerButtons.Y, "Y"},
                {(int)XboxControllerButtons.Leftstick, "Leftstick"},
                {(int)XboxControllerButtons.Rightstick, "Rightstick"},
                {(int)XboxControllerButtons.View, "View"},
                {(int)XboxControllerButtons.Menu, "Menu"},
                {(int)XboxControllerButtons.LeftBumper, "Left Bumper"},
                {(int)XboxControllerButtons.RightBumper, "Right Bumper"},
            };

            public InputButton(KeyCode key, XboxControllerButtons controllerButton) {
                this.key = key;
                this.controllerButton = controllerButton;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fixedUpdateHappened">是否在fixedUpdate期间发生按键</param>
            /// <param name="inputType">按键的类型</param>
            public void Get(bool fixedUpdateHappened, InputType inputType) {
                if (!enabled) {
                    Down = false;
                    Held = false;
                    Up = false;
                    return;
                }

                if (!gettingInput) {
                    return;
                }

                if (inputType == InputType.Controller) {
                    if (fixedUpdateHappened) {
                        Down = Input.GetButtonDown(buttonsToName[(int)controllerButton]);
                        Held = Input.GetButton(buttonsToName[(int)controllerButton]);
                        Up = Input.GetButtonUp(buttonsToName[(int)controllerButton]);

                        afterFixedUpdateDown = Down;
                        afterFixedUpdateHeld = Held;
                        afterFixedUpdateUp = Up;
                    } else {
                        Down = Input.GetButtonDown(buttonsToName[(int)controllerButton]) || afterFixedUpdateDown;
                        Held = Input.GetButton(buttonsToName[(int)controllerButton]) || afterFixedUpdateHeld;
                        Up = Input.GetButtonUp(buttonsToName[(int)controllerButton]) || afterFixedUpdateUp;

                        afterFixedUpdateDown |= Down;
                        afterFixedUpdateHeld |= Held;
                        afterFixedUpdateUp |= Up;
                    }
                } else if (inputType == InputType.MouseAndKeyboard) {
                    if (fixedUpdateHappened) {
                        Down = Input.GetKeyDown(key);
                        Held = Input.GetKey(key);
                        Up = Input.GetKeyUp(key);

                        afterFixedUpdateDown = Down;
                        afterFixedUpdateHeld = Held;
                        afterFixedUpdateUp = Up;
                    } else {
                        Down = Input.GetKeyDown(key) || afterFixedUpdateDown;
                        Held = Input.GetKey(key) || afterFixedUpdateHeld;
                        Up = Input.GetKeyUp(key) || afterFixedUpdateUp;

                        afterFixedUpdateDown |= Down;
                        afterFixedUpdateHeld |= Held;
                        afterFixedUpdateUp |= Up;
                    }
                }
            }

            public void Enable() {
                enabled = true;
            }

            public void Disable() {
                enabled = false;
            }

            public void GainControl() {
                gettingInput = true;
            }

            public IEnumerator ReleaseControl(bool resetValues) {
                gettingInput = false;

                if (!resetValues) {
                    yield break;
                }

                // 如果按下了某个键，则自动发出Up信号
                if (Down) {
                    Up = true;
                }
                Down = false;
                Held = false;

                afterFixedUpdateDown = false;
                afterFixedUpdateHeld = false;
                afterFixedUpdateUp = false;

                yield return null;

                Up = false;
            }
        }

        [Serializable]
        public class InputAxis {
            public KeyCode positive;
            public KeyCode negative;
            public XboxControllerAxes controllerAxis;
            public float Value { get; protected set; }
            public bool ReceivingInput { get; protected set; }
            public bool Enabled {
                get { return enabled; }
            }

            protected bool enabled = true;
            protected bool gettingInput = true;

            protected readonly static Dictionary<int, string> axisToName = new Dictionary<int, string> {
                {(int)XboxControllerAxes.LeftstickHorizontal, "Leftstick Horizontal"},
                {(int)XboxControllerAxes.LeftstickVertical, "Leftstick Vertical"},
                {(int)XboxControllerAxes.DpadHorizontal, "Dpad Horizontal"},
                {(int)XboxControllerAxes.DpadVertical, "Dpad Vertical"},
                {(int)XboxControllerAxes.RightstickHorizontal, "Rightstick Horizontal"},
                {(int)XboxControllerAxes.RightstickVertical, "Rightstick Vertical"},
                {(int)XboxControllerAxes.LeftTrigger, "Left Trigger"},
                {(int)XboxControllerAxes.RightTrigger, "Right Trigger"},
            };

            public InputAxis(KeyCode positive, KeyCode negative, XboxControllerAxes controllerAxis) {
                this.positive = positive;
                this.negative = negative;
                this.controllerAxis = controllerAxis;
            }

            public void Get(InputType inputType) {
                if (!enabled) {
                    Value = 0f;
                    return;
                }

                if (!gettingInput) {
                    return;
                }

                bool positiveHeld = false;
                bool negativeHeld = false;

                if (inputType == InputType.Controller) {
                    float value = Input.GetAxisRaw(axisToName[(int)controllerAxis]);
                    positiveHeld = value > Single.Epsilon;
                    negativeHeld = value < -Single.Epsilon;
                } else if (inputType == InputType.MouseAndKeyboard) {
                    positiveHeld = Input.GetKey(positive);
                    negativeHeld = Input.GetKey(negative);
                }
                if (positiveHeld == negativeHeld) {
                    Value = 0f;
                } else if (positiveHeld) {
                    Value = 1f;
                } else {
                    Value = -1f;
                }

                ReceivingInput = positiveHeld || negativeHeld;
            }

            public void Enable() {
                enabled = true;
            }

            public void Disable() {
                enabled = false;
            }

            public void GainControl() {
                gettingInput = true;
            }

            public void ReleaseControl(bool resetValues) {
                gettingInput = false;
                if (resetValues) {
                    Value = 0f;
                    ReceivingInput = false;
                }
            }
        }

        public InputType inputType = InputType.MouseAndKeyboard;

        bool fixedUpdateHappened;

        private void Update() {
            GetInputs(fixedUpdateHappened || Mathf.Approximately(Time.timeScale, 0));

            fixedUpdateHappened = false;
        }

        private void FixedUpdate() {
            fixedUpdateHappened = true;
        }

        protected abstract void GetInputs(bool fixedUpdateHappened);

        public abstract void GainControl();

        public abstract void ReleaseControl(bool resetValues = true);

        protected void GainControl(InputButton inputButton) {
            inputButton.GainControl();
        }

        protected void GainControl(InputAxis inputAxis) {
            inputAxis.GainControl();
        }

        protected void ReleaseControl(InputButton inputButton, bool resetValues) {
            StartCoroutine(inputButton.ReleaseControl(resetValues));
        }

        protected void ReleaseControl(InputAxis inputAxis, bool resetValues) {
            inputAxis.ReleaseControl(resetValues);
        }
    }
}
