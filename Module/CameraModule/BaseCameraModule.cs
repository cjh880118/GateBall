using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.UI;

namespace CellBig.Module
{
	public class BaseCameraModule : IModule
    {
        public int _CameraCount = 1;
        Dictionary<string, UICamera> _UICamera = new Dictionary<string, UICamera>();

        protected override void OnLoadStart()
        {
            var fullpath = "BaseCamera/UICamera";
            StartCoroutine(ResourceLoader.Instance.Load<GameObject>(fullpath, LoadComplete));
        }

        void LoadComplete(Object o)
        {
            for (int i = 0; i < _CameraCount; i++)
            {
                //UICamera uiCamera = new 
                var obj = Instantiate(o) as GameObject;
                obj.SetActive(true);
                obj.transform.SetParent(this.transform);
                obj.transform.localPosition = new Vector3(20.0f * i, -20.0f, 0.0f);
                obj.name = string.Format("{0}_{1}", o.name, i + 1);

                var uiCamera = obj.GetComponent<UICamera>();
                uiCamera.Setup(i);

                CanvasObjs.Add(uiCamera._Canvas[0].gameObject);
            }

            UIManager.Instance.SetCanvasObj(CanvasObjs);

            SetResourceLoadComplete();
        }
    }
}
