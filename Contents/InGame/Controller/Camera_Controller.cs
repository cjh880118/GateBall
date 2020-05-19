
using System;
using System.Collections;
using CellBig.Constants;
using CellBig.UI.Event;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Camera_Controller : MonoBehaviour
{
    Vector3 oriPosition;
    Vector3 oriRotation;

    GameObject temp1;
    GameObject temp2;

    bool isCameraMove;
    bool isMoveOn;
    bool isRotateOn;
    float dist = 10;

    public Camera[] cameras;

    private void Start()
    {
        AddMessage();
    }

    public void Init(Vector3 postion, Vector3 rotation)
    {
        oriPosition = postion;
        oriRotation = rotation;
        isMoveOn = false;
        isRotateOn = false;
        isCameraMove = false;
    }

    void AddMessage()
    {
        Message.AddListener<CameraMoveMsg>(CameraMove);
        Message.AddListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
        Message.AddListener<BallTouchMsg>(BallTouch);
    }

    private void CameraMove(CameraMoveMsg msg)
    {
        isCameraMove = true;
        StartCoroutine(BallDistance(msg.Ball, msg.Camera));
    }

    private void SetMissionResultInfo(SetMissionResultInfoMsg msg)
    {
        isCameraMove = false;
        StopAllCoroutines();
    }

    private void BallTouch(BallTouchMsg msg)
    {
        isCameraMove = false;
        StopAllCoroutines();
    }

    IEnumerator BallDistance(GameObject ball, GameObject camera)
    {
        temp1 = ball;
        temp2 = camera;
        while (isCameraMove)
        {
            yield return null;
            float distance = Vector3.Distance(ball.transform.position, camera.transform.position);

            Vector3 targetDir = ball.transform.position - transform.position;
            targetDir.y = 0.0f;
            //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * 0.05f);
            Vector3 tempPosition = ball.transform.position + ((this.gameObject.transform.forward * -1f) * dist);

            if (distance > dist && isCameraMove)
            {
                Vector3 pos = new Vector3(tempPosition.x, this.gameObject.transform.position.y, tempPosition.z);
                this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, pos, Time.deltaTime * 0.5f);
                //this.gameObject.transform.position = new Vector3(tempPosition.x, this.gameObject.transform.position.y, tempPosition.z);
            }
        }
    }

    public void SetPostProcessProfile(PostProcessProfile postProcessProfile)
    {
        foreach(var o in cameras)
        {
            o.GetComponent<PostProcessVolume>().profile = postProcessProfile;
        }

    }

    public void SetPosition(Vector3 position, Vector3 target, Vector3 vecLevel)
    {
        this.gameObject.transform.position = new Vector3(position.x, 2.3f, position.z);
        this.gameObject.transform.LookAt(target);
        this.gameObject.transform.eulerAngles = new Vector3(0, this.gameObject.transform.eulerAngles.y, this.gameObject.transform.eulerAngles.z + vecLevel.z);

        float distance = Vector3.Distance(this.gameObject.transform.position, target);
        while (distance < 5)
        {
            distance = Vector3.Distance(this.gameObject.transform.position, target);
            this.gameObject.transform.position = this.gameObject.transform.position + (this.gameObject.transform.forward * -0.1f);
        }


        //Todo.. 카메라 회전 값 ?
    }

    public void SetPositionReset()
    {
        this.gameObject.transform.position = oriPosition;
        this.gameObject.transform.eulerAngles = oriRotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            isMoveOn = true;
            isRotateOn = false;
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            isMoveOn = false;
            isRotateOn = true;
        }
        if (isMoveOn)
        {
            float x = Input.GetAxis("Vertical") * 5;
            x *= Time.deltaTime;
            transform.Translate(0, x, 0);
        }
        if (isRotateOn)
        {
            float y = Input.GetAxis("Horizontal") * 5;
            y *= Time.deltaTime;
            transform.Rotate(0, y, 0);
        }
    }

    private void OnGUI()
    {
        if (isMoveOn || isRotateOn)
        {
            GUI.skin.label.fontSize = 30;
            GUI.Label(new Rect(10, 10, 350, 50), "Position : " + this.gameObject.transform.position.ToString());
            GUI.Label(new Rect(10, 60, 350, 50), "Rotation : " + this.gameObject.transform.eulerAngles.ToString());
        }
    }

    private void OnDestroy()
    {
        RemoveMessage();
    }

    void RemoveMessage()
    {
        Message.RemoveListener<CameraMoveMsg>(CameraMove);
        Message.RemoveListener<SetMissionResultInfoMsg>(SetMissionResultInfo);
        Message.RemoveListener<BallTouchMsg>(BallTouch);
    }
}
