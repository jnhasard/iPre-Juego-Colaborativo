using System.Collections;
using UnityEngine;

public enum CameraState
{
    Normal,
    NormalWithDifferentTarget,
    Backwards,
    FixedX,
    FixedY,
    Zoomed,
    TargetZoom,
    TargetZoomInCutscene,
    NoFollowUp,
    NoFollowAhead,
};

public class CameraController : MonoBehaviour
{

    #region Attributes

    public CameraState currentState;

    public float smoothCamera;
    public float followAhead;
    public float startTime;
    public float followUp;
    public int cameraMovementsAsked;
    public int cameraMovementsDone;
    public TriggerCamera.CameraMovementData[] myCameraMovements;

    private LevelManager levelManager;
    private Vector3 currentStepPos;
    private Vector3 targetPosition;
    private GameObject target;
    private Camera thisCamera;
    private GameObject inputChat;
    private GameObject panelChat;
    private GameObject canvas; 

    public float stepsToTarget = 100;
    public float initialSize = 2.8f;
    public float globalFreezeTime = 70;

    private float cameraRate;
    private int zoomSteps;

    #endregion

    #region Start & Update

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        thisCamera = GetComponent<Camera>();
        canvas = GameObject.Find("Canvas");
        inputChat = GameObject.Find("PanelInput");
        panelChat = GameObject.Find("PanelChat");

