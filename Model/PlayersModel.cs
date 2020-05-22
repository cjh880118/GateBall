using JHchoi.Constants;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Models
{
    public class PlayersModel : Model
    {
        GameModel _owner;
        PlayerModel[] playerModel;

        public void Setup(GameModel owner)
        {
            _owner = owner;
        }

        public void SetPlayer(int totlaRound, int playerCount)
        {
            playerModel = new PlayerModel[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerModel[i] = new PlayerModel();
                playerModel[i].Setup(_owner);
            }
        }
     
        public void SetIsMissionSuccess(int playerIndex, bool isSuccess)
        {
            playerModel[playerIndex].ListIsSuccessMission.Add(isSuccess);
        }

        //내기 모드 다시 하기 위한 마지막 시도 삭제
        public void SetMissionRemove(int playerIndex)
        {
            int num = playerModel[playerIndex].ListIsSuccessMission.Count;
            playerModel[playerIndex].ListIsSuccessMission.RemoveAt(num - 1);
        }

        public bool GetIsRoundPlayPossible(int playerIndex, int round)
        {
            //1라운드 모드 플레이 가능
            if (round == 1)
                return true;
            //현재 미션 수행 카운트가 라운드 보다 적을시 이미 아웃 상태
            else if (GetPlayMissionCount(playerIndex) < round - 1)
                return false;
            else
                return playerModel[playerIndex].ListIsSuccessMission[round - 2];
        }

        public int GetPlayMissionCount(int playerIndex)
        {
            return playerModel[playerIndex].ListIsSuccessMission.Count;
        }

        public void InitMissionPlayerModel(int playerIndex)
        {
            playerModel[playerIndex].InitMissionPlayerModel();
        }

        public void SetMissionSuccess(int playerIndex, MissionModeGame mission, bool isSuccess)
        {
            if (playerModel[playerIndex].DicMissionSuccess.ContainsKey(mission))
            {
                playerModel[playerIndex].DicMissionSuccess.Remove(mission);
            }
            playerModel[playerIndex].DicMissionSuccess.Add(mission, isSuccess);
        }

        public Dictionary<MissionModeGame, bool> GetMissionSuccess(int playerIndex)
        {
            return playerModel[playerIndex].DicMissionSuccess;
        }

        public List<bool> GetListMissionSuccess(int playerIndex)
        {
            return playerModel[playerIndex].ListIsSuccessMission;
        }

        public int GetBetModePlayPossiblePlayerCount()
        {
            int cnt = 0;
            for(int i = 0; i < playerModel.Length; i++)
            {
                if(playerModel[i].ListIsSuccessMission.Count == 0)
                {
                    cnt++;
                }
                else
                {
                    if (playerModel[i].ListIsSuccessMission[playerModel[i].ListIsSuccessMission.Count - 1])
                        cnt++;
                }
            }
            return cnt;
        }

        public void SetMissionModeNowMission(int playerIndex, MissionModeGame mission)
        {
            playerModel[playerIndex].NowMission = mission;
        }

        public MissionModeGame GetMissionModeNowMission(int playerIndex)
        {
            return playerModel[playerIndex].NowMission;
        }

        public void SetMissionChance(int playerIndex, int count)
        {
            playerModel[playerIndex].MissionChance = count;
        }

        public int GetMissionChance(int playerIndex)
        {
            return playerModel[playerIndex].MissionChance;
        }

        public bool GetIsPlayPossible(int playerIndex)
        {
            return playerModel[playerIndex].IsPlayPossible;
        }

        public Vector3 GetMissionStartPosition(int playerIndex)
        {
            return playerModel[playerIndex].Postition;
        }

        public void SetMissionStartPosition(int playerIndex, Vector3 position)
        {
            playerModel[playerIndex].Postition = position;
        }

        //터치볼 관련
        public Vector3 GetTouchBallPosition(int playerIndex, int ballNum)
        {
            return playerModel[playerIndex].DicTouchBallPosition[ballNum];
        }

        public bool GetTouchBallAlive(int playerIndex, int ballNum)
        {
            return playerModel[playerIndex].DicTouchBallPosition.ContainsKey(ballNum);
        }

        public void SetTouchBallPosition(int playerIndex, int ballNum, Vector3 position)
        {
            if (playerModel[playerIndex].DicTouchBallPosition.ContainsKey(ballNum))
            {
                playerModel[playerIndex].DicTouchBallPosition.Remove(ballNum);
            }
            playerModel[playerIndex].DicTouchBallPosition.Add(ballNum, position);
        }

        public void RemoveTouchBallPostion(int playerIndex, int ballNum)
        {
            playerModel[playerIndex].DicTouchBallPosition.Remove(ballNum);
        }

        public int GetTouchBallCount(int playerIndex)
        {
            return playerModel[playerIndex].DicTouchBallPosition.Count;
        }
    }
}