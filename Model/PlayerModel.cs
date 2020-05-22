using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.Contents;
using JHchoi.Constants;

namespace JHchoi.Models
{
    public class PlayerModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            listIsSuccessMission = new List<bool>();
            dicMissionSuccess = new Dictionary<MissionModeGame, bool>();
            dicTouchBallPosition = new Dictionary<int, Vector3>();
        }

        int missionChance;
        public int MissionChance
        {
            set { missionChance = value; }
            get { return missionChance; }
        }

        Dictionary<MissionModeGame, bool> dicMissionSuccess;
        public Dictionary<MissionModeGame, bool> DicMissionSuccess
        {
            set { dicMissionSuccess = value; }
            get { return dicMissionSuccess; }
        }

        public bool IsPlayPossible
        {
            get
            {
                int TempCount = 0;
                for (int i = 0; i < listIsSuccessMission.Count; i++)
                {
                    if (listIsSuccessMission[i])
                    {
                        TempCount++;
                    }
                }
                if (TempCount == 4 || missionChance <= 0)
                    return false;
                else
                    return true;
            }
        }

        List<bool> listIsSuccessMission;
        public List<bool> ListIsSuccessMission
        {
            set { listIsSuccessMission = value; }
            get { return listIsSuccessMission; }
        }

        MissionModeGame nowMission = MissionModeGame.Gate_1;
        public MissionModeGame NowMission
        {
            set { nowMission = value; }
            get { return nowMission; }
        }


        //내기 모드
        BetModeGame betNowMission;
        public BetModeGame BetNowMission
        {
            set { betNowMission = value; }
            get { return betNowMission; }
        }

        Vector3 position;
        public Vector3 Postition
        {
            set { position = value; }
            get { return position; }
        }

        Dictionary<int, Vector3> dicTouchBallPosition;
        public Dictionary<int, Vector3> DicTouchBallPosition
        {
            set { dicTouchBallPosition = value; }
            get { return dicTouchBallPosition; }
        }

        public void InitMissionPlayerModel()
        {
            missionChance = 3;
            dicMissionSuccess.Clear();
            listIsSuccessMission.Clear();
            dicTouchBallPosition.Clear();
            nowMission = MissionModeGame.Gate_1;
        }
    }
}
