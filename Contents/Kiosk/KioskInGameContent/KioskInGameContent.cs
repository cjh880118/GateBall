 using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public class KioskInGameContent : IContent
    {
        static string TAG = "KioskInGameContent :: ";

        PlayersModel playersModel;
        InGamePlayModel inGameplayModel;

        protected override void OnLoadStart()
        {
            inGameplayModel = Model.First<InGamePlayModel>();
            playersModel = Model.First<PlayersModel>();
            Debug.Log(TAG + "OnLoadComplete");
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            if (inGameplayModel.GameMode == ModeType.MissionMode)
            {
                UI.IDialog.RequestDialogEnter<UI.KioskMissionScoreDialog>();
                Message.Send<SetScoreBoardUpdateMsg>(new SetScoreBoardUpdateMsg());
            }
            else
            {
                UI.IDialog.RequestDialogEnter<UI.KioskBetScoreDialog>();
                Message.Send<SetBetBoardUpdateMsg>(new SetBetBoardUpdateMsg());
            }
            AddMessage();
        }

        void AddMessage()
        {
         
        }

      

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            UI.IDialog.RequestDialogExit<UI.KioskMissionScoreDialog>();
            RemoveMessage();
        }

        void RemoveMessage()
        {
         
        }

        protected override void OnUnload()
        {

        }

    }
}