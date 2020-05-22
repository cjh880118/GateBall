using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.UI.Event;
using System;
using JHchoi.Constants;

namespace JHchoi.UI
{
    public class InGameDialog : IDialog
    {
        static string TAG = "InGameDialog :: ";

        public GameObject missionMode;
        public GameObject betMode;

        public GameObject mission;
        public Text txtMissionTitle;
        public Text txtMissionInfo;
        public GameObject timer;
        public Text txtCount;

        public Text txtBetModeMission;
        public Text txtBetAlivePlayerCount;
        public Text txtBetRound;

        public GameObject hitReady;
        public GameObject missonInfo;

        float timerCount;
        Coroutine Timer;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            AddMessage();
            mission.SetActive(true);
            timer.SetActive(false);
            missonInfo.SetActive(false);
            Message.Send<MissionInfoCloseMsg>(new MissionInfoCloseMsg());
        }

        void AddMessage()
        {
            Message.AddListener<BallStartMsg>(BallStart);
            Message.AddListener<MissionTimerStartMsg>(MissionTimerStar);
            Message.AddListener<SetMissionModeInfoMsg>(SetMissionModeInfo);
            Message.AddListener<SetBetModeInfoMsg>(SetBetModeInfo);
        }

        private void BallStart(BallStartMsg msg)
        {
            if (Timer != null)
                StopCoroutine(Timer);
        }

        private void MissionTimerStar(MissionTimerStartMsg msg)
        {
            Timer = StartCoroutine(TimerStart(msg.goalDistance));
        }

        IEnumerator TimerStart(float distance)
        {
            hitReady.SetActive(false);
            mission.SetActive(false);
            missonInfo.SetActive(true);
            timer.SetActive(true);
            timerCount = 30;
            while (timerCount > 0)
            {
                yield return null;
                timerCount -= Time.deltaTime;
                txtCount.text = Math.Truncate(timerCount).ToString();
            }

            if (timerCount <= 0)
            {
                Message.Send<TimeOutMsg>(new TimeOutMsg());
            }
        }

        private void SetMissionModeInfo(SetMissionModeInfoMsg msg)
        {
            hitReady.SetActive(true);
            missionMode.SetActive(true);
            betMode.SetActive(false);
            int soundIndex = 0;
            if (msg.mission == MissionModeGame.Gate_1)
            {
                soundIndex = (int)SoundType.Mission_Gate1;
                txtMissionTitle.text = "미션 : 게이트1";
                txtMissionInfo.text = "1번 게이트를 통과하세요.";
            }
            else if (msg.mission == MissionModeGame.Gate_2)
            {
                soundIndex = (int)SoundType.Mission_Gate2;
                txtMissionTitle.text = "미션 : 게이트2";
                txtMissionInfo.text = "2번 게이트를 통과하세요.";
            }
            else if (msg.mission == MissionModeGame.Gate_3)
            {
                soundIndex = (int)SoundType.Mission_Gate3;
                txtMissionTitle.text = "미션 : 게이트3";
                txtMissionInfo.text = "3번 게이트를 통과하세요.";
            }
            else if (msg.mission == MissionModeGame.Pole)
            {
                soundIndex = (int)SoundType.Mission_Pole;
                txtMissionTitle.text = "미션 : 골폴";
                txtMissionInfo.text = "골폴을 맞추세요.";
            }

            SoundManager.Instance.PlaySound(soundIndex);
            StartCoroutine(GateStartSound(soundIndex));
        }

        private void SetBetModeInfo(SetBetModeInfoMsg msg)
        {
            hitReady.SetActive(true);
            missionMode.SetActive(false);
            betMode.SetActive(true);
            int soundIndex = 0;
            if (msg.bet == BetModeGame.Gate_1)
            {
                soundIndex = (int)SoundType.Mission_Gate;
                txtBetModeMission.text = "미션 : 게이트";
                txtMissionInfo.text = "게이트를 통과하세요.";
            }
            else if (msg.bet == BetModeGame.Touch)
            {
                soundIndex = (int)SoundType.Mission_Touch;
                txtBetModeMission.text = "미션 : 터치";
                txtMissionInfo.text = "상대방 공을 터치하세요.";
            }
            else if (msg.bet == BetModeGame.Pole)
            {
                soundIndex = (int)SoundType.Mission_Pole;
                txtBetModeMission.text = "미션 : 골폴";
                txtMissionInfo.text = "골폴을 맞추세요.";
            }

            SoundManager.Instance.PlaySound(soundIndex);
            StartCoroutine(GateStartSound(soundIndex));
            txtBetRound.text = string.Format("라운드 : {0}", msg.round);
            txtBetAlivePlayerCount.text = string.Format("잔여인원 : {0}", msg.alivePlayerCount);
        }

        IEnumerator GateStartSound(int index)
        {
            while (SoundManager.Instance.IsPlaySound(index))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

            //if(SoundManager.Instance.IsPlaySound((int)SoundType.Crowd_Effect))
            //    SoundManager.Instance.SoundVolumeReset((int)SoundType.Crowd_Effect);
        }

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            txtCount.text = 30.ToString();
            if (Timer != null)
                StopCoroutine(Timer);
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<BallStartMsg>(BallStart);
            Message.RemoveListener<MissionTimerStartMsg>(MissionTimerStar);
            Message.RemoveListener<SetMissionModeInfoMsg>(SetMissionModeInfo);
            Message.RemoveListener<SetBetModeInfoMsg>(SetBetModeInfo);
        }

        protected override void OnUnload()
        {
            Debug.Log(TAG + "OnUnload");
        }
    }
}