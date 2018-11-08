using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    #region Enums

    public enum TypeOfActivation { Stepping, Shooting, Bubble };
    public enum Color { Red, Blue, Yellow, Any };

    #endregion

    #region Attributes

    private SwitchManager manager;
    public GroupOfSwitchs switchGroup;
    public PlannerSwitch switchObj;
    public GameObject particles;

    public TypeOfActivation activation; // Forma en que se activa (pisando o disparando)
    public Color switchColor; // Color del switch. Determina quien lo puede apretar.

    public bool desactivable; // Si puede apagarse una vez activado
    public int individualId; // Identificador del switch dentro del grupo
    public int groupId; // Identificador del grupo de switchs.

    public int damageDone;
    public bool damageIfIncorrect;
    public bool isActivated; // Si esta encendido o no
    private bool jobDone; // true si es que su grupo de botones ya terminó su función

    #endregion

    #region Start

    private void Start()
    {
        TurnParticlesOff();
        IgnoreCollisionWithPlayers();
        RegisterOnManager();
        SetSprite();
    }

    #endregion

    #region Common

    private void Activate()
    {
        if (jobDone)
        {
            return;
        }

        isActivated = true;
        SetSprite();
        if (activation == TypeOfActivation.Shooting)
        {
            TurnParticlesOn();
        }
        SendOnDataToServer(isActivated);
        switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());
    }

    private void Desactivate()
    {
        if (jobDone)
        {
            return;
        }

        isActivated = false;
        SetSprite();
        TurnParticlesOff();
        SendOnDataToServer(isActivated);

        if (switchObj != null)
        {
            switchObj.DeactivateSwitch();
            Planner planner = FindObjectOfType<Planner>();
            planner.Monitor();
        }
    }

    #endregion

    #region Events

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool correctActivation = false;

        switch (activation)
        {
            case TypeOfActivation.Stepping:
                correctActivation = ColliderIsCorrectPlayer(collision);
                break;
            case TypeOfActivation.Shooting:
                correctActivation = ColliderIsCorrectAttack(collision);
                break;
			/*case TypeOfActivation.Bubble:
				correctActivation = ColliderIsCorrectBubble (collision);
				break;*/
        }

        if (correctActivation)
        {
            HandleActivation();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (ColliderIsCorrectPlayer(collision))
        {
            if (desactivable)
            {
                Desactivate();
            }
        }
    }

    #endregion

    #region Utils

    private void HandleActivation()
    {
        if (isActivated)
        {
            if (desactivable)
            {
                Desactivate();
            }
        }
        else
        {
            Activate();
        }
    }

    public void SetJobDone()
    {
        jobDone = true;
    }

    public void ReceiveDataFromServer(bool _isPressed)
    {
        isActivated = _isPressed;
        SetSprite();
        switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());
    }

    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (isActivated)
        {
            if (activation == TypeOfActivation.Shooting)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.ShootBlueOn;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.ShootRedOn;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.ShootYellowOn;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.ShootAnyOn;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.StepBlueOn;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.StepRedOn;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.StepYellowOn;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.StepAnyOn;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            if (activation == TypeOfActivation.Shooting)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.ShootBlueOff;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.ShootRedOff;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.ShootYellowOff;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.ShootAnyOff;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.StepBlueOff;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.StepRedOff;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.StepYellowOff;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.StepAnyOff;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    protected void RegisterOnManager()
    {
        manager = FindObjectOfType<SwitchManager>();
        manager.Add(this);
    }

    private bool ColliderIsCorrectPlayer(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player && player.localPlayer)
        {
            return CheckIfPlayerMatchWithColor(other.gameObject);
        }

        return false;
    }

    private void IgnoreCollisionWithPlayers()
    {
        if (activation == TypeOfActivation.Shooting)
        {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Verde").GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Rojo").GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Amarillo").GetComponent<BoxCollider2D>());
        }
    }

    private void TurnParticlesOff()
    {
        if (activation == TypeOfActivation.Shooting) // porque los otros no tienen
        {
            if (particles != null)
            {
                particles.SetActive(false);
            }
            else if (particles == null)
            {
                Debug.Log("te falta poner las partículas de " + name);
            }
        }
    }

    private void TurnParticlesOn()
    {
        if (particles)
        {
            particles.SetActive(true);
        }
    }



    private bool CheckIfAttackMatchWithColor(GameObject gameObject)
    {
        switch (switchColor)
        {
            case Color.Blue:
                return gameObject.GetComponent<FireballController>();
            case Color.Yellow:
                return gameObject.GetComponent<ProjectileController>();
            case Color.Red:
                return gameObject.GetComponent<PunchController>();
            case Color.Any:
                return gameObject.GetComponent<AttackController>(); ;
            default:
                return false;
        }
    }

	private bool CheckIfBubbleMatchWithColor(GameObject gameObject)
	{
		switch (switchColor)
		{
		case Color.Blue:
			return gameObject.GetComponent<FireballController>();
		case Color.Yellow:
			return gameObject.GetComponent<ProjectileController>();
		case Color.Red:
			return gameObject.GetComponent<WarriorBubbleController>();
		case Color.Any:
			return gameObject.GetComponent<AttackController>(); ;
		default:
			return false;
		}
	}

    private bool CheckIfPlayerMatchWithColor(GameObject gameObject)
    {

        switch (switchColor)
        {
            case Color.Blue:
                return gameObject.GetComponent<MageController>();
            case Color.Yellow:
                return gameObject.GetComponent<EngineerController>();
            case Color.Red:
                return gameObject.GetComponent<WarriorController>();
            case Color.Any:
                return gameObject.GetComponent<PlayerController>(); ;
            default:
                return false;
        }

    }

    private bool ColliderIsCorrectAttack(Collision2D other)
    {
        AttackController attack = other.gameObject.GetComponent<AttackController>();

        if (attack)
        {
            if (damageIfIncorrect && !CheckIfAttackMatchWithColor(other.gameObject))
            {
                DamageAllPlayers();
            }

			if (CheckIfAttackMatchWithColor(other.gameObject))
			{
				TurnParticlesOn();
			}

            return attack.caster.localPlayer && CheckIfAttackMatchWithColor(other.gameObject); // ¿Es necesario chequear aquí por el LocalPlayer?
        }

        return false;
    }

	/*private bool ColliderIsCorrectBubble(Collision2D other)
	{
		BubbleController bubble = other.gameObject.GetComponent<BubbleController>();

		if (bubble)
		{
			if (damageIfIncorrect && !CheckIfBubbleMatchWithColor(other.gameObject))
			{
				DamageAllPlayers();
			}

			if (CheckIfBubbleMatchWithColor(other.gameObject))
			{
				TurnParticlesOn();
			}

			return attack.caster.localPlayer && CheckIfAttackMatchWithColor(other.gameObject); // ¿Es necesario chequear aquí por el LocalPlayer?
		}

		return false;
		
	}
	*/
    private void DamageAllPlayers()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        GameObject[] players = levelManager.players;
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().TakeDamage(damageDone, new Vector2(-700f, 100f)); 
            }
        }

    }
    #endregion

    #region Messaging

    private void SendOnDataToServer(bool data)
    {

        string message = "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + data;

        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, true);
        }
    }

    #endregion
}
