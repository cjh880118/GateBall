using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using UnityEngine;

namespace JHchoi.Contents
{
    public class KioskLobbyContent : IContent
    {
        static string TAG = "KioskLobbyContent :: ";

        PlayersModel playersModel;
        InGamePlayModel inGameplayModel;

        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadComplete");
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
            Init();
        }

        void Init()
        {
            inGameplayModel = Model.First<InGamePlayModel>();
            playersModel = Model.First<PlayersModel>();
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            UI.IDialog.RequestDialogEnter<UI.KioskModeSelectDialog>();
            SoundManager.Instance.PlaySound((int)SoundType.LobbyMode_Select);
        }

        void AddMessage()
        {
            Message.AddListener<OnModeSelectMsg>(OnModeSelect);
            Message.AddListener<OnLevelSelectMsg>(OnLevelSelect);
            Message.AddListener<InputPlayerNumMsg>(InputPlayerNum);
            Message.AddListener<PrevModeSelectMsg>(PrevModeSelect);
            Message.AddListener<PlayerPlusMsg>(PlayerPlus);
            Message.AddListener<PlayerMinusMsg>(PlayerMinus);
        }

        void OnModeSelect(OnModeSelectMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.KioskModeSelectDialog>();
            inGameplayModel.GameMode = msg.modeType;

            if (msg.modeType == ModeType.BetMode)
            {
                UI.IDialog.RequestDialogEnter<UI.KioskBetPlayerDialog>();
                inGameplayModel.TotalPlayerCount = 2;
                Message.Send<SetBetPlayerMsg>(new SetBetPlayerMsg(inGameplayModel.TotalPlayerCount));
            }
            else if (msg.modeType == ModeType.MissionMode)
            {
                UI.IDialog.RequestDialogEnter<UI.KioskLevelSelectDialog>();
                SoundManager.Instance.PlaySound((int)SoundType.Mission_Level_Select);
            }
        }

        void OnLevelSelect(OnLevelSelectMsg msg)
        {
            inGameplayModel.PlayLevel = msg.level;
            inGameplayModel.TotalPlayRound = 1;
            inGameplayModel.TotalPlayerCount = 2;
            playersModel.SetPlayer(inGameplayModel.TotalPlayRound, inGameplayModel.TotalPlayerCount);
            UI.IDialog.RequestDialogExit<UI.KioskLevelSelectDialog>();
            //todo..
            Scene.SceneManager.Instance.Load(Constants.SceneName.MissionMode);
        }

        void InputPlayerNum(InputPlayerNumMsg msg)
        {
            inGameplayModel.TotalPlayRound = 10;
            playersModel.SetPlayer(inGameplayModel.TotalPlayRound, inGameplayModel.TotalPlayerCount);
            UI.IDialog.RequestDialogExit<UI.KioskBetPlayerDialog>();
            //todo..
            Scene.SceneManager.Instance.Load(Constants.SceneName.BetMode);
        }


        void PrevModeSelect(PrevModeSelectMsg msg)
        {
            UI.IDialog.RequestDialogEnter<UI.KioskModeSelectDialog>();
            UI.IDialog.RequestDialogExit<UI.KioskLevelSelectDialog>();
            UI.IDialog.RequestDialogExit<UI.KioskBetPlayerDialog>();
        }

        void PlayerMinus(PlayerMinusMsg msg)
        {
            if (inGameplayModel.TotalPlayerCount <= 2) return;
            inGameplayModel.TotalPlayerCount = inGameplayModel.TotalPlayerCount - 1;
            Message.Send<SetBetPlayerMsg>(new SetBetPlayerMsg(inGameplayModel.TotalPlayerCount));
        }

        void PlayerPlus(PlayerPlusMsg msg)
        {
            if (inGameplayModel.TotalPlayerCount >= 10) return;
            inGameplayModel.TotalPlayerCount = inGameplayModel.TotalPlayerCount + 1;
            Message.Send<SetBetPlayerMsg>(new SetBetPlayerMsg(inGameplayModel.TotalPlayerCount));
        }

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<OnModeSelectMsg>(OnModeSelect);
            Message.RemoveListener<OnLevelSelectMsg>(OnLevelSelect);
            Message.RemoveListener<InputPlayerNumMsg>(InputPlayerNum);
            Message.RemoveListener<PrevModeSelectMsg>(PrevModeSelect);
            Message.RemoveListener<PlayerPlusMsg>(PlayerPlus);
            Message.RemoveListener<PlayerMinusMsg>(PlayerMinus);
        }
    }
}