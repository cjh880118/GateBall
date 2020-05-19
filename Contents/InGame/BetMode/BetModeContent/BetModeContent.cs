using System.Collections;
using UnityEngine;
using CellBig.UI.Event;
using CellBig.T3;
using CellBig.Models;
using System;
using CellBig.Constants;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace CellBig.Contents
{
    public class BetModeContent : IContent
    {
        static string TAG = "BetModeContent :: ";

        GameObject Red_Ball;
        GameObject White_Ball;

        BetModeBall_Controller red_Ball_Controller;
        BetModeBall_Controller white_Ball_Controller;

        GameObject Gate1;
        GameObject Pole;

        Vector3 cameraPosition;
        Vector3 cameraRotation;
        Vector3 targetPosition;
        Vector3 targetRotation;
        Vector3 gateScale;
        Vector3 ballScale;
        Vector3 poleScale;
        Vector3 targetScale;

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
        InGamePlayModel inGameplayModel;
        BetModeModel betModeModel;

        GameObject ModuleManager;

        MissionLevelSettingModel levelSettingModel;

        Camera_Controller inGameCamera;
        Camera frontCamera;

        Vector3 startPosition;
        Vector3 startRotation;

        int totalPlayerCount;
        int totalPlayRound;

        bool isPlayPossible;
        public bool IsPlayPossible { get => isPlayPossible; set => isPlayPossible = value; }
        //Vector3 vecTargetObj;

        public PostProcessProfile postProcessProfile;
        GameObject inGame;
        GameObject targetArrow;
        GameObject missionEffet;

        List<Coroutine> ListSoundLoop = new List<Coroutine>();

        //test
        string testMode1;
        string testMode2;
        string tempSensor1;
        string tempSensor2;

        #region Contents Load

        protected override void OnLoadStart()
        {
            //todo... 씬 이동후  오브젝트 생성필요
            ModuleManager = GameObject.Find("ModuleManager");
            inGameCamera = GameObject.Find("ProjectionCamera").GetComponent<Camera_Controller>();
            inGameCamera.transform.parent = ModuleManager.transform;
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
            UI.IDialog.RequestDialogEnter<UI.LoadingDialog>();
            Message.Send<LoadingModeInfoMsg>(new LoadingModeInfoMsg(ModeType.BetMode));
            Debug.Log(TAG + "OnLoadStart");
            StartCoroutine(LoadInitialData());
        }

        IEnumerator LoadInitialData()
        {
            string path;
            path = "Object/GateBall/BetModeObject";
            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>(path,
                o =>
                {
                    var inGameObject = Instantiate(o) as GameObject;
                    inGame = inGameObject;
                    inGame.transform.parent = ModuleManager.transform;
                    inGameplayModel = Model.First<InGamePlayModel>();
                    settingModel = Model.First<SettingModel>();
                    playersModel = Model.First<PlayersModel>();
                    levelSettingModel = Model.First<MissionLevelSettingModel>();
                    betModeModel = Model.First<BetModeModel>();
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

        protected override void OnEnter()
        {
            AddMessage();
            Init();
            InitGameObjet();
            InitPlayerBall();
            testMode1 = "목표물 : " + Gate1.name + " 미션 레벨 : " + inGameplayModel.BetLevel;
            SetMissionObject(new SetMissionObjectMsg(MissionModeGame.Gate_1));
            ListSoundLoop.Add(StartCoroutine(Sound()));
            ListSoundLoop.Add(StartCoroutine(SoundWave()));
            SensorSetting(new SensorSettingMsg());
            StartCoroutine(LoadingVideo());
        }

        IEnumerator LoadingVideo()
        {
            yield return new WaitForSeconds(4.0f);
            Message.Send<LoadingCompleteMsg>(new LoadingCompleteMsg());
        }

        void Init()
        {
            inGameplayModel.Round = 1;
            inGameplayModel.PlayerNum = 0;
            inGameplayModel.BetLevel = betModeModel.StartLV;
            totalPlayerCount = inGameplayModel.TotalPlayerCount;
            totalPlayRound = inGameplayModel.TotalPlayRound;
        }

        void InitGameObjet()
        {
            Gate1 = GameObject.Find("GATE01");
            Pole = GameObject.Find("PoleBase");
            gateScale = Gate1.transform.localScale;
            targetScale = Gate1.transform.localScale;
            poleScale = Pole.transform.localScale;

            Pole.SetActive(false);
            inGameCamera.SetPostProcessProfile(postProcessProfile);
            frontCamera = GameObject.Find("FrontEye").GetComponent<Camera>();

            //1 = Y 90   3 = y90
            int startNum = UnityEngine.Random.Range(0, 4);
            if (startNum == 1 || startNum == 3)
            {
                Gate1.transform.localEulerAngles = new Vector3(0, 90, 0);
            }

            Gate1.transform.localPosition = betModeModel.GetTargetPosition((BetStartPosition)startNum);

            inGameCamera.SetPosition(betModeModel.GetStartPosition((BetStartPosition)startNum),
                betModeModel.GetTargetPosition((BetStartPosition)startNum),
                Vector3.zero);

            cameraPosition = inGameCamera.gameObject.transform.localPosition;
            cameraRotation = inGameCamera.gameObject.transform.localEulerAngles;

            targetPosition = Gate1.transform.localPosition;
            targetRotation = Gate1.transform.localEulerAngles;
        }

        void InitPlayerBall()
        {
            Red_Ball = GameObject.Find("Prop_BALL01");
            Red_Ball.AddComponent<BetModeBall_Controller>();
            White_Ball = GameObject.Find("Prop_BALL02");
            White_Ball.AddComponent<BetModeBall_Controller>();

            ballScale = Vector3.one * settingModel.BallScale;

            red_Ball_Controller = Red_Ball.GetComponent<BetModeBall_Controller>();
            white_Ball_Controller = White_Ball.GetComponent<BetModeBall_Controller>(); ;

            red_Ball_Controller.Init(Gate1.transform.position);
            white_Ball_Controller.Init(Gate1.transform.position);

            Red_Ball.SetActive(false);
            red_Ball_Controller.isPlayerBall = true;
            White_Ball.SetActive(false);
        }

        IEnumerator Sound()
        {
            while (true)
            {
                float range = UnityEngine.Random.Range(25, 30);
                yield return new WaitForSeconds(range);
                SoundManager.Instance.PlaySound((int)SoundType.Bet_Bird_Effect);
            }
        }

        IEnumerator SoundWave()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);
                int num = UnityEngine.Random.Range(0, 2);
                if (num == 0) SoundManager.Instance.PlaySound((int)SoundType.Wave_Effect1);
                else SoundManager.Instance.PlaySound((int)SoundType.Wave_Effect2);
            }
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
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(0));
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
            Log.Instance.log("Input Senor2 value : " + pos2 + " / fix : " + fixpos2 + "Input Senor2 Time : " + time2);

            tempSensor1 = "Input Senor1 value : " + Math.Truncate(pos1 * 100) / 100 + " / fix : " + Math.Truncate(fixpos1 * 100) / 100 + "Input Senor1 Time : " + Math.Truncate(time1 * 100) / 100;
            tempSensor2 = "Input Senor2 value : " + Math.Truncate(pos2 * 100) / 100 + " / fix : " + Math.Truncate(fixpos2 * 100) / 100 + "Input Senor2 Time : " + Math.Truncate(time2 * 100) / 100;

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

            Ray ray = frontCamera.ViewportPointToRay(new Vector3(fixpos2, 0.05f));
            RaycastHit rHit;

            dir = Quaternion.LookRotation(ray.direction) * dir;
            Vector3 startPos = frontCamera.transform.position;

            if (Physics.Raycast(ray, out rHit, 1 << LayerMask.NameToLayer("Ground")))
            {
                startPos = rHit.point - ray.direction * 0.15f;
            }
            dir.y = 0f;
            dir = dir.normalized;

            startPos -= dir;

            float time = time2 - time1;
            float speed = (sensorDistance / time);
            Debug.Log("speed : " + speed);
            force = dir * speed * forceScale;
            GameObject tempBall;
            BetModeBall_Controller tempBallController;

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

            tempBall.SetActive(true);
            red_Ball_Controller._rigidbody.constraints = RigidbodyConstraints.None;
            white_Ball_Controller._rigidbody.constraints = RigidbodyConstraints.None;

            startPos.y = -1.0f;

            Message.Send<CameraMoveMsg>(new CameraMoveMsg(tempBall, frontCamera.gameObject));

            tempBallController.BallAddForce(startPos, force, inGameplayModel.BetModeMission);
        }

        void TimeOut(TimeOutMsg msg)
        {
            Message.Send<MissionEndMsg>(new MissionEndMsg(false));
        }

        void SetMissionObject(SetMissionObjectMsg msg)
        {
            int level = inGameplayModel.BetLevel;
            if (level > betModeModel.MaxLV)
                level = betModeModel.MaxLV;

            int distance = 0;
            int position = 0;
            int scale = 0;

            int startNum = UnityEngine.Random.Range(0, 4);

            //카메라 배치
            inGameCamera.SetPosition(betModeModel.GetStartPosition((BetStartPosition)startNum),
              betModeModel.GetTargetPosition((BetStartPosition)startNum),
              Vector3.zero);

            GameObject Target = null;
            distance = UnityEngine.Random.Range(0, level);
            if (distance > betModeModel.PlusZCount)
                distance = betModeModel.PlusZCount;
            level -= distance;

            position = UnityEngine.Random.Range(0, level);
            if (position > betModeModel.PlusXCount)
                position = betModeModel.PlusXCount;
            level -= position;


            scale = UnityEngine.Random.Range(0, level);
            if (scale > betModeModel.ScaleCount)
                scale = betModeModel.ScaleCount;
            level -= scale;

            int mission;
            if (level >= betModeModel.PoleLV)
                mission = UnityEngine.Random.Range(0, 3);
            else if (level >= betModeModel.PoleLV && level < betModeModel.TouchLV)
                mission = UnityEngine.Random.Range(0, 2);
            else
                mission = 0;

            Gate1.transform.localScale = gateScale;
            White_Ball.transform.localScale = ballScale;
            Red_Ball.transform.localScale = ballScale;
            Pole.transform.localScale = poleScale;

            Gate1.SetActive(false);
            White_Ball.SetActive(false);
            Red_Ball.SetActive(false);
            Pole.SetActive(false);

            if (mission == 0)
            {
                level -= betModeModel.GateLV;
                Target = Gate1;
                inGameplayModel.BetModeMission = BetModeGame.Gate_1;

                if (startNum == 1 || startNum == 3)
                {
                    Gate1.transform.localEulerAngles = new Vector3(0, 90, 0);
                }
                else
                {
                    Gate1.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else if (mission == 1)
            {
                level -= betModeModel.TouchLV;
                inGameplayModel.BetModeMission = BetModeGame.Touch;

                if (inGameplayModel.PlayerNum % 2 == 0)
                {
                    Target = White_Ball;
                }
                else
                {
                    Target = Red_Ball;
                }
            }
            else if (mission == 2)
            {
                Target = Pole;
                level -= betModeModel.PoleLV;
                inGameplayModel.BetModeMission = BetModeGame.Pole;
            }

            if (level > 0)
                distance += level;

            if (distance > betModeModel.PlusZCount)
                distance = betModeModel.PlusZCount;

            level -= distance;

            if (level > 0)
                position += level;

            testMode1 = "목표물 : " + Target.name + " 미션 레벨 : " + inGameplayModel.BetLevel;
            Debug.Log("목표물 : " + Target.name);
            testMode2 = "스타팅 : " + startNum + " 거리 : " + distance + " 위치 : " + position + " 크기 : " + scale;
            Debug.Log("스타팅 : " + startNum + " 거리 : " + distance + " 위치 : " + position + " 크기 : " + scale);

            Target.SetActive(true);
            Target.transform.localPosition = betModeModel.GetTargetPosition((BetStartPosition)startNum);

            int plusMinus = UnityEngine.Random.Range(0, 2);
            if (plusMinus == 0)
                plusMinus = -1;
            else
                plusMinus = 1;

            if (startNum == 1 || startNum == 3)
            {
                //1 3
                if (Target.transform.position.x > inGameCamera.transform.position.x)
                {
                    Target.transform.position = new Vector3(Target.transform.position.x + (distance * betModeModel.PlusZ),
                        Target.transform.position.y,
                        Target.transform.position.z);


                    Target.transform.position = new Vector3(Target.transform.position.x,
                        Target.transform.position.y,
                        Target.transform.position.z + (position * betModeModel.PlusX) * plusMinus);
                }
                else
                {
                    Target.transform.position = new Vector3(Target.transform.position.x - (distance * betModeModel.PlusZ),
                        Target.transform.position.y,
                        Target.transform.position.z);


                    Target.transform.position = new Vector3(Target.transform.position.x,
                        Target.transform.position.y,
                        Target.transform.position.z + (position * betModeModel.PlusX) * plusMinus);
                }
            }
            else
            {
                //0 2
                if (Target.transform.position.z > inGameCamera.transform.position.z)
                {
                    Target.transform.position = new Vector3(Target.transform.position.x,
                        Target.transform.position.y,
                        Target.transform.position.z + (distance * betModeModel.PlusZ));


                    Target.transform.position = new Vector3(Target.transform.position.x + (position * betModeModel.PlusX) * plusMinus,
                        Target.transform.position.y,
                        Target.transform.position.z);
                }
                else
                {
                    Target.transform.position = new Vector3(Target.transform.position.x,
                      Target.transform.position.y,
                      Target.transform.position.z - (distance * betModeModel.PlusZ));


                    Target.transform.position = new Vector3(Target.transform.position.x + (position * betModeModel.PlusX) * plusMinus,
                        Target.transform.position.y,
                        Target.transform.position.z);
                }
            }
            // 2 0.1
            Target.transform.localScale *= 1 - (scale * betModeModel.Scale);
            targetScale = Target.transform.localScale;
            //카메라 위치 저장
            cameraPosition = inGameCamera.gameObject.transform.localPosition;
            cameraRotation = inGameCamera.gameObject.transform.localEulerAngles;

            targetPosition = Target.transform.localPosition;
            targetRotation = Target.transform.localEulerAngles;
        }

        void TurnChange(TurnChangeMsg msg)
        {
            Debug.Log("턴 체인지");
            GameObject temp_Start;
            GameObject temp_Target;
            BetModeBall_Controller temp_Start_Controller;

            inGameCamera.gameObject.transform.localPosition = cameraPosition;
            inGameCamera.gameObject.transform.localEulerAngles = cameraRotation;

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

            temp_Start.transform.localScale = ballScale;
            temp_Start.SetActive(false);

            if (inGameplayModel.BetModeMission == BetModeGame.Gate_1)
            {
                temp_Target.SetActive(false);
                Gate1.transform.localPosition = targetPosition;
                Gate1.transform.localEulerAngles = targetRotation;
                Gate1.transform.localScale = targetScale;

                targetArrow.transform.position = Gate1.transform.position + new Vector3(0, 1.5f, 0);
                missionEffet.transform.position = Gate1.transform.position + new Vector3(0, 2f, 0);
            }
            else if (inGameplayModel.BetModeMission == BetModeGame.Touch)
            {
                temp_Target.SetActive(true);
                temp_Target.transform.localPosition = targetPosition;
                temp_Target.transform.localEulerAngles = targetRotation;
                temp_Target.transform.localScale = targetScale;
                temp_Target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                temp_Start.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                targetArrow.transform.position = temp_Target.transform.position + new Vector3(0, 0.5f, 0);
                missionEffet.transform.position = temp_Target.transform.position + new Vector3(0, 1f, 0);
            }
            else if (inGameplayModel.BetModeMission == BetModeGame.Pole)
            {
                temp_Target.SetActive(false);
                Pole.transform.localPosition = targetPosition;
                Pole.transform.localEulerAngles = targetRotation;
                Pole.transform.localScale = targetScale;

                targetArrow.transform.position = Pole.transform.position + new Vector3(0, 1f, 0);
                missionEffet.transform.position = Pole.transform.position + new Vector3(0, 2f, 0);
            }

            targetArrow.transform.LookAt(frontCamera.transform);
            temp_Start_Controller.isPlayerBall = true;
        }

        private void BallStop(BallStopMsg msg)
        {
            playersModel.SetMissionStartPosition(inGameplayModel.PlayerNum, msg.ballPosition);
        }

        private void MissionEnd(MissionEndMsg msg)
        {
            targetArrow.SetActive(false);
        }

        private void MissionEffect(MissionEffectMsg msg)
        {
            int num = UnityEngine.Random.Range(0, 3);
            missionEffet.SetActive(true);
            missionEffet.transform.localEulerAngles = Vector3.zero;

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
            Destroy(targetArrow);
            Destroy(inGame);
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
            Message.Send<MissionTimerStartMsg>(new MissionTimerStartMsg(0));
        }


        private void OnGUI()
        {
            if (settingModel != null && settingModel.IsTestMode)
            {
                GUIStyle myStyle = new GUIStyle();
                myStyle.normal.textColor = Color.red;
                myStyle.fontSize = 20;
                GUI.Label(new Rect(10, 10, 300, 50), testMode1, myStyle);
                GUI.Label(new Rect(10, 60, 300, 50), testMode2, myStyle);
                GUI.Label(new Rect(10, 110, 300, 50), tempSensor1, myStyle);
                GUI.Label(new Rect(10, 160, 300, 50), tempSensor2, myStyle);
            }
        }

    }
}