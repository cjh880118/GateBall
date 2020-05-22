using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Contents
{
    public class SettingContent : IContent
    {
        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                UI.IDialog.RequestDialogEnter<UI.SettingDialog>();
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                UI.IDialog.RequestDialogExit<UI.SettingDialog>();
            }

        }
    }
}
