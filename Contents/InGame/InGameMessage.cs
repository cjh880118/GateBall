using CellBig.Constants;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Contents;

namespace CellBig.UI.Event
{
    public class LoadingCompleteMsg : Message
    {

    }

    public class SetMissionResultInfoMsg : Message
    {
        public bool isSuccess;

        public SetMissionResultInfoMsg(bool isSuccess)
        {
            this.isSuccess = isSuccess;
        }
    }

    public class MissionResultCloseMsg : Message
    {
        public bool isSuccess;
        public MissionResultCloseMsg(bool isSuccess)
        {
            this.isSuccess = isSuccess;
        }
    }

    public class SetPlayerInfoMsg : Message
    {
        public int nowPlayerNum;
        public int missionChance;
        public ModeType gameMode;

        public SetPlayerInfoMsg(int nowPlayer, int missinChance, ModeType gameMode)
        {
            nowPlayerNum = nowPlayer;
            this.missionChance = missinChance;
            this.gameMode = gameMode;
        }
    }

    public class RoundInfoCloseMsg : Message
    {
    }

    public class MissionEndMsg : Message
    {
        public bool isSuccess;

        public MissionEndMsg(bool success)
        {
            isSuccess = success;
        }
    }

    public class TurnChangeMsg : Message
    {
    }

    public class SetGameStartInfoMsg : Message
    {
        public int round;
        public Level level;
        public ModeType modeType;
        public SetGameStartInfoMsg(int setRound, Level setLevel, ModeType setModeType)
        {
            round = setRound;
            level = setLevel;
            modeType = setModeType;
        }
    }

    public class TimeOutMsg : Message
    {
    }

    public class BallStartMsg : Message
    {
    }

    public class BallStopMsg : Message
    {
        public Vector3 ballPosition;
        public BallStopMsg(Vector3 position)
        {
            ballPosition = position;
        }
    }

    public class BallTouchMsg : Message
    {
    }

    public class BallSparkCloseMsg : Message
    {
    }

    public class MissionModeGameEndMsg : Message
    {
    }

    public class MissionInfoCloseMsg : Message
    {
    }

    public class MissionTimerStartMsg : Message
    {
        public float goalDistance;
        //public Vector3 targetPosition;
        public MissionTimerStartMsg(float goalDistance)//, Vector3 targetPosition)
        {
            this.goalDistance = goalDistance;
            //  this.targetPosition = targetPosition;
        }
    }

    public class SetMissionModeInfoMsg : Message
    {
        public MissionModeGame mission;
        public SetMissionModeInfoMsg(MissionModeGame mission)
        {
            this.mission = mission;
        }
    }

    public class SetBetModeInfoMsg : Message
    {
        public BetModeGame bet;
        public int alivePlayerCount;
        public int round;

        public SetBetModeInfoMsg(BetModeGame bet, int alivePlayerCount, int round)
        {
            this.bet = bet;
            this.alivePlayerCount = alivePlayerCount;
            this.round = round;
        }
    }

    public class SetMissionObjectMsg : Message
    {
        public MissionModeGame mission;
        public SetMissionObjectMsg(MissionModeGame mission)
        {
            this.mission = mission;
        }
    }

    public class SetScoreBoardMsg : Message
    {
        public int totalPlayerCount;
        public List<Dictionary<MissionModeGame, bool>> listMissionSuccess;
        public Dictionary<int, int> dicPlayerMissionChance;
        public SetScoreBoardMsg(int totalPlayerCount, List<Dictionary<MissionModeGame, bool>> listMissionSuccess, Dictionary<int, int> dicPlayerMissionChance)
        {
            this.totalPlayerCount = totalPlayerCount;
            this.listMissionSuccess = listMissionSuccess;
            this.dicPlayerMissionChance = dicPlayerMissionChance;
        }
    }

