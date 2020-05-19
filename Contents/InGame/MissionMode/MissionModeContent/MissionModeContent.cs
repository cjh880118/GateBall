using System;
using System.Collections;
using System.Collections.Generic;
using CellBig.Constants;
using CellBig.Models;
using CellBig.T3;
using CellBig.UI.Event;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

namespace CellBig.Contents
{
    public class MissionModeContent : IContent
    {
        static string TAG = "MissionModeContent :: ";

        GameObject Red_Ball;
        GameObject White_Ball;

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

        [Header("<Test Input Sensor>")]
        public Vector3 firstSensor;
        public float firstTime;
        public Vector3 SecondSensor;
        public float secondTime;

        [Header("<Sensor Value>")]
        float sensorDistance;
        float forceScale;

        public float sensor1Scale = 1f;
        public float sensor1Offset = 0f;
        public float sensor2Scale = 1f;
        public float sensor2Offset = 0f;

        SettingModel settingModel;
        PlayersModel playersModel;

        GameObject ModuleManager;
        InGamePlayModel inGameplayModel;
        CameraModel cameraModel;

        MissionLevelSettingModel levelSettingModel;
        TouchBallSettingModel touchBallSettingModel;

        Camera_Controller inGameCamera;
        Camera frontCamera;

        Vector3 startPosition;
        Vector3 startRotation;

        int totalPlayerCount;
        int totalPlayRound;
        bool isPlayPossible;
        public bool IsPlayPossible { get => isPlayPossible; set => isPlayPossible = value; }
        float goalDistance;
        public float GoalDistance { get => goalDistance; set => goalDistance = value; }

        public PostProcessProfile postProcessProfile;
        GameObject inGame;
        GameObject targetArrow;
        GameObject missionEffet;

        List<Coroutine> ListSoundLoop = new List<Coroutine>();
        #region Contents Load

        protected override void OnLoadStart()
        {
            //todo... 씬 이동후  오브젝트 생성필요
            ModuleManager = GameObject.Find("ModuleManager");
            inGameCamera = GameObject.Find("ProjectionCamera").GetComponent<Camera_Controller>();
            inGameCamera.transform.parent = ModuleManager.transform;
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            UI.IDialog.RequestDialogEnter<UI.LoadingDialog>();
            Message.Send<LoadingModeInfoMsg>(new LoadingModeInfoMsg(ModeType.MissionMode));
            Debug.Log(TAG + "OnLoadStart");
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
                    inGameplayModel = Model.First<InGamePlayModel>();
                    settingModel = Model.First<SettingModel>();
                    playersModel = Model.First<PlayersModel>();
                    cameraModel = Model.First<CameraModel>();
                    levelSettingModel = Model.First<MissionLevelSettingModel>();
                    touchBallSettingModel = Model.First<TouchBallSettingModel>();
                    
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

            SetLoadComplete();
        }
        #endregion

        protected override void OnLoadComplete()
        {
            Debug.Log(TAG + "OnLoadComplete");
        }

        protected override void OnEnter()
        {
            InitGameObjet();
            InitPlayerBall();
            InitObjectPosition();
            Init();
            ListSoundLoop.Add(StartCoroutine(SoundEffect()));
            SoundManager.Instance.PlaySound((int)SoundType.Mission_BGM);
            //ListSoundLoop.Add(StartCoroutine(SoundBGM()));
            //SoundManager.Instance.PlaySound((int)SoundType.Crowd_Effect);
            SensorSetting(new SensorSettingMsg());
            AddMessage();
            StartCoroutine(LoadingVideo());
        }

        IEnumerator LoadingVideo()
        {
            yield return new WaitForSeconds(4.0f);
            Message.Send<LoadingCompleteMsg>(new LoadingCompleteMsg());
        }

