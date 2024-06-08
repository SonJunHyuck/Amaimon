using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    Camera camPlayer;
    Camera camNPC;
    
    CameraMovement mainCamMovement;

    public Transform playerTrans;

    [Range(0.5f, 2.0f)] public float sensitivity;

    private void Awake()
    {
        instance = this;

        // 0번 자식이 main이라고 가정해야함
        mainCamMovement = transform.GetChild(0).GetComponent<CameraMovement>();

        sensitivity = 1.0f;
    }

    private void Start()
    {
        mainCamMovement.SetTarget(playerTrans);
    }

    private void LateUpdate()
    {
        // Tracking
        if (playerTrans != null)
        {
            mainCamMovement.TrackingTarget();
        }


    }

    // 외부 접근 함수
    public void RotateCamera()
    {
        Vector2 inputAxis;

        inputAxis.y = Input.GetAxis("Mouse Y");
        inputAxis.x = Input.GetAxis("Mouse X");

        mainCamMovement.RotateCamera(inputAxis);
    }

    public void ZoomCamera(float zoomValue)
    {
        mainCamMovement.ZoomCamera(zoomValue);
    }

    public void ShakeCamera(float delayTime, float duration)
    {
        StartCoroutine(mainCamMovement.ShakeCamera(delayTime, duration));
    }
}
