using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDDisplay : MonoBehaviour
{

    #region Attributes

    public GameObject HPParticles;
    public GameObject MPParticles;

    public float maxHP;
    public float maxMP;
    public float maxExp;

    public float hpCurrentPercentage;
    public float mpCurrentPercentage;
    public float expCurrentValue;

    private float hpLastPercentage;
    private float mpLastPercentage;
    private float expLastPercentage;

    #endregion

    #region Start

    public void Start()
    {
        // TODO: Setear estos remotamente
        maxHP = 250;
        maxMP = 250;
        //maxExp = 250;

        hpCurrentPercentage = 1f;
        mpCurrentPercentage = 1f;

        hpLastPercentage = 1f;
        mpLastPercentage = 1f;

        InitializeParticles();

    }

    #endregion

    #region Common

    public void ChangeHPAndMP(float delta)
    {
        ChangeHP(delta);
        ChangeMP(delta);
    }

    public void ChangeHP(float deltaHP)
    {
        if (deltaHP == 0)
        {
            return;
        }

        float currentHP = maxHP * hpCurrentPercentage;

        currentHP += deltaHP;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
            StopHPParticles();
        }
        else if (currentHP < 0)
        {
            currentHP = 0;
        }

        float percentage = currentHP / maxHP;
        CurrentHPPercentage(percentage);

    }

    public void ChangeMP(float deltaMP)
    {
        if (deltaMP == 0)
        {
            return;
        }

        float currentMP = maxMP * mpCurrentPercentage;

        currentMP += deltaMP;

        if (currentMP > maxMP)
        {
            currentMP = maxMP;
            StopMPParticles();
        }
        else if (currentMP < 0)
        {
            currentMP = 0;
        }

        float percentage = currentMP / maxMP;
        CurrentMPPercentage(percentage);

    }


    public void CurrentHPPercentage(float percentage)
    {
        hpCurrentPercentage = percentage;

        if (percentage == 1)
        {
            StopHPParticles();
        }
        else
        {
            StartHPParticles();
        }

        Vector2 healthMaskSizeDelta = GameObject.Find("HealthMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("HealthPercentage").GetComponent<Text>();

        if (hpCurrentPercentage <= 0.2f)
        {
            percentageText.text = "<color=#e67f84ff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 0, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 0, 0, 255);
        }
        else if (hpCurrentPercentage <= 0.5f)
        {
            percentageText.text = "<color=#f9ca45ff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 174, 0, 190);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 77, 0, 255);
        }
        else
        {
            percentageText.text = "<color=#64b78eff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(0, 135, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(0, 77, 0, 255);
        }

        float maxLimitWidth = healthMaskSizeDelta.x;

        float currentX = hpCurrentPercentage * maxLimitWidth;
        float currentY = healthMaskSizeDelta.y;

        GameObject.Find("CurrentHealth").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }

    public void CurrentMPPercentage(float percentage)
    {
        mpCurrentPercentage = percentage;

        if (percentage == 1)
        {
            StopMPParticles();
        }
        else
        {
            StartMPParticles();
        }

        Vector2 manaMaskSizeDelta = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("ManaPercentage").GetComponent<Text>();

        float maxLimitWidth = manaMaskSizeDelta.x;

        float currentX = mpCurrentPercentage * maxLimitWidth;
        float currentY = manaMaskSizeDelta.y;

        GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);

        percentageText.text = (mpCurrentPercentage * 100).ToString("0") + "%";
    }

    public void CurrentExpValue(string value)
    {
        expCurrentValue = Int32.Parse(value);
        
        Text valueText = GameObject.Find("ExpPercentage").GetComponent<Text>();

        valueText.text = "Exp: " + expCurrentValue;
    }

    /*
    public void CurrentExpPercentage(string percentage)
    {
        expCurrentPercentage = float.Parse(percentage);

        Vector2 expMaskSizeDelta = GameObject.Find("ExpMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("ExpPercentage").GetComponent<Text>();

        float maxLimitWidth = expMaskSizeDelta.x;

        float currentX = expCurrentPercentage * maxLimitWidth;
        float currentY = expMaskSizeDelta.y;

        GameObject.Find("CurrentExp").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);

        percentageText.text = "Exp: " + (expCurrentPercentage * 100).ToString("0") + "%";
    }
    */

    #endregion

    #region Utils

    private void InitializeParticles()
    {
		GameObject healthBar = GameObject.Find("HealthBar");
        if (healthBar)
        {
            ParticleSystem particles = healthBar.GetComponentInChildren<ParticleSystem>();

            if (particles)
            {
                HPParticles = particles.gameObject;
                HPParticles.SetActive(false);
            }
        }

        GameObject manaBar = GameObject.Find("ManaBar");

        if (manaBar)
        {
            ParticleSystem particles = manaBar.GetComponentInChildren<ParticleSystem>();

            if (particles)
            {
                MPParticles = particles.gameObject;
                MPParticles.SetActive(false);
            }
        }

    }

    private void StartHPParticles()
    {
        if (hpCurrentPercentage > hpLastPercentage)
        {
            if (HPParticles && !HPParticles.activeInHierarchy)
            {
                HPParticles.SetActive(true);
            }
        }
        else if (hpCurrentPercentage == 1f && HPParticles && HPParticles.activeInHierarchy)
        {
            StopHPParticles();
        }

        hpLastPercentage = hpCurrentPercentage;
    }

    private void StartMPParticles()
    {
        if (mpCurrentPercentage > mpLastPercentage)
        {
            if (MPParticles && !MPParticles.activeInHierarchy)
            {
                MPParticles.SetActive(true);
            }
        }
        else if (mpCurrentPercentage == 1f && MPParticles && MPParticles.activeInHierarchy)
        {
            StopMPParticles();
        }

        mpLastPercentage = mpCurrentPercentage;
    }

    public void StopLocalParticles()
    {
        StopHPParticles();
        StopMPParticles();
    }

    public void StopParticles()
    {
        SendMessageToServer("StopChangeHpAndMpHUDToRoom/");
        StopLocalParticles();
    }

    private void StopMPParticles()
    {
        if (MPParticles && MPParticles.activeInHierarchy)
        {
            MPParticles.SetActive(false);
        }
    }

    private void StopHPParticles()
    {
        if (HPParticles && HPParticles.activeInHierarchy)
        {
            HPParticles.SetActive(false);
        }
    }

    private void SendMessageToServer(string message)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, false);
        }
    }

    #endregion

}
