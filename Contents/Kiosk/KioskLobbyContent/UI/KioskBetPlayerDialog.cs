using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.UI.Event;
using System.Runtime.InteropServices;

namespace JHchoi.UI
{
    public class KioskBetPlayerDialog : IDialog
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        int xPos = 0;
        int yPos = 0;

        public Button btnPlayerPlus;
        public Button btnPlayerMinus;

        public Button btnOK;
        public Button btnPrev;

        public Text txtPlayerCount10;
        public Text txtPlayerCount9;

        protected override void OnLoad()
        {
            base.OnLoad();
            btnPlayerPlus.onClick.AddListener(() => PlayerPlus()) ;
            btnPlayerMinus.onClick.AddListener(()=> PlayerMinus());
            btnOK.onClick.AddListener(() => InputPlayer());
            btnPrev.onClick.AddListener(() => Prev());
        }

        void PlayerPlus()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<PlayerPlusMsg>(new PlayerPlusMsg());
        }

        void PlayerMinus()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<PlayerMinusMsg>(new PlayerMinusMsg());
        }

        protected override void OnEnter()
        {
            AddMessage();
            SetCursorPos(xPos, yPos);
        }

        void AddMessage()
        {
            Message.AddListener<SetBetPlayerMsg>(SetBetPlayer);
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

        void InputPlayer()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<InputPlayerNumMsg>(new InputPlayerNumMsg());
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
            Message.RemoveListener<SetBetPlayerMsg>(SetBetPlayer);
        }

        protected override void OnUnload()
        {
            
        }

        public void OnMouseOver(bool isPlus)
        {
            Message.Send<PlayerInputButtonMouseOverMsg>(new PlayerInputButtonMouseOverMsg(isPlus));
        }

        public void OnMouseExit(bool isPlus)
        {
            Message.Send<PlayerInputButtonMouseOutMsg>(new PlayerInputButtonMouseOutMsg(isPlus));
        }
    }
}
