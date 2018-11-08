using UnityEngine;
using System.Collections;

public class SlideRock : DamagingObject
{

    #region Attributes

    private GameObject pasadizo;

    #endregion

    #region Start

    protected override void Start()
    {
        base.Start();
        damage = 50;
        pasadizo = GameObject.Find("PasadizoJ1J2");
    }

    #endregion

    #region Common

	public void Slide()
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        sceneAnimator.SetBool("caidaOn", true, this.gameObject);
    }

    private void KillAndDestroy(GameObject pasadizo)
    {
        GameObject humo = (GameObject)Instantiate(Resources.Load("Prefabs/Humo"));
        humo.GetComponent<Transform>().position = new Vector2(34.1f, -7.07f);

        Destroy(humo, 5f);

        GameObject particulasEffect = GameObject.Find("ParticulasMageRoca");

        SendMessageToServer("ObstacleDestroyed/" + name, true);
        SendMessageToServer("ObstacleDestroyed/" + pasadizo.name, true);

        Destroy(particulasEffect, .1f);
        Destroy(pasadizo, .1f);
        Destroy(gameObject, .1f);
    }

    #endregion

    #region Events

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "PasadizoJ1J2")
        {
            KillAndDestroy(collision.gameObject);
        }

    }

    #endregion

    #region Messaging

    private new void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance && Client.instance.GetLocalPlayer() && Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
