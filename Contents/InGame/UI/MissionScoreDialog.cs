using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CellBig.UI.Event;
using CellBig.Constants;
using System;
using UnityEngine.Video;

namespace CellBig.UI
{
    public class MissionScoreDialog : IDialog
    {
        static string TAG = "MissionScoreDialog :: ";
        public ScoreBoardPlayer_Controller[] Player;
        public GameObject victoryPar;
        public GameObject[] victory;
        public GameObject[] first;
        //public GameObject[] winerPanel;
        public GameObject draw;
        public VideoPlayer videoPlayer;
        public GameObject videoParent;
        //public GameObject modeText;
        public RawImage imgWinner;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Score_Result);
            imgWinner.texture = null;
            videoParent.SetActive(false);
            victoryPar.SetActive(false);
            Debug.Log(TAG + "ScoreDialog");
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<SetResultScoreBoardMsg>(SetResultScoreBoard);
        }

        void SetResultScoreBoard(SetResultScoreBoardMsg msg)
        {
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
                    //완전 무승부
                }
            }

            if (winerNum != 3)
            {
                first[winerNum].SetActive(true);
            }
            StartCoroutine(PlayVideo(winerNum));
        }

        IEnumerator PlayVideo(int winerNum)
        {
            yield return new WaitForSeconds(5.0f);
            SoundManager.Instance.StopSound((int)SoundType.Score_Result, true);
            if (winerNum != 3)
            {
                first[winerNum].SetActive(false);

            }
            string path;
            if (winerNum == 1) path = "Video/mission_win_white";
            else path = "Video/mission_win_red";
            //modeText.SetActive(false);
            videoParent.SetActive(true);
            //videoPlayer.gameObject.SetActive(true);
            videoPlayer.clip = Resources.Load(path, typeof(VideoClip)) as VideoClip;
            videoPlayer.time = 0;
            videoPlayer.Play();
            yield return new WaitForSeconds(0.2f);
            imgWinner.texture = Resources.Load<Texture>("Rander/MissionWinRender");

            while (videoPlayer.isPlaying)
            {
                yield return null;
            }

            videoPlayer.time = 0;
            videoPlayer.clip = null;
            StartCoroutine(SetVictoryAni(winerNum));
        }

        IEnumerator SetVictoryAni(int winerNum)
        {
            //yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlaySound((int)SoundType.Game_Result1);
            SoundManager.Instance.PlaySound((int)SoundType.Game_Result2);
            if (winerNum != 3)
            {
                victoryPar.SetActive(true);
                victory[winerNum].SetActive(true);
            }
            else
            {
                draw.SetActive(true);
            }

            yield return new WaitForSeconds(3.0f);
            Message.Send<ScoreBoardCloseMsg>(new ScoreBoardCloseMsg());
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetResultScoreBoardMsg>(SetResultScoreBoard);
        }

        protected override void OnUnload()
        {

        }
    }
}
