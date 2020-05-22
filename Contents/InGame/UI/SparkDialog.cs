using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.UI.Event;
using UnityEngine.Video;
using System;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class SparkDialog : IDialog
    {
        public RawImage imgSpark;
        public VideoPlayer videoPlayer;
        public GameObject sparkEffect;
        public Coroutine corSaprk;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            imgSpark.texture = null;
            sparkEffect.SetActive(false);
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SparkPlayerMsg>(SparkPlayer);
        }

        private void SparkPlayer(SparkPlayerMsg msg)
        {
            corSaprk = StartCoroutine(SparkAnimationEnd(msg.playerNum));
        }

        IEnumerator SparkAnimationEnd(int playerNum)
        {
            //스파크창 애니메이션 종료
            string path;
            if (playerNum == 0)
                path = "Video/mission_spark_white";
            else
                path = "Video/mission_spark_red";

            videoPlayer.clip = Resources.Load(path, typeof(VideoClip)) as VideoClip;
            videoPlayer.time = 0;
            videoPlayer.Play();
            //RTT에 이전 영상 남아 있어서 재생 후 살짝 딜레이
            yield return new WaitForSeconds(0.2f);
            imgSpark.texture = Resources.Load<Texture>("Rander/SparkRender");
            bool isPlay = false;

            while (videoPlayer.isPlaying)
            {
                if (videoPlayer.time > 0.8f && !isPlay)
                {
                    isPlay = true;
                    sparkEffect.SetActive(true);
                    SoundManager.Instance.PlaySound((int)SoundType.Mission_Spark);
                }

                yield return null;
            }

            yield return new WaitForSeconds(1.5f);
            videoPlayer.time = 0;
            videoPlayer.clip = null;
            Message.Send<BallSparkCloseMsg>(new BallSparkCloseMsg());
        }

        protected override void OnExit()
        {
            if (corSaprk != null)
                StopCoroutine(corSaprk);

            RemoveMessage();
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SparkPlayerMsg>(SparkPlayer);
        }

        protected override void OnUnload()
        {
        }
    }
}