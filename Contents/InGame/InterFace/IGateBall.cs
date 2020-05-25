using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace JHchoi.Contents
{
    public class IGateBall : IContent
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

        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}