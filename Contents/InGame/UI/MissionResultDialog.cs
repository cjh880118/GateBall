using JHchoi.UI.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI
{
    public class MissionResultDialog : IDialog
    {
        public GameObject success;
        public GameObject fail;
        public ParticleSystem successParticle;
        public ParticleSystem failParticle;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            success.SetActive(false);
            fail.SetActive(false);
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
        }

        void SetMissionResultInfo(SetMissionResultInfoMsg msg)
        {
            if (msg.isSuccess)
            {
                //0~4
                int num = Random.Range(20, 25);
                SoundManager.Instance.PlaySound(num);
                SoundManager.Instance.PlaySound((int)SoundType.Mission_Success_Effect);
                success.SetActive(true);
                successParticle.Play();
            }
            else
            {
                SoundManager.Instance.PlaySound((int)SoundType.Mission_Fail);
                fail.SetActive(true);
                failParticle.Play();
            }

            StartCoroutine(DialogOff(msg.isSuccess));
        }

        IEnumerator DialogOff(bool isSuccess)
        {
            yield return new WaitForSeconds(3.0f);
            Message.Send<MissionResultCloseMsg>(new MissionResultCloseMsg(isSuccess));
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
        }

        protected override void OnUnload()
        {

        }
    }
}
