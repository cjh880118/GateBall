using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Scene;
using CellBig.UI.Event;

namespace CellBig.Contents
{
    public class TitleContent : IContent
    {
        static string TAG = "TitleContent :: ";
        GameObject aniParient;
        GameObject projectionCamera;
        List<Coroutine> ListSoundLoop = new List<Coroutine>();
      
        protected override void OnLoadStart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            SetLoadComplete();
        }

        protected override void OnLoadComplete()
        {
            aniParient = GameObject.Find("camera_01");
            projectionCamera = GameObject.Find("ProjectionCamera");
            projectionCamera.transform.parent = aniParient.transform;
            projectionCamera.transform.localPosition = new Vector3(0, 0, 1.1f);
            projectionCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        }

        protected override void OnEnter()
        {
            Debug.Log(TAG + "OnEnter");
            SoundManager.Instance.PlaySound((int)SoundType.Mission_BGM);
            ListSoundLoop.Add(StartCoroutine(Sound()));
            //ListSoundLoop.Add(StartCoroutine(SoundBgm()));
           
            UI.IDialog.RequestDialogEnter<UI.TitleDialog>();
        }

        IEnumerator Sound()
        {
            while (true)
            {
                yield return new WaitForSeconds(15);
                int num = Random.Range(3, 7);
                SoundManager.Instance.PlaySound(num);
            }
        }

        IEnumerator SoundBgm()
        {
            while (true)
            {
                yield return new WaitForSeconds(12);
                SoundManager.Instance.PlaySound((int)SoundType.Title_BGM);
            }
        }

        protected override void OnExit()
        {
            Debug.Log(TAG + "OnExit");
            foreach(var o in ListSoundLoop)
                StopCoroutine(o);

            SoundManager.Instance.StopSound((int)SoundType.Mission_BGM);
            ListSoundLoop.Clear();
            UI.IDialog.RequestDialogEnter<UI.TitleDialog>();
        }
    }
}