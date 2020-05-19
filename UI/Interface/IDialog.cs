using UnityEngine;
using CellBig.Constants;
using JHC;

namespace CellBig.UI
{
	public class IDialog : MonoBehaviour
	{
		protected RectTransform _rt;
		protected string _name;
		public GameObject dialogView;

        public bool isProjection;
        public bool isKiosk;

        int canvasIndex;

		void Awake()
		{
			if (dialogView == null)
				throw new System.NullReferenceException(string.Format("{0} dialogView Null", this.name));
		}

		public void Load()
		{
            //Debug.Log(gameObject.name);

            _name = GetType().Name;
			_rt = GetComponent<RectTransform>();

			Message.AddListener<Event.ShowDialogMsg>(_name, Enter);
			Message.AddListener<Event.HideDialogMsg>(_name, Exit);

			int sibling = EnumExtensions.ParseToInt<Constants.UISibling>(_name);
            UIManager.Instance.SetSibling(_rt, sibling);

			dialogView.SetActive(false);
			OnLoad();
		}

        protected virtual void SetCanvas()
        {
            Debug.Log("SetCanvas");

            if (isKiosk)
                canvasIndex = SceenMrg.I.kioskCanvas.targetDisplay;

            transform.SetParent(SceenMrg.I.CanvasArray[canvasIndex]._addCanvas.transform);
            SceenMrg.I.CanvasArray[canvasIndex].dialog = transform;
            transform.localPosition = new Vector3(0f, -420f, 0f);
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;

            SetSibling();
        }

        void SetSibling()
        {
            IDialog[] dialogs = transform.parent.GetComponentsInChildren<IDialog>();
            int[] sib = new int[dialogs.Length];

            for (int index = 0; index < dialogs.Length; index++)
            {
                sib[index] = EnumExtensions.ParseToInt<UISibling>(dialogs[index].gameObject.name);
            }

            for(int i = 0; i < sib.Length - 1; i++)
            {
                for(int j = i + 1; j < sib.Length; j++)
                {
                    if(sib[i] < sib[j])
                    {
                        int temp = sib[i];
                        sib[i] = sib[j];
                        sib[j] = temp;
                    }
                }
            }

            for (int i = 0; i < sib.Length; i++)
            {
                for(int j = 0; j < dialogs.Length; j++)
                {
                    if (EnumExtensions.ParseToInt<UISibling>(dialogs[j].gameObject.name) == sib[i])
                    {
                        //Debug.Log(EnumExtensions.ParseToInt<UISibling>(dialogs[j].gameObject.name) + " : " + sib[i]);
                        dialogs[j].transform.SetSiblingIndex(0);
                    }
                }
            }
        }

		protected virtual void OnLoad()
		{
            if (isProjection)
                SetCanvas();
        }

		public void Unload()
		{
			Message.RemoveListener<Event.ShowDialogMsg>(_name, Enter);
			Message.RemoveListener<Event.HideDialogMsg>(_name, Exit);

			OnExit();
			OnUnload();
		}

		protected virtual void OnUnload()
		{
		}

		private void Enter(Event.ShowDialogMsg msg)
		{
			if (dialogView != null)
			{
				if (dialogView.activeSelf)
					return;
				dialogView.SetActive(true);
			}
			
			OnEnter();
		}

		private void Exit(Event.HideDialogMsg msg)
		{
			if (dialogView != null)
				dialogView.SetActive(false);

			OnExit();
		}

        protected virtual void OnDestroy()
        {
            Unload();
        }

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnExit()
		{
		}

		public static void RequestDialogEnter<T>() where T : IDialog
		{
			Message.Send<UI.Event.ShowDialogMsg>(typeof(T).Name, new UI.Event.ShowDialogMsg());
		}

		public static void RequestDialogExit<T>() where T : IDialog
		{
			Message.Send<UI.Event.HideDialogMsg>(typeof(T).Name, new UI.Event.HideDialogMsg());
		}

		/// <summary>
		/// 왠만하면 이 메소드 보다 제네릭 메소드를 사용하세요.
		/// </summary>
		public static void RequestDialogEnter(System.Type t)
		{
			Message.Send<UI.Event.ShowDialogMsg>(t.Name, new UI.Event.ShowDialogMsg());
		}

		/// <summary>
		/// 왠만하면 이 메소드 보다 제네릭 메소드를 사용하세요.
		/// </summary>
		public static void RequestDialogExit(System.Type t)
		{
			Message.Send<UI.Event.HideDialogMsg>(t.Name, new UI.Event.HideDialogMsg());
		}
	}
}