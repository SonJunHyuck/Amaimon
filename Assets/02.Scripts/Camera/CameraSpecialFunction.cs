using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraSpecialFunction : MonoBehaviour
{
    // 컷씬을 위한 컴포넌트
    Vector3 originPos;
    Transform target;

    float limit;
    float t;

    // 시작 지점부터 Target까지 다가가는데, limit까지만 다가가기

    private void Update()
    {
        Vector3 direction = (target.position - originPos);
        transform.position = Vector3.Lerp(originPos, originPos + direction, t);
        transform.LookAt(target);
        
        t += Time.deltaTime;

        if (t > limit)
        {
            t = limit;
        }
    }

    public void InitializeCutScene(Transform inTarget, Vector3 camInitPos)
    {
        target = inTarget;
        originPos = camInitPos;
        transform.position = camInitPos;

        limit = 0.7f;
        t = 0;
    }
}
