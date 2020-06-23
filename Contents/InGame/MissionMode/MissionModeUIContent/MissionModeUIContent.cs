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
    public class MissionModeUIContent : IContent
    {
        static string TAG = "MissionModeUIContent :: ";
        InGamePlayModel inGameplayModel;
        PlayersModel playersModel;
        MissionLevelSettingModel levelSettingModel;
        TouchBallSettingModel touchBallSettingModel;

        List<Dictionary<MissionModeGame, bool>> ListMissionSuccess;
        Dictionary<int, int> DicPlayerMissionChance;
        Dictionary<MissionModeGame, MissionObject> dicMiniMapObject;
        bool isSpark = false;
        bool isErrorProcess = false;

        protected override void OnLoadStart()
        {
            inGameplayModel = Model.First<InGamePlayModel>();
            playersModel = Model.First<PlayersModel>();
            levelSettingModel = Model.First<MissionLevelSettingModel>();
            touchBallSettingModel = Model.First<TouchBallSettingModel>();
            Debug.Log(TAG + "OnLoadStart");
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            Init();
        }

        void Init()
        {
            ListMissionSuccess = new List<Dictionary<MissionModeGame, bool>>();
            DicPlayerMissionChance = new Dictionary<int, int>();
            dicMiniMapObject = new Dictionary<MissionModeGame, MissionObject>();
            MiniMapObjectSetting();
        }

        void MiniMapObjectSetting()
        {
            //미니맵 배치
            Level tempLevel = inGameplayModel.PlayLevel;
            dicMiniMapObject.Clear();
            dicMiniMapObject.Add(MissionModeGame.Gate_1, SetMissionObjectPostion(MissionModeGame.Gate_1, tempLevel));
            dicMiniMapObject.Add(MissionModeGame.Gate_2, SetMissionObjectPostion(MissionModeGame.Gate_2, tempLevel));
            dicMiniMapObject.Add(MissionModeGame.Gate_3, SetMissionObjectPostion(MissionModeGame.Gate_3, tempLevel));
            dicMiniMapObject.Add(MissionModeGame.Pole, SetMissionObjectPostion(MissionModeGame.Pole, tempLevel));
        }

        MissionObject SetMissionObjectPostion(MissionModeGame mission, Level level)
        {
            MissionObject missionObject = new MissionObject();
            if (mission == MissionModeGame.Gate_1)
            {
                missionObject.Position = NormalizationPosition(levelSettingModel.GetGate1Position(level));
                missionObject.Rotation = levelSettingModel.GetGate1Rotation(level);
            }
            else if (mission == MissionModeGame.Gate_2)
            {
                missionObject.Position = NormalizationPosition(levelSettingModel.GetGate2Position(level));
                missionObject.Rotation = levelSettingModel.GetGate2Rotation(level);
            }
            else if (mission == MissionModeGame.Gate_3)
            {
                missionObject.Position = NormalizationPosition(levelSettingModel.GetGate3Position(level));
                missionObject.Rotation = levelSettingModel.GetGate3Rotation(level);
            }
            else if (mission == MissionModeGame.Pole)
            {
                missionObject.Position = NormalizationPosition(levelSettingModel.GetPolePosition(level));
            }
            return missionObject;
        }

        //미니맵 배치용 노멀라이즈
        Vector3 NormalizationPosition(Vector3 value)
        {
            float maxWidth = 11.5f;
            float minWidth = -11.5f;

            float maxHeight = 9.5f;
            float minHeight = -9.5f;

            float tempX = (value.x - minWidth) / (maxWidth - minWidth);
            float tempY = (value.z - minHeight) / (maxHeight - minHeight);

            Vector3 tempVec3 = new Vector3(tempX, tempY, 0);
            return tempVec3;
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
            Message.AddListener<MissionModeGameEndMsg>(MissionModeGameEnd);
            Message.AddListener<SetScoreBoardUpdateMsg>(SetScoreBoardUpdate);
            Message.AddListener<TurnChangeMsg>(TurnChange);
            Message.AddListener<MissionResultCloseMsg>(MissionResultClose);
            Message.AddListener<ScoreBoardCloseMsg>(ScoreBoardClose);
            Message.AddListener<BallStartMsg>(BallStart);
            Message.AddListener<BallTouchMsg>(BallTouch);
            Message.AddListener<BallSparkCloseMsg>(BallSparkClose);
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
            UI.IDialog.RequestDialogExit<UI.MiniMapDialog>();
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
            Debug.Log(TAG + "MissionInfoClose");
            StartCoroutine(SensorStart());
        }

        IEnumerator SensorStart()
        {
            
            yield return new WaitForSeconds(2.0f);
            Debug.Log(TAG + "SensorStart");
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
            //todo..
            Debug.Log(TAG + "GateBallStart");
            UI.IDialog.RequestDialogExit<UI.AlertDialog>();

            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempMissionChance = playersModel.GetMissionChance(tempPlayerNum);
            Level tempLevel = inGameplayModel.PlayLevel;
            int tempTouchBallCount = touchBallSettingModel.GetBallCount(tempLevel);

            Vector3 tempVec3 = playersModel.GetMissionStartPosition(tempPlayerNum);
            Dictionary<int, Vector3> tempDicTouchBallNomalize = new Dictionary<int, Vector3>();

            for (int i = 0; i < tempTouchBallCount; i++)
            {
                if (playersModel.GetTouchBallAlive(tempPlayerNum, i))
                {
                    Vector3 tempNomalVec3 = NormalizationPosition(playersModel.GetTouchBallPosition(tempPlayerNum, i));
                    tempDicTouchBallNomalize.Add(i, tempNomalVec3);
                }
            }
            UI.IDialog.RequestDialogEnter<UI.MiniMapDialog>();
            Message.Send<SetMiniMapObjectMsg>(new SetMiniMapObjectMsg(playersModel.GetMissionModeNowMission(tempPlayerNum), dicMiniMapObject, NormalizationPosition(tempVec3), tempDicTouchBallNomalize, tempTouchBallCount));
        }

        void MissionModeGameEnd(MissionModeGameEndMsg msg)
        {
            UI.IDialog.RequestDialogEnter<UI.MissionScoreDialog>();
            SetResultBoardUpdate();
        }

        void SetResultBoardUpdate()
        {
            ListMissionSuccess.Clear();
            DicPlayerMissionChance.Clear();
            for (int i = 0; i < inGameplayModel.TotalPlayerCount; i++)
            {
                ListMissionSuccess.Add(playersModel.GetMissionSuccess(i));
                DicPlayerMissionChance.Add(i, playersModel.GetMissionChance(i));
            }

            Message.Send<SetResultScoreBoardMsg>(new SetResultScoreBoardMsg(inGameplayModel.TotalPlayerCount, ListMissionSuccess, DicPlayerMissionChance));
        }

        void SetScoreBoardUpdate(SetScoreBoardUpdateMsg msg)
        {
            ListMissionSuccess.Clear();
            DicPlayerMissionChance.Clear();
            for (int i = 0; i < inGameplayModel.TotalPlayerCount; i++)
            {
                ListMissionSuccess.Add(playersModel.GetMissionSuccess(i));
                DicPlayerMissionChance.Add(i, playersModel.GetMissionChance(i));
            }

            Message.Send<SetScoreBoardMsg>(new SetScoreBoardMsg(inGameplayModel.TotalPlayerCount, ListMissionSuccess, DicPlayerMissionChance));
        }



        void TurnChange(TurnChangeMsg msg)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempMissionChance = playersModel.GetMissionChance(tempPlayerNum);
            //인게임 다이얼로그 미션 셋팅
            SoundManager.Instance.PlaySound((int)SoundType.Mission_Popup);
            //SoundManager.Instance.SoundVolume((int)SoundType.Crowd_Effect, 0.3f);
            SoundType tempType;

            if (isSpark)
            {
                if (tempPlayerNum % 2 == 0)
                {
                    tempType = SoundType.Mission_Spark_Red;
                    //SoundManager.Instance.PlaySound((int)SoundType.Mission_Spark_Red);
                }
                else
                {
                    tempType = SoundType.Mission_Spark_White;
                    //SoundManager.Instance.PlaySound((int)SoundType.Mission_Spark_White);
                }
                isSpark = false;
            }
            else
            {
                if (tempPlayerNum % 2 == 0)
                {
                    tempType = SoundType.Mission_Player_Red;
                    //SoundManager.Instance.PlaySound((int)SoundType.Mission_Player_Red);
                }
                else
                {
                    tempType = SoundType.Mission_Player_White;
                }
            }

            SoundManager.Instance.PlaySound((int)tempType);
            //플레이어 다이얼로그 셋팅
            UI.IDialog.RequestDialogEnter<UI.PlayerDialog>();
            Message.Send<SetScoreBoardOrderInfoMsg>(new SetScoreBoardOrderInfoMsg(tempPlayerNum));
            Message.Send<SetPlayerInfoMsg>(new SetPlayerInfoMsg(tempPlayerNum, tempMissionChance, inGameplayModel.GameMode));
            //미션 및 플레이어에 따라 오브젝트 배치
            Message.Send<SetMissionObjectMsg>(new SetMissionObjectMsg(playersModel.GetMissionModeNowMission(tempPlayerNum)));

            StartCoroutine(MissionInfo(tempPlayerNum, tempType));
        }

        IEnumerator MissionInfo(int playerNum, SoundType soundType)
        {

            while (SoundManager.Instance.IsPlaySound((int)soundType))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            UI.IDialog.RequestDialogEnter<UI.InGameDialog>();
            Message.Send<SetMissionModeInfoMsg>(new SetMissionModeInfoMsg(playersModel.GetMissionModeNowMission(playerNum)));
        }

        private void MissionResultClose(MissionResultCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.MissionResultDialog>();
        }

        //스코어 보여준 후 게임 끝
        void ScoreBoardClose(ScoreBoardCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.MissionScoreDialog>();
            for (int i = 0; i < inGameplayModel.TotalPlayerCount; i++)
            {
                playersModel.InitMissionPlayerModel(i);
            }

            if (inGameplayModel.Round > inGameplayModel.TotalPlayRound)
            {
                //게임끝
                //UI.IDialog.RequestDialogEnter<UI.KioskGlobalDialog>();
                Message.Send<FadeInMsg>(new FadeInMsg(true, true, 0.1f));
                Scene.SceneManager.Instance.Load(Constants.SceneName.Title);
            }
            else
            {
                inGameplayModel.PlayerNum = 0;
                UI.IDialog.RequestDialogEnter<UI.GameStartDialog>();
                Message.Send<SetGameStartInfoMsg>(new SetGameStartInfoMsg(inGameplayModel.Round, inGameplayModel.PlayLevel, inGameplayModel.GameMode));
                Message.Send<SetMissionObjectMsg>(new SetMissionObjectMsg(playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum)));
            }
        }

        private void BallStart(BallStartMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.InGameDialog>();
            UI.IDialog.RequestDialogExit<UI.PlayerDialog>();
            UI.IDialog.RequestDialogExit<UI.MiniMapDialog>();
        }

        private void BallTouch(BallTouchMsg msg)
        {
            //스파크 창 띄우기
            isSpark = true;
            //SoundManager.Instance.PlaySound((int)SoundType.Mission_Spark);
            UI.IDialog.RequestDialogEnter<UI.SparkDialog>();
            Message.Send<SparkPlayerMsg>(new SparkPlayerMsg(inGameplayModel.PlayerNum));
        }

        private void BallSparkClose(BallSparkCloseMsg msg)
        {
            UI.IDialog.RequestDialogExit<UI.SparkDialog>();
            Message.Send<TurnChangeMsg>(new TurnChangeMsg());
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
            Message.RemoveListener<MissionModeGameEndMsg>(MissionModeGameEnd);
            Message.RemoveListener<SetScoreBoardUpdateMsg>(SetScoreBoardUpdate);
            Message.RemoveListener<TurnChangeMsg>(TurnChange);
            Message.RemoveListener<MissionResultCloseMsg>(MissionResultClose);
            Message.RemoveListener<ScoreBoardCloseMsg>(ScoreBoardClose);
            Message.RemoveListener<BallStartMsg>(BallStart);
            Message.RemoveListener<BallTouchMsg>(BallTouch);
            Message.RemoveListener<BallSparkCloseMsg>(BallSparkClose);
        }

        protected override void OnUnload()
        {
            Debug.Log(TAG + "OnUnload");
        }
    }
}
