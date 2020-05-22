using JHchoi.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.UI.Event;
using System.Runtime.InteropServices;

namespace JHchoi.UI
{
    public class KioskLevelSelectDialog : IDialog
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        int xPos = 0;
        int yPos = 0;

        public Button btnEasy;
        public Button btnNormal;
        public Button btnHard;
        public Button btnPrev;

        protected override void OnLoad()
        {
            base.OnLoad();
            btnEasy.onClick.AddListener(() => SelectLevel(Level.Easy));
            btnNormal.onClick.AddListener(() => SelectLevel(Level.Normal));
            btnHard.onClick.AddListener(() => SelectLevel(Level.Hard));
            btnPrev.onClick.AddListener(() => Prev());
        }

        protected override void OnEnter()
        {
            AddMessage();
            SetCursorPos(xPos, yPos);
        }

        void AddMessage()
        {

        }

        private void SelectLevel(Level level)
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<OnLevelSelectMsg>(new OnLevelSelectMsg(level));
        }

        private void Prev()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<PrevModeSelectMsg>(new PrevModeSelectMsg());
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
            RemoveMessage();
        }

        public void OnMouseOver(int i)
        {
            Debug.Log("In : " + i);
            Message.Send<OnLevelSelectMouseOverMsg>(new OnLevelSelectMouseOverMsg((Level)i));
        }

        public void OnMouseExit(int i)
        {
            Debug.Log("Out : " + i);
            Message.Send<OnLevelSelectMouseOutMsg>(new OnLevelSelectMouseOutMsg((Level)i));
        }
    }
}