        void AddMessage()
        {
            Message.AddListener<T3SensorStartMsg>(OnSensorReady);
            Message.AddListener<T3ResultMsg>(OnT3Result);
            Message.AddListener<SensorSettingMsg>(SensorSetting);
            Message.AddListener<TimeOutMsg>(TimeOut);
            Message.AddListener<SetMissionObjectMsg>(SetMissionObject);
            Message.AddListener<TurnChangeMsg>(TurnChange);
            Message.AddListener<BallStopMsg>(BallStop);
            Message.AddListener<MissionEndMsg>(MissionEnd);
            Message.AddListener<MissionEffectMsg>(MissionEffect);
            Message.AddListener<MissionTimerStartMsg>(MissionTimerStart);
            //테스트
            Message.AddListener<TempEditorSensorCheckMsg>(TempEditorSensorCheck);
        }

        void Init()
        {
            Debug.Log(TAG + "Init");

            inGameCamera.Init(cameraModel.Start_Position, cameraModel.Start_Rotation);
            inGameCamera.SetPositionReset();
            inGameplayModel.Round = 1;
            inGameplayModel.PlayerNum = 0;
            totalPlayerCount = inGameplayModel.TotalPlayerCount;
            totalPlayRound = inGameplayModel.TotalPlayRound;
            for (int i = 0; i < totalPlayerCount; i++)
            {
                playersModel.InitMissionPlayerModel(i);
                playersModel.SetMissionStartPosition(i, frontCamera.transform.position);
            }
            GoalDistance = Vector3.Distance(frontCamera.transform.position, Gate1.transform.position);

            for (int i = 0; i < totalPlayerCount; i++)
            {
                InitTouchBall(i);
            }
            SetTouchBall(inGameplayModel.PlayerNum);

            SetMissionObject(new SetMissionObjectMsg(playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum)));
        }

        void InitGameObjet()
        {
            Red_Ball = GameObject.Find("Prop_BALL01");
            Red_Ball.AddComponent<MissionModeBall_Controller>();
            White_Ball = GameObject.Find("Prop_BALL02");
            White_Ball.AddComponent<MissionModeBall_Controller>();
            Player_Touch_Ball = new GameObject[inGameplayModel.TotalPlayerCount];

            for (int i = 0; i < inGameplayModel.TotalPlayerCount; i++)
            {
                Player_Touch_Ball[i] = GameObject.Find("PlayerTouch" + (i + 1).ToString());
            }
            Gate1 = GameObject.Find("GATE01");
            Gate2 = GameObject.Find("GATE02");
            Gate3 = GameObject.Find("GATE03");
            Pole = GameObject.Find("PoleBase");

            inGameCamera.SetPostProcessProfile(postProcessProfile);
            frontCamera = GameObject.Find("FrontEye").GetComponent<Camera>();
        }

        void InitPlayerBall()
        {
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
            tempLevel = inGameplayModel.PlayLevel;

            Gate1.transform.position = levelSettingModel.GetGate1Position(tempLevel);
            Gate1.transform.eulerAngles = levelSettingModel.GetGate1Rotation(tempLevel);
            Gate1.transform.localScale = levelSettingModel.GetGate1Scale(tempLevel);

            Gate2.transform.position = levelSettingModel.GetGate2Position(tempLevel);
            Gate2.transform.eulerAngles = levelSettingModel.GetGate2Rotation(tempLevel);
            Gate2.transform.localScale = levelSettingModel.GetGate2Scale(tempLevel);

            Gate3.transform.position = levelSettingModel.GetGate3Position(tempLevel);
            Gate3.transform.eulerAngles = levelSettingModel.GetGate3Rotation(tempLevel);
            Gate3.transform.localScale = levelSettingModel.GetGate3Scale(tempLevel);

            Pole.transform.position = levelSettingModel.GetPolePosition(tempLevel);
            Pole.transform.localScale = levelSettingModel.GetPoleScale(tempLevel);
        }

