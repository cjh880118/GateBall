using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class KioskTitleDialog : IDialog
    {
        static string TAG = "KioskTitleDialog :: ";

        public Button btnOnLobby;

        protected override void OnLoad()
        {
            base.OnLoad();
            btnOnLobby.onClick.AddListener(OnModeSelect);
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
        }

        void AddMessage()
        {

        }

        void OnModeSelect()
        {
            Debug.Log(TAG + "OnModeSelect");
            SoundManager.Instance.PlaySound((int)SoundType.Button_Input);
            Message.Send<OnLobbyMsg>(new OnLobbyMsg());
        }

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Debug.Log(TAG + "RemoveMessage");
        }

        protected override void OnUnload()
        {

        }
    }
}
