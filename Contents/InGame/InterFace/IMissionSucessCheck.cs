using JHchoi.Models;
using JHchoi.UI.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents {
    public abstract class IMissionSucessCheck : IContent
    {
        protected SettingModel settingModel;
        protected PlayersModel playersModel;
        protected InGamePlayModel inGameplayModel;
        protected CameraModel cameraModel;
        protected int totalPlayerCount;

        protected override void OnLoadStart()
        {
            settingModel = Model.First<SettingModel>();
            playersModel = Model.First<PlayersModel>();
            inGameplayModel = Model.First<InGamePlayModel>();
            cameraModel = Model.First<CameraModel>();
            SetLoadComplete();
        }

        protected override void OnEnter()
        {
            totalPlayerCount = inGameplayModel.TotalPlayerCount;
            AddMessage();
        }

        private void AddMessage()
        {
            Message.AddListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
            Message.AddListener<MissionResultCloseMsg>(MissionResultClose);
        }

        private void SetMissionResultInfo(SetMissionResultInfoMsg msg)
        {
            MissionResult(msg.isSuccess);
        }

        protected abstract void MissionResult(bool isSuccess);

        private void MissionResultClose(MissionResultCloseMsg msg)
        {
            ResultClose(msg.isSuccess);
        }

        protected abstract void ResultClose(bool isSuccess);

        protected override void OnExit()
        {
            RemoveMessage();
        }
        void RemoveMessage()
        {
            Message.RemoveListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
            Message.RemoveListener<MissionResultCloseMsg>(MissionResultClose);
        }
    }
}