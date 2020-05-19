using CellBig.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;
using System.Runtime.InteropServices;

namespace CellBig.UI
{
    public class KioskModeSelectDialog : IDialog
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        int xPos = 0;
        int yPos = 0;

        public Button btnMissionMode;
        public Button btnBetMode;

        protected override void OnLoad()
        {
            base.OnLoad();
            btnMissionMode.onClick.AddListener(() => SelectMode(ModeType.MissionMode));
            btnBetMode.onClick.AddListener(() => SelectMode(ModeType.BetMode));
        }

        protected override void OnEnter()
        {
            AddMessage();
            SetCursorPos(xPos, yPos);
        }

        void AddMessage()
        {
            
        }

        void SelectMode(ModeType modeType)
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<OnModeSelectMsg>(new OnModeSelectMsg(modeType));
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
           
        }

        protected override void OnUnload()
        {

        }

        public void MouseOver(int mode)
        {
            Message.Send<OnModeSelectMouseOverMsg>(new OnModeSelectMouseOverMsg((ModeType)mode));
        }

        public void MouseOut(int mode)
        {
            Message.Send<OnModeSelectMouseOutMsg>(new OnModeSelectMouseOutMsg((ModeType)mode));
        }
    }
}