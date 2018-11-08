using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{

    #region Attributes

    public float destroyDelayTime;
    public bool reinforced;
    public GameObject particle;

    #endregion

    #region Start

    protected virtual void Start()
    {
        destroyDelayTime = .4f;

        if (particle != null)
        {
            particle.SetActive(false);
        }
    }

    #endregion

    #region Common

    public virtual void DestroyMe(bool destroyedFromLocal)
    {
        if (particle != null)
        {
            particle.SetActive(true);
        }

        if (destroyedFromLocal)
        {
            SendDestroyDataToServer();
        }

        if (gameObject.GetComponent<ParticleSystem>())
        {
            gameObject.GetComponent<ParticleSystem>().Play();
        }

        Destroy(gameObject, destroyDelayTime);
    }

    #endregion

    #region Messaging

    protected void SendDestroyDataToServer()
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer("ObjectDestroyed/" + name + "/", true);
        }
    }

    #endregion

}