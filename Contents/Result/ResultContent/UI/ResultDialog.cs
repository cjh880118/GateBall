using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.UI
{
    public class ResultDialog : IDialog
    {
        // Start is called before the first frame update
        void Start()
        {
            AddListener();
        }

        void AddListener()
        {

        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        protected override void OnUnload()
        {
            RemoveListener();
        }

        void RemoveListener()
        {

        }
    }
}