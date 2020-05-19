using System.Collections;
using UnityEngine;
using CellBig.UI.Event;
using CellBig.Models;
using CellBig.Constants;

namespace CellBig.Contents
{
    public class MissionModeBall_Controller : MonoBehaviour
    {
        public Rigidbody _rigidbody;
        public bool isPlayerBall;
        public bool isTouch;

        Vector3 startPos;
        Vector3 endPos;
        Vector3 vec3GatePos;
        Vector3 vec3Force;
        Vector3 prevPosition;

        SettingModel settingModel;
        bool _addForce = false;
        bool isMissinSuccess;
        MissionModeGame nowMission;

        //int colliderCount;
        RaycastHit[] raycastHit;
        //float rayDistance = 1f;
        Coroutine corRay;

        public void Init(Vector3 gatePos)
        {
            vec3GatePos = gatePos;
            settingModel = Model.First<SettingModel>();
            startPos = this.transform.position;
            this.gameObject.tag = "Ball";
            this.gameObject.transform.localScale = new Vector3(settingModel.BallScale, settingModel.BallScale, settingModel.BallScale);
            _rigidbody = this.gameObject.GetComponent<Rigidbody>();
            _rigidbody.mass = settingModel.BallMass;
            _rigidbody.drag = settingModel.BallDrag;
            _rigidbody.angularDrag = settingModel.BallAngularDrag;
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<BallSettingMsg>(BallSetting);
        }

        void BallSetting(BallSettingMsg msg)
        {
            settingModel.LoadSetting();
            this.gameObject.transform.localScale = new Vector3(settingModel.BallScale, settingModel.BallScale, settingModel.BallScale);
            _rigidbody.mass = settingModel.BallMass;
            _rigidbody.drag = settingModel.BallDrag;
            _rigidbody.angularDrag = settingModel.BallAngularDrag;
        }

        public void BallAddForce(Vector3 startPos, Vector3 force, MissionModeGame mission, ForceMode forceMode = ForceMode.Impulse)
        {
            if (_rigidbody == null)
                return;

            Message.Send<BallStartMsg>(new BallStartMsg());
            //colliderCount = 0;
            //TouchBall = null;
            _addForce = true;
            isMissinSuccess = false;
            isTouch = false;
            nowMission = mission;
            _rigidbody.constraints = RigidbodyConstraints.None;
            gameObject.transform.position = startPos;
            vec3Force = force;
            _rigidbody.AddForce(force, forceMode);
            this.startPos = this.gameObject.transform.position;
            corRay = StartCoroutine(rayCheck());
            StartCoroutine(CheckFinishAddForce());
        }

        IEnumerator rayCheck()
        {
            while (true)
            {
                prevPosition = this.gameObject.transform.position;
                yield return null;
                Vector3 dir = prevPosition - transform.position;
                float distance = Vector3.Distance(prevPosition, transform.position);
                raycastHit = Physics.RaycastAll(transform.position, dir, distance);
                Debug.DrawRay(transform.position, dir, Color.red, distance);
                foreach (RaycastHit hit in raycastHit)
                {
                    if (hit.collider.name == "Gate (1)" && nowMission == MissionModeGame.Gate_1 && isPlayerBall)
                    {
                        isMissinSuccess = true;
                        Debug.Log("게이트1 미션 성공");
                        Message.Send<MissionEffectMsg>(new MissionEffectMsg());
                    }
                    else if (hit.collider.name == "Gate (2)" && nowMission == MissionModeGame.Gate_2 && isPlayerBall)
                    {
                        isMissinSuccess = true;
                        Debug.Log("게이트2 미션 성공");
                        Message.Send<MissionEffectMsg>(new MissionEffectMsg());
                    }
                    if (hit.collider.name == "Gate (3)" && nowMission == MissionModeGame.Gate_3 && isPlayerBall)
                    {
                        isMissinSuccess = true;
                        Debug.Log("게이트3 미션 성공");
                        Message.Send<MissionEffectMsg>(new MissionEffectMsg());
                    }
                }
            }
        }

        IEnumerator CheckFinishAddForce()
        {
            float tempForceScale = 0.1f;
            while (_rigidbody.velocity.magnitude < 0.5)
            {
                yield return null;
                _rigidbody.AddForce(vec3Force * tempForceScale, ForceMode.Impulse);
                tempForceScale += 0.1f;
            }

            while (_rigidbody != null && _addForce)
            {
                yield return null;
                if (_rigidbody.velocity.magnitude < 0.5f)
                {
                    StopCoroutine(corRay);
                    _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    _addForce = false;

                    /*볼위치 저장 
                    1. 미션 성공일때 저장
                    2. 현재 미션이 게이트 1번 외 미션실패시 저장
                    3. 터치 성공시 항시 저장
                    */
                    if (isMissinSuccess || nowMission != MissionModeGame.Gate_1)
                    {
                        Message.Send<BallStopMsg>(new BallStopMsg(this.gameObject.transform.position));
                    }

                    StopCoroutine(CheckFinishAddForce());

                    if (!isTouch)
                        Message.Send<MissionEndMsg>(new MissionEndMsg(isMissinSuccess));
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<BallSettingMsg>(BallSetting);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "GateBar")
            {
                SoundManager.Instance.PlaySound((int)SoundType.Gate_Touch);
            }

            if(collision.collider.tag == "Pole")
            {
                SoundManager.Instance.PlaySound((int)SoundType.Pole_Touch);
            }

            if (collision.collider.tag == "Pole" && nowMission == MissionModeGame.Pole && isPlayerBall)
            {
                SoundManager.Instance.PlaySound((int)SoundType.Pole_Touch);
                isMissinSuccess = true;
                Debug.Log("폴 미션 성공");
                Message.Send<MissionEffectMsg>(new MissionEffectMsg());
            }

            if (collision.collider.tag == "TouchBall")
            {
                int num = Random.Range(0, 2);
                if(num == 0)
                    SoundManager.Instance.PlaySound((int)SoundType.Ball_Touch1);
                else
                    SoundManager.Instance.PlaySound((int)SoundType.Ball_Touch2);

                isTouch = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                StopCoroutine(corRay);
                Message.Send<BallStopMsg>(new BallStopMsg(this.gameObject.transform.position));

                if (isMissinSuccess)
                    Message.Send<MissionEndMsg>(new MissionEndMsg(isMissinSuccess));
                else
                {
                    if (nowMission == MissionModeGame.Gate_1)
                    {
                        collision.collider.GetComponent<TouchBall_Controller>().ResetTouch();
                        Message.Send<MissionEndMsg>(new MissionEndMsg(isMissinSuccess));
                    }
                    else
                        Message.Send<BallTouchMsg>(new BallTouchMsg());
                }
            }

            //아웃 체크
            if (collision.collider.tag == "Out" && isPlayerBall)
            {
                Debug.Log("공 아웃");
                StopCoroutine(corRay);
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                if (nowMission == MissionModeGame.Gate_1)
                    isMissinSuccess = false;
                else
                    Message.Send<BallStopMsg>(new BallStopMsg(this.gameObject.transform.position));
            }
        }
    }
}