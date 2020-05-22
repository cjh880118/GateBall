using UnityEngine;
using JHchoi.Constants;

namespace JHchoi.Models
{
    public class GameModel : Model
    {
        //여기에는 테이블 같은 정적 데이터(모드별 Default 값)
        public ModelRef<SettingModel> setting = new ModelRef<SettingModel>();
        public ModelRef<PlayersModel> players = new ModelRef<PlayersModel>();
        public ModelRef<InGamePlayModel> inGame = new ModelRef<InGamePlayModel>();
        public ModelRef<MissionLevelSettingModel> missionLeve = new ModelRef<MissionLevelSettingModel>();
        public ModelRef<CameraModel> cameraModel = new ModelRef<CameraModel>();
        public ModelRef<TouchBallSettingModel> touchBallModel = new ModelRef<TouchBallSettingModel>();
        public ModelRef<BetModeModel> betModeModel = new ModelRef<BetModeModel>();



        #region [ GameData ]

        Transform mainCamera;
        public Transform MainCamera
        {
            get { return mainCamera; }
            set { mainCamera = value; }
        }

        int StartTime = 0;
        public int GetStartTime
        {
            get { return StartTime; }
        }

        #endregion
        public void Setup()
        {
            setting.Model = new SettingModel();
            setting.Model.Setup(this);

            inGame.Model = new InGamePlayModel();
            inGame.Model.Setup(this);

            players.Model = new PlayersModel();
            players.Model.Setup(this);

            missionLeve.Model = new MissionLevelSettingModel();
            missionLeve.Model.Setup(this);

            cameraModel.Model = new CameraModel();
            cameraModel.Model.Setup(this);

            touchBallModel.Model = new TouchBallSettingModel();
            touchBallModel.Model.Setup(this);

            betModeModel.Model = new BetModeModel();
            betModeModel.Model.Setup(this);
        }
    }
}

