using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box : MonoBehaviour
{
    public bool isGrapThumb;
    public bool isGrapIndex;
    GameObject hand;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isGrapThumb && isGrapIndex)
        {
            if(hand != null)
                this.gameObject.transform.position = hand.transform.position;
        }
        else
        {
            this.gameObject.transform.position = this.gameObject.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.parent != null)
        {
            if (other.transform.parent.name == "index")
            {
                this.gameObject.transform.position = other.transform.position;
            }
        }
    }
      

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.transform.parent != null)
        {
            if (collision.collider.transform.parent.name == "index" || collision.collider.transform.parent.name == "thumb")
            {
                hand = collision.collider.gameObject;
            }
        }
        if(collision.collider.name == "Plane")
        {

        }
    }
}
