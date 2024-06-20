using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraSpecialFunction : MonoBehaviour
{
    // �ƾ��� ���� ������Ʈ
    Vector3 originPos;
    Transform target;

    float limit;
    float t;

    // ���� �������� Target���� �ٰ����µ�, limit������ �ٰ�����

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
