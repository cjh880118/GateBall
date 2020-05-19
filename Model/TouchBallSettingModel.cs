using CellBig.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Models
{
    public class TouchBallSettingModel : Model
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
            string path = "Setting/ObjectSetting/TouchBallSetting.txt";
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;
                    //easy
                    if (line.StartsWith("EasyBall1Position"))
                        easy_ball1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyBall2Position"))
                        easy_ball2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyBall3Position"))
                        easy_ball3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyBall4Position"))
                        easy_ball4_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyBall5Position"))
                        easy_ball5_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("EasyTouchBallCount"))
                        easy_ball_Count = int.Parse(line.Split('=')[1]);

                    //normal
                    if (line.StartsWith("NormalBall1Position"))
                        normal_ball1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalBall2Position"))
                        normal_ball2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalBall3Position"))
                        normal_ball3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalBall4Position"))
                        normal_ball4_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalBall5Position"))
                        normal_ball5_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("NormalTouchBallCount"))
                        normal_ball_Count = int.Parse(line.Split('=')[1]);

                    //hard
                    if (line.StartsWith("HardBall1Position"))
                        hard_ball1_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardBall2Position"))
                        hard_ball2_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardBall3Position"))
                        hard_ball3_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardBall4Position"))
                        hard_ball4_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardBall5Position"))
                        hard_ball5_Position = StringToVector3(line.Split('=')[1]);
                    else if (line.StartsWith("HardTouchBallCount"))
                        hard_ball_Count = int.Parse(line.Split('=')[1]);
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

        int easy_ball_Count;
        Vector3 easy_ball1_Position;
        Vector3 easy_ball2_Position;
        Vector3 easy_ball3_Position;
        Vector3 easy_ball4_Position;
        Vector3 easy_ball5_Position;

        int normal_ball_Count;
        Vector3 normal_ball1_Position;
        Vector3 normal_ball2_Position;
        Vector3 normal_ball3_Position;
        Vector3 normal_ball4_Position;
        Vector3 normal_ball5_Position;

        int hard_ball_Count;
        Vector3 hard_ball1_Position;
        Vector3 hard_ball2_Position;
        Vector3 hard_ball3_Position;
        Vector3 hard_ball4_Position;
        Vector3 hard_ball5_Position;


        public Vector3 GetBallPosition(Level level, int ballNum)
        {
            if (level == Level.Easy)
            {
                if (ballNum == 0)
                    return easy_ball1_Position;
                else if (ballNum == 1)
                    return easy_ball2_Position;
                else if (ballNum == 2)
                    return easy_ball3_Position;
                else if (ballNum == 3)
                    return easy_ball4_Position;
                else
                    return easy_ball5_Position;
            }
            else if (level == Level.Normal)
            {
                if (ballNum == 0)
                    return normal_ball1_Position;
                else if (ballNum == 1)
                    return normal_ball2_Position;
                else if (ballNum == 2)
                    return normal_ball3_Position;
                else if (ballNum == 3)
                    return normal_ball4_Position;
                else
                    return normal_ball5_Position;
            }
            else
            {
                if (ballNum == 0)
                    return hard_ball1_Position;
                else if (ballNum == 1)
                    return hard_ball2_Position;
                else if (ballNum == 2)
                    return hard_ball3_Position;
                else if (ballNum == 3)
                    return hard_ball4_Position;
                else
                    return hard_ball3_Position;
            }
        }

        public int GetBallCount(Level level)
        {
            if (level == Level.Easy)
                return easy_ball_Count;
            else if (level == Level.Normal)
                return normal_ball_Count;
            else
                return hard_ball_Count;
        }
    }
}