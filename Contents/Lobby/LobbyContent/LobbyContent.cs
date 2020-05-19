using CellBig.Constants;
using CellBig.Models;
using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Contents
{
    public class LobbyContent : IContent
    {
        static string TAG = "LobbyContent :: ";
        List<Coroutine> ListSoundLoop = new List<Coroutine>();

        protected override void OnEnter()
        {
            UI.IDialog.RequestDialogEnter<UI.ModeSelectDialog>();
            SoundManager.Instance.PlaySound((int)SoundType.Mission_BGM);
            //ListSoundLoop.Add(StartCoroutine(SoundBgm()));
            AddMessage();
        }

        IEnumerator SoundBgm()
        {
            while (true)
            {
                yield return new WaitForSeconds(12);
                SoundManager.Instance.PlaySound((int)SoundType.Title_BGM);
            }
        }

        void AddMessage()
        {
            Message.AddListener<OnModeSelectMsg>(OnModeSelect);
            Message.AddListener<PrevModeSelectMsg>(PrevModeSelect);
        }

        void OnModeSelect(OnModeSelectMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.ModeSelectDialog>();

            if (msg.modeType == ModeType.BetMode)
            {
                UI.IDialog.RequestDialogEnter<UI.BetPlayerDialog>();
            }
            else if (msg.modeType == ModeType.MissionMode)
            {
                UI.IDialog.RequestDialogEnter<UI.LevelSelectDialog>();
            }
        }

        void PrevModeSelect(PrevModeSelectMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.BetPlayerDialog>();
            UI.IDialog.RequestDialogExit<UI.LevelSelectDialog>();
            UI.IDialog.RequestDialogEnter<UI.ModeSelectDialog>();
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound((int)SoundType.Mission_BGM);
            foreach (var o in ListSoundLoop)
                StopCoroutine(o);

            ListSoundLoop.Clear();
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<OnModeSelectMsg>(OnModeSelect);
            Message.RemoveListener<PrevModeSelectMsg>(PrevModeSelect);
        }
    }
}
