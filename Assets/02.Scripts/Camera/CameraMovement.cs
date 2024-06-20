using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    enum MovementType
    {
        Idle,
        Tracking,
        Moving,
        Shaking,
        ZoomIn,
        ZoomOut,
        DirectedCut  // 보스 연출
    }
    MovementType movementType;

    private PlayerCamera playerCamera;
    private Camera cam;
    private Vector3 direction
    {
        get
        {
            return playerCamera.direction;
        }
    }
    private Transform target 
    { 
        get 
        {
            return playerCamera.target;
        } 
    }


    [Header("Tracking Params")]
    public Vector3 trackingOffset;
    [Range(0.1f, 1.0f)]
    public float trackingSmoothTime;

    [Header("Rotate Params")]
    public float clampAngleUpperMin;
    public float clampAngleUpperMax;
    public float clampAngleLowerMin;
    public float clampAngleLowerMax;
    public float rotateSensitivity;

    [Header("Zoom Params")]
    [Range(0.1f, 1.0f)]
    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;

    private void Awake()
    {
        movementType = MovementType.Tracking;

        clampAngleUpperMin = -1f;  // 359
        clampAngleUpperMax = 70f;
        clampAngleLowerMin = 335f;
        clampAngleLowerMax = 361f;  // 1

        playerCamera = GetComponent<PlayerCamera>();

        cam = Camera.main;
        cam.transform.localPosition = trackingOffset;
    }

    public void TrackingTarget()
    {
        if(target == null)
        {
            return;
        }

        cam.transform.LookAt(target);
    }

    public void RotateCamera()
    {
        Vector2 inputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = transform.rotation.eulerAngles;

        float rotateX = camAngle.x - inputAxis.y * rotateSensitivity;  // X축 회전 (위 아래)는 -로 갈 수록 위로 본다.
        float rotateY = camAngle.y + inputAxis.x * rotateSensitivity;

        if(rotateX < 180f)
        {
            rotateX = Mathf.Clamp(rotateX, clampAngleUpperMin, clampAngleUpperMax);
        }
        else
        {
            rotateX = Mathf.Clamp(rotateX, clampAngleLowerMin, clampAngleLowerMax);
        }
        
        transform.rotation = Quaternion.Euler(rotateX, rotateY, camAngle.z);
    }

    public void ZoomCamera(float zoomValue)
    {
        // cam to target
        float distance = direction.magnitude;

        // negative : ZoomIn
        distance += zoomValue;
        distance = Mathf.Clamp(distance, zoomMin, zoomMax);

        cam.transform.position = target.position - direction.normalized * distance;
    }

    public IEnumerator ZoomInOut()
    {
        while (direction.magnitude > zoomMin)
        {
            ZoomCamera(-Time.deltaTime * 2.0f);

            yield return null;
        }

        while (direction.magnitude < zoomMax)
        {
            ZoomCamera(Time.deltaTime * 2.0f);

            yield return null;
        }
    }

    public IEnumerator MoveCameraLerp(Vector3 targetPos, float duration = 1.0f)
    {
        //if (movementType == MovementType.Moving)
        //{
        //    yield break;
        //}

        movementType = MovementType.Moving;

        float t = 0;
        Vector3 originPos = cam.transform.position;
        while(t < 1.0f)
        {
            cam.transform.position = Vector3.Lerp(originPos, targetPos, t / duration);
            cam.transform.LookAt(targetPos);
            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = targetPos;
        movementType = MovementType.Idle;
    }

    public IEnumerator ShakeCamera(float delay, float duration, float amount)
    {
        //if (movementType != MovementType.Tracking)
        //{
        //    // todo : string builder
        //    Debug.Log("Camera is moving with another order : " + movementType.ToString());
        //    yield break;
        //}
        movementType = MovementType.Shaking;

        yield return new WaitForSeconds(delay);

        // .. 동작 시작
        Vector3 originPos = cam.transform.localPosition;
        while (duration > 0)
        {
            cam.transform.localPosition = originPos + UnityEngine.Random.insideUnitSphere * amount * Time.deltaTime;
            duration -= Time.deltaTime;

            yield return null;
        }

        // .. 원상 복귀
        cam.transform.localPosition = originPos;

        movementType = MovementType.Idle;
    }

    public void ResetPosition()
    {
        cam.transform.localPosition = trackingOffset;

        if (target != null)
        {
            cam.transform.LookAt(target);
        }
    }
}