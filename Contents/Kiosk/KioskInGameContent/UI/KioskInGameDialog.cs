using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.UI.Event;

namespace JHchoi.UI
{
    public class KioskInGameDialog : IDialog
    {

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        void AddMessage()
        {
        
        }

        void OnPlayerInfo()
        {
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
            base.OnUnload();
        }
    }
}