using UnityEngine;
using System.Collections;


namespace JHchoi.Contents
{
	public abstract class IContent : MonoBehaviour
	{
		protected string _name;

		public IContentUILoader _uiLoader;
		public bool dontDestroy = false;
		public bool stackable = true;

		public delegate void OnComplete(GameObject obj);
		OnComplete _onLoadComplete;

		protected bool _contentLoadComplete = true;
		protected bool _uiLoadComplete = true;

		public bool isActive { get; private set; }


		public void Load(OnComplete complete)
		{
			_name = GetType().Name;
			_onLoadComplete = complete;

			Message.AddListener<Event.EnterContentMsg>(_name, Enter);
			Message.AddListener<Event.ExitContentMsg>(_name, Exit);

			StartCoroutine(LoadingProcess());
		}

		void LoadContentsUI()
		{
			if (_uiLoader != null)
			{
				_uiLoader.Load(
					() =>
					{
						_uiLoadComplete = true;
						OnUILoadComplete();
					});
			}
			else
			{
				_uiLoadComplete = true;
			}
		}

		IEnumerator LoadingProcess()
		{
			_uiLoadComplete = false;
			_contentLoadComplete = false;

            OnLoadStart();
            LoadContentsUI();

			do
			{
				yield return null;
			}
			while (!_uiLoadComplete || !_contentLoadComplete);

			OnLoadComplete();

			if (_onLoadComplete != null)
				_onLoadComplete(gameObject);
		}

		protected void SetLoadComplete()
		{
			_contentLoadComplete = true;
		}

		/// <summary>
		/// 생성과 동시에 메시지 및 모델을 생성해야 할 경우 재정의 한 후 구현한다.
		/// 이 콜백을 재정의 하게 되면 적절한 타이밍에 SetLoadComplete() 를 호출해주어야 한다.
		/// </summary>
		protected virtual void OnLoadStart()
		{
			SetLoadComplete();
		}

		protected virtual void OnLoading(float progress)
		{
			/* BLANK */
		}

		protected virtual void OnLoadComplete()
		{
			/* BLANK */
		}

		protected virtual void OnUILoadComplete()
		{
			/* BLANK */
		}

		public void Unload()
		{
			Message.RemoveListener<Event.EnterContentMsg>(_name, Enter);
			Message.RemoveListener<Event.ExitContentMsg>(_name, Exit);

			OnExit();

			if (_uiLoader != null)
				_uiLoader.Unload();

			OnUnload();
		}

		/// <summary>
		/// OnLoad()에서 생성된 메세지나 모델을 이곳에서 해제 한다.
		/// </summary>
		protected virtual void OnUnload()
		{
			/* BLANK */
		}

		void Enter(Event.EnterContentMsg msg)
		{
			if (isActive)
			{
				Debug.LogWarningFormat("{0} are entered.", _name);
				return;
			}

			isActive = true;
			
			OnEnter();
		}

		void Exit(Event.ExitContentMsg msg)
		{
			OnExit();

			isActive = false;
		}

		protected abstract void OnEnter();
		protected abstract void OnExit();

        public static void RequestContentEnter<T>() where T : IContent
        {
            Message.Send<Event.EnterContentMsg>(typeof(T).Name, new Event.EnterContentMsg());
        }

        public static void RequestContentExit<T>() where T : IContent
        {
            Message.Send<Event.ExitContentMsg>(typeof(T).Name, new Event.ExitContentMsg());
        }

        public static string GetMsgName<T>()
		{
			return typeof(T).Name;
		}
	}
}
