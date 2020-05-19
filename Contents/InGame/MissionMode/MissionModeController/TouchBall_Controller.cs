using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchBall_Controller : MonoBehaviour
{
    public bool isTouch;
    Vector3 startPosition;
    public void Init(Vector3 position)
    {
        isTouch = false;
        this.gameObject.tag = "TouchBall";
        startPosition = position;
        this.gameObject.transform.position = startPosition;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ball" || collision.collider.tag == "Out")
        {
            isTouch = true;
            Debug.Log("ToucBall");
        }
    }

    public void ResetTouch()
    {
        isTouch = false;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.gameObject.transform.position = startPosition;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
