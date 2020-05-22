using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class TitleDialog : IDialog
    {
        public Image imgLog;
        public Text txtInfo;

        protected override void OnEnter()
        {
            //UI.IDialog.RequestDialogExit<UI.KioskGlobalDialog>();
            Message.Send<FadeOutMsg>(new FadeOutMsg(true, true, 0.5f));
        }

        protected override void OnExit()
        {

        }
    }
}