using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class KioskGlobalDialog : IDialog
    {
        public GameObject TouchEffect;
        public GameObject parent;
        GameObject[] TouchEffects;
        MemoryPool memoryPoolTouch;

        Camera camera;
        Coroutine corTouchCheck;
        Coroutine corAnimationCheck;

        float offsetX;
        float offsetY;

        protected override void OnLoad()
        {
            base.OnLoad();
            TouchEffects = new GameObject[10];
            memoryPoolTouch = new MemoryPool();
            memoryPoolTouch.Create(TouchEffect, 10, parent);
            offsetX = Screen.width / 2;
            offsetY = Screen.height / 2;
        }

        protected override void OnEnter()
        {
            AddMessage();
            corTouchCheck = StartCoroutine(TouchCheck());
            corAnimationCheck = StartCoroutine(AniCheck());
        }

        private void AddMessage()
        {
            Message.AddListener<SetKioskCameraMsg>(SetKioskCamera);
        }

        private void SetKioskCamera(SetKioskCameraMsg msg)
        {
            camera = msg.kioskCamera;
        }

        protected override void OnExit()
        {
            RemoveMessage();

            if (corTouchCheck != null)
                StopCoroutine(corTouchCheck);

            if (corAnimationCheck != null)
                StopCoroutine(corAnimationCheck);
        }

        private void RemoveMessage()
        {
            Message.RemoveListener<SetKioskCameraMsg>(SetKioskCamera);
        }

        IEnumerator TouchCheck()
        {
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {

                    Vector3 pos = Input.mousePosition;
                    pos = new Vector3(pos.x - offsetX, pos.y - offsetY, pos.z);

                    for (int i = 0; i < 10; i++)
                    {
                        if (TouchEffects[i] == null)
                        {
                            TouchEffects[i] = memoryPoolTouch.NewItem();
                            TouchEffects[i].transform.localPosition = pos;
                            TouchEffects[i].transform.localScale = TouchEffect.transform.localScale;
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator AniCheck()
        {
            while (true)
            {
                yield return null;
                for (int i = 0; i < 10; i++)
                {
                    if (TouchEffects[i] != null && !TouchEffects[i].GetComponent<ParticleSystem>().isPlaying)
                    {
                        memoryPoolTouch.RemoveItem(TouchEffects[i]);
                        TouchEffects[i] = null;
                        break;
                    }
                }

            }
        }
    }
}

