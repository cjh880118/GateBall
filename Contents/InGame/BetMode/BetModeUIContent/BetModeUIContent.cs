using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.T3;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public class BetModeUIContent : IContent
    {
        static string TAG = "InGameUIContent :: ";
        InGamePlayModel inGameplayModel;
        PlayersModel playersModel;
        MissionLevelSettingModel levelSettingModel;
        SettingModel settingModel;

        Dictionary<int, int> DicPlayerMissionChance;
        List<List<bool>> ListPlayerMissionSuccess;
        bool isErrorProcess = false;

        protected override void OnLoadStart()
        {
            inGameplayModel = Model.First<InGamePlayModel>();
            playersModel = Model.First<PlayersModel>();
            levelSettingModel = Model.First<MissionLevelSettingModel>();
            settingModel = Model.First<SettingModel>();
            Debug.Log(TAG + "OnLoadStart");
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
            //UI.IDialog.RequestDialogExit<UI.LoadingDialog>();
            Init();
        }

        void Init()
        {
            DicPlayerMissionChance = new Dictionary<int, int>();
            ListPlayerMissionSuccess = new List<List<bool>>();
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            
        }

        void AddMessage()
        {
            Message.AddListener<LoadingCompleteMsg>(LoadingComplete);
            Message.AddListener<T3SensorReRequestMsg>(T3SensorReRequest);
            Message.AddListener<RoundInfoCloseMsg>(RoundInfoClose);
            Message.AddListener<MissionInfoCloseMsg>(MissionInfoClose);
            Message.AddListener<MissionEndMsg>(MissionEnd);
            Message.AddListener<T3SensorStartMsg>(OnSensorReady);
            Message.AddListener<TempEditorSensorCheckMsg>(TempEditorSensorCheck);
            Message.AddListener<SetBetBoardUpdateMsg>(SetBetBoardUpdate);
            Message.AddListener<BetModeGameEndMsg>(BetModeGameEnd);
            Message.AddListener<TurnChangeMsg>(TurnChange);
            Message.AddListener<MissionResultCloseMsg>(MissionResultClose);
            Message.AddListener<ScoreBoardCloseMsg>(ScoreBoardClose);
            Message.AddListener<BallStartMsg>(BallStart);
        }

        private void LoadingComplete(LoadingCompleteMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.LoadingDialog>();
            UI.IDialog.RequestDialogEnter<UI.GameStartDialog>();
            Message.Send<SetGameStartInfoMsg>(new SetGameStartInfoMsg(inGameplayModel.Round, inGameplayModel.PlayLevel, inGameplayModel.GameMode));
        }

        private void T3SensorReRequest(T3SensorReRequestMsg msg)
        {
            //센서값 재요청 5초뒤 다시 굴려라
            if (isErrorProcess)
                return;

            isErrorProcess = true;
            UI.IDialog.RequestDialogEnter<UI.AlertDialog>();
            StartCoroutine(ResetMission());
        }

        IEnumerator ResetMission()
        {
            UI.IDialog.RequestDialogExit<UI.InGameDialog>();
            UI.IDialog.RequestDialogExit<UI.PlayerDialog>();
            yield return new WaitForSeconds(3.0f);
            isErrorProcess = false;
            UI.IDialog.RequestDialogExit<UI.AlertDialog>();
            TurnChange(new TurnChangeMsg());
        }

        void MissionEnd(MissionEndMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.InGameDialog>();
            UI.IDialog.RequestDialogExit<UI.PlayerDialog>();
            UI.IDialog.RequestDialogEnter<UI.MissionResultDialog>();
            Message.Send<SetMissionResultInfoMsg>(new SetMissionResultInfoMsg(msg.isSuccess));
        }

        //라운드 표시 후 미션, 플레이어 다이얼로그 키기
        void RoundInfoClose(RoundInfoCloseMsg msg)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempMissionChance = playersModel.GetMissionChance(tempPlayerNum);
            UI.IDialog.RequestDialogExit<UI.GameStartDialog>();
            Message.Send<TurnChangeMsg>(new TurnChangeMsg());
        }

        //미션 알림 표시
        void MissionInfoClose(MissionInfoCloseMsg msg)
        {
            StartCoroutine(SensorStart());
        }

        IEnumerator SensorStart()
        {
            yield return new WaitForSeconds(2.0f);
            Message.Send<T3SensorCatchMsg>(new T3SensorCatchMsg(true));
#if (UNITY_EDITOR)
            Message.Send<TempEditorSensorCheckMsg>(new TempEditorSensorCheckMsg());
#endif
        }

        void OnSensorReady(T3SensorStartMsg msg)
        {
            GateBallStart();
        }

        void TempEditorSensorCheck(TempEditorSensorCheckMsg msg)
        {
            GateBallStart();
        }

        //미션 시작
        void GateBallStart()
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempMissionChance = playersModel.GetMissionChance(tempPlayerNum);
            Vector3 tempVec3 = playersModel.GetMissionStartPosition(tempPlayerNum);
        }

        private void SetBetBoardUpdate(SetBetBoardUpdateMsg msg)
        {
            // 넘겨야 하는 데이터
            // 플레이어 정보 
            ListPlayerMissionSuccess.Clear();
            for (int i = 0; i < inGameplayModel.TotalPlayerCount; i++)
            {
                ListPlayerMissionSuccess.Add(playersModel.GetListMissionSuccess(i));
            }

            Message.Send<SetBetBoardMsg>(new SetBetBoardMsg(ListPlayerMissionSuccess));
        }

        void BetModeGameEnd(BetModeGameEndMsg msg)
        {
            if (msg.IsGameEnd)
            {
                //todo..
                //SoundManager.Instance.PlaySound((int)SoundType.Crowd_Effect);
                UI.IDialog.RequestDialogEnter<UI.BetScoreDialog>();
                SetBetBoardUpdate(new SetBetBoardUpdateMsg());
            }
            else
            {
                //턴바꾸기
                UI.IDialog.RequestDialogEnter<UI.GameStartDialog>();
                Message.Send<SetGameStartInfoMsg>(new SetGameStartInfoMsg(inGameplayModel.Round, inGameplayModel.PlayLevel, inGameplayModel.GameMode));
                Message.Send<SetMissionObjectMsg>(new SetMissionObjectMsg(MissionModeGame.Gate_1));
                SetBetBoardUpdate(new SetBetBoardUpdateMsg());

                //키오스크 라운드 갱신
                Message.Send<SetBetKioskRoundUpdateMsg>(new SetBetKioskRoundUpdateMsg(inGameplayModel.Round));

                Debug.Log("다음 라운드 오브젝트 배치");
            }
        }

        void TurnChange(TurnChangeMsg msg)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempMissionChance = playersModel.GetMissionChance(tempPlayerNum);
            SoundManager.Instance.PlaySound((int)SoundType.Mission_Popup);
            SoundManager.Instance.PlaySound((int)SoundType.Player_Order);
            //if (tempPlayerNum % 2 == 0)
            //{
            //    SoundManager.Instance.PlaySound((int)SoundType.Player_Order);
            //}
            //else
            //{
            //    SoundManager.Instance.PlaySound((int)SoundType.Player_Order);
            //}

            //플레이어 다이얼로그 셋팅
            UI.IDialog.RequestDialogEnter<UI.PlayerDialog>();
            Message.Send<SetPlayerInfoMsg>(new SetPlayerInfoMsg(tempPlayerNum, tempMissionChance, inGameplayModel.GameMode));

            //키오스크 순서 표시
            Message.Send<SetBetBoardInfoMsg>(new SetBetBoardInfoMsg(inGameplayModel.Round, tempPlayerNum));
            Message.Send<SetScoreBoardOrderInfoMsg>(new SetScoreBoardOrderInfoMsg(tempPlayerNum));

            if (settingModel.IsTestMode)
            {
                UI.IDialog.RequestDialogEnter<UI.InGameDialog>();
                Message.Send<SetBetModeInfoMsg>(new SetBetModeInfoMsg(inGameplayModel.BetModeMission, playersModel.GetBetModePlayPossiblePlayerCount(), inGameplayModel.Round));
            }
            else
            {
                StartCoroutine(MissionInfo(tempPlayerNum));
            }
        }

        IEnumerator MissionInfo(int playerNum)
        {
            while (SoundManager.Instance.IsPlaySound((int)SoundType.Player_Order))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            UI.IDialog.RequestDialogEnter<UI.InGameDialog>();
            Message.Send<SetBetModeInfoMsg>(new SetBetModeInfoMsg(inGameplayModel.BetModeMission, playersModel.GetBetModePlayPossiblePlayerCount(), inGameplayModel.Round));
        }

        private void MissionResultClose(MissionResultCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.MissionResultDialog>();
        }

        //스코어 보여준 후 게임 끝
        void ScoreBoardClose(ScoreBoardCloseMsg msg)
        {
            Message.Send<FadeInMsg>(new FadeInMsg(true, true, 0.1f));
            Scene.SceneManager.Instance.Load(Constants.SceneName.Title);
        }

        private void BallStart(BallStartMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.InGameDialog>();
            UI.IDialog.RequestDialogExit<UI.PlayerDialog>();
        }

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            //SoundManager.Instance.StopSound((int)SoundType.Crowd_Effect, true);
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<LoadingCompleteMsg>(LoadingComplete);
            Message.RemoveListener<T3SensorReRequestMsg>(T3SensorReRequest);
            Message.RemoveListener<RoundInfoCloseMsg>(RoundInfoClose);
            Message.RemoveListener<MissionInfoCloseMsg>(MissionInfoClose);
            Message.RemoveListener<MissionEndMsg>(MissionEnd);
            Message.RemoveListener<T3SensorStartMsg>(OnSensorReady);
            Message.RemoveListener<TempEditorSensorCheckMsg>(TempEditorSensorCheck);
            Message.RemoveListener<SetBetBoardUpdateMsg>(SetBetBoardUpdate);
            Message.RemoveListener<BetModeGameEndMsg>(BetModeGameEnd);
            Message.RemoveListener<TurnChangeMsg>(TurnChange);
            Message.RemoveListener<MissionResultCloseMsg>(MissionResultClose);
            Message.RemoveListener<ScoreBoardCloseMsg>(ScoreBoardClose);
            Message.RemoveListener<BallStartMsg>(BallStart);
        }

        protected override void OnUnload()
        {
            Debug.Log(TAG + "OnUnload");
        }
    }
}
