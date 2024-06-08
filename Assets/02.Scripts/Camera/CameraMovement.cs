using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{
    enum MovementType
    {
        Idle,
        Tracking,
        Shaking,
        ZoomIn,
        ZoomOut,
        DirectedCut  // 보스 연출
    }
    MovementType movementType;

    private Transform target;

    [Header("Tracking Params")]
    [Range(0.1f, 1.0f)] public float trackingSmoothTime;
    [Range(1.0f, 4.0f)] public float minDistanceTarget;
    [Range(6.0f, 10.0f)] public float maxDistanceTarget;

    private Vector2 targetRotation;
    private float curDistanceTarget;

    private Vector3 curTrackingVelocity;

    [Header("Rotate Params")]
    public float rotateSensitivity;
    private float smoothTime;
    private Vector3 currentVelocity;
    private Vector3 currentRotation;


    [Header("Shake Params")]
    public float shakingScale;

    [Header("Zoom Params")]
    public float zoomSpeed;
    public float zoomInLength;
    public float zoomOutLength;

    private void Awake()
    {
        movementType = MovementType.Tracking;

        // 카메라와 플레이어 사이의 초기 거리
        curDistanceTarget = 5.0f;
    }

    public void SetTarget(Transform inTarget)
    {
        target = inTarget;
    }

    public void TrackingTarget()
    {
        if(target == null)
        {
            return;
        }

        Vector3 destPos = target.position - transform.forward * curDistanceTarget;

        // 플레이어와 카메라 사이에 물체가 있을 때,
        RaycastHit hit;
        if (Physics.Linecast(transform.position, transform.forward, out hit))
        {
            float zoomDistanceTarget = Mathf.Clamp(hit.distance, minDistanceTarget, curDistanceTarget);

            destPos = target.position - transform.forward * zoomDistanceTarget;
        }

        transform.position = Vector3.SmoothDamp(transform.position, destPos, ref curTrackingVelocity, 0.2f);  // 0.2초만에 따라가기
    }

    public void RotateCamera(Vector2 inputAxis)
    {
        // X축 회전 (위 아래)는 -로 갈 수록 위로 본다.
        targetRotation.y += inputAxis.x * rotateSensitivity;
        targetRotation.x -= inputAxis.y * rotateSensitivity;

        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref currentVelocity, smoothTime);
        transform.eulerAngles = targetRotation;
    }

    public void ZoomCamera(float zoomValue)
    {
        // positive : zoomin
        curDistanceTarget -= Time.deltaTime * zoomValue * zoomSpeed;
        curDistanceTarget = Mathf.Clamp(curDistanceTarget, minDistanceTarget, maxDistanceTarget);
    }

    public IEnumerator ShakeCamera(float delay, float duration)
    {
        if (movementType != MovementType.Tracking)
        {
            // todo : string builder
            Debug.Log("Camera is moving with another order : " + movementType.ToString());
            yield break;
        }
        movementType = MovementType.Shaking;

        // Time.timeScale = 1.0f;

        yield return new WaitForSeconds(delay);

        // .. 동작 시작
        Vector3 originPos = transform.localPosition;
        while (duration > 0)
        {
            transform.localPosition = originPos + UnityEngine.Random.insideUnitSphere * shakingScale * Time.deltaTime;
            duration -= Time.deltaTime;

            yield return null;
        }

        // .. 원상 복귀
        transform.localPosition = originPos;

        movementType = MovementType.Tracking;
    }

}