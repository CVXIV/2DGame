﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CVXIV {
    public class PlayerInput : InputComponent<PlayerInput> {


        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu);
        public InputButton Interact = new InputButton(KeyCode.E, XboxControllerButtons.Y);
        public InputButton NormalAttack = new InputButton(KeyCode.J, XboxControllerButtons.X);
        public InputButton RangedAttack = new InputButton(KeyCode.K, XboxControllerButtons.B);
        public InputButton Jump = new InputButton(KeyCode.Space, XboxControllerButtons.A);
        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizontal);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical);
        /*[HideInInspector]
        public DataSettings dataSettings;*/

        protected bool haveControl = true;
        public bool HaveControl { get { return haveControl; } }


        protected bool debugMenuIsOpen = false;


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

        protected override void GetInputs(bool fixedUpdateHappened) {
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            NormalAttack.Get(fixedUpdateHappened, inputType);
            RangedAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);
            Horizontal.Get(inputType);
            Vertical.Get(inputType);

            if (Input.GetKeyDown(KeyCode.F12)) {
                debugMenuIsOpen = !debugMenuIsOpen;
            }
        }

        public override void GainControl() {
            haveControl = true;

            GainControl(Pause);
            GainControl(Interact);
            GainControl(NormalAttack);
            GainControl(RangedAttack);
            GainControl(Jump);
            GainControl(Horizontal);
            GainControl(Vertical);
        }

        public override void ReleaseControl(bool resetValues = true) {
            haveControl = false;

            ReleaseControl(Pause, resetValues);
            ReleaseControl(Interact, resetValues);
            ReleaseControl(NormalAttack, resetValues);
            ReleaseControl(RangedAttack, resetValues);
            ReleaseControl(Jump, resetValues);
            ReleaseControl(Horizontal, resetValues);
            ReleaseControl(Vertical, resetValues);
        }

        public void DisableMeleeAttacking() {
            NormalAttack.Disable();
        }

        public void EnableMeleeAttacking() {
            NormalAttack.Enable();
        }

        public void DisableRangedAttacking() {
            RangedAttack.Disable();
        }

        public void EnableRangedAttacking() {
            RangedAttack.Enable();
        }

        /*public DataSettings GetDataSettings() {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType) {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData() {
            return new Data<bool, bool>(MeleeAttack.Enabled, RangedAttack.Enabled);
        }

        public void LoadData(Data data) {
            Data<bool, bool> playerInputData = (Data<bool, bool>)data;

            if (playerInputData.value0)
                MeleeAttack.Enable();
            else
                MeleeAttack.Disable();

            if (playerInputData.value1)
                RangedAttack.Enable();
            else
                RangedAttack.Disable();
        }*/

        void OnGUI() {
            if (debugMenuIsOpen) {
                const float height = 100;

                GUILayout.BeginArea(new Rect(30, Screen.height - height, 200, height));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Press F12 to close");

                bool meleeAttackEnabled = GUILayout.Toggle(NormalAttack.Enabled, "Enable Melee Attack");
                bool rangeAttackEnabled = GUILayout.Toggle(RangedAttack.Enabled, "Enable Range Attack");

                if (meleeAttackEnabled) {
                    NormalAttack.Enable();
                } else {
                    NormalAttack.Disable();
                }

                if (rangeAttackEnabled) {
                    RangedAttack.Enable();
                } else {
                    RangedAttack.Disable();
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}