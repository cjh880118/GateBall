using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;

namespace CellBig.UI
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