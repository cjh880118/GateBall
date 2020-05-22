using JHchoi.Models;
using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public class KioskTitleContent : IContent
    {
        static string TAG = "KioskTitleContent :: ";

        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadComplete");
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
        }
        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.KioskTitleDialog>();
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<OnLobbyMsg>(OnLobby);
        }

        void OnLobby(OnLobbyMsg msg)
        {
            Scene.SceneManager.Instance.Load(Constants.SceneName.Lobby);
        }


        protected override void OnExit()
        {
            UI.IDialog.RequestDialogExit<UI.KioskTitleDialog>();
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<OnLobbyMsg>(OnLobby);
        }
    }
}