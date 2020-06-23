using System;
using System.Collections;
using System.Collections.Generic;
using JHchoi.Constants;
using JHchoi.Models;
using JHchoi.T3;
using JHchoi.UI.Event;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

namespace JHchoi.Contents
{
    public class MissionModeContent : IGateBall
    {
        static string TAG = "MissionModeContent :: ";

        //GameObject Red_Ball;
        //GameObject White_Ball;

        //터치 장애물 볼
        GameObject[] Player_Touch_Ball;

        MissionModeBall_Controller red_Ball_Controller;
        MissionModeBall_Controller white_Ball_Controller;

        GameObject Gate1;
        GameObject Gate2;
        GameObject Gate3;
        GameObject Pole;

        Vector3 cameraPosition;
        Vector3 cameraRotation;
        Vector3 force;

        CameraModel cm;

        TouchBallSettingModel tbsm;

        Vector3 startPosition;
        Vector3 startRotation;

        int totalPlayerCount;
        int totalPlayRound;
        //bool isPlayPossible;
        float goalDistance;

        //public bool IsPlayPossible { get => isPlayPossible; set => isPlayPossible = value; }

        public float GoalDistance { get => goalDistance; set => goalDistance = value; }



        #region Contents Load

        protected override void OnLoadStart()
        {
            base.OnLoadStart();
            Debug.Log(TAG + "OnLoadStart");
            cm = Model.First<CameraModel>();
            tbsm = Model.First<TouchBallSettingModel>();
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            Message.Send<LoadingModeInfoMsg>(new LoadingModeInfoMsg(ModeType.MissionMode));
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path;
            path = "Object/GateBall/MissionModeObject";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                o =>
                {
                    var inGameObject = Instantiate(o) as GameObject;
                    inGame = inGameObject;
                    inGame.transform.parent = ModuleManager.transform;
                }));

