using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Models
{
    public class CameraModel : Model
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
            string path = "Setting/CameraSetting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;
                    //StartCamera
                    if (line.StartsWith("StartPosition"))
                        start_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("StartRotation"))
                        start_Rotation = StringToVector3(line.Split('=')[1]);

                }
                file.Close();
                line = string.Empty;
            }
        }

        public Vector3 StringToVector3(string rString)
        {
            string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
            Vector3 rValue = new Vector3(x, y, z);
            return rValue;
        }

        Vector3 start_Position;
        public Vector3 Start_Position { get => start_Position; }

        Vector3 start_Rotation;
        public Vector3 Start_Rotation { get => start_Rotation; }


    }
}
