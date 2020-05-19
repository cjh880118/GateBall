using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class BetPlayerDialog : IDialog
    {
        public Image imgPlus;
        public Image imgMinus;

        public Sprite spritePlus;
        public Sprite spritePlusPush;

        public Sprite spriteMinus;
        public Sprite spriteMinusPush;

        public Text txtPlayerCount10;
        public Text txtPlayerCount9;

        protected override void OnLoad()
        {
            imgPlus.sprite = spritePlus;
            imgMinus.sprite = spriteMinus;
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<PlayerInputButtonMouseOverMsg>(PlayerInputButtonMouseOver);
            Message.AddListener<PlayerInputButtonMouseOutMsg>(PlayerInputButtonMouseOut);
            Message.AddListener<SetBetPlayerMsg>(SetBetPlayer);
        }

        private void PlayerInputButtonMouseOver(PlayerInputButtonMouseOverMsg msg)
        {
            if (msg.isPlus)
            {
                imgPlus.sprite = spritePlusPush;
            }
            else
            {
                imgMinus.sprite = spriteMinusPush;
            }
        }

        private void PlayerInputButtonMouseOut(PlayerInputButtonMouseOutMsg msg)
        {
            if (msg.isPlus)
            {
                imgPlus.sprite = spritePlus;
            }
            else
            {
                imgMinus.sprite = spriteMinus;
            }
        }

        void SetBetPlayer(SetBetPlayerMsg msg)
        {
            txtPlayerCount9.gameObject.SetActive(false);
            txtPlayerCount10.gameObject.SetActive(false);

            if (msg.playerCount > 9)
            {
                txtPlayerCount10.gameObject.SetActive(true);
                txtPlayerCount10.text = msg.playerCount.ToString();
            }
            else
            {
                txtPlayerCount9.gameObject.SetActive(true);
                txtPlayerCount9.text = msg.playerCount.ToString();
            }
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<PlayerInputButtonMouseOverMsg>(PlayerInputButtonMouseOver);
            Message.RemoveListener<PlayerInputButtonMouseOutMsg>(PlayerInputButtonMouseOut);
            Message.RemoveListener<SetBetPlayerMsg>(SetBetPlayer);
        }

        protected override void OnUnload()
        {

        }
    }
}