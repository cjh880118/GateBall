using CellBig.Constants;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class LoadingDialog : IDialog
    {
        public Text txtComment;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<LoadingModeInfoMsg>(LoadingModeInfo);
        }

        private void LoadingModeInfo(LoadingModeInfoMsg msg)
        {
            if (msg.mode == ModeType.MissionMode)
                txtComment.text = "미션 모드는 상대와 미션을 누가 많이 깨는지 경쟁하는 모드입니다.";
            else
                txtComment.text = "경쟁 모드는 많은 경쟁자들 중 최후까지 살아 남아야하는 모드입니다.";
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<LoadingModeInfoMsg>(LoadingModeInfo);
        }
    }
}