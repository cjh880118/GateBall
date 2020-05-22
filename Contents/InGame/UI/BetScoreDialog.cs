using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JHchoi.UI
{
    public class BetScoreDialog : IDialog
    {
        public BetScorePlayer[] Player;
        public GameObject victoryPar;
        public GameObject victory;
        public GameObject videoParent;
        public VideoPlayer videoPlayer;
        public GameObject playResult;
        public RawImage imgWiner;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            SoundManager.Instance.PlaySound((int)SoundType.Score_Result);
            imgWiner.texture = null;
            videoParent.SetActive(false);
            victoryPar.SetActive(false);
            victory.SetActive(false);
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SetBetBoardMsg>(SetBetBoard);
        }

        private void SetBetBoard(SetBetBoardMsg msg)
        {
            int tempPlayerCount = msg.listIsMissionSccess.Count;
            int winerNum = 0;
            Message.Send<BetSendGameEndToKioskMsg>(new BetSendGameEndToKioskMsg());

            foreach (var o in Player)
                o.Init();


            for (int i = 0; i < tempPlayerCount; i++)
            {
                int tempPlayMissionCount = msg.listIsMissionSccess[i].Count;

                if (tempPlayMissionCount == 0)
                {
                    Player[i].Init();
                }
                else
                {
                    bool isMissionSuccess = msg.listIsMissionSccess[i][tempPlayMissionCount - 1];
                    Player[i].SetBoard(tempPlayMissionCount + "R", isMissionSuccess, i + 1 + "Player");
                    if (isMissionSuccess)
                        winerNum = i;
                }
            }

            StartCoroutine(PlayVideo(winerNum));
        }

        IEnumerator PlayVideo(int winerNum)
        {
            yield return new WaitForSeconds(5.0f);
            SoundManager.Instance.StopSound((int)SoundType.Score_Result, true);
            string path = string.Format("{0}{1}", "Video/bet_win_player", (winerNum + 1).ToString());
            playResult.SetActive(false);
            videoParent.SetActive(true);
            videoPlayer.clip = Resources.Load(path, typeof(VideoClip)) as VideoClip;
            videoPlayer.time = 0;
            videoPlayer.Play();

            yield return new WaitForSeconds(0.2f);
            imgWiner.texture = Resources.Load<Texture>("Rander/BetWinRender");

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
            SoundManager.Instance.PlaySound((int)SoundType.Game_Result1);
            SoundManager.Instance.PlaySound((int)SoundType.Game_Result2);
            victoryPar.SetActive(true);
            victory.SetActive(true);
            victory.transform.GetChild(winerNum).gameObject.SetActive(true);

            yield return new WaitForSeconds(3f);
            Message.Send<ScoreBoardCloseMsg>(new ScoreBoardCloseMsg());
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SetBetBoardMsg>(SetBetBoard);
        }
    }
}
