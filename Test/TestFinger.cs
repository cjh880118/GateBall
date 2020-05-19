using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFinger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if(this.gameObject.transform.parent.name == "thumb" && collision.collider.tag == "Item")
        {
            collision.collider.GetComponent<box>().isGrapThumb = true;
        }
        if (this.gameObject.transform.parent.name == "index" && collision.collider.tag == "Item")
        {
            collision.collider.GetComponent<box>().isGrapIndex = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (this.gameObject.transform.parent.name == "thumb" && collision.collider.tag == "Item")
        {
            collision.collider.GetComponent<box>().isGrapThumb = false;
        }
        if (this.gameObject.transform.parent.name == "index" && collision.collider.tag == "Item")
        {
            collision.collider.GetComponent<box>().isGrapIndex = false;
        }
    }
}
