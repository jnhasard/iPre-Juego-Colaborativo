using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    #region Attributes

    public Utils _;
    public PlayerController localPlayer;
    public GameObject[] players;
    public PlayerController[] playerControllers;
    public GameObject initialCutsceneController;
    public HUDDisplay hpAndMp;
    public GameObject canvas;
    public GameObject npcLog;
    public GameObject spiderLog;
    public GameObject decisionButtons;
    private Dictionary<string, GameObject> decisionFaces;

    private GameObject reconnectText;
    private Client client;

    private float waitToKillNPCCountdown;
    private float waitToKillSpiderCountdown;
    private float waitToGrabItem;
    private int?[] currentChoice;
    private Vector2[] playersLastPosition;
    public float waitToRespawn;
    public bool isPlayingFeedback;


    private Image playerFaceImage;
    private Text NPCFeedbackText;
    private Text SpiderFeedbackText;

    #endregion

    #region Start

    void Start()
    {
        SetCanvas();
        SetFeedbackUI();
        StorePlayers();
        StorePlayerControllers();
        playersLastPosition = new Vector2[3];
        isPlayingFeedback = false;

        _ = new Utils();

        hpAndMp = canvas.GetComponent<HUDDisplay>();
        if (GameObject.Find("PlayerFace"))
        {
            playerFaceImage = GameObject.Find("PlayerFace").GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Players Have no Face");
        }
        waitToKillSpiderCountdown = 5f;
        waitToKillNPCCountdown = 5f;
        waitToGrabItem = 2f;
        currentChoice = null;

        if (GameObject.Find("ClientObject"))
        {
            client = GameObject.Find("ClientObject").GetComponent<Client>();
            client.RequestPlayerIdToServer();
        }
    }

    #endregion

    #region Common

    public void MoveItemInGame(string itemName, string posX, string posY, string rotZ)
    {
        GameObject obj = GameObject.Find(itemName);
        Transform gameObjectToMove;
        if (!gameObject)
        {
            return;
        }
        gameObjectToMove = obj.GetComponent<Transform>();
        gameObjectToMove.position = new Vector3(float.Parse(posX), float.Parse(posY), gameObjectToMove.position.z);
        Quaternion gORotation = gameObjectToMove.rotation;
        gORotation = new Quaternion(gameObjectToMove.rotation.x, gameObjectToMove.rotation.y, float.Parse(rotZ), gameObjectToMove.rotation.w);
    }

    public void DestroyObjectInGame(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
        return;
    }

    public void ActivateSystem(string systemName)
    {

        GameObject activableSystem = GameObject.Find(systemName);

        if (activableSystem)
        {
            new ActivableSystemActions().DoSomething(activableSystem, false);
        }
        else
        {
            Debug.LogError("ActivableSystem " + systemName + " does not exists");
        }

    }


    // NPC FeedBack System
    public void ActivateNPCFeedback(string message)
    {
        if (localPlayer)
        {
            if (isPlayingFeedback)
            {
                return;
            }
            isPlayingFeedback = true;
            SetNPCText(message);
            ShutNPCFeedback(false);
        }
    }

    public void ShutNPCFeedback(bool now)
    {
        if (now)
        {
            KillNPC();
        }
        else
        {
            StartCoroutine(WaitToKillNPC());
        }
    }

    public void SetNPCText(string message)
    {
        if (!npcLog.activeInHierarchy)
        {
            npcLog.SetActive(true);
        }

        if (!NPCFeedbackText)
        {
            if (GameObject.Find("NPCLogText"))
            {
                NPCFeedbackText = GameObject.Find("NPCLogText").GetComponent<Text>();
            }
        }

        if (NPCFeedbackText)
        {
            NPCFeedbackText.text = message;
        }
    }

    public void ActivateSpiderFeedback(string message)
    {
        if (localPlayer)
        {
            SetSpiderText(message);
            ShutSpiderFeedBack(false);
        }
    }

    public void ShutSpiderFeedBack(bool now)
    {
        if (now)
        {
            KillSpider();
        }
        else
        {
            StartCoroutine(WaitToKillSpider());
        }
    }

    public void SetSpiderText(string message)
    {
        if (!spiderLog.activeInHierarchy)
        {
            spiderLog.SetActive(true);
        }

        if (!SpiderFeedbackText)
        {
            if (GameObject.Find("SpiderLogText"))
            {
                SpiderFeedbackText = GameObject.Find("SpiderLogText").GetComponent<Text>();
            }
        }

        if (SpiderFeedbackText)
        {
            SpiderFeedbackText.text = message;
        }
    }


    public void SetLocalPlayer(int id)
    {
        switch (id)
        {
            case 0:
                localPlayer = players[0].GetComponent<MageController>();
                Debug.Log("Activating Mage local player");
                client.SendMessageToServer("CoordinatePlayerId" + "/" + id, true);
                if (playerFaceImage != null)
                {
                    playerFaceImage.color = new Color32(255, 255, 255, 255);
                    playerFaceImage.sprite = decisionFaces["2Mage"].GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 1:
                Debug.Log("Activating Warrior local player");
                localPlayer = players[1].GetComponent<WarriorController>();
                client.SendMessageToServer("CoordinatePlayerId" + "/" + id, true);

                if (playerFaceImage != null)
                {
                    playerFaceImage.color = new Color32(255, 255, 255, 255);
                    playerFaceImage.sprite = decisionFaces["2Warrior"].GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 2:
                Debug.Log("Activating Engineer local player");
                localPlayer = players[2].GetComponent<EngineerController>();
                client.SendMessageToServer("CoordinatePlayerId" + "/" + id, true);

                if (playerFaceImage != null)
                {
                    playerFaceImage.color = new Color32(255, 255, 255, 255);
                    playerFaceImage.sprite = decisionFaces["2Engineer"].GetComponent<SpriteRenderer>().sprite;
                }
                break;
        }

        localPlayer.Activate(id);
        Camera.main.GetComponent<CameraController>().SetTarget(localPlayer.gameObject);
    }

    public void Respawn()
    {
        StartCoroutine(Respawning());
    }

    public void Respawn(PlayerController player)
    {
        StartCoroutine(Respawning(player));
    }

    public void GoToNextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string currentSceneNumber = Regex.Match(currentSceneName, @"\d+").Value;
        int nextSceneNumber = int.Parse(currentSceneNumber) + 1;
        string nextSceneName = "Escena" + nextSceneNumber;


        Debug.Log("Changing to scene " + nextSceneName);

        client.SendMessageToServer("ChangeScene/" + nextSceneName, true);
    }

    public void ShowReconnectingMessage(bool valor)
    {
        reconnectText.SetActive(valor);
    }

    public void CreateGameObject(string spriteName, int playerId)
    {
        GameObject newObject = (GameObject)Instantiate(Resources.Load("Prefabs/Items/" + spriteName));
        GameObject player = null;

        switch (playerId)
        {
            case 0:
                player = GameObject.Find("Verde");
                break;
            case 1:
                player = GameObject.Find("Rojo");
                break;
            case 2:
                player = GameObject.Find("Amarillo");
                break;
            default:
                break;
        }

        Physics2D.IgnoreCollision(newObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        newObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 1);

        StartCoroutine(WaitForCollision(newObject, player));
    }

    public void InsantiateGameObject(string[] msg)
    {
        Debug.LogError("Sistema Instanciación Planner no tiene Vector de instanciación");
        string objectName = "";
        for (int i = 1; i < msg.Length; i++)
        {
            objectName += msg[i];
            if (i != msg.Length - 1)
            {
                objectName += "/";
            }
        }
        Instantiate(Resources.Load(objectName));
    }

    public void ReloadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Utils

    protected void SetFeedbackUI()
    {
        npcLog = GameObject.Find("NPCLog");
        npcLog.SetActive(false);

        spiderLog = GameObject.Find("SpiderLog");
        spiderLog.SetActive(false);

        reconnectText = GameObject.Find("ReconnectingText");
        reconnectText.SetActive(false);

        InitializeLilFaces();

        decisionButtons = GameObject.Find("DecisionsUI");
        decisionButtons.SetActive(false);

    }

    protected void SetCanvas()
    {
        if (!canvas)
        {
            canvas = GameObject.Find("Canvas");
        }

        if (!canvas.activeInHierarchy)
        {
            canvas.SetActive(true);
        }
    }

    protected void StorePlayers()
    {
        players = new GameObject[3];

        players[0] = GameObject.Find("Verde");
        players[1] = GameObject.Find("Rojo");
        players[2] = GameObject.Find("Amarillo");
    }

    protected void StorePlayerControllers()
    {
        playerControllers = new PlayerController[3];

        playerControllers[0] = GameObject.Find("Verde").GetComponent<MageController>();
        playerControllers[1] = GameObject.Find("Rojo").GetComponent<WarriorController>();
        playerControllers[2] = GameObject.Find("Amarillo").GetComponent<EngineerController>();
    }

    public PlayerController[] GetPlayerControllers()
    {
        return playerControllers;
    }
    public GameObject GetLocalPlayer()
    {
        return localPlayer.gameObject;
    }

    public PlayerController GetLocalPlayerController()
    {
        return localPlayer;
    }


    public GameObject GetPlayer(int position)
    {
        if (position < players.Length)
        {
            return players[position];
        }

        return null;
    }

    public MageController GetMage()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[0];
        MageController magecontroller = player.GetComponent<MageController>();
        return magecontroller;
    }

    public WarriorController GetWarrior()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[1];
        WarriorController script = player.GetComponent<WarriorController>();
        return script;
    }

    public EngineerController GetEngineer()
    {
        if (players == null)
        {
            return null;
        }

        GameObject player = players[2];
        EngineerController script = player.GetComponent<EngineerController>();
        return script;
    }

    public void IgnoreCollisionBetweenObjects(string[] array)
    {
        bool ignores = bool.Parse(array[1]);

        GameObject objectA = GameObject.Find(array[2]);
        GameObject objectB = GameObject.Find(array[3]);

        if (!objectA || !objectB)
        {
            return;
        }

        Collider2D[] collidersA = objectA.GetComponents<Collider2D>();
        Collider2D[] collidersB = objectB.GetComponents<Collider2D>();

        foreach (Collider2D colliderA in collidersA)
        {
            if (!colliderA.isTrigger)
            {
                foreach (Collider2D colliderB in collidersB)
                {
                    if (!colliderB.isTrigger)
                    {
                        Physics2D.IgnoreCollision(colliderA, colliderB, ignores);
                    }
                }
            }
        }

    }

    public void DestroyObject(string name, float time)
    {
        GameObject gameObject = GameObject.Find(name);

        if (gameObject)
        {
            Destroy(gameObject, time);
        }

    }

    public void DeactivateObject(string name)
    {
        GameObject go = GameObject.Find(name);

        if (go)
        {
            go.SetActive(false);
        }
    }


    private void KillNPC()
    {

        if (NPCFeedbackText)
        {
            NPCFeedbackText.text = "";
        }
        isPlayingFeedback = false;
        npcLog.SetActive(false);
    }

    private void KillSpider()
    {

        if (SpiderFeedbackText)
        {
            SpiderFeedbackText.text = "";
        }

        spiderLog.SetActive(false);
    }

    public void StartVoting(string[] choiceTexts)
    {
        decisionButtons.SetActive(true);
        DeactivateLilDecisionFaces();
        SetVotingTextButtons(choiceTexts);
    }

    private void SetVotingTextButtons(string[] choiceTexts)
    {
        Text decisionText;

        if (GameObject.Find("DecisionAText"))
        {
            decisionText = GameObject.Find("DecisionAText").GetComponent<Text>();
            decisionText.text = choiceTexts[0];
        }

        if (GameObject.Find("DecisionBText"))
        {
            decisionText = GameObject.Find("DecisionBText").GetComponent<Text>();
            decisionText.text = choiceTexts[1];
        }

        if (GameObject.Find("DecisionCText"))
        {
            decisionText = GameObject.Find("DecisionCText").GetComponent<Text>();
            decisionText.text = choiceTexts[2];
        }
    }

    private void InitializeLilFaces()
    {
        decisionFaces = new Dictionary<string, GameObject>()
        {
            { "0Mage", GameObject.Find("DecisionAMageFace") },
            { "0Warrior", GameObject.Find("DecisionAWarriorFace") },
            { "0Engineer", GameObject.Find("DecisionAEnginFace") },

            { "1Mage", GameObject.Find("DecisionBMageFace") },
            { "1Warrior", GameObject.Find("DecisionBWarriorFace") },
            { "1Engineer", GameObject.Find("DecisionBEnginFace") },

            { "2Mage", GameObject.Find("DecisionCMageFace") },
            { "2Warrior", GameObject.Find("DecisionCWarriorFace") },
            { "2Engineer", GameObject.Find("DecisionCEnginFace") }
        };
        DeactivateLilDecisionFaces();
    }

    public void DeactivateLilDecisionFaces()
    {
        foreach (var decisionFace in decisionFaces)
        {
            decisionFace.Value.SetActive(false);
        }
    }

    public void PreVote(int choiceVoted)
    {
        if (localPlayer.decisionName != null)
        {
            if (currentChoice[localPlayer.playerId] != null)
            {
                decisionFaces[currentChoice[localPlayer.playerId] + localPlayer.name].SetActive(false);
                currentChoice[localPlayer.playerId] = null;
            }

            currentChoice[localPlayer.playerId] = choiceVoted;
            decisionFaces[choiceVoted + localPlayer.name].SetActive(true);
            SendPreVote();

        }
    }

    public void SendPreVote()
    {
        DecisionSystem actualDecision = GameObject.Find(localPlayer.decisionName).GetComponent<DecisionSystem>();
        actualDecision.SendPreVote(currentChoice[localPlayer.playerId].Value);
    }

    public void ReceivePreVote(int playerId, int preVote)
    {
        if (currentChoice != null)
        {
            decisionFaces[preVote + players[playerId].name].SetActive(false);
        }

        decisionFaces[preVote + players[playerId].name].SetActive(true);
    }

    public void SendVote()
    {
        if (localPlayer.decisionName != null)
        {
            Debug.Log("Sending my Vote");
            DecisionSystem actualDecision = GameObject.Find(localPlayer.decisionName).GetComponent<DecisionSystem>();
            actualDecision.Vote(actualDecision.choices[currentChoice[localPlayer.playerId].Value]);
            currentChoice = null;
        }
        else
        {
            ActivateNPCFeedback("Debes elegir una opción antes de votar...");
        }
    }

    public void RestartVoting()
    {
        currentChoice = null;
        DeactivateLilDecisionFaces();
        decisionButtons.SetActive(false);
        localPlayer.ResumeMoving();
    }

    #endregion

    public void StartAnimatorBool(string parameter, bool value, GameObject gameObject)
    {
        SceneAnimator sceneAnimator = FindObjectOfType<SceneAnimator>();
        sceneAnimator.SetBool(parameter, value, gameObject);
    }

    public void ShowFeedbackParticles(string name, Vector2 position, float liveTime)
    {
        GameObject feedbackParticles = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/" + name));
        feedbackParticles.GetComponent<Transform>().position = position;

        Destroy(feedbackParticles, liveTime);
    }

    public void TogglePlayerFilter(string filterName, bool active)
    {

        GameObject filterObject = GameObject.Find(filterName);

        if (filterObject)
        {
            PlayerFilter playerFilter = filterObject.GetComponent<PlayerFilter>();

            if (playerFilter)
            {
                playerFilter.SetActive(true);
            }
        }
    }

    public void SetMovingObjectData(GameObject movingObject, Vector2 startPos, Vector2 endPos, float moveSpeed, float timeToWait, bool ignoreCollisionWithPlayers)
    {
        MovingObject movingController = movingObject.GetComponent<MovingObject>();

        if (movingController)
        {
            movingController.SetData(startPos, endPos, moveSpeed, timeToWait, ignoreCollisionWithPlayers);
        }
    }

    public GameObject InstantiatePrefab(string name, Vector2 initialPos)
    {
        GameObject prefab = (GameObject)Instantiate(Resources.Load("Prefabs/" + name));

        if (prefab)
        {
            prefab.GetComponent<Transform>().position = initialPos;
        }

        return prefab;
    }

    public void HandleIncomingPoiReached(string poiId, string playerName)
    {
        Poi poiActivated = GameObject.Find("Poi" + poiId).GetComponent<Poi>();
        GameObject player = GameObject.Find(playerName);

        poiActivated.HandlePoiEnterFromServer(poiId, player);
    }

    public void HandlePoiReady(string poiId)
    {
        Poi poiActivated = GameObject.Find("Poi" + poiId).GetComponent<Poi>();

        poiActivated.HandlePoiReadyFromServer();

    }
    public void InstantiatePortal(string portalType, Vector2 initialPosition, Vector2 teleportPosition)
    {
        GameObject portal = (GameObject)Instantiate(Resources.Load("Prefabs/Portals/" + portalType));
        if (portal)
        {
            portal.GetComponent<Transform>().position = initialPosition;
            portal.GetComponent<PlayerTeleporter>().teleportPosition = teleportPosition;
        }
    }


    public void InstantiatePortal(string portalType, Vector2 initialPosition, Vector2 teleportPosition, bool mustDoSomething, int id)
    {
        GameObject portal = (GameObject)Instantiate(Resources.Load("Prefabs/Portals/" + portalType));
        if (portal)
        {
            portal.GetComponent<Transform>().position = initialPosition;
            portal.GetComponent<PlayerTeleporter>().teleportPosition = teleportPosition;
            portal.GetComponent<PlayerTeleporter>().mustDoSomething = mustDoSomething;
            portal.GetComponent<PlayerTeleporter>().id = id;
        }
    }

    public void InstantiatePortal(string portalType, Vector2 initialPosition, Vector2 teleportPosition, bool mustDoSomething, int id, string _groveStreet, string _whereToGo)
    {
        GameObject portal = (GameObject)Instantiate(Resources.Load("Prefabs/Portals/" + portalType));
        if (portal)
        {
            portal.GetComponent<Transform>().position = initialPosition;
            portal.GetComponent<PlayerTeleporter>().teleportPosition = teleportPosition;
            portal.GetComponent<PlayerTeleporter>().mustDoSomething = mustDoSomething;
            portal.GetComponent<PlayerTeleporter>().id = id;
            portal.GetComponent<PlayerTeleporter>().groveStreet = _groveStreet;
            portal.GetComponent<PlayerTeleporter>().placeToGo = _whereToGo;


        }
    }

    public void InstantiateBubbleWithTargets(string bubbleType, Vector2 initialPosition, Vector2[] targetPositions, float movementSpeed, float timeToWait, float timeToKillBubble)
    {
        GameObject bubble = (GameObject)Instantiate(Resources.Load("Prefabs/Bubbles/" + bubbleType));
        if (bubble)
        {
            if (bubbleType == "BurbujaN")
            {
                BubbleController bController = bubble.GetComponent<BubbleController>();
                bController.InitializeNeutralBubble(BubbleController.MoveType.Targets);
                bController.SetMovement(initialPosition, targetPositions, movementSpeed, timeToWait, timeToKillBubble);
            }
            else
            {
                GameObject bubbleParticle = InstantiatePrefab("BubbleParticles/" + bubbleType, initialPosition);
                PlayerController powerCaster = GetPowerableBubbleCaster(bubbleType);
                BubbleController bController = bubble.GetComponent<BubbleController>();
                bController.InitializeColouredBubbles(BubbleController.MoveType.Targets, powerCaster, bubbleParticle);
                bController.SetMovement(initialPosition, targetPositions, movementSpeed, timeToWait, timeToKillBubble);
            }
        }
    }

    public void PowerableToggleLavaIntoWater(string parameter, bool value, int damagingId)
    {
        LavaIntoWaterIdentifier[] lavas = FindObjectsOfType<LavaIntoWaterIdentifier>();
        SceneAnimator sceneAnimator = FindObjectOfType<SceneAnimator>();

        foreach (LavaIntoWaterIdentifier lava in lavas)
        {
            if (lava.id == damagingId)
            {
                sceneAnimator.SetBool(parameter, value, lava.gameObject);
            }
        }
    }

    private PlayerController GetPowerableBubbleCaster(string bubbleType)
    {
        if (bubbleType == "BurbujaV")
        {
            return GetMage();
        }

        if (bubbleType == "BurbujaR")
        {
            return GetWarrior();
        }

        else
        {
            return GetEngineer();
        }

    }

    public IEnumerator WaitForCamera(float stepsToTarget, float freezeTime)
    {
        yield return new WaitForSeconds(SecondsForCamera(stepsToTarget, freezeTime));
    }

    private float SecondsForCamera(float stepsToTarget, float freezeTime)
    {
        stepsToTarget *= 2;
        return ((stepsToTarget + freezeTime) / 30);
    }

    public float GetWaitToNPC()
    {
        return waitToKillNPCCountdown;
    }

    public GameObject InstatiateSprite(string name, Vector2 initialPos)
    {
        GameObject sprite = (GameObject)Instantiate(Resources.Load("Sprites/" + name));
        sprite.GetComponent<Transform>().position = initialPos;

        return sprite;
    }

    public GameObject FindGameObject(string name)
    {
        GameObject gameObject = GameObject.Find(name);
        return gameObject;
    }

    public void DamageAllPlayers(bool withExplotion)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        GameObject[] players = levelManager.players;

        foreach (GameObject player in players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.TakeDamage(15, new Vector2(300, 100));
            if (withExplotion)
            {
                InstantiatePrefab("PoisonParticles", player.transform.position);
            }
        }
    }

    public void TogglePowerableAnimatorsWithName(string animatorParameter, bool value, string name)
    {
        GameObject gameObject = GameObject.Find(name);
        SceneAnimator sceneAnimator = FindObjectOfType<SceneAnimator>();
        sceneAnimator.SetBool(animatorParameter, value, gameObject);
    }

    #region Coordinators

    public void CoordinateReconnectionElements()
    {
        CoordinatePlayers();
        CoordinateBubbleInstantiators();
        CoordinateRotators();
        CoordinatePlatformInstantiators();
        CoordinateMovingObjects();
        CoordinateCircuitMovingElements();
        CoordinateEnemyControllers();
        CoordinateTeleporters();
        CoordinateMovableTriggers();
    }

    private void CoordinateMovableTriggers()
    {
        MovableTriggerInstantiator[] mTriggers = FindObjectsOfType<MovableTriggerInstantiator>();
        foreach (MovableTriggerInstantiator mInstantiator in mTriggers)
        {
            if (mInstantiator.jobDone)
            {
                mInstantiator.InstantiateObjects(mInstantiator.objectNeeded);
            }
        }
    }

    private void CoordinateTeleporters()
    {
        PlayerTeleporter[] pTeleporters = FindObjectsOfType<PlayerTeleporter>();
        foreach (PlayerTeleporter teleporter in pTeleporters)
        {
            if (teleporter.DidYourThing())
            {
                teleporter.DoYourTeleportedThing(teleporter.id);
            }
        }

    }

    private void CoordinatePlayers()
    {
        PlayerController[] pControllers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController pController in pControllers)
        {
            pController.playerHasReturned = true; 
        }
    }
    private void CoordinateMovingObjects()
    {
        MovingObject[] mObjects = FindObjectsOfType<MovingObject>();
        foreach (MovingObject mObject in mObjects)
        {
            if (mObject.isWorking == true)
            {
                mObject.playerHasReturned = true;
            }
        }
    }

    private void CoordinatePlatformInstantiators()
    {
        MovingPlatformInstantiator[] mPlatforms = FindObjectsOfType<MovingPlatformInstantiator>();
        foreach (MovingPlatformInstantiator mPlatform in mPlatforms)
        {
            if (mPlatform.isWorking == true)
            {
                mPlatform.playerHasReturned = true;
            }
        }
    }

    private void CoordinateEnemyControllers()
    {
        EnemyController[] eControllers = FindObjectsOfType<EnemyController>();
        foreach (EnemyController eController in eControllers)
        {
            eController.ThePlayerReturned(true);
        }
    }
    private void CoordinateBubbleInstantiators()
    {
        BubbleRotatingInstantiator[] bInstantiators = FindObjectsOfType<BubbleRotatingInstantiator>();
        foreach (BubbleRotatingInstantiator bInstantiator in bInstantiators)
        {
            if (bInstantiator.isWorking == true)
            {
                bInstantiator.playerHasReturned = true;
            }
        }
    }
    private void CoordinateRotators()
    {
        Rotator[] rotators = FindObjectsOfType<Rotator>();

        foreach (Rotator rotator in rotators)
        {
            if (rotator.isWorking == true)
            {
                rotator.playerHasReturned = true;
            }
        }
    }

    private void CoordinateCircuitMovingElements()
    {
        ObjectInCircuitMovementController[] mObjects = FindObjectsOfType<ObjectInCircuitMovementController>();
        foreach(ObjectInCircuitMovementController mController in mObjects)
        {
            if (mController.move)
            {
                mController.playerHasReturned = true;
            }
        }
    }
    #endregion
    #region Coroutines

    public IEnumerator Respawning(PlayerController player)
    {
        player.HardReset();

        yield return new WaitForSeconds(waitToRespawn * .9f); // Respawn a bit sooner than local

        player.transform.position = player.respawnPosition;
        player.gameObject.SetActive(true);
        player.ResetTransform();
        player.ResumeMoving();
    }

    public IEnumerator Respawning()
    {
        localPlayer.HardReset();

        yield return new WaitForSeconds(waitToRespawn);

        localPlayer.transform.position = localPlayer.respawnPosition + Vector3.up * .1f;
        localPlayer.gameObject.SetActive(true);
        localPlayer.ResetTransform();
        localPlayer.SendPlayerDataToServer();
        localPlayer.ResumeMoving();
    }

    private IEnumerator WaitForCollision(GameObject gameObject, GameObject player)
    {
        yield return new WaitForSeconds(waitToGrabItem);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }

    private IEnumerator WaitToKillNPC()
    {
        yield return new WaitForSeconds(waitToKillNPCCountdown);
        KillNPC();
    }
    private IEnumerator WaitToKillSpider()
    {
        yield return new WaitForSeconds(waitToKillSpiderCountdown);
        KillSpider();
    }

    #endregion

}
