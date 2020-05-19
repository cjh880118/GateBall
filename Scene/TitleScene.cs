using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI.Event;

namespace CellBig.Scene
{
    public class TitleScene : IScene
    {
        static string TAG = "TitleScene :: ";
        protected override void OnLoadStart()
        {
            Debug.Log(TAG + "OnLoadStart");
        }


        protected override void OnLoadComplete()
        {
            
            Debug.Log(TAG + "OnLoadComplete");
        }
    }
}