            path = "Object/GateBall/Arrow";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                o =>
                {
                    var target = Instantiate(o) as GameObject;
                    targetArrow = target;
                    targetArrow.transform.parent = inGame.transform;
                    targetArrow.SetActive(false);
                }));

            path = "Object/GateBall/MissionSuccess";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                o =>
                {
                    var mission = Instantiate(o) as GameObject;
                    missionEffet = mission;
                    missionEffet.transform.parent = inGame.transform;
                    missionEffet.SetActive(false);
                }));

            GameObjFind();
            SetLoadComplete();
        }
        #endregion

        void GameObjFind()
        {
            frontCamera = GameObject.Find("FrontEye").GetComponent<Camera>();
            Red_Ball = GameObject.Find("Prop_BALL01");
            White_Ball = GameObject.Find("Prop_BALL02");
            Gate1 = GameObject.Find("GATE01");
            Gate2 = GameObject.Find("GATE02");
            Gate3 = GameObject.Find("GATE03");
            Pole = GameObject.Find("PoleBase");
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            ListSoundLoop.Add(StartCoroutine(SoundEffect()));
            SoundManager.Instance.PlaySound((int)SoundType.Mission_BGM);
            SensorSetting(new SensorSettingMsg());
            StartCoroutine(LoadingVideo());
        }

        IEnumerator LoadingVideo()
        {
            yield return new WaitForSeconds(4.0f);
            Message.Send<LoadingCompleteMsg>(new LoadingCompleteMsg());
        }

        protected override void SensorReady()
        {
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(GoalDistance));
        }

        protected override void InitContent()
        {
            Debug.Log(TAG + "InitContent");
            inGameCamera.Init(cm.Start_Position, cm.Start_Rotation);
            inGameCamera.SetPositionReset();
            inGameCamera.SetPostProcessProfile(postProcessProfile);
            igm.Round = 1;
            igm.PlayerNum = 0;
            totalPlayerCount = igm.TotalPlayerCount;
            totalPlayRound = igm.TotalPlayRound;

            for (int i = 0; i < totalPlayerCount; i++)
            {
                pm.InitMissionPlayerModel(i);
                pm.SetMissionStartPosition(i, frontCamera.transform.position);
            }
            GoalDistance = Vector3.Distance(frontCamera.transform.position, Gate1.transform.position);

            for (int i = 0; i < totalPlayerCount; i++)
            {
                InitTouchBall(i);
            }

            SetTouchBall(igm.PlayerNum);
            InitPlayerBall();
            InitObjectPosition();
            MissionSetting(new SetMissionObjectMsg(pm.GetMissionModeNowMission(igm.PlayerNum)));
        }

        void InitTouchBall(int playerNum)
        {
            Level tempLevel = igm.PlayLevel;
            int tempTouchBallCount = tbsm.GetBallCount(tempLevel);
            Player_Touch_Ball = new GameObject[igm.TotalPlayerCount];

            for (int i = 0; i < igm.TotalPlayerCount; i++)
            {
                Player_Touch_Ball[i] = GameObject.Find("PlayerTouch" + (i + 1).ToString());
            }

            //볼 오브젝트 닫기
            for (int i = 0; i < Player_Touch_Ball[playerNum].transform.childCount; i++)
            {
                Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.SetActive(false);
            }

            //터치볼 오브젝트 모델에 셋팅값으로 셋팅
            for (int i = 0; i < tempTouchBallCount; i++)
            {
                pm.SetTouchBallPosition(playerNum, i, tbsm.GetBallPosition(tempLevel, i));
            }

            //실질적 볼 포지션 배치
            for (int i = 0; i < tempTouchBallCount; i++)
            {
                Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.GetComponent<TouchBall_Controller>().Init(tbsm.GetBallPosition(tempLevel, i));
            }
        }



        void SetTouchBall(int playerNum)
        {
            //볼 오브젝트 열기
            Level tempLevel = igm.PlayLevel;
            int tempTouchBallCount = tbsm.GetBallCount(tempLevel);

            for (int i = 0; i < totalPlayerCount; i++)
            {
                for (int j = 0; j < tempTouchBallCount; j++)
                {
                    Player_Touch_Ball[i].transform.GetChild(j).localScale = new Vector3(sm.BallScale, sm.BallScale, sm.BallScale);
                    Player_Touch_Ball[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < tempTouchBallCount; i++)
            {
                if (!Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.GetComponent<TouchBall_Controller>().isTouch)
                    Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.SetActive(true);
                else
                    pm.RemoveTouchBallPostion(playerNum, i);
            }
        }

        void InitPlayerBall()
        {
            Red_Ball.AddComponent<MissionModeBall_Controller>();
            White_Ball.AddComponent<MissionModeBall_Controller>();
            red_Ball_Controller = Red_Ball.GetComponent<MissionModeBall_Controller>();
            white_Ball_Controller = White_Ball.GetComponent<MissionModeBall_Controller>(); ;

            red_Ball_Controller.Init(Gate1.transform.position);
            white_Ball_Controller.Init(Gate1.transform.position);

            Red_Ball.SetActive(false);
            red_Ball_Controller.isPlayerBall = true;
            White_Ball.SetActive(false);
        }

        //난이도별 오브젝트 배치
        void InitObjectPosition()
        {
            Level tempLevel;
            tempLevel = igm.PlayLevel;

            Gate1.transform.position = lsm.GetGate1Position(tempLevel);
            Gate1.transform.eulerAngles = lsm.GetGate1Rotation(tempLevel);
            Gate1.transform.localScale = lsm.GetGate1Scale(tempLevel);

            Gate2.transform.position = lsm.GetGate2Position(tempLevel);
            Gate2.transform.eulerAngles = lsm.GetGate2Rotation(tempLevel);
            Gate2.transform.localScale = lsm.GetGate2Scale(tempLevel);

            Gate3.transform.position = lsm.GetGate3Position(tempLevel);
            Gate3.transform.eulerAngles = lsm.GetGate3Rotation(tempLevel);
            Gate3.transform.localScale = lsm.GetGate3Scale(tempLevel);

            Pole.transform.position = lsm.GetPolePosition(tempLevel);
            Pole.transform.localScale = lsm.GetPoleScale(tempLevel);
        }

        //완전 초기화 볼 생성 셋팅 파일에 오브젝트로 위치 전환 


        IEnumerator SoundEffect()
        {
            while (true)
            {
                float range = Random.Range(25, 30);
                yield return new WaitForSeconds(range);
                SoundManager.Instance.PlaySound((int)SoundType.Missoin_Bird_Effect);
            }
        }

        IEnumerator SoundBGM()
        {
            while (true)
            {
                yield return new WaitForSeconds(12f);
                SoundManager.Instance.PlaySound((int)SoundType.Title_BGM);
            }
        }


        protected override void InputSensor(float pos1, float pos2, float time1, float time2)
        {
            targetArrow.SetActive(false);
            IsPlayPossible = false;
            float fixpos1 = (pos1 * sensor1Scale + sensor1Offset);
            float fixpos2 = (pos2 * sensor2Scale + sensor2Offset);
            Log.Instance.log("Input Senor1 value : " + pos1 + " / fix : " + fixpos1 + "Input Senor1 Time : " + time1);
            Log.Instance.log("Input Senor2 value : " + pos2 + "/ fix : " + fixpos2 + "Input Senor2 Time : " + time2);

            if (fixpos1 < 0)
                fixpos1 = 0;

            if (fixpos1 > 1)
                fixpos1 = 1;

            if (fixpos2 < 0)
                fixpos2 = 0;

            if (fixpos2 > 1)
                fixpos2 = 1;


            //셋팅값 확인용 전달 메세지
            Message.Send<SensorValueMsg>(new SensorValueMsg(pos1, pos2, fixpos1, fixpos2));

            Vector3 firstVec = new Vector3(fixpos1, 0, 0);
            Vector3 secondVec = new Vector3(fixpos2, 0, sensorDistance);
            Vector3 dir = secondVec - firstVec;

            //todo..
            //Ray ray = Camera.main.ViewportPointToRay(new Vector3(fixpos2, 0.05f));
            Ray ray = frontCamera.ViewportPointToRay(new Vector3(fixpos2, 0.075f));
            RaycastHit rHit;

            dir = Quaternion.LookRotation(ray.direction) * dir;
            //Vector3 startPos = Camera.main.transform.position;
            Vector3 startPos = frontCamera.transform.position;

            if (Physics.Raycast(ray, out rHit, 1 << LayerMask.NameToLayer("Ground")))
            {
                //0.0415 = 스케일1일때 공 크기  (현재스케일 5).
                startPos = rHit.point - ray.direction * (sm.BallScale * 0.0415f);
            }
            dir.y = 0f;
            dir = dir.normalized;

            startPos -= dir;

            float time = time2 - time1;
            float speed = (sensorDistance / time);

            Debug.Log("speed : " + speed);
            if (speed > 3)
            {
                speed = 3f;
            }

            force = dir * speed * forceScale;
            GameObject tempBall;
            MissionModeBall_Controller tempBallController;

            if (igm.PlayerNum % 2 == 0)
            {
                tempBall = Red_Ball;
                tempBallController = red_Ball_Controller;
            }
            else
            {
                tempBall = White_Ball;
                tempBallController = white_Ball_Controller;
            }

            cameraPosition = inGameCamera.gameObject.transform.position;
            cameraRotation = inGameCamera.gameObject.transform.eulerAngles;

            tempBall.SetActive(true);
            red_Ball_Controller._rigidbody.constraints = RigidbodyConstraints.None;
            white_Ball_Controller._rigidbody.constraints = RigidbodyConstraints.None;

            startPos.y = -1.0f;

            Message.Send<CameraMoveMsg>(new CameraMoveMsg(tempBall, frontCamera.gameObject));
            //StartCoroutine(BallDistance(tempBall, frontCamera.gameObject));
            tempBallController.BallAddForce(startPos, force, pm.GetMissionModeNowMission(igm.PlayerNum));
        }

        protected override void MissionSetting(SetMissionObjectMsg msg)
        {
            if (pm.GetMissionModeNowMission(igm.PlayerNum) == MissionModeGame.Gate_1)
            {
                inGameCamera.SetPosition(pm.GetMissionStartPosition(igm.PlayerNum),
                    Gate1.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate1.transform.position);

                //                targetArrow.SetActive(true);
                targetArrow.transform.position = Gate1.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate1.transform;
            }
            else if (pm.GetMissionModeNowMission(igm.PlayerNum) == MissionModeGame.Gate_2)
            {
                inGameCamera.SetPosition(pm.GetMissionStartPosition(igm.PlayerNum),
                  Gate2.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate2.transform.position);

                //                targetArrow.SetActive(true);
                targetArrow.transform.position = Gate2.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate2.transform;
            }
            else if (pm.GetMissionModeNowMission(igm.PlayerNum) == MissionModeGame.Gate_3)
            {
                inGameCamera.SetPosition(pm.GetMissionStartPosition(igm.PlayerNum),
                    Gate3.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate3.transform.position);

                //targetArrow.SetActive(true);
                targetArrow.transform.position = Gate3.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate3.transform;
            }
            else if (pm.GetMissionModeNowMission(igm.PlayerNum) == MissionModeGame.Pole)
            {
                inGameCamera.SetPosition(pm.GetMissionStartPosition(igm.PlayerNum),
                    Pole.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position, Pole.transform.position);

                //targetArrow.SetActive(true);
                targetArrow.transform.position = Pole.transform.position + new Vector3(0, 1.0f, 0);

                missionEffet.transform.parent = Pole.transform;
            }
            targetArrow.transform.LookAt(frontCamera.transform);
        }

        protected override void TurnEnd(TurnChangeMsg msg)
        {
            GameObject temp_Start;
            GameObject temp_Target;
            MissionModeBall_Controller temp_Start_Controller;

            if (igm.PlayerNum % 2 == 0)
            {
                temp_Start = Red_Ball;
                temp_Target = White_Ball;
                temp_Start_Controller = red_Ball_Controller;
            }
            else
            {
                temp_Start = White_Ball;
                temp_Target = Red_Ball;
                temp_Start_Controller = white_Ball_Controller;
            }

            temp_Start.SetActive(false);
            temp_Target.SetActive(false);
            temp_Start_Controller.isPlayerBall = true;

            //터치볼 오브젝트 배치
            SetTouchBall(igm.PlayerNum);
        }

        protected override void MissionEndInfo(MissionEndMsg msg)
        {
            Debug.Log(TAG + "MissionEndInfo");

            targetArrow.SetActive(false);
            int tempPlayerNum = igm.PlayerNum;
            MissionModeGame tempMission = pm.GetMissionModeNowMission(tempPlayerNum);
            Level tempLevel = igm.PlayLevel;
            int tempTouchBallCount = tbsm.GetBallCount(tempLevel);
            //미션 실패이고 첫번째 미션이면 터치볼 배치 리셋
            if (!msg.isSuccess && tempMission == MissionModeGame.Gate_1)
            {
                InitTouchBall(tempPlayerNum);
            }
            else
            {
                //터치볼 오브젝트 모델에 셋팅값으로 셋팅
                for (int i = 0; i < tempTouchBallCount; i++)
                {
                    if (!Player_Touch_Ball[tempPlayerNum].transform.GetChild(i).gameObject.GetComponent<TouchBall_Controller>().isTouch)
                        pm.SetTouchBallPosition(tempPlayerNum, i, Player_Touch_Ball[tempPlayerNum].transform.GetChild(i).transform.position);
                    else
                    {
                        pm.RemoveTouchBallPostion(tempPlayerNum, i);
                    }
                }
            }
        }

        protected override void MissionInfoEffect()
        {
            int num = Random.Range(0, 3);
            missionEffet.SetActive(true);
            missionEffet.transform.localEulerAngles = Vector3.zero;
            missionEffet.transform.localPosition = new Vector3(0, 2f, 0);

            for (int i = 0; i < missionEffet.transform.childCount; i++)
            {
                missionEffet.transform.GetChild(i).gameObject.SetActive(false);
            }

            missionEffet.transform.GetChild(num).gameObject.SetActive(true);
            missionEffet.transform.GetChild(num).gameObject.transform.LookAt(frontCamera.transform);
        }


        protected override void OnExit()
        {
            base.OnExit();

            foreach (var o in ListSoundLoop)
                StopCoroutine(o);

            ListSoundLoop.Clear();
            SoundManager.Instance.StopSound((int)SoundType.Mission_BGM);
        }

        protected override void OnUnload()
        {
            Debug.Log(TAG + "OnUnload");
        }

        private void OnDestroy()
        {
            Destroy(inGame);
            Destroy(targetArrow);
            Destroy(missionEffet);
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse0) && IsPlayPossible)
            {
                InputSensor(firstSensor.x, SecondSensor.x, firstTime, secondTime);
            }
#endif
        }

        //에디터 테스트용
        protected override void EditSensorCheck()
        {
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(GoalDistance));
        }

    }
}
