using CellBig.UI.Event;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Constants;

namespace CellBig.UI
{
    public class GameStartDialog : IDialog
    {
        public GameObject missionModeInfo;
        public GameObject betModeInfo;
        public Text txtRound;
        public GameObject missionLevel;
        public ParticleSystem particle;
        Animator animator;
        
        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
            StartCoroutine(AnimationEnd());
        }

        IEnumerator AnimationEnd()
        {
            yield return new WaitForSeconds(3.0f);
            Message.Send<RoundInfoCloseMsg>(new RoundInfoCloseMsg());
        }


        void AddMessage()
        {
            Message.AddListener<SetGameStartInfoMsg>(SetGameStartInfo);
        }

        void SetGameStartInfo(SetGameStartInfoMsg msg)
        {
            missionModeInfo.SetActive(false);
            betModeInfo.SetActive(false);

            if (msg.modeType == ModeType.MissionMode)
            {
                animator = missionModeInfo.GetComponent<Animator>();
                missionModeInfo.SetActive(true);
                for (int i = 0; i < missionLevel.transform.childCount ; i++)
                {
                    missionLevel.transform.GetChild(i).gameObject.SetActive(false);
                }

                missionLevel.transform.GetChild((int)msg.level).gameObject.SetActive(true);
            }
            else
            {
                animator = betModeInfo.GetComponent<Animator>();
                betModeInfo.SetActive(true);
                txtRound.text = msg.round.ToString();
            }
            particle.Play();
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetGameStartInfoMsg>(SetGameStartInfo);
        }

        protected override void OnUnload()
        {

        }
    }
}
