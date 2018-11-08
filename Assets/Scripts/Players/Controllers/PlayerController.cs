using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    #region Attributes

    public PlannerPlayer playerObj;
    public Vector3 respawnPosition;
    public LayerMask whatIsGround;
    public LayerMask whatIsAlsoGround;
    public GameObject[] particles;
    public Transform groundCheck;
    public GameObject parent;

    // Remote data
    public bool remoteAttacking;
    public bool remoteJumping;
    public bool remoteRight;
    public bool remoteLeft;
    public bool remoteUp;

    // Local data
    public bool rightPressed;
    public bool leftPressed;
    public bool jumpPressed;
    public bool localPlayer;
    public bool isGrounded;
    public bool upPressed;

    public GameObject availablePowerable;
    public GameObject availableChatZone;
    public GameObject availableInstantiatorTrigger;
    public GameObject availableParticleTrigger;
    public GameObject availableEndOfScene;
    public GameObject mainCamera;
    public GameObject availableColliderZone;
    public string decisionName;
    public bool controlOverEnemies;
    public float groundCheckRadius;
    public bool canAccelerate;
    public float acceleration;
    public float actualSpeed;
    public int mpUpdateFrame;
    public int actualFramesForPowerCheck;
    public int sortingOrder;
    public int playerId;
    public bool mpDepleted;
    public bool isPowerOn;
    public int directionY;
    // 1 = de pie, -1 = de cabeza
    public int directionX;
    // 1 = derecha, -1 = izquierda
    public float gravityPower;
    public bool playerHasReturned;

    protected SceneAnimator sceneAnimator;
    protected LevelManager levelManager;
    protected SpriteRenderer sprite;
    protected Vector3 lastPosition;
    protected Rigidbody2D rb2d;

    // Statics
    protected static float maxAcceleration = 1f;
    protected static float takeDamageRate = 1f;
    protected static float attackRate = .25f;
    protected static float jumpRate = .15f;
    protected static float poweredRare = .25f;
    protected static float maxXSpeed = 3f;
    protected static float maxYSpeed = 8f;

    protected static string attackPrefabName = "Prefabs/Attacks/";

    protected static int mpUpdateFrameRate = 40;
    protected static int mpSpendRate = -1;
    protected static int attackSpeed = 4;
    protected static int framesForPowerableCheck = 150;

    protected string attackAnimName;
    protected bool isTakingDamage;
    protected bool isAttacking;
    protected bool justPowered;
    protected bool connected;
    protected bool canMove;
    protected bool justJumped;
    protected bool mustIgnoreCollisionWithVerde;
    protected bool mustIgnoreCollisionWithRojo;
    protected bool mustIgnoreCollisionWithAmarillo;
    protected float speedX;
    protected float speedY;
    protected CameraState cameraState;


    protected int debuger;

    #endregion

    #region Start

    protected virtual void Start()
    {

        sceneAnimator = FindObjectOfType<SceneAnimator>();

        if (!sceneAnimator)
        {
            Debug.LogError(name + " did not found the SceneAnimator");
        }

        levelManager = FindObjectOfType<LevelManager>();
        mainCamera = FindObjectOfType<CameraController>().gameObject;
        rb2d = GetComponent<Rigidbody2D>();

        respawnPosition = transform.position;

        attackAnimName = "Attacking";

        controlOverEnemies = false;
        canAccelerate = false;
        isAttacking = false;
        localPlayer = false;
        isGrounded = false;
        mpDepleted = false;
        isPowerOn = false;
        connected = true;
        canMove = true;
        justJumped = false;
        availableChatZone = null;
        decisionName = null;
        availablePowerable = null;
        gravityPower = 2.3f;

        remoteAttacking = false;
        remoteJumping = false;
        remoteRight = false;
        remoteLeft = false;
        remoteUp = false;

        actualFramesForPowerCheck = 0;
        mpUpdateFrame = 0;
        acceleration = 0f;
        sortingOrder = 0;
        directionY = 1;
        directionX = 1;
        debuger = 0;


        SetPositiveGravity(true);
        InitializeParticles();
        IgnoreCollisionWithObjectsWhoHateMe();
        IgnoreCollisionBetweenPlayers();

    }

    #endregion

    #region Update

    protected virtual void FixedUpdate()
    {
        if (!connected || !canMove)
        {
            return;
        }

        if (transform.parent != null)
        {
            parent = transform.parent.gameObject;
        }

        if (playerHasReturned)
        {
            SendPlayerDataToServer();
            SendPowerDataToServer();
            playerHasReturned = false;
        }
        CheckCollisionWithPlayers();
        Move();
        Attack();
        UsePower();
        CheckPowerableState();
        //UseItem();
    }

    #endregion

    #region Common

    #region Connection

    public void Connect(bool _connected)
    {
        connected = _connected;

        remoteJumping = false;
        remoteRight = false;
        remoteLeft = false;

        SendPlayerDataToServer();
    }

    public void Activate(int _playerId)
    {
        localPlayer = true;
        playerId = _playerId;
        sprite = GetComponent<SpriteRenderer>();

        if (sprite)
        {
            sprite.sortingOrder = sortingOrder + 1;
        }

        if (Chat.instance)
        {
            Chat.instance.EnterFunction(name + ": Ha Aparecido!");
        }

    }

    #endregion

    #region Loop

    #region Attack

    protected void Attack()
    {

        if (!localPlayer || isAttacking)
        {
            return;
        }

        bool attackButtonPressed = CnInputManager.GetButtonDown("Attack Button");

        if (attackButtonPressed)
        {
            CastAttack();
        }

    }

    protected virtual void CastAttack()
    {
        CastLocalAttack(transform.position);
        SendAttackDataToServer();
    }

    public virtual void CastLocalAttack(Vector2 startPosition, Vector2 targetPosition)
    {
        AttackController attack = GetAttack();
        attack.Initialize(this, AttackController.MoveType.Target);
        attack.SetMovement(startPosition, targetPosition, attackSpeed);
    }

    public virtual void CastLocalAttack(Vector2 startPosition)
    {
        isAttacking = true;
        int yAxis = directionY;

        AttackController attack = GetAttack();
        attack.Initialize(this, AttackController.MoveType.Direction);
        attack.SetMovement(startPosition, directionX, yAxis, attackSpeed);

        StartCoroutine(WaitAttacking());
        AnimateAttack();
    }

    #endregion

    #region Move
    
    protected void CheckCollisionWithPlayers()
    {
        if (mustIgnoreCollisionWithVerde)
        {
            GameObject verde = GameObject.Find("Verde");
            if (verde)
            {
                IgnoreCollisionWithOnePlayerOnly(verde);
                mustIgnoreCollisionWithVerde = false;
            }
        }

        if (mustIgnoreCollisionWithRojo)
        {
            GameObject rojo = GameObject.Find("Rojo");
            if (rojo)
            {
                IgnoreCollisionWithOnePlayerOnly(rojo);
                mustIgnoreCollisionWithRojo = false;
            }
        }

        if (mustIgnoreCollisionWithAmarillo)
        {
            GameObject amarillo = GameObject.Find("Amarillo");
            if (amarillo)
            {
                IgnoreCollisionWithOnePlayerOnly(amarillo);
                mustIgnoreCollisionWithRojo = false;
            }
        }
    }

    protected void Move()
    {
        isGrounded = IsItGrounded();

        if (IsJumping(isGrounded))
        {
            justJumped = true;
            if(localPlayer)
            {
                SoundManager sManager = FindObjectOfType<SoundManager>();
                sManager.PlaySound(gameObject, GameSounds.PlayerJump, false);
            }
            speedY = maxYSpeed * directionY;
            StartCoroutine(WaitJumping());
        }
        else
        {
            speedY = rb2d.velocity.y;
        }

        if (IsGoingRight())
        {
            // Si estaba yendo a la izquierda resetea la aceleración
            if (directionX == -1)
            {
                ResetDirectionX(1);
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
            {
                Accelerate();
            }

            actualSpeed = maxXSpeed * acceleration;
            speedX = actualSpeed;
        }
        else if (IsGoingLeft())
        {

            // Si estaba yendo a la derecha resetea la aceleración
            if (directionX == 1)
            {
                ResetDirectionX(-1);
            }

            // sino acelera
            else if (acceleration < maxAcceleration)
            {
                Accelerate();
            }

            actualSpeed = maxXSpeed * acceleration;
            speedX = -actualSpeed;
        }
        else
        {
            speedX = 0f;
            acceleration = 0;
        }

        if (lastPosition != transform.position)
        {
            if (sceneAnimator)
            {
                sceneAnimator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x), this.gameObject);
                sceneAnimator.SetBool("IsGrounded", isGrounded, this.gameObject);
            }
        }

        rb2d.velocity = new Vector2(speedX, speedY);
        lastPosition = transform.position;

    }

    #endregion

    #region Power
    protected void CheckPowerableState()
    {
        actualFramesForPowerCheck++;
        {
            if (actualFramesForPowerCheck == framesForPowerableCheck)
            {
                actualFramesForPowerCheck = 0;
                if (!isPowerOn && justPowered)
                {
                    justPowered = false;
                }
            }
        }
    }
    public void UsePower()
    {
        if (!localPlayer || justPowered)
        {
            return;
        }

        if (localPlayer)
        {

            if (!levelManager.hpAndMp)
            {
                Debug.LogError("Levelmanager HpAndMp is not set");
                return;
            }

            bool powerButtonPressed = CnInputManager.GetButtonDown("Power Button");
            float mpCurrentPercentage = levelManager.hpAndMp.mpCurrentPercentage;

            // Se acabó el maná
            if (mpCurrentPercentage <= 0f)
            {
                // Si no he avisado que se acabó el maná, aviso
                if (!mpDepleted)
                {
                    mpUpdateFrame = 0;
                    mpDepleted = true;

                    SetPowerState(false);
                    SendPowerDataToServer();
                }
            }

            // Hay maná
            else
            {
                // Vuelvo a setear la variable que indica que tengo maná
                if (mpDepleted)
                {
                    mpDepleted = false;
                }

                // Toggle power button
                if (powerButtonPressed)
                {
                    SetPowerState(!isPowerOn);
                    SendPowerDataToServer();
                }

                if (isPowerOn)
                {

                    if (mpUpdateFrame == mpUpdateFrameRate)
                    {
                        levelManager.hpAndMp.ChangeMP(mpSpendRate); // Change local
                        SendMPDataToServer(); // Change remote
                        mpUpdateFrame = 0;
                    }

                    mpUpdateFrame++;

                }

            }
        }

    }

    #endregion

    #region Item

  /*  public void UseItem()
    {
        if (localPlayer)
        {
            bool itemButtonPressed = CnInputManager.GetButtonDown("Bag Button");

            if (itemButtonPressed)
            {
                Inventory.instance.SelectItem(this);
            }
        }
    }*/

    #endregion

    #endregion

    #region Callable

    public void HardReset()
    {
        StopMoving();
        if (isPowerOn)
        {
            SetPowerState(false);
            SendPowerDataToServer();
        }
        ResetTransform();
        ResetDamagingObjects();
        ResetChatZones();
        //ResetCamera();                  //For Test // ShouldTry Again
        ResetDamagingTriggers();
        ResetParticleZones();
        ResetDecisions();
        ResetPowerables();
        justPowered = false;
        justJumped = false;
        availablePowerable = null;
        gameObject.SetActive(false);
    }

    protected void ResetCamera()
    {
        GameObject mCamera = GameObject.Find("MainCamera");
        if (mCamera)
        {
            mCamera.GetComponent<CameraController>().SetDefaultValues();
            TriggerCamera[] cTriggers = FindObjectsOfType<TriggerCamera>();
            foreach (TriggerCamera tCamera in cTriggers)
            {
                tCamera.CheckIfPlayerLeft(gameObject);
            }

        }
    }

    public void ResetPowerables()
    {
        PowerableObject[] powerableObjects = FindObjectsOfType<PowerableObject>();
        foreach (PowerableObject powerable in powerableObjects)
        {
            if (availablePowerable == powerable.gameObject)
            {
                availablePowerable = null;
                powerable.ErasePlayerInPowerZone(gameObject);
            }
        }
    }

    public void ResetChatZones()
    {
        if (availableChatZone != null)
        {
            ChatZone chatZoneOff = availableChatZone.GetComponent<ChatZone>();
            chatZoneOff.TurnChatZoneOff();
            availableChatZone = null;
        }
    }

    public void ResetParticleZones()
    {
        if (availableParticleTrigger != null)
        {
            ParticleSetActiveTrigger availableTrigger = availableInstantiatorTrigger.GetComponent<ParticleSetActiveTrigger>();
            availableTrigger.ExitTrigger(gameObject);
            availableTrigger = null;
        }
    }

    public void ResetDamagingTriggers()
    {
        if (availableInstantiatorTrigger != null)
        {
            DamagingInstantiatorTrigger availableTrigger = availableInstantiatorTrigger.GetComponent<DamagingInstantiatorTrigger>();
            availableTrigger.ExitTrigger();
            availableTrigger = null;
        }
    }

    public void ResetDecisions()
    {
        if (decisionName != null)
        {
            DecisionSystem decisionOff = GameObject.Find(decisionName).GetComponent<DecisionSystem>();
            decisionOff.ResetDecision();
            decisionOff = null;
        }
    }
    
    public void ResetTransform()
    {
        transform.parent = null;
        SetPositiveGravity(true);
        IgnoreCollisionBetweenPlayers();
    }

    public void TakeDamage(int damage, Vector2 force)
    {
        if (isTakingDamage)
        {
            return;
        }

        isTakingDamage = true;

        if (force.x != 0 || force.y != 0)
        {
            rb2d.AddForce(force); // Take force local
            SendMessageToServer("PlayerTookDamage/" + playerId + "/" + force.x + "/" + force.y); // Take force remote
        }

        if (damage != 0)
        {

            // Always send negative values tu HPHUD
            if (damage > 0)
            {
                damage *= -1;
            }

            levelManager.hpAndMp.ChangeHP(damage); // Change local HP
            SendMessageToServer("ChangeHpHUDToRoom/" + damage); // Change remote HP
        }

        AnimateTakingDamage();
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(WaitTakingDamage());
        }
    }

    #endregion

    #endregion

    #region Utils

    // Set variables in their default state

    #region Initializers

    protected void IgnoreCollisionWithObjectsWhoHateMe()
    {
        IgnoreCollisionWithPlayers[] objectsWhoHateMe = FindObjectsOfType<IgnoreCollisionWithPlayers>();

        if (objectsWhoHateMe != null)
        {
            foreach (IgnoreCollisionWithPlayers objectWhoHatesMe in objectsWhoHateMe)
            {
                BoxCollider2D colliderWhoHatesMe = objectWhoHatesMe.GetComponent<BoxCollider2D>();
                Physics2D.IgnoreCollision(colliderWhoHatesMe, gameObject.GetComponent<BoxCollider2D>(), true);
            }

        }
    }
    public void IgnoreCollisionBetweenPlayers()
    {
        Collider2D collider = GetComponent<Collider2D>();

        GameObject player1 = GameObject.Find("Verde");
        GameObject player2 = GameObject.Find("Rojo");
        GameObject player3 = GameObject.Find("Amarillo");

        if (player1)
        {
            Physics2D.IgnoreCollision(collider, player1.GetComponent<Collider2D>());
        }
        else
        {
            mustIgnoreCollisionWithVerde = true; 
        }

        if (player2)
        {
            Physics2D.IgnoreCollision(collider, player2.GetComponent<Collider2D>());
        }
        else
        {
            mustIgnoreCollisionWithRojo = true; 
        }
        if (player3)
        {
            Physics2D.IgnoreCollision(collider, player3.GetComponent<Collider2D>());
        }
        else
        {
            mustIgnoreCollisionWithAmarillo = true; 
        }
    }

    public void IgnoreCollisionWithOnePlayerOnly(GameObject otherPlayer)
    {
        Collider2D[] myColliders = GetComponents<Collider2D>();
        Collider2D[] otherColliders = otherPlayer.GetComponents<Collider2D>();

        foreach (Collider2D myCollider in myColliders)
        {
            if (myCollider.isTrigger == false)
            {
                foreach (Collider2D otherCollider in otherColliders)
                {
                    if (otherCollider.isTrigger == false)
                    {
                        Physics2D.IgnoreCollision(myCollider, otherCollider, true);
                    }
                }
            }
        }
    }


    #endregion

    // Validate for player conditions

    #region Validations

    protected bool GameObjectIsPOI(GameObject other)
    {
        return other.GetComponent<PlannerPoi>();
    }

    protected bool GameObjectIsNewPOI(GameObject other)
    {
        return other.GetComponent<Poi>();
    }


    #endregion

    // Set player data from other classes

    #region Remote Setters

    public virtual void StopMoving()
    {
        canMove = false;

        isTakingDamage = false;
        isAttacking = false;

        remoteJumping = false;
        remoteRight = false;
        remoteLeft = false;

        SendPlayerDataToServer();

        if (sceneAnimator)
        {
            sceneAnimator.SetFloat("Speed", 0, gameObject);
            sceneAnimator.SetBool("IsGrounded", true, gameObject);
            sceneAnimator.SetBool("Attacking", false, gameObject);
        }
    }

    public virtual void ResumeMoving()
    {
        canMove = true;
    }

    public void SetPositiveGravity(bool hasPositiveGravity)
    {
        if (hasPositiveGravity)
        {
            directionY = 1;
            rb2d.gravityScale = 2.5f;
            cameraState = CameraState.Normal; 
            if (localPlayer)
            {
                SetCamera();
            }
        }
        else
        {
            directionY = -1;
            rb2d.gravityScale = -1.5f;
            cameraState = CameraState.Backwards;
            if(localPlayer)
            {
                SetCamera();
            }
        }

        transform.localScale = new Vector3(directionX, directionY, 1f);
    }

    private void SetCamera()
    {
        GameObject camera = GameObject.Find("MainCamera");
        if (camera != null)
        {
            CameraController cameraController = camera.GetComponent<CameraController>();
            float cameraSize = cameraController.initialSize;
            cameraController.ChangeState(cameraState);
        }
    }

    protected void SetRespawn(Vector3 placeToGo)
    {
        if (!localPlayer)
        {
            return;
        }

        respawnPosition = placeToGo;

    }

    public virtual void SetPowerState(bool active)
    {
        justPowered = true;
        ToggleParticles(active);
        isPowerOn = active;

        if (availablePowerable != null)
        {
            TogglePowerable(active);
        }

        if (active == false)
        {
            TurnPowerablePlayersOff();
        }

        StartCoroutine(WaitPowerable());
    }

    protected void TurnPowerablePlayersOff()
    {
        GameObject[] players = levelManager.players;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PowerablePlayer>())
            {
                if (player.GetComponent<PowerablePlayer>().IsPowered())
                {
                    player.GetComponent<PowerablePlayer>().ShutDownPowerable();
                }
            }
        }
    }

    #region MusicHandlers

    protected void HandlerMusicScene1( )
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena1, true, true);
    }
    protected void HandlerMusicScene2( )
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena2, true, true);
    }
    protected void HandlerMusicScene3()
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena3, true, true);
    }
    protected void HandlerMusicScene4()
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena4, true, true);
    }
    protected void HandlerMusicScene5( )
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena5, true, true);
    }
    protected void HandlerMusicScene6( )
    {
        SoundManager sManager = FindObjectOfType<SoundManager>();
        sManager.PlaySound(mainCamera, GameSounds.Escena6, true, true);
    }

    #endregion


    protected void ResetDamagingObjects()
    {
        DamagingObject[] damagingObjects = FindObjectsOfType<DamagingObject>();
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        foreach (DamagingObject damaging in damagingObjects)
        {
            damaging.UpdateCollisionsWithPlayer(gameObject, false);
        }
        foreach (EnemyController enemy in enemies)
        {
            enemy.UpdateCollisionsWithPlayer(gameObject, false);
        }
    }

    /*protected void TurnPlayerPowerablesOff(bool isPowerOn)
    {
        if (isPowerOn == false)
        {
            TurnPowerablePlayersOff();
        }
    }*/


    protected void TogglePowerable(bool activate)
    {
        GameObject availablePowerableObject = (availablePowerable);
        if (availablePowerableObject)
        {
            PowerableObject powerable = availablePowerableObject.GetComponent<PowerableObject>();

            for (int i = 0; i < powerable.powers.Length; i++)
            {
                if (powerable.PlayerActivatesPower(powerable.powers[i].caster, gameObject))
                {
                    if (powerable.IsPowered() && !activate)
                    {
                        powerable.DeactivatePower();
                    }
                    else if (!powerable.IsPowered() && activate)
                    {
                        powerable.ActivatePower(powerable.powers[i]);
                    }
                    break;
                }

            }
        }

    }

    public void SetDamageFromServer(Vector2 force)
    {
        rb2d.AddForce(force);
    }

    public void SetPlayerDataFromServer(float positionX, float positionY, int directionX, int directionY, float speedX, bool isGrounded, bool remoteJumping, bool remoteLeft, bool remoteRight)
    {

        this.remoteJumping = remoteJumping;
        this.remoteRight = remoteRight;
        this.remoteLeft = remoteLeft;
        this.isGrounded = isGrounded;
        this.directionX = directionX;
        this.directionY = directionY;
        this.speedX = speedX;

        if (sceneAnimator)
        {
            sceneAnimator.SetFloat("Speed", Mathf.Abs(speedX), this.gameObject);
            sceneAnimator.SetBool("IsGrounded", isGrounded, this.gameObject);
        }

        transform.position = new Vector3(positionX, positionY, transform.position.z);
        transform.localScale = new Vector3(directionX, directionY, 1f);
    }

    #endregion

    // Manage particles

    #region Particles

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = GetComponentsInChildren<ParticleSystem>();

        if (_particles == null || _particles.Length == 0)
        {
            return;
        }

        particles = new GameObject[_particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = _particles[i].gameObject;
        }

        ToggleParticles(false);

    }

    protected virtual void ToggleParticles(bool active)
    {

        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(active);
            }
        }
    }

    #endregion

    // Manage animations

    #region Animations

    public void PlayMusic()
    {
        SendMessageToServer("WhichMusicShloudIPlay");
    }

    public void HandleMusicAssignment(string sceneName)
    {
        switch (sceneName)
        {
            case "Escena1":
                HandlerMusicScene1();
                break;
            case "Escena2":
                HandlerMusicScene2();
                break;
            case "Escena3":
                HandlerMusicScene3();
                break;
            case "Escena4":
                HandlerMusicScene4();
                break;
            case "Escena5":
                HandlerMusicScene5();
                break;
            case "Escena6":
                HandlerMusicScene6();
                break;
        }
    }
    protected void AnimateAttack()
    {
        if (sceneAnimator && attackAnimName != null)
        {
            sceneAnimator.StartAnimation(attackAnimName, gameObject);
        }
    }

    protected void AnimateTakingDamage()
    {
        if (sceneAnimator)
        {
            sceneAnimator.StartAnimation("TakingDamage", this.gameObject);
        }
    }

    #endregion

    // Doh...

    #region Attacks

    protected virtual AttackController GetAttack()
    {
        throw new NotImplementedException("Every player must implement a GetAttack method");
    }

    #endregion

    #region Movement

    protected bool IsGoingRight()
    {
        if (localPlayer)
        {

            bool buttonRightPressed = CnInputManager.GetAxisRaw("Horizontal") == 1;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if (buttonRightPressed && !remoteRight)
            {
                remoteRight = true;
                remoteLeft = false;
                SendPlayerDataToServer();
            }

            // si no se esta apretando el joystick
            else if (!buttonRightPressed && remoteRight)
            {
                remoteRight = false;
                SendPlayerDataToServer();
            }

        }

        return remoteRight;

    }

    protected bool IsGoingLeft()
    {
        if (localPlayer)
        {

            bool buttonLeftPressed = CnInputManager.GetAxisRaw("Horizontal") == -1f;

            // si el wn esta apuntando hacia arriba/abajo con menor inclinacion que hacia la derecha, start moving
            if (buttonLeftPressed && !remoteLeft)
            {
                remoteLeft = true;
                remoteRight = false;
                SendPlayerDataToServer();
            }

            // si no se esta apretando el joystick
            else if (!buttonLeftPressed && remoteLeft)
            {
                remoteLeft = false;
                SendPlayerDataToServer();
            }

        }

        return remoteLeft;
    }

    public bool IsGoingUp()
    {
        return false;
    }

    protected bool IsItGrounded()
    {
        // El radio del groundChecker debe ser menor a la medida del collider del player/2 para que no haga contactos laterales.
        groundCheckRadius = GetComponent<Collider2D>().bounds.extents.x;

        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround) == true)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        }
        else if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsAlsoGround) == true)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsAlsoGround);
        }

        return false;
    }

    protected virtual bool IsJumping(bool isGrounded)
    {
        if (localPlayer)
        {
            if (justJumped)
            {
                return false;
            }

            bool pressedJump = CnInputManager.GetButtonDown("Jump Button") && !justJumped;

            bool isJumping = pressedJump && isGrounded;

            if (isJumping && !remoteJumping)
            {
                remoteJumping = true;
                SendPlayerDataToServer();
            }
            else if (!isJumping && remoteJumping)
            {
                remoteJumping = false;
                SendPlayerDataToServer();
            }

        }

        return remoteJumping;

    }

    protected void ResetDirectionX(int newDirectionX)
    {
        transform.localScale = new Vector3(newDirectionX, directionY, 1f);
        directionX = newDirectionX;
        acceleration = .1f;
    }

    protected void Accelerate()
    {
        if (canAccelerate)
        {
            acceleration += .1f;
            canAccelerate = false;
        }
        else
        {
            canAccelerate = true;
        }

    }

    #endregion

    #endregion

    #region Events

   /* protected void OnTriggerStay2D(Collider2D other)
    {
        if (GameObjectIsPOI(other.gameObject))
        {
            PlannerPoi newPoi = other.GetComponent<PlannerPoi>();
            if (!playerObj.playerAt.name.Equals(newPoi.name))
            {
                Debug.Log("Change OK: " + newPoi.name);
                playerObj.playerAt = newPoi;
                playerObj.luring = false;
                if (newPoi.araña != null && this.playerId == 0)
                {
                    playerObj.luring = true;
                    newPoi.araña.blocked = false;
                    newPoi.araña.open = true;
                }
                Planner planner = FindObjectOfType<Planner>();
                planner.Monitor();
                Client.instance.SendMessageToServer("EnterPOI/" + newPoi.name, true);
            }
        }
    }
    */
    #endregion

    #region Messaging

    public void SendPlayerDataToServer()
    {
        if (!localPlayer)
        {
            return;
        }

        string message = "PlayerChangePosition/" +
                               playerId + "/" +
                               transform.position.x + "/" +
                               transform.position.y + "/" +
                               directionX + "/" +
                               directionY + "/" +
                               Mathf.Abs(rb2d.velocity.x) + "/" +
                               isGrounded + "/" +
                               remoteJumping + "/" +
                               remoteLeft + "/" +
                               remoteRight;

        SendMessageToServer(message);
    }

    protected virtual void SendAttackDataToServer()
    {
        string message = "PlayerAttack/" + playerId + "/" + transform.position.x + "/" + transform.position.y;
        SendMessageToServer(message);
    }

    protected void SendPowerDataToServer()
    {
        string message = "PlayerPower/" + playerId + "/" + isPowerOn;
        SendMessageToServer(message);
    }

    public void SendPlayerPowerDataToServer()
    {
        SendPowerDataToServer();
    }

    public void SendMPDataToServer()
    {
        SendMessageToServer("ChangeMpHUDToRoom/" + mpSpendRate);
    }

    protected void SendMessageToServer(string message)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, false);
        }
    }

    #endregion

    #region Coroutines


    public IEnumerator WaitTakingDamage()
    {
        yield return new WaitForSeconds(takeDamageRate);
        isTakingDamage = false;
    }

    public IEnumerator WaitAttacking()
    {
        yield return new WaitForSeconds(attackRate + .5f);
        isAttacking = false;
    }


    public IEnumerator WaitPowerable()
    {
        yield return new WaitForSeconds(attackRate + .1f);
        justPowered = false;
    }

    public IEnumerator WaitJumping()
    {
        yield return new WaitForSeconds(jumpRate);
        justJumped = false;
    }




    #endregion

}