    public class SetResultScoreBoardMsg : Message
    {
        public int totalPlayerCount;
        public List<Dictionary<MissionModeGame, bool>> listMissionSuccess;
        public Dictionary<int, int> dicPlayerMissionChance;
        public SetResultScoreBoardMsg(int totalPlayerCount, List<Dictionary<MissionModeGame, bool>> listMissionSuccess, Dictionary<int, int> dicPlayerMissionChance)
        {
            this.totalPlayerCount = totalPlayerCount;
            this.listMissionSuccess = listMissionSuccess;
            this.dicPlayerMissionChance = dicPlayerMissionChance;
        }
    }

    public class SetScoreBoardOrderInfoMsg : Message
    {
        public int order;
        public SetScoreBoardOrderInfoMsg(int order)
        {
            this.order = order;
        }
    }

    public class ScoreBoardCloseMsg : Message
    {
    }

    public class SetScoreBoardUpdateMsg : Message
    {

    }

    public class SetScoreBoardResultUpdateMsg : Message
    {

    }

    public class BetModeGameEndMsg : Message
    {
        public bool IsGameEnd;
        public BetModeGameEndMsg(bool isGameEnd)
        {
            IsGameEnd = isGameEnd;
        }
    }

    public class SetBetBoardInfoMsg : Message
    {
        public int round;
        public int playerNum;
        public SetBetBoardInfoMsg(int round, int playerNum)
        {
            this.round = round;
            this.playerNum = playerNum;
        }
    }

    public class SetBetBoardMsg : Message
    {
        public List<List<bool>> listIsMissionSccess;
        public SetBetBoardMsg(List<List<bool>> listIsMissionSccess)
        {
            this.listIsMissionSccess = listIsMissionSccess;
        }
    }

    public class SetBetBoardUpdateMsg : Message
    {

    }

    public class SetNextRoundMsg : Message
    {
    }

    public class SetMiniMapObjectMsg : Message
    {
        public MissionModeGame mission;
        public Dictionary<MissionModeGame, MissionObject> dicMissionObject;
        public Vector3 ballPosition;
        public Dictionary<int, Vector3> dicTouchBall;
        public int touchBallcount;
        public SetMiniMapObjectMsg(MissionModeGame mission, Dictionary<MissionModeGame, MissionObject> MissionObject, Vector3 ballPosition, Dictionary<int, Vector3> touchBallPosition, int touchBallcount)
        {
            this.mission = mission;
            this.dicMissionObject = MissionObject;
            this.ballPosition = ballPosition;
            this.dicTouchBall = touchBallPosition;
            this.touchBallcount = touchBallcount;
        }
    }

    public class CameraMoveMsg : Message
    {
        public GameObject Ball;
        public GameObject Camera;
        public CameraMoveMsg(GameObject ball, GameObject camera)
        {
            Ball = ball;
            Camera = camera;
        }

    }

    public class SensorSettingMsg : Message
    {
    }

    public class SensorValueMsg : Message
    {
        public float sensor1_Value;
        public float sensor2_Value;
        public float sensor1_Fix_Value;
        public float sensor2_Fix_Value;

        public SensorValueMsg(float val1, float val2, float fixVal1, float fixVal2)
        {
            sensor1_Value = val1;
            sensor2_Value = val2;
            sensor1_Fix_Value = fixVal1;
            sensor2_Fix_Value = fixVal2;
        }
    }

    public class BallSettingMsg : Message
    {
    }

    public class MissionEffectMsg : Message
    {

    }

    public class SparkPlayerMsg : Message
    {
        public int playerNum;
        public SparkPlayerMsg(int playerNum)
        {
            this.playerNum = playerNum;
        }
    }

    public class BetSendGameEndToKioskMsg : Message
    {

    }

    public class LoadingModeInfoMsg: Message
    {
        public ModeType mode;
        public LoadingModeInfoMsg(ModeType mode)
        {
            this.mode = mode;
        }
    }

    public class TempEditorSensorCheckMsg : Message
    {
    }
}