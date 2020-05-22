using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi.Models;
using JHchoi.UI.Event;
using System;

namespace JHchoi.UI
{
    public class SettingDialog : IDialog
    {
        SettingModel sm = Model.First<SettingModel>();
        [Header("Ball Setting")]
        public InputField ballScale;
        public InputField ballMass;
        public InputField ballDrag;
        public InputField ballAngularDrag;
        public InputField ballForceScale;

        [Header("Sensor Setting")]
        public InputField sensorDistance;
        public InputField sensor1OffSet;
        public InputField sensor1Scale;
        public InputField sensor2OffSet;
        public InputField sensor2Scale;

        [Header("Sensor Value")]
        public Text sensor1Value;
        public Text sensor1FixValue;
        public Text sensor2Value;
        public Text sensor2FixValue;

        public Button btnSave;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
            sm.LoadSetting();
            SetValue();
        }

        void AddMessage()
        {
            btnSave.onClick.AddListener(() => SaveSetting());
            Message.AddListener<SensorValueMsg>(SetSensorValue);
        }

        void SaveSetting()
        {
            SaveSettingValue();
            sm.SaveSetting();
        }

        void SetSensorValue(SensorValueMsg msg)
        {
            sensor1Value.text = msg.sensor1_Value.ToString();
            sensor1FixValue.text = msg.sensor1_Fix_Value.ToString();
            sensor2Value.text = msg.sensor2_Value.ToString();
            sensor2FixValue.text = msg.sensor2_Fix_Value.ToString();
        }

        void SaveSettingValue()
        {
            sm.BallScale = Convert.ToSingle(ballScale.text);
            sm.ForceScale = Convert.ToSingle(ballForceScale.text);
            sm.BallMass = Convert.ToSingle(ballMass.text);
            sm.BallDrag = Convert.ToSingle(ballDrag.text);
            sm.BallAngularDrag = Convert.ToSingle(ballAngularDrag.text);

            sm.SensorDistance = Convert.ToSingle(sensorDistance.text);
            sm.Sensor1Scale = Convert.ToSingle(sensor1Scale.text);
            sm.Sensor1Offset = Convert.ToSingle(sensor1OffSet.text);
            sm.Sensor2Scale = Convert.ToSingle(sensor2Scale.text);
            sm.Sensor2Offset = Convert.ToSingle(sensor2OffSet.text);
        }

        void SetValue()
        {
            ballScale.text = sm.BallScale.ToString();
            ballMass.text = sm.BallMass.ToString();
            ballDrag.text = sm.BallDrag.ToString(); ;
            ballAngularDrag.text = sm.BallAngularDrag.ToString();
            ballForceScale.text = sm.ForceScale.ToString();

            sensorDistance.text = sm.SensorDistance.ToString();
            sensor1OffSet.text = sm.Sensor1Offset.ToString();
            sensor1Scale.text = sm.Sensor1Scale.ToString();
            sensor2OffSet.text = sm.Sensor2Offset.ToString();
            sensor2Scale.text = sm.Sensor2Scale.ToString();
        }

        protected override void OnExit()
        {
            RemoveMessage();
            Message.Send<SensorSettingMsg>(new SensorSettingMsg());
            Message.Send<BallSettingMsg>(new BallSettingMsg());
        }

        void RemoveMessage()
        {
            btnSave.onClick.RemoveListener(() => SaveSetting());
            Message.RemoveListener<SensorValueMsg>(SetSensorValue);
        }

        protected override void OnUnload()
        {

        }
    }
}