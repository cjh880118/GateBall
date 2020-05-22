using JHchoi.Constants;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class KioskMissionScoreDialog : IDialog
    {
        static string TAG = "KioskMissionScoreDialog :: ";
        public GameObject gameResult;
        public GameObject orderInfo1;
        public GameObject orderInfo2;
        public ScoreBoardPlayer_Controller[] Player;
        public GameObject[] WinLine;
        public Text txtInPlay;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            txtInPlay.text = "플레이 중 입니다.";
            gameResult.SetActive(false);
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<SetScoreBoardMsg>(SetScoreBoard);
            Message.AddListener<SetScoreBoardOrderInfoMsg>(SetScoreBoardOrderInfo);
            Message.AddListener<SetResultScoreBoardMsg>(SetResultScoreBoard);
        }

        private void SetScoreBoardOrderInfo(SetScoreBoardOrderInfoMsg msg)
        {
            if (msg.order == 0)
            {
                orderInfo1.SetActive(true);
                orderInfo2.SetActive(false);
            }
            else
            {
                orderInfo1.SetActive(false);
                orderInfo2.SetActive(true);
            }
        }

        void SetScoreBoard(SetScoreBoardMsg msg)
        {
            for (int i = 0; i < msg.totalPlayerCount; i++)
            {
                for (int j = 0; j < Enum.GetNames(typeof(MissionModeGame)).Length; j++)
                {
                    if (msg.listMissionSuccess[i].ContainsKey((MissionModeGame)j) && msg.listMissionSuccess[i][(MissionModeGame)j])
                    {
                        Player[i].SetScore((MissionModeGame)j, 1, msg.dicPlayerMissionChance[i]);
                    }
                    else if(msg.listMissionSuccess[i].ContainsKey((MissionModeGame)j) && !msg.listMissionSuccess[i][(MissionModeGame)j])
                    {
                        Player[i].SetScore((MissionModeGame)j, 0, msg.dicPlayerMissionChance[i]);
                    }
                    else
                    {
                        Player[i].SetScore((MissionModeGame)j, 3, msg.dicPlayerMissionChance[i]);
                    }
                }
            }
        }

        void SetResultScoreBoard(SetResultScoreBoardMsg msg)
        {
            gameResult.SetActive(true);
            txtInPlay.text = "플레이 결과";
            int[] playerSuccessCount = new int[msg.totalPlayerCount];
            for (int i = 0; i < msg.totalPlayerCount; i++)
            {
                playerSuccessCount[i] = 0;
                for (int j = 0; j < Enum.GetNames(typeof(MissionModeGame)).Length; j++)
                {
                    if (msg.listMissionSuccess[i].ContainsKey((MissionModeGame)j) && msg.listMissionSuccess[i][(MissionModeGame)j])
                    {
                        Player[i].SetScore((MissionModeGame)j, 1, msg.dicPlayerMissionChance[i]);
                        playerSuccessCount[i]++;
                    }
                    else if (msg.listMissionSuccess[i].ContainsKey((MissionModeGame)j) && !msg.listMissionSuccess[i][(MissionModeGame)j])
                    {
                        Player[i].SetScore((MissionModeGame)j, 0, msg.dicPlayerMissionChance[i]);
                    }
                    else
                    {
                        Player[i].SetScore((MissionModeGame)j, 3, msg.dicPlayerMissionChance[i]);
                    }
                }
            }
            int winerNum;

            if (msg.dicPlayerMissionChance[0] > msg.dicPlayerMissionChance[1])
            {
                winerNum = 0;
                //1번 승
            }
            else if (msg.dicPlayerMissionChance[0] < msg.dicPlayerMissionChance[1])
            {
                winerNum = 1;
                //2번 승
            }
            else
            {
                if (playerSuccessCount[0] > playerSuccessCount[1])
                {
                    winerNum = 0;
                    //1번승
                }
                else if (playerSuccessCount[0] < playerSuccessCount[1])
                {
                    winerNum = 1;
                    //2번 승
                }
                else
                {
                    winerNum = 3;
                }
            }

            foreach (var o in WinLine)
                o.SetActive(false);

            if (winerNum != 3)
            {
                WinLine[winerNum].SetActive(true);
                WinLine[winerNum].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetScoreBoardMsg>(SetScoreBoard);
            Message.RemoveListener<SetScoreBoardOrderInfoMsg>(SetScoreBoardOrderInfo);
            Message.RemoveListener<SetResultScoreBoardMsg>(SetResultScoreBoard);
        }

        protected override void OnUnload()
        {

        }
    }
}