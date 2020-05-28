using JHchoi.Models;
using JHchoi.T3;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JHchoi.Contents
{
    public abstract class IGateBall : IContent
    {
        static string TAG = "IGateBall :: ";

        protected GameObject Red_Ball;
        protected GameObject White_Ball;
        protected GameObject ModuleManager;
        protected Camera_Controller inGameCamera;
        protected Camera frontCamera;
        protected GameObject inGame;
        protected GameObject targetArrow;
        protected GameObject missionEffet;
        protected List<Coroutine> ListSoundLoop = new List<Coroutine>();

        [Header("<Test Input Sensor>")]
        public Vector3 firstSensor;
        public float firstTime;
        public Vector3 SecondSensor;
        public float secondTime;

        [Header("<Sensor Value>")]
        protected float sensorDistance;
        protected float forceScale;

        [Header("<Sensor Offset Scale>")]
        public float sensor1Scale = 1f;
        public float sensor1Offset = 0f;
        public float sensor2Scale = 1f;
        public float sensor2Offset = 0f;

        public PostProcessProfile postProcessProfile;

        protected SettingModel sm;
        protected PlayersModel pm;
        protected InGamePlayModel igm;
        protected MissionLevelSettingModel lsm;

        bool isPlayPossible;
        public bool IsPlayPossible { get => isPlayPossible; set => isPlayPossible = value; }
        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
            igm = Model.First<InGamePlayModel>();
            sm = Model.First<SettingModel>();
            pm = Model.First<PlayersModel>();
            lsm = Model.First<MissionLevelSettingModel>();

            ModuleManager = GameObject.Find("ModuleManager");
            inGameCamera = GameObject.Find("ProjectionCamera").GetComponent<Camera_Controller>();
            inGameCamera.transform.parent = ModuleManager.transform;
            UI.IDialog.RequestDialogEnter<UI.LoadingDialog>();
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            InitContent();
        }

        protected void AddMessage()
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

        protected abstract void InitContent();

        protected void SensorSetting(SensorSettingMsg msg)
        {
            sm.LoadSetting();
            //볼 셋팅
            forceScale = sm.ForceScale;

            //센서 셋팅
            sensorDistance = sm.SensorDistance;
            sensor1Scale = sm.Sensor1Scale;
            sensor1Offset = sm.Sensor1Offset;
            sensor2Scale = sm.Sensor2Scale;
            sensor2Offset = sm.Sensor2Offset;
        }

        //protected abstract void AddMessage();
        void OnSensorReady(T3SensorStartMsg msg)
        {
            IsPlayPossible = true;
            SensorReady();
        }

        protected abstract void SensorReady();

        private void OnT3Result(T3ResultMsg msg)
        {
            Log.Instance.log("OnT3Result");
            if (IsPlayPossible)
                InputSensor(msg.Datas[0].posX, msg.Datas[1].posX, msg.Datas[0].time, msg.Datas[1].time);
        }

        protected abstract void InputSensor(float pos1, float pos2, float time1, float time2);

        private void TimeOut(TimeOutMsg msg)
        {
            Message.Send<MissionEndMsg>(new MissionEndMsg(false));
        }

        private void SetMissionObject(SetMissionObjectMsg msg)
        {
            MissionSetting(msg);
        }

        protected abstract void MissionSetting(SetMissionObjectMsg msg);

        private void TurnChange(TurnChangeMsg msg)
        {
            TurnEnd(msg);
        }

        protected abstract void TurnEnd(TurnChangeMsg msg);

        private void BallStop(BallStopMsg msg)
        {
            pm.SetMissionStartPosition(igm.PlayerNum, msg.ballPosition);
        }

        private void MissionEnd(MissionEndMsg msg)
        {
            MissionEndInfo(msg);
        }

        protected abstract void MissionEndInfo(MissionEndMsg msg);

        private void MissionEffect(MissionEffectMsg msg)
        {
            MissionInfoEffect();
        }

        protected abstract void MissionInfoEffect();

        private void MissionTimerStart(MissionTimerStartMsg msg)
        {
            targetArrow.SetActive(true);
        }

        private void TempEditorSensorCheck(TempEditorSensorCheckMsg msg)
        {
            IsPlayPossible = true;
            EditSensorCheck();
        }

        protected abstract void EditSensorCheck();
    
        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            RemoveMessage();
        }

        protected void RemoveMessage()
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
    }
}