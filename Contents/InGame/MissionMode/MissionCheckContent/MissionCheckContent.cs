using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using System;
using System.Collections;
using UnityEngine;

namespace JHchoi.Contents
{
    public class MissionCheckContent : IMissionSucessCheck
    {
        static string TAG = "MissionCheckContent :: ";
        //int totalPlayerCount;
        protected override void MissionResult(bool isSuccess)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            MissionModeGame tempMission = playersModel.GetMissionModeNowMission(tempPlayerNum);
            if (isSuccess)
            {
                Debug.Log(TAG + "미션 성공");
            }
            else
            {
                Debug.Log(TAG + "미션 실패");
                if (tempMission == MissionModeGame.Gate_1)
                {
                    playersModel.SetMissionStartPosition(inGameplayModel.PlayerNum, cameraModel.Start_Position);
                }
            }

            playersModel.SetMissionSuccess(tempPlayerNum, tempMission, isSuccess);
            playersModel.SetIsMissionSuccess(tempPlayerNum, isSuccess);
            Message.Send<SetBetBoardUpdateMsg>(new SetBetBoardUpdateMsg());
        }

        protected override void ResultClose(bool isSuccess)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            MissionModeGame tempMission = playersModel.GetMissionModeNowMission(tempPlayerNum);
            int tempPlayerChance = playersModel.GetMissionChance(inGameplayModel.PlayerNum);
            if (isSuccess)
            {
                Debug.Log(TAG + "MissionEndPrecess : " + isSuccess);
                if (tempMission == MissionModeGame.Gate_1
                    || tempMission == MissionModeGame.Gate_2
                    || tempMission == MissionModeGame.Gate_3)
                {
                    playersModel.SetMissionModeNowMission(tempPlayerNum, tempMission + 1);
                }
            }
            else
            {
                Debug.Log(TAG + "MissionEndPrecess : " + isSuccess);
                playersModel.SetMissionChance(tempPlayerNum, tempPlayerChance - 1);
            }

            Message.Send<SetScoreBoardUpdateMsg>(new SetScoreBoardUpdateMsg());
            MissionTurnEnd();
        }

        void MissionTurnEnd()
        {
            //게임 종료 알림
            int gameEndPlayerCount = 0;
            for (int i = 0; i < totalPlayerCount; i++)
            {
                if (!playersModel.GetIsPlayPossible(i))
                {
                    Debug.Log(TAG + "GameEnd Player :" + i);
                    gameEndPlayerCount++;
                }
            }

            if (gameEndPlayerCount == totalPlayerCount)
            {
                Debug.Log(TAG + "Mission GameEnd");
                inGameplayModel.Round = inGameplayModel.Round + 1;
                Message.Send<MissionModeGameEndMsg>(new MissionModeGameEndMsg());
                return;
            }

            //순서 변경 
            if (inGameplayModel.PlayerNum == totalPlayerCount - 1)
            {
                //마지막 플레이 후 다음 플레이 가능 유저 탐색
                int tempNum = 0;
                while (true)
                {
                    if (playersModel.GetIsPlayPossible(tempNum))
                    {
                        Debug.Log(TAG + "Play Possible : " + tempNum);
                        break;
                    }
                    else
                        tempNum++;
                }
                inGameplayModel.PlayerNum = tempNum;
            }
            else
            {
                //플레이어 후 다음 플레이어가 플레이 가능한지 체크
                int tempNum = inGameplayModel.PlayerNum + 1;
                while (true)
                {
                    if (playersModel.GetIsPlayPossible(tempNum))
                    {
                        Debug.Log(TAG + "Play Possible : " + tempNum);
                        break;
                    }
                    else
                    {
                        //2인 플레이 때는 0으로 고정이지만 추후 플레이어 증가할 대비
                        tempNum++;
                        if (tempNum > totalPlayerCount - 1)
                        {
                            tempNum = 0;
                        }
                    }
                }
                inGameplayModel.PlayerNum = tempNum;
            }
            Message.Send<TurnChangeMsg>(new TurnChangeMsg());
        }


       
    }
}