        ToggleCanvas(true);
        ChangeState(CameraState.Normal);
    }

    void Update()
    {
        if (!target)
        {
            return;
        }

        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);

        switch (currentState)
        {
            case CameraState.NormalWithDifferentTarget:
                MovableFocusedTarget();
                break;

            case CameraState.Normal:
                MoveNormal();
                break;

            case CameraState.Backwards:
                MoveNormal();
                break;

            case CameraState.Zoomed:    // Por qué aquí no hay nada???
                break;

            case CameraState.FixedX:
                MoveFixedX();
                break;

            case CameraState.FixedY:
                MoveFixedY();
                break;

            case CameraState.TargetZoom:
                MoveToZoom();
                break;

            case CameraState.TargetZoomInCutscene:
                MoveToZoomInCutScene();
                break;

            case CameraState.NoFollowUp:
                MoveNoFollowUp();
                break;

            case CameraState.NoFollowAhead:
                MoveNoFollowAhead();
                break;
        }
    }

    #endregion

    #region Common

    private void MoveNormal()
    {
        if (target.transform.localScale.x > 0f)
        {
            targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y + followUp, targetPosition.z);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y + followUp, targetPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);

    }

    private void MovableFocusedTarget()
    {
        if (target.transform.localScale.x > 0f)
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
    }

    private void MoveFixedX()
    {
        transform.position = new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
    }

    private void MoveFixedY()
    {
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
    }

    private void MoveToZoom()
    {
        if (zoomSteps < stepsToTarget)
        {
            transform.position = new Vector3(transform.position.x + currentStepPos.x, transform.position.y + currentStepPos.y, transform.position.z);
            thisCamera.orthographicSize = thisCamera.orthographicSize + cameraRate;
            zoomSteps++;
        }
        else if (zoomSteps < stepsToTarget + globalFreezeTime)
        {
            zoomSteps++;
        }
        else if (zoomSteps < stepsToTarget + globalFreezeTime + stepsToTarget)
        {
            transform.position = new Vector3(transform.position.x - currentStepPos.x, transform.position.y - currentStepPos.y, transform.position.z);
            thisCamera.orthographicSize = thisCamera.orthographicSize - cameraRate;
            zoomSteps++;
        }
        else
        {
            levelManager.localPlayer.ResumeMoving();
            SetDefaultValues();
        }
    }
    private void MoveToZoomInCutScene()
    {
        if (zoomSteps < stepsToTarget)
        {
            transform.position = new Vector3(transform.position.x + currentStepPos.x, transform.position.y + currentStepPos.y, transform.position.z);
            thisCamera.orthographicSize = thisCamera.orthographicSize + cameraRate;
            zoomSteps++;
        }
        else if (zoomSteps < stepsToTarget + globalFreezeTime)
        {
            zoomSteps++;
        }
    }

    private void MoveNoFollowUp()
    {
        if (target.transform.localScale.x > 0f)
        {
            targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y, targetPosition.z);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y, targetPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
    }

    private void MoveNoFollowAhead()
    {
        if (target.transform.localScale.y > 0f)
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y + followUp, targetPosition.z);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y + followUp, targetPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothCamera * Time.deltaTime);
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
    }

    public void ChangeState(CameraState playerCameraState)
    {
        switch(playerCameraState)
        {
            case CameraState.Normal:
                SetDefaultValues();
                break;
            case CameraState.FixedX:
                SetFixedX();
                break;
            case CameraState.FixedY:
                SetFixedY();
                break;
            case CameraState.NoFollowAhead:
                SetNoFollowAhead();
                break;
            case CameraState.NoFollowUp:
                SetNofollowUp();
                break;
            case CameraState.Backwards:
                SetBackwardsCamera();
                break;

            default:
                return;
        }
    }

    public void ChangeState(CameraState state, TriggerCamera.CameraMovementData movement)
    {
        switch (state)
        {
            case CameraState.Normal:
                SetDefaultValues();
                break;
            case CameraState.NormalWithDifferentTarget:
                SetNewTarget(movement);
                break;
            case CameraState.Zoomed:
                SetZoomedValues(movement);
                break;
            case CameraState.TargetZoom:
                TargetedZoom(movement);
                break;
            case CameraState.TargetZoomInCutscene:
                SetDataTargetZoomInCutscene(movement);
                break;
            default:
                return;
        }
    }

    public void SetZoomedValues(TriggerCamera.CameraMovementData movement)
    {
        currentState = movement.state;
        thisCamera.orthographicSize = movement.ortographic_size;
        transform.position = new Vector3(movement.target.transform.position.x, movement.target.transform.position.y, transform.position.z);

        //ToggleChat(movement.hideChat);
        ToggleCanvas(movement.hideCanvas);
    }

    private void TargetedZoom(TriggerCamera.CameraMovementData movement)
    {
        if (movement.playerCantMove == true)
        {
            levelManager.localPlayer.StopMoving();
        }
        if (movement.hideCanvas)
        {
            ToggleCanvas(false);
        }

        globalFreezeTime = movement.freezeTime;
        Vector3 targetPosition = new Vector3(movement.target.transform.position.x, movement.target.transform.position.y, -10);
        currentState = CameraState.TargetZoom;

        currentStepPos = (targetPosition - transform.position) / movement.stepsToTarget;
        cameraRate = (movement.ortographic_size - initialSize) / movement.stepsToTarget;
        zoomSteps = 0;
    }

    #endregion

    #region Utils

    private void SetBackwardsCamera()
    {
        currentState = CameraState.Backwards;
        followUp = -1f;
    }

    private void SetNofollowUp()
    {
        currentState = CameraState.NoFollowUp;
    }

    private void SetNoFollowAhead()
    {
        currentState = CameraState.NoFollowAhead;
        followAhead = .5f;
    }

    private void SetFixedX()
    {
        currentState = CameraState.FixedX;
    }

    private void SetFixedY()
    {
        currentState = CameraState.FixedY;
    }

    public IEnumerator StartCutscene(TriggerCamera.CameraMovementData[] cutSceneMovements)
    {
        for (int i = 0; i< cutSceneMovements.Length; i++)
        {
            if (cutSceneMovements[i].playerCantMove == true)
            {
                levelManager.localPlayer.StopMoving();
            }
            if (cutSceneMovements[i].hideCanvas)
            {
                ToggleCanvas(false);
            }
            if (cutSceneMovements[i].target != target)
            {
                targetPosition = new Vector3(cutSceneMovements[i].target.transform.position.x, cutSceneMovements[i].target.transform.position.y, transform.position.z);
            }

            currentStepPos = (targetPosition - transform.position) / cutSceneMovements[i].stepsToTarget;
            cameraRate = (cutSceneMovements[i].ortographic_size - initialSize) / stepsToTarget;
            zoomSteps = 0;

            ChangeState(cutSceneMovements[i].state, cutSceneMovements[i]);

            if (cutSceneMovements[i].state == CameraState.TargetZoomInCutscene)
            {
                yield return new WaitForSeconds(WaitForCamera(cutSceneMovements[i].stepsToTarget, cutSceneMovements[i].freezeTime));

            }
            else if (cutSceneMovements[i].state == CameraState.TargetZoom)
            {
                yield return new WaitForSeconds(WaitForCamera(cutSceneMovements[i].stepsToTarget, cutSceneMovements[i].freezeTime));
                Debug.Log(WaitForCamera(cutSceneMovements[i].stepsToTarget * 2, cutSceneMovements[i].freezeTime) * Time.deltaTime);
            }
            else
            {
                yield return new WaitForSeconds(cutSceneMovements[i].timeWaiting);
                Debug.Log("ImWaiting");
            }

        }

        SetDefaultValues(); 

    }

    public void SetDataTargetZoomInCutscene(TriggerCamera.CameraMovementData cutSceneMovement)
    {
          if (cutSceneMovement.playerCantMove == true)
            {
                levelManager.localPlayer.StopMoving();
            }
            if (cutSceneMovement.hideCanvas)
            {
                ToggleCanvas(false);
            }
            globalFreezeTime = cutSceneMovement.freezeTime;
            target = cutSceneMovement.target;
            Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            currentState = cutSceneMovement.state;

            currentStepPos = (targetPosition - transform.position) / cutSceneMovement.stepsToTarget;
            cameraRate = (cutSceneMovement.ortographic_size - initialSize) / stepsToTarget;
            zoomSteps = 0;
            currentState = cutSceneMovement.state;
    }

    public void SetDefaultValues()
    {
        thisCamera.orthographicSize = initialSize;
        currentState = CameraState.Normal;

        if (levelManager.GetLocalPlayer() != null)
        {
            target = levelManager.GetLocalPlayer();
        }

        smoothCamera = 3.9f;
        followAhead = .9f;
        followUp = 1f;

        stepsToTarget = 100;
        initialSize = 2.8f;
        globalFreezeTime = 70;

        ToggleCanvas(true);
        //ToggleChat(false);
    }

    public void SetNewTarget(TriggerCamera.CameraMovementData cameraMovement)
    {
        thisCamera.orthographicSize = cameraMovement.ortographic_size;
        currentState = CameraState.NormalWithDifferentTarget;

        target = cameraMovement.target;
        smoothCamera = 3.9f;
        followAhead = 0;
        followUp = 0;

        stepsToTarget = 100;
        initialSize = 2.8f;

        ToggleCanvas(true);
        //ToggleChat(cameraMovement.hideCanvas);
    }



    /*private void ToggleChat(bool apagarChat)
    {
        if (apagarChat)
        {
            if (panelChat.activeInHierarchy)
            {
                panelChat.SetActive(false);
            }

            if (inputChat.activeInHierarchy)
            {
                inputChat.SetActive(false);
            }
        }

        else
        {
            if (!panelChat.activeInHierarchy)
            {
                panelChat.SetActive(true);
            }
            if (!inputChat.activeInHierarchy)
            {
                inputChat.SetActive(true);
            }
        }
    }*/

    private GameObject GetLocalPlayerMyself()
    {
        GameObject myHero;
        PlayerController[] pControllers = FindObjectsOfType<PlayerController>();
        for (int i = 0; i<pControllers.Length; i++)
        {
            if (pControllers[i].localPlayer)
            {
                myHero = pControllers[i].gameObject;
                return myHero;
            }
        }

        return levelManager.GetLocalPlayer();
    }

    private float WaitForCamera(float stepsToTarget, float freezeTime)
    {
        return (stepsToTarget + freezeTime) / 30;
    }

    private void ToggleCanvas(bool active)
    {
        canvas.SetActive(active);        
    }

    #endregion

}