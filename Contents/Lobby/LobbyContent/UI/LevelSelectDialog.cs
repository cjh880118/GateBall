using CellBig.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class LevelSelectDialog : IDialog
    {
        public Image imgEasy;
        public Image imgNormal;
        public Image imgHard;

        public Sprite spriteEasy;
        public Sprite spriteEasyPush;
        public Sprite spriteNormal;
        public Sprite spriteNormalPush;
        public Sprite spriteHard;
        public Sprite spriteHardPush;

        protected override void OnLoad()
        {
            base.OnLoad();
            imgEasy.sprite = spriteEasy;
            imgNormal.sprite = spriteNormal;
            imgHard.sprite = spriteHard;
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<OnLevelSelectMouseOverMsg>(OnLevelSelectMouseOver);
            Message.AddListener<OnLevelSelectMouseOutMsg>(OnLevelSelectMouseOut);
        }

        private void OnLevelSelectMouseOver(OnLevelSelectMouseOverMsg msg)
        {
            if (msg.level == Level.Easy)
            {
                imgEasy.sprite = spriteEasyPush;
            }
            else if (msg.level == Level.Normal)
            {
                imgNormal.sprite = spriteNormalPush;
            }
            else if (msg.level == Level.Hard)
            {
                imgHard.sprite = spriteHardPush;
            }
        }

        private void OnLevelSelectMouseOut(OnLevelSelectMouseOutMsg msg)
        {
            if (msg.level == Level.Easy)
            {
                imgEasy.sprite = spriteEasy;
            }
            else if (msg.level == Level.Normal)
            {
                imgNormal.sprite = spriteNormal;
            }
            else if (msg.level == Level.Hard)
            {
                imgHard.sprite = spriteHard;
            }
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<OnLevelSelectMouseOverMsg>(OnLevelSelectMouseOver);
            Message.RemoveListener<OnLevelSelectMouseOutMsg>(OnLevelSelectMouseOut);
        }

        protected override void OnUnload()
        {
            RemoveMessage();
        }
    }
}
