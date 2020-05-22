using UnityEngine;
using System.Collections;
using JHchoi.Models;
using JHchoi.UI.Event;

namespace JHchoi.Contents
{
	public class GlobalContent : IContent
	{
        Camera Camera;
        protected override void OnLoadStart()
        {
            //Camera = GameObject.Find("BackEye").GetComponent<Camera>();
            SetLoadComplete();
        }

        protected override void OnEnter()
		{
            AddMessage();
            UI.IDialog.RequestDialogEnter<UI.KioskGlobalDialog>();
            //Message.Send<SetKioskCameraMsg>(new SetKioskCameraMsg(Camera));
        }

		protected override void OnExit()
		{
			RemoveMessage();
        }

		void AddMessage()
		{
        }

		void RemoveMessage()
		{
        }
    }
}
