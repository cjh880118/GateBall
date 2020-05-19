using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;

namespace CellBig.Models
{
    public class MissionLevelSettingModel : Model
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
            string path = "Setting/ObjectSetting/MissionLevelSetting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;
                    //Gate1
                    if (line.StartsWith("EasyGate1Position"))
                        easy_gate1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate1Rotation"))
                        easy_gate1_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate1Scale"))
                        easy_gate1_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("NormalGate1Position"))
                        normal_gate1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate1Rotation"))
                        normal_gate1_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate1Scale"))
                        normal_gate1_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("HardGate1Position"))
                        hard_gate1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate1Rotation"))
                        hard_gate1_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate1Scale"))
                        hard_gate1_Scale = StringToVector3(line.Split('=')[1]);

                    //Gate2
                    if (line.StartsWith("EasyGate2Position"))
                        easy_gate2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate2Rotation"))
                        easy_gate2_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate2Scale"))
                        easy_gate2_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("NormalGate2Position"))
                        normal_gate2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate2Rotation"))
                        normal_gate2_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate2Scale"))
                        normal_gate2_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("HardGate2Position"))
                        hard_gate2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate2Rotation"))
                        hard_gate2_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate2Scale"))
                        hard_gate2_Scale = StringToVector3(line.Split('=')[1]);

                    //Gate3
                    if (line.StartsWith("EasyGate3Position"))
                        easy_gate3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate3Rotation"))
                        easy_gate3_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyGate3Scale"))
                        easy_gate3_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("NormalGate3Position"))
                        normal_gate3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate3Rotation"))
                        normal_gate3_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalGate3Scale"))
                        normal_gate3_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("HardGate3Position"))
                        hard_gate3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate3Rotation"))
                        hard_gate3_Rotation = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardGate3Scale"))
                        hard_gate3_Scale = StringToVector3(line.Split('=')[1]);

                    //Pole
                    if (line.StartsWith("EasyPolePosition"))
                        easy_pole_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyPoleScale"))
                        easy_pole_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("NormalPolePosition"))
                        normal_pole_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalPoleScale"))
                        normal_pole_Scale = StringToVector3(line.Split('=')[1]);

                    if (line.StartsWith("HardPolePosition"))
                        hard_pole_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardPoleScale"))
                        hard_pole_Scale = StringToVector3(line.Split('=')[1]);
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
        /// <summary>
        /// Easy
        /// </summary>
        Vector3 easy_gate1_Position;
        Vector3 easy_gate1_Rotation;
        Vector3 easy_gate1_Scale;

        Vector3 easy_gate2_Position;
        Vector3 easy_gate2_Rotation;
        Vector3 easy_gate2_Scale;

        Vector3 easy_gate3_Position;
        Vector3 easy_gate3_Rotation;
        Vector3 easy_gate3_Scale;

        Vector3 easy_pole_Position;
        Vector3 easy_pole_Scale;

        /// <summary>
        /// Normal
        /// </summary>
        Vector3 normal_gate1_Position;
        Vector3 normal_gate1_Rotation;
        Vector3 normal_gate1_Scale;

        Vector3 normal_gate2_Position;
        Vector3 normal_gate2_Rotation;
        Vector3 normal_gate2_Scale;

        Vector3 normal_gate3_Position;
        Vector3 normal_gate3_Rotation;
        Vector3 normal_gate3_Scale;

        Vector3 normal_pole_Position;
        Vector3 normal_pole_Scale;

        /// <summary>
        /// Hard
        /// </summary>
        Vector3 hard_gate1_Position;
        Vector3 hard_gate1_Rotation;
        Vector3 hard_gate1_Scale;

        Vector3 hard_gate2_Position;
        Vector3 hard_gate2_Rotation;
        Vector3 hard_gate2_Scale;

        Vector3 hard_gate3_Position;
        Vector3 hard_gate3_Rotation;
        Vector3 hard_gate3_Scale;

        Vector3 hard_pole_Position;
        Vector3 hard_pole_Scale;

        public Vector3 GetGate1Position(Level level)
        {
            if (level == Level.Easy)
                return easy_gate1_Position;
            else if (level == Level.Normal)
                return normal_gate1_Position;
            else
                return hard_gate1_Position;
        }

        public Vector3 GetGate1Rotation(Level level)
        {
            if (level == Level.Easy)
                return easy_gate1_Rotation;
            else if (level == Level.Normal)
                return normal_gate1_Rotation;
            else
                return hard_gate1_Rotation;
        }

        public Vector3 GetGate1Scale(Level level)
        {
            if (level == Level.Easy)
                return easy_gate1_Scale;
            else if (level == Level.Normal)
                return normal_gate1_Scale;
            else
                return hard_gate1_Scale;
        }

        public Vector3 GetGate2Position(Level level)
        {
            if (level == Level.Easy)
                return easy_gate2_Position;
            else if (level == Level.Normal)
                return normal_gate2_Position;
            else
                return hard_gate2_Position;
        }

        public Vector3 GetGate2Rotation(Level level)
        {
            if (level == Level.Easy)
                return easy_gate2_Rotation;
            else if (level == Level.Normal)
                return normal_gate2_Rotation;
            else
                return hard_gate2_Rotation;
        }

        public Vector3 GetGate2Scale(Level level)
        {
            if (level == Level.Easy)
                return easy_gate2_Scale;
            else if (level == Level.Normal)
                return normal_gate2_Scale;
            else
                return hard_gate2_Scale;
        }

        public Vector3 GetGate3Position(Level level)
        {
            if (level == Level.Easy)
                return easy_gate3_Position;
            else if (level == Level.Normal)
                return normal_gate3_Position;
            else
                return hard_gate3_Position;
        }

        public Vector3 GetGate3Rotation(Level level)
        {
            if (level == Level.Easy)
                return easy_gate3_Rotation;
            else if (level == Level.Normal)
                return normal_gate3_Rotation;
            else
                return hard_gate3_Rotation;
        }

        public Vector3 GetGate3Scale(Level level)
        {
            if (level == Level.Easy)
                return easy_gate3_Scale;
            else if (level == Level.Normal)
                return normal_gate3_Scale;
            else
                return hard_gate3_Scale;
        }

        public Vector3 GetPolePosition(Level level)
        {
            if (level == Level.Easy)
                return easy_pole_Position;
            else if (level == Level.Normal)
                return normal_pole_Position;
            else
                return hard_pole_Position;
        }

        public Vector3 GetPoleScale(Level level)
        {
            if (level == Level.Easy)
                return easy_pole_Scale;
            else if (level == Level.Normal)
                return normal_pole_Scale;
            else
                return hard_pole_Scale;
        }
    }
}