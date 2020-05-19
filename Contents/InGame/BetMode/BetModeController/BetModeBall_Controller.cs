using System.Collections;
using UnityEngine;
using CellBig.UI.Event;
using CellBig.Models;
using CellBig.Constants;

namespace CellBig.Contents
{
    public class BetModeBall_Controller : MonoBehaviour
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
        BetModeGame nowMission;

        //int colliderCount;
        RaycastHit[] raycastHit;
        //float rayDistance = 0.1f;
        Coroutine corRay;
        string temp;
        //GameObject TouchBall;

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

        public void BallAddForce(Vector3 startPos, Vector3 force, BetModeGame mission, ForceMode forceMode = ForceMode.Impulse)
        {
            if (_rigidbody == null)
                return;

            temp = null;
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
                    if (hit.collider.name == "Gate (1)" && nowMission == BetModeGame.Gate_1 && isPlayerBall)
                    {
                        isMissinSuccess = true;
                        Debug.Log("게이트1 미션 성공");
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

            if (collision.collider.tag == "Pole" && nowMission == BetModeGame.Pole && isPlayerBall)
            {
                SoundManager.Instance.PlaySound((int)SoundType.Pole_Touch);
                isMissinSuccess = true;
                temp = "폴 미션 성공";
                Debug.Log("폴 미션 성공");
                Message.Send<MissionEffectMsg>(new MissionEffectMsg());
            }

            if (collision.collider.tag == "Ball" && nowMission == BetModeGame.Touch && isPlayerBall)
            {
                Debug.Log("터치 미션 성공");
                temp = "터치 미션 성공";
                Message.Send<MissionEffectMsg>(new MissionEffectMsg());

                int num = Random.Range(0, 2);
                if (num == 0)
                    SoundManager.Instance.PlaySound((int)SoundType.Ball_Touch1);
                else
                    SoundManager.Instance.PlaySound((int)SoundType.Ball_Touch2);

                isMissinSuccess = true;
            }

            //아웃 체크
            if (collision.collider.tag == "Out" && isPlayerBall)
            {
                temp = "공 아웃";
                Debug.Log("공 아웃");
                //isMissinSuccess = false;
                StopCoroutine(corRay);
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void OnGUI()
        {
            if (settingModel.IsTestMode && isPlayerBall)
            {
                GUIStyle myStyle = new GUIStyle();
                myStyle.normal.textColor = Color.red;
                myStyle.fontSize = 20;

                
                GUI.Label(new Rect(10, 210, 300, 50), temp, myStyle);
                GUI.backgroundColor = Color.black;
            }
        }

        private void Update()
        {
            if (settingModel.IsTestMode && isPlayerBall)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    StopAllCoroutines();
                    _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    Message.Send<TurnChangeMsg>(new TurnChangeMsg());
                }
            }
        }
    }
}