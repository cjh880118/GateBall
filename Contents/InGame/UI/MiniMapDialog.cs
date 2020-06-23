using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JHchoi.UI.Event;
using System;
using JHchoi.Constants;

namespace JHchoi.UI
{
    public class MiniMapDialog : IDialog
    {
        public GameObject gate1;
        public GameObject gate2;
        public GameObject gate3;
        public GameObject pole;
        public GameObject playerBall;
        public GameObject touchBall;
        public GameObject[] missionInfo;

        float maxWidht = 137.5f;
        float minWidth = -137.5f;

        float maxHeight = 131.5f;
        float minHeight = -131.5f;

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            AddMessage();
        }

        void AddMessage()
        {
            Message.AddListener<SetMiniMapObjectMsg>(SetMiniMapObject);
        }

        private void SetMiniMapObject(SetMiniMapObjectMsg msg)
        {
            for(int i = 0; i < missionInfo.Length; i++)
            {
                missionInfo[i].SetActive(false);
            }

            missionInfo[(int)msg.mission].SetActive(true);

            gate1.GetComponent<RectTransform>().localPosition = miniMapPos(msg.dicMissionObject[MissionModeGame.Gate_1].Position);
            gate1.transform.localEulerAngles = ObjectRotation(msg.dicMissionObject[MissionModeGame.Gate_1].Rotation);

            gate2.GetComponent<RectTransform>().localPosition = miniMapPos(msg.dicMissionObject[MissionModeGame.Gate_2].Position);
            gate2.transform.localEulerAngles = ObjectRotation(msg.dicMissionObject[MissionModeGame.Gate_2].Rotation);

            gate3.GetComponent<RectTransform>().localPosition = miniMapPos(msg.dicMissionObject[MissionModeGame.Gate_3].Position);
            gate3.transform.localEulerAngles = ObjectRotation(msg.dicMissionObject[MissionModeGame.Gate_3].Rotation);

            pole.GetComponent<RectTransform>().localPosition = miniMapPos(msg.dicMissionObject[MissionModeGame.Pole].Position);

            playerBall.GetComponent<RectTransform>().localPosition = miniMapPos(msg.ballPosition);
              
            for(int i = 0; i < touchBall.transform.childCount; i++)
            {
                touchBall.transform.GetChild(i).gameObject.SetActive(false);
            }
         
            for (int i = 0; i < msg.touchBallcount; i++)
            {
                if (msg.dicTouchBall.ContainsKey(i))
                {
                    touchBall.transform.GetChild(i).gameObject.SetActive(true);
                    touchBall.transform.GetChild(i).gameObject.transform.localPosition = miniMapPos(msg.dicTouchBall[i]);
                }
            }
        }

        Vector3 miniMapPos(Vector3 worldPos)
        {
            Vector3 mapPos = new Vector3(); ;
            mapPos = GetMinimapPos(worldPos);
            return mapPos;
        }

        Vector3 GetMinimapPos(Vector3 worldPos)
        {
            Vector3 minimapPos = new Vector3();
            minimapPos.x = (worldPos.x * (maxWidht - minWidth)) + minWidth;
            minimapPos.y = (worldPos.y * (maxHeight - minHeight)) + minHeight;
            return minimapPos;
        }
    
        //3d y 2d z (-)
        Vector3 ObjectRotation(Vector3 vec3)
        {
            Vector3 tempVec3;
            tempVec3 = new Vector3(0, 0, -vec3.y);
            return tempVec3;
        }

        protected override void OnExit()
        {
            RemoveMessage();
        }

        void RemoveMessage()
        {
            Message.RemoveListener<SetMiniMapObjectMsg>(SetMiniMapObject);
        }

        protected override void OnUnload()
        {

        }
    }
}