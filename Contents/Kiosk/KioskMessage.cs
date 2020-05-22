using JHchoi.Constants;
using UnityEngine;

namespace JHchoi.UI.Event
{
    public class OnLobbyMsg : Message
    {

    }

    public class OnModeSelectMsg : Message
    {
        public ModeType modeType;
        public OnModeSelectMsg(ModeType modeType)
        {
            this.modeType = modeType;
        }
    }

    public class OnModeSelectMouseOverMsg : Message
    {
        public ModeType modeType;
        public OnModeSelectMouseOverMsg(ModeType modeType)
        {
            this.modeType = modeType;
        }
    }

    public class OnModeSelectMouseOutMsg : Message
    {
        public ModeType modeType;
        public OnModeSelectMouseOutMsg(ModeType modeType)
        {
            this.modeType = modeType;
        }
    }

    public class OnLevelSelectMsg : Message
    {
        public Level level;
        public OnLevelSelectMsg(Level level)
        {
            this.level = level;
        }
    }

    public class OnLevelSelectMouseOverMsg : Message
    {
        public Level level;
        public OnLevelSelectMouseOverMsg(Level level)
        {
            this.level = level;
        }
    }

    public class OnLevelSelectMouseOutMsg : Message
    {
        public Level level;
        public OnLevelSelectMouseOutMsg(Level level)
        {
            this.level = level;
        }
    }

    public class PrevModeSelectMsg : Message
    {

    }

    public class InputPlayerNumMsg : Message
    {
   
    }

    public class PlayerPlusMsg : Message
    {

    }

    public class PlayerMinusMsg : Message
    {

    }

    public class PlayerInputButtonMouseOverMsg : Message
    {
        public bool isPlus;
        public PlayerInputButtonMouseOverMsg(bool isPlus)
        {
            this.isPlus = isPlus;
        }
    }

    public class PlayerInputButtonMouseOutMsg : Message
    {
        public bool isPlus;
        public PlayerInputButtonMouseOutMsg(bool isPlus)
        {
            this.isPlus = isPlus;
        }
    }

    public class SetBetPlayerMsg : Message
    {
        public int playerCount;
        public SetBetPlayerMsg(int playerCount)
        {
            this.playerCount = playerCount;
        }
    }

    public class SetKioskCameraMsg : Message
    {
        public Camera kioskCamera;
        public SetKioskCameraMsg(Camera kioskCamera)
        {
            this.kioskCamera = kioskCamera;
        }
    }

    public class SetBetKioskRoundUpdateMsg : Message
    {
        public int round;
        public SetBetKioskRoundUpdateMsg(int round)
        {
            this.round = round;
        }
    }
}
