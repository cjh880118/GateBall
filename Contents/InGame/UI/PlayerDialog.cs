using UnityEngine;
using CellBig.UI.Event;
using UnityEngine.UI;
using CellBig.Constants;
using System;

namespace CellBig.UI
{
    public class PlayerDialog : IDialog
    {
        static string TAG = "PlayerDialog :: ";

        public GameObject missionParent;
        public GameObject[] missionPlayer;
        public GameObject[] missionChance;

        public GameObject betParent;
        public Text txtPlayerName;

        protected override void OnLoad()
        {
            Debug.Log(TAG + "OnLoad");
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            missionParent.SetActive(false);
            betParent.SetActive(false);
            Debug.Log(TAG + "OnEnter");
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<SetPlayerInfoMsg>(SetPlayerInfo);
        }

        void SetPlayerInfo(SetPlayerInfoMsg msg)
        {
            int playerNum = msg.nowPlayerNum + 1;
            if (msg.gameMode == ModeType.BetMode)
            {
                betParent.SetActive(true);
                txtPlayerName.gameObject.SetActive(true);
                txtPlayerName.text = playerNum.ToString(); ;
                for (int i = 0; i < missionChance.Length; i++)
                {
                    missionChance[i].SetActive(false);
                }
            }
            else
            {
                missionParent.SetActive(true);
                for (int i = 0; i < missionPlayer.Length; i++)
                {
                    if (i == msg.nowPlayerNum)
                    {
                        missionPlayer[i].SetActive(true);
                        missionChance[i].SetActive(true);
                    }
                    else
                    {
                        missionPlayer[i].SetActive(false);
                        missionChance[i].SetActive(false);
                    }
                }

                for (int i = 0; i < missionChance[msg.nowPlayerNum].transform.childCount; i++)
                {
                    missionChance[msg.nowPlayerNum].transform.GetChild(i).gameObject.SetActive(false);
                }

                for (int i = 0; i < msg.missionChance; i++)
                {
                    missionChance[msg.nowPlayerNum].transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetPlayerInfoMsg>(SetPlayerInfo);
        }

        protected override void OnUnload()
        {
            RemoveMessage();
        }
    }
}