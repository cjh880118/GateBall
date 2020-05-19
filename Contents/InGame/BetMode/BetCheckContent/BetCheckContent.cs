using CellBig.Constants;
using CellBig.Models;
using CellBig.UI.Event;
using System;
using System.Collections;
using UnityEngine;

namespace CellBig.Contents
{
    public class BetCheckContent : IMissionSucessCheck
    {
        static string TAG = "BetCheckContent :: ";

        BetModeModel betModeModel;
        int totalPlayRound;

        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
            betModeModel = Model.First<BetModeModel>();
            base.OnLoadStart();
            //SetLoadComplete();
        }

        protected override void MissionResult(bool isSuccess)
        {
            int tempPlayerNum = inGameplayModel.PlayerNum;
            playersModel.SetIsMissionSuccess(tempPlayerNum, isSuccess);
            Message.Send<SetBetBoardUpdateMsg>(new SetBetBoardUpdateMsg());
        }

        protected override void ResultClose(bool isSuccess)
        {
            //내가 공을 치고 다음 플레이어가 현재 플레이 가능한지 체크
            int tempPlayerNum = inGameplayModel.PlayerNum;
            int tempRound = inGameplayModel.Round;

            //현재 라운드에 플레이 가능 유저 탐색
            for (int i = tempPlayerNum + 1; i < totalPlayerCount; i++)
            {
                if (playersModel.GetIsRoundPlayPossible(i, tempRound))
                {
                    inGameplayModel.PlayerNum = i;
                    Message.Send<TurnChangeMsg>(new TurnChangeMsg());
                    return;
                }
            }

            //다음 플레이 가능유저가 없을시 라운드 증가 후 다음 라운드 플레이 가능 유저 수 파악
            int tempPlayPossibleCount = 0;
            for (int i = 0; i < totalPlayerCount; i++)
            {
                if (playersModel.GetIsRoundPlayPossible(i, tempRound + 1))
                {
                    tempPlayPossibleCount++;
                }
            }

            //라운드 진행 가능 플레이어가 2명 이상이면 다음 라운드 진행
            if (tempPlayPossibleCount >= 2)
            {
                for (int i = 0; i < totalPlayerCount; i++)
                {
                    if (playersModel.GetIsRoundPlayPossible(i, tempRound + 1))
                    {
                        inGameplayModel.PlayerNum = i;
                        inGameplayModel.BetLevel += betModeModel.LvUp;
                        inGameplayModel.Round += 1;
                        //배팅 모드 종료
                        //다음 라운드 진행
                        Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(false));
                        return;
                    }
                }
            }
            else if (tempPlayPossibleCount == 1)
            {
                //게임이 종료
                Debug.Log(TAG + "배팅 모드 게임 종료");
                Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(true));
            }
            else
            {
                //모두 탈락시 현재 라운드 다시 
                int num = 100;
                for (int i = 0; i < totalPlayerCount; i++)
                {
                    if (playersModel.GetIsRoundPlayPossible(i, tempRound))
                    {
                        if (num == 100)
                            num = i;
                        //모델에 실패 데이터 삭제
                        playersModel.SetMissionRemove(i);
                        playersModel.SetIsMissionSuccess(i, true);
                        playersModel.SetMissionChance(i, 1);
                    }
                }

                inGameplayModel.BetLevel -= betModeModel.LvDown;
                if (inGameplayModel.BetLevel < betModeModel.StartLV)
                    inGameplayModel.BetLevel = betModeModel.StartLV;

                inGameplayModel.Round += 1;
                inGameplayModel.PlayerNum = num;

                Debug.Log(TAG + "모두 탈락");
                Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(false));
            }
        }
    }
}
