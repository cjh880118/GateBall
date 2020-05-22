using JHchoi.Constants;

namespace JHchoi.Models
{
    public class InGamePlayModel : Model
    {
        GameModel _owner;

        public void Setup(GameModel owner)
        {
            _owner = owner;
        }

        ModeType Mode = ModeType.MissionMode;
        public ModeType GameMode
        {
            get { return Mode; }
            set { Mode = value; }
        }

        Level playLevel;
        public Level PlayLevel
        {
            set { playLevel = value; }
            get { return playLevel; }
        }

        int round = 1;
        public int Round
        {
            set { round = value; }
            get { return round; }
        }

        int betLevel;
        public int BetLevel { get => betLevel; set => betLevel = value; }

        int playerNum;
        public int PlayerNum
        {
            set { playerNum = value; }
            get { return playerNum; }
        }

        int totalplayRound;
        public int TotalPlayRound
        {
            set { totalplayRound = value; }
            get { return totalplayRound; }
        }

        int totalplayerCount;
        public int TotalPlayerCount
        {
            set { totalplayerCount = value; }
            get { return totalplayerCount; }
        }

        BetModeGame betModeMission = BetModeGame.Gate_1;
        public BetModeGame BetModeMission { get => betModeMission; set => betModeMission = value; }
    }
} 