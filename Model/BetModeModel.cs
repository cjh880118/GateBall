using CellBig.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Models
{
    public class BetModeModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            LoadSettingFile();
        }
        void LoadSettingFile()
        {
            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/ObjectSetting/BetSetting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("startPosition1"))
                        startPosition1 = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("targetPosition1"))
                        targetPosition1 = StringToVector3(line.Split('=')[1]);

                    else if (line.StartsWith("startPosition2"))
                        startPosition2 = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("targetPosition2"))
                        targetPosition2 = StringToVector3(line.Split('=')[1]);

                    else if (line.StartsWith("startPosition3"))
                        startPosition3 = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("targetPosition3"))
                        targetPosition3 = StringToVector3(line.Split('=')[1]);

                    else if (line.StartsWith("startPosition4"))
                        startPosition4 = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("targetPosition4"))
                        targetPosition4 = StringToVector3(line.Split('=')[1]);

                    else if (line.StartsWith("plusX"))
                        plusX = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("xCount"))
                        plusXCount = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("plusZ"))
                        plusZ = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("zCount"))
                        plusZCount = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("scale"))
                        scale = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("sCount"))
                        scaleCount = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("startLV"))
                        startLV = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("maxLV"))
                        maxLV = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("lvUp"))
                        lvUp = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("lvDown"))
                        lvDown = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("GateLV"))
                        gateLV = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("PoleLV"))
                        poleLV = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("TouchLV"))
                        touchLV = int.Parse(line.Split('=')[1]);
                }

                file.Close();
                line = string.Empty;
            }
        }

        Vector3 StringToVector3(string rString)
        {
            string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
            Vector3 rValue = new Vector3(x, y, z);
            return rValue;
        }

        Vector3 startPosition1;
        Vector3 targetPosition1;

        Vector3 startPosition2;
        Vector3 targetPosition2;

        Vector3 startPosition3;
        Vector3 targetPosition3;

        Vector3 startPosition4;
        Vector3 targetPosition4;

        float plusX;
        int plusXCount;

        float plusZ;
        int plusZCount;

        float scale;
        int scaleCount;

        int startLV;
        int maxLV;
        int lvUp;
        int lvDown;

        int gateLV;
        int poleLV;
        int touchLV;

        public Vector3 GetStartPosition(BetStartPosition betStartPosition)
        {
            if (BetStartPosition.Start1 == betStartPosition)
            {
                return startPosition1;
            }
            if (BetStartPosition.Start2 == betStartPosition)
            {
                return startPosition2;
            }
            if (BetStartPosition.Start3 == betStartPosition)
            {
                return startPosition3;
            }
            else
            {
                return startPosition4;
            }
        }

        public Vector3 GetTargetPosition(BetStartPosition betStartPosition)
        {
            if (BetStartPosition.Start1 == betStartPosition)
            {
                return targetPosition1;
            }
            if (BetStartPosition.Start2 == betStartPosition)
            {
                return targetPosition2;
            }
            if (BetStartPosition.Start3 == betStartPosition)
            {
                return targetPosition3;
            }
            else
            {
                return targetPosition4;
            }
        }

        //public Vector3 StartPosition1 { get => startPosition1; set => startPosition1 = value; }
        //public Vector3 TargetPosition1 { get => targetPosition1; set => targetPosition1 = value; }
        //public Vector3 StartPosition2 { get => startPosition2; set => startPosition2 = value; }
        //public Vector3 TargetPosition2 { get => targetPosition2; set => targetPosition2 = value; }
        //public Vector3 StartPosition3 { get => startPosition3; set => startPosition3 = value; }
        //public Vector3 TargetPosition3 { get => targetPosition3; set => targetPosition3 = value; }
        //public Vector3 StartPosition4 { get => startPosition4; set => startPosition4 = value; }
        //public Vector3 TargetPosition4 { get => targetPosition4; set => targetPosition4 = value; }
        public float PlusX { get => plusX; set => plusX = value; }
        public int PlusXCount { get => plusXCount; set => plusXCount = value; }
        public float PlusZ { get => plusZ; set => plusZ = value; }
        public int PlusZCount { get => plusZCount; set => plusZCount = value; }
        public float Scale { get => scale; set => scale = value; }
        public int ScaleCount { get => scaleCount; set => scaleCount = value; }
        public int StartLV { get => startLV; set => startLV = value; }
        public int MaxLV { get => maxLV; set => maxLV = value; }
        public int GateLV { get => gateLV; set => gateLV = value; }
        public int PoleLV { get => poleLV; set => poleLV = value; }
        public int TouchLV { get => touchLV; set => touchLV = value; }
        public int LvUp { get => lvUp; set => lvUp = value; }
        public int LvDown { get => lvDown; set => lvDown = value; }
    }
}