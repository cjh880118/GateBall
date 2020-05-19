using CellBig.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellBig.UI
{
    public class KioskBetScoreDialog : IDialog
    {
        public BetScorePlayer[] Player;
        public Text txtRound;
        public Text txtInPlay;
        public GameObject round;
        public GameObject gameEnd;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
            round.SetActive(true);
            gameEnd.SetActive(false);
            txtInPlay.text = "플레이 중 입니다.";
            for (int i = 0; i < Player.Length; i++)
            {
                Player[i].Init();
            }
        }

        private void AddMessage()
        {
            Message.AddListener<SetBetBoardInfoMsg>(SetBetBoardInfo);
            Message.AddListener<SetBetBoardMsg>(SetBetBoard);
            Message.AddListener<SetBetKioskRoundUpdateMsg>(SetBetKioskRoundUpdate);
            Message.AddListener<BetSendGameEndToKioskMsg>(BetSendGameEndToKiosk);
        }

        private void BetSendGameEndToKiosk(BetSendGameEndToKioskMsg msg)
        {
            txtInPlay.text = "플레이 결과";
            round.SetActive(false);
            gameEnd.SetActive(true);

            foreach (var o in Player)
                o.SetOrder(false);
        }

        private void SetBetBoardInfo(SetBetBoardInfoMsg msg)
        {
            txtRound.text = string.Format("ROUND {0}", msg.round);
            for(int i = 0; i < Player.Length; i++)
            {
                if(i == msg.playerNum)
                {
                    Player[i].SetOrder(true);
                }else
                    Player[i].SetOrder(false);
            }
        }

        private void SetBetBoard(SetBetBoardMsg msg)
        {
            int tempPlayerCount = msg.listIsMissionSccess.Count;
            for (int i = 0; i < tempPlayerCount; i++)
            {
             
                int tempPlayMissionCount = msg.listIsMissionSccess[i].Count;
                if (tempPlayMissionCount == 0)
                {
                    Player[i].SetBoard("1R", true, i + 1 + "Player");
                }
                else
                {
                    bool isMissionSuccess = msg.listIsMissionSccess[i][tempPlayMissionCount - 1];
                    Player[i].SetBoard(tempPlayMissionCount + "R", isMissionSuccess, i + 1 + "Player");
                }
            }
        }

        private void SetBetKioskRoundUpdate(SetBetKioskRoundUpdateMsg msg)
        {
            txtRound.text = string.Format("ROUND {0}", msg.round);
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SetBetBoardInfoMsg>(SetBetBoardInfo);
            Message.RemoveListener<SetBetBoardMsg>(SetBetBoard);
            Message.RemoveListener<SetBetKioskRoundUpdateMsg>(SetBetKioskRoundUpdate);
            Message.RemoveListener<BetSendGameEndToKioskMsg>(BetSendGameEndToKiosk);
        }

        protected override void OnDestroy()
        {

        }
    }
}
