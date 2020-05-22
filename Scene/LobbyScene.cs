using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JHchoi.Scene
{
    public class LobbyScene : IScene
    {
        static string TAG = "LobbyScene :: ";
        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
        }

        protected override void OnLoadComplete()
        {

        }
    }
}