        //완전 초기화 볼 생성 셋팅 파일에 오브젝트로 위치 전환 
        void InitTouchBall(int playerNum)
        {
            Level tempLevel = inGameplayModel.PlayLevel;
            int tempTouchBallCount = touchBallSettingModel.GetBallCount(tempLevel);

            //볼 오브젝트 닫기
            for (int i = 0; i < Player_Touch_Ball[playerNum].transform.childCount; i++)
            {
                Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.SetActive(false);
            }

            //터치볼 오브젝트 모델에 셋팅값으로 셋팅
            for (int i = 0; i < tempTouchBallCount; i++)
            {
                playersModel.SetTouchBallPosition(playerNum, i, touchBallSettingModel.GetBallPosition(tempLevel, i));
            }

            //실질적 볼 포지션 배치
            for (int i = 0; i < tempTouchBallCount; i++)
            {
                Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.GetComponent<TouchBall_Controller>().Init(touchBallSettingModel.GetBallPosition(tempLevel, i));
            }
        }

        void SetTouchBall(int playerNum)
        {
            //볼 오브젝트 열기
            Level tempLevel = inGameplayModel.PlayLevel;
            int tempTouchBallCount = touchBallSettingModel.GetBallCount(tempLevel);

            for (int i = 0; i < totalPlayerCount; i++)
            {
                for (int j = 0; j < tempTouchBallCount; j++)
                {
                    Player_Touch_Ball[i].transform.GetChild(j).localScale = new Vector3(settingModel.BallScale, settingModel.BallScale, settingModel.BallScale);
                    Player_Touch_Ball[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < tempTouchBallCount; i++)
            {
                if (!Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.GetComponent<TouchBall_Controller>().isTouch)
                    Player_Touch_Ball[playerNum].transform.GetChild(i).gameObject.SetActive(true);
                else
                    playersModel.RemoveTouchBallPostion(playerNum, i);
            }
        }

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

        void SensorSetting(SensorSettingMsg msg)
        {
            settingModel.LoadSetting();
            //볼 셋팅
            forceScale = settingModel.ForceScale;

            //센서 셋팅
            sensorDistance = settingModel.SensorDistance;
            sensor1Scale = settingModel.Sensor1Scale;
            sensor1Offset = settingModel.Sensor1Offset;
            sensor2Scale = settingModel.Sensor2Scale;
            sensor2Offset = settingModel.Sensor2Offset;
        }

        void OnSensorReady(T3SensorStartMsg msg)
        {
            IsPlayPossible = true;
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(GoalDistance));
        }

        //물리센서에서 받은 값으로 스톤의 방향과 속도를 결정하는 프로세스
        void OnT3Result(T3ResultMsg msg)
        {
            Log.Instance.log("OnT3Result");
            if (IsPlayPossible)
                InputSensor(msg.Datas[0].posX, msg.Datas[1].posX, msg.Datas[0].time, msg.Datas[1].time);
        }

        void InputSensor(float pos1, float pos2, float time1, float time2)
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
            Vector3 secondtVec = new Vector3(fixpos2, 0, sensorDistance);
            Vector3 dir = secondtVec - firstVec;

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
                startPos = rHit.point - ray.direction * (settingModel.BallScale * 0.0415f);
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

            if (inGameplayModel.PlayerNum % 2 == 0)
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
            tempBallController.BallAddForce(startPos, force, playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum));
        }

        void TimeOut(TimeOutMsg msg)
        {
            Message.Send<MissionEndMsg>(new MissionEndMsg(false));
        }

        void SetMissionObject(SetMissionObjectMsg msg)
        {
            if (playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum) == MissionModeGame.Gate_1)
            {
                inGameCamera.SetPosition(playersModel.GetMissionStartPosition(inGameplayModel.PlayerNum),
                    Gate1.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate1.transform.position);

//                targetArrow.SetActive(true);
                targetArrow.transform.position = Gate1.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate1.transform;
            }
            else if (playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum) == MissionModeGame.Gate_2)
            {
                inGameCamera.SetPosition(playersModel.GetMissionStartPosition(inGameplayModel.PlayerNum),
                  Gate2.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate2.transform.position);

//                targetArrow.SetActive(true);
                targetArrow.transform.position = Gate2.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate2.transform;
            }
            else if (playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum) == MissionModeGame.Gate_3)
            {
                inGameCamera.SetPosition(playersModel.GetMissionStartPosition(inGameplayModel.PlayerNum),
                    Gate3.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position,
                    Gate3.transform.position);

                //targetArrow.SetActive(true);
                targetArrow.transform.position = Gate3.transform.position + new Vector3(0, 1.5f, 0);

                missionEffet.transform.parent = Gate3.transform;
            }
            else if (playersModel.GetMissionModeNowMission(inGameplayModel.PlayerNum) == MissionModeGame.Pole)
            {
                inGameCamera.SetPosition(playersModel.GetMissionStartPosition(inGameplayModel.PlayerNum),
                    Pole.transform.position, Vector3.zero);

                GoalDistance = Vector3.Distance(frontCamera.transform.position, Pole.transform.position);

                //targetArrow.SetActive(true);
                targetArrow.transform.position = Pole.transform.position + new Vector3(0, 1.0f, 0);

                missionEffet.transform.parent = Pole.transform;
            }
            targetArrow.transform.LookAt(frontCamera.transform);
        }

        void TurnChange(TurnChangeMsg msg)
        {
            GameObject temp_Start;
            GameObject temp_Target;
            MissionModeBall_Controller temp_Start_Controller;
            if (inGameplayModel.PlayerNum % 2 == 0)
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
            SetTouchBall(inGameplayModel.PlayerNum);
        }

        private void BallStop(BallStopMsg msg)
        {
            playersModel.SetMissionStartPosition(inGameplayModel.PlayerNum, msg.ballPosition);
        }

        private void MissionEnd(MissionEndMsg msg)
        {
            targetArrow.SetActive(false);
            int tempPlayerNum = inGameplayModel.PlayerNum;
            MissionModeGame tempMission = playersModel.GetMissionModeNowMission(tempPlayerNum);
            Level tempLevel = inGameplayModel.PlayLevel;
            int tempTouchBallCount = touchBallSettingModel.GetBallCount(tempLevel);
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
                        playersModel.SetTouchBallPosition(tempPlayerNum, i, Player_Touch_Ball[tempPlayerNum].transform.GetChild(i).transform.position);
                    else
                    {
                        playersModel.RemoveTouchBallPostion(tempPlayerNum, i);
                    }
                }
            }
        }

        private void MissionEffect(MissionEffectMsg msg)
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

        private void MissionTimerStart(MissionTimerStartMsg msg)
        {
            targetArrow.SetActive(true);
        }

        protected override void OnExit()
        {
            foreach (var o in ListSoundLoop)
                StopCoroutine(o);

            ListSoundLoop.Clear();
            SoundManager.Instance.StopSound((int)SoundType.Mission_BGM);
            //SoundManager.Instance.StopSound((int)SoundType.Crowd_Effect);
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<T3SensorStartMsg>(OnSensorReady);
            Message.RemoveListener<T3ResultMsg>(OnT3Result);
            Message.RemoveListener<SensorSettingMsg>(SensorSetting);
            Message.RemoveListener<TimeOutMsg>(TimeOut);
            Message.RemoveListener<SetMissionObjectMsg>(SetMissionObject);
            Message.RemoveListener<TurnChangeMsg>(TurnChange);
            Message.RemoveListener<BallStopMsg>(BallStop);
            Message.RemoveListener<MissionEndMsg>(MissionEnd);
            Message.RemoveListener<MissionEffectMsg>(MissionEffect);
            Message.RemoveListener<MissionTimerStartMsg>(MissionTimerStart);
            //테스트
            Message.RemoveListener<TempEditorSensorCheckMsg>(TempEditorSensorCheck);
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
        void TempEditorSensorCheck(TempEditorSensorCheckMsg msg)
        {
            IsPlayPossible = true;
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(GoalDistance));
        }

    }
}
