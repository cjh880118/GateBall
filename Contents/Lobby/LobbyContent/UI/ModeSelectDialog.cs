using JHchoi.Constants;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class ModeSelectDialog : IDialog
    {
        public Image imgMission;
        public Image imgBet;

        public Sprite spriteMission;
        public Sprite spritrMissionPush;
        public Sprite spriteBet;
        public Sprite spritrBetPush;


        protected override void OnLoad()
        {
            imgMission.sprite = spriteMission;
            imgBet.sprite = spriteBet;
            base.OnLoad();
        }


        protected override void OnEnter()
        {
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<OnModeSelectMouseOverMsg>(OnModeSelectMouseOver);
            Message.AddListener<OnModeSelectMouseOutMsg>(OnModeSelectMouseOut);
        }

        private void OnModeSelectMouseOver(OnModeSelectMouseOverMsg msg)
        {
            if (msg.modeType == ModeType.MissionMode)
            {
                imgMission.sprite = spritrMissionPush;
            }
            else
            {
                imgBet.sprite = spritrBetPush;
            }
        }

        private void OnModeSelectMouseOut(OnModeSelectMouseOutMsg msg)
        {
            if (msg.modeType == ModeType.MissionMode)
            {
                imgMission.sprite = spriteMission;
            }
            else
            {
                imgBet.sprite = spriteBet;
            }
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<OnModeSelectMouseOverMsg>(OnModeSelectMouseOver);
            Message.RemoveListener<OnModeSelectMouseOutMsg>(OnModeSelectMouseOut);
        }

        protected override void OnUnload()
        {

        }
    }
}
