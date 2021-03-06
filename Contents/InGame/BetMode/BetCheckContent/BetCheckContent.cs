﻿using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.UI.Event;
using System;
using System.Collections;
using UnityEngine;

namespace JHchoi.Contents
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
            int playerNum = inGameplayModel.PlayerNum;
            int nowRound = inGameplayModel.Round;

            //현재 라운드에 플레이 가능 유저 탐색
            for (int i = playerNum + 1; i < totalPlayerCount; i++)
            {
                if (playersModel.GetIsRoundPlayPossible(i, nowRound))
                {
                    inGameplayModel.PlayerNum = i;
                    Message.Send<TurnChangeMsg>(new TurnChangeMsg());
                    return;
                }
            }

            //다음 플레이 가능유저가 없을시 라운드 증가 후 다음 라운드 플레이 가능 유저 수 파악
            int playPossibleCount = 0;
            for (int i = 0; i < totalPlayerCount; i++)
            {
                if (playersModel.GetIsRoundPlayPossible(i, nowRound + 1))
                {
                    playPossibleCount++;
                }
            }

            //라운드 진행 가능 플레이어가 2명 이상이면 다음 라운드 진행
            if (playPossibleCount >= 2)
            {
                for (int i = 0; i < totalPlayerCount; i++)
                {
                    if (playersModel.GetIsRoundPlayPossible(i, nowRound + 1))
                    {
                        //배팅 모드 종료
                        //다음 라운드 진행
                        inGameplayModel.PlayerNum = i;
                        inGameplayModel.BetLevel += betModeModel.LvUp;
                        inGameplayModel.Round += 1;
                        Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(false));
                        return;
                    }
                }
            }
            else if (playPossibleCount == 1)
            {
                //게임이 종료
                Debug.Log(TAG + "내기 모드 게임 종료");
                Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(true));
            }
            else
            {
                //모두 탈락시 현재 라운드 다시 
                Debug.Log(TAG + "모두 탈락");
                int num = 100;
                for (int i = 0; i < totalPlayerCount; i++)
                {
                    if (playersModel.GetIsRoundPlayPossible(i, nowRound))
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
                Message.Send<BetModeGameEndMsg>(new BetModeGameEndMsg(false));
            }
        }
    }
}
