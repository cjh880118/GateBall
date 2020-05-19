using System;
using UnityEngine;
using System.Collections.Generic;
using CellBig.Constants;
using System.IO;

namespace CellBig.Models
{
    public class SettingModel : Model
    {
        GameModel _owner;

        public LocalizingType LocalizingType = LocalizingType.KR;

        float forceScale = 0;
        public float ForceScale
        {
            set { forceScale = value; }
            get { return forceScale; }
        }

        float ballMass;
        public float BallMass
        {
            set { ballMass = value; }
            get { return ballMass; }
        }

        float ballScale;
        public float BallScale
        {
            set { ballScale = value; }
            get { return ballScale; }
        }

        float ballDrag;
        public float BallDrag
        {
            set { ballDrag = value; }
            get { return ballDrag; }
        }

        float ballAngularDrag;
        public float BallAngularDrag
        {
            set { ballAngularDrag = value; }
            get { return ballAngularDrag; }
        }

        float sensorDistance = 0;
        public float SensorDistance
        {
            set { sensorDistance = value; }
            get { return sensorDistance; }
        }

        float sensor1Scale;
        public float Sensor1Scale
        {
            set { sensor1Scale = value; }
            get { return sensor1Scale; }
        }

        float sensor1Offset;
        public float Sensor1Offset
        {
            set { sensor1Offset = value; }
            get { return sensor1Offset; }
        }

        float sensor2Scale;
        public float Sensor2Scale
        {
            set { sensor2Scale = value; }
            get { return sensor2Scale; }
        }

        float sensor2Offset;
        public float Sensor2Offset
        {
            set { sensor2Offset = value; }
            get { return sensor2Offset; }
        }

        

        public void Setup(GameModel owner)
        {
            _owner = owner;
            LoadSettingFile();
        }

        public void LoadSetting()
        {
            LoadSettingFile();
        }

        bool isTestMode;
        public bool IsTestMode { get => isTestMode; set => isTestMode = value; }

        public void SaveSetting()
        {
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/Setting.txt";
            using (StreamWriter sw = new StreamWriter(@pathBasic + path, false))
            {
                sw.WriteLine("Localizing=0");
                sw.WriteLine("ForceScale=" + forceScale);
                sw.WriteLine("BallMass=" + ballMass);
                sw.WriteLine("BallScale=" + ballScale);
                sw.WriteLine("BallDrag=" + ballDrag);
                sw.WriteLine("BallAngularDrag=" + ballAngularDrag);
                sw.WriteLine("SensorDistance=" + sensorDistance);
                sw.WriteLine("Sensor1Scale=" + sensor1Scale);
                sw.WriteLine("Sensor1Offset=" + sensor1Offset);
                sw.WriteLine("Sensor2Scale=" + sensor2Scale);
                sw.WriteLine("Sensor2Offset=" + sensor2Offset);
                sw.Close();
            }
        }

        void LoadSettingFile()
        {
            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/Setting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("Localizing"))
                        LocalizingType = (LocalizingType)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("ForceScale"))
                        forceScale = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallMass"))
                        ballMass = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallScale"))
                        ballScale = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallDrag"))
                        ballDrag = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallAngularDrag"))
                        ballAngularDrag = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("SensorDistance"))
                        sensorDistance = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Sensor1Scale"))
                        sensor1Scale = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Sensor1Offset"))
                        sensor1Offset = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Sensor2Scale"))
                        sensor2Scale = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Sensor2Offset"))
                        sensor2Offset = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("TestMode"))
                        isTestMode = bool.Parse(line.Split('=')[1]);
                }
                file.Close();
                line = string.Empty;
            }
        }

        public string GetLocalizingPath()
        {
            string path = "";
            switch (LocalizingType)
            {
                case LocalizingType.KR: path = "KR"; break;
                case LocalizingType.JP: path = "JP"; break;
                case LocalizingType.EN: path = "EN"; break;
                case LocalizingType.CH: path = "CH"; break;
            }
            return LocalizingType.ToString();
        }
    }
}
