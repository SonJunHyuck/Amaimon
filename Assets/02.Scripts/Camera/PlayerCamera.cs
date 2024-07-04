using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCamera : MonoBehaviour
{
    public enum CameraMode
    {
        IDLE, FOLLOW, NPC, INTRO, BOSS_END, ZOOM, HOLD
    }
    CameraMode camMode;

    public UIManager uiM;

    private Transform player;

    public Transform target;
    public Vector3 direction;

    public Camera mainCam;
    private Camera specialCam;

    private CameraMovement camMovement;
    private CameraEffect camEffect;

    [SerializeField]
    private Renderer obstacleRenderer;

    private void Awake()
    {
        camMode = CameraMode.FOLLOW;

        player = transform.root;

        mainCam = Camera.main;
        specialCam = transform.Find("Special Camera").GetComponent<Camera>();

        camMovement = GetComponent<CameraMovement>();
        camEffect = GetComponent<CameraEffect>();

        target = transform;
    }

    void Start()
    {
        direction = target.transform.position - mainCam.transform.position;
        camMovement.ZoomCamera(4);
    }

    private void LateUpdate()
    {
        if (camMode == CameraMode.HOLD)
        {
            return;
        }
        else if (camMode == CameraMode.FOLLOW || camMode == CameraMode.NPC)
        {
            camMovement.TrackingTarget();

            direction = target.transform.position - mainCam.transform.position;

            //.. 장애물에 플레이어가 가려지면 장애물 반투명
            DetectObstacle();

            if (Input.GetMouseButton(1))
            {
                camMovement.RotateCamera();
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                camMovement.ZoomCamera(-scroll);
            }
        }
    }

    private void DetectObstacle()
    {
        float distance = direction.magnitude;

        // 1. Ray발사 : Camera -> Target
        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, direction.normalized, out hit, distance, 1 << 7))
        {
            // 2.맞았으면 Renderer를 얻어온다. (1 << 7은 Obstacle Layer)
            obstacleRenderer = hit.transform.GetComponentInChildren<Renderer>();

            // 3. Metrial의 Aplha를 바꾼다.
            if (obstacleRenderer != null)
            {
                float opacity = 0.5f;
                camEffect.MakeToTransparent(obstacleRenderer, UtilityKit.BlendMode.Transparent, opacity);
            }
        }
        else
        {
            // 4. 카메라에 걸리는게 없는데, 장애물로 걸려있는 것이 있으면 해제
            if (obstacleRenderer != null)
            {
                float opacity = 1.0f;
                camEffect.MakeToTransparent(obstacleRenderer, UtilityKit.BlendMode.Opaque, opacity);
                obstacleRenderer = null;
            }
        }
    }

    public void SetCameraMode(CameraMode camMode, Transform inTarget)
    {
        this.camMode = camMode;
        target = inTarget;

        if (this.camMode == CameraMode.NPC)
        {
            StartCoroutine(camMovement.MoveCameraLerp(inTarget.GetChild(2).position));
        }
        else if (this.camMode == CameraMode.FOLLOW)
        {
            camMovement.ResetPosition();
        }
    }

    public void EffectGameOver()
    {
        StartCoroutine(camEffect.GrayScale());
    }

    public void EffectFadeInOut()
    {
        StartCoroutine(camEffect.FadeInOut(3.0f));
    }

    public void EffectZoomInOut()
    {
        //camMode = CameraMode.ZOOM;
        StartCoroutine(camMovement.ZoomInOut());
    }

    public void EffectShakingCamera(float delayTime, float shakingTime, float shakingAmount = 10)
    {
        StartCoroutine(camMovement.ShakeCamera(delayTime, shakingTime, shakingAmount));
    }

    public void BossCutScene(BossMapInfo bossMapInfo)
    {
        camMode = CameraMode.HOLD;  // 사용 못하게 막기

        StartCoroutine(StartBossCutScene(bossMapInfo));
    }

    IEnumerator StartBossCutScene(BossMapInfo bossMapInfo)
    {
        yield return StartCoroutine(camEffect.FadeOut());
        uiM.SetActiveUI(false);

        // CutScene 세팅
        CameraSpecialFunction cutSceneFunc = specialCam.GetComponent<CameraSpecialFunction>();
        cutSceneFunc.InitializeCutScene(bossMapInfo.bossEntity.transform, bossMapInfo.playerInitialPos.position + Vector3.up * 2.0f);
        specialCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);

        // CutScene 지속 시간 (Special Cam이 동작하는 시간)
        yield return new WaitForSeconds(9.0f);

        specialCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);

        yield return StartCoroutine(camEffect.FadeOut());

        // TODO : Broadcast
        // 전투 시작
        bossMapInfo.StartBattle(bossMapInfo.player);
        player.GetComponent<UserPlayerCtrl>().mPlayerState = UserPlayerCtrl.PlayerState.IDLE;
        uiM.SetActiveUI(true);
        uiM.ActiveBossState(bossMapInfo.bossEntity);

        camMode = CameraMode.FOLLOW;
    }

}
