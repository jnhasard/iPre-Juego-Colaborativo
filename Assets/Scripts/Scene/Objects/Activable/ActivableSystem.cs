using System.Collections;
using UnityEngine;

public class ActivableSystem : MonoBehaviour
{

    #region Attributes

    public Sprite activatedSprite;

    public float activationDistance = 1f;
    public float activationTime = 5f;
    public bool activated;

    [System.Serializable]
    public struct Component { public Sprite sprite; public bool placed; };
    public Component[] components;

    protected ActivableSystemActions systemActions;
    protected GameObject[] particles;

    #endregion

    protected virtual void Start()
    {
        InitializeParticles();
    }

    #region Common

    public GameObject[] GetParticles()
    {
        return particles;
    }

    public virtual bool PlaceItem(Sprite item)
    {
        if (!activated)
        {
            int pos = ComponentPosition(item);

            if (pos != -1)
            {
                PlaceComponent(pos);

                if (AllComponentsPlaced())
                {
                    activated = true;
                    StartCoroutine(Actioned());
                }

                return true;
            }

        }

        return false;
    }

    protected void PlaceComponent(int pos)
    {
        components[pos].placed = true;

        SpriteRenderer[] componentSlots = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < componentSlots.Length; i++)
        {
            if (componentSlots[i].sprite == null)
            {
                componentSlots[i].sprite = components[pos].sprite;
            }
        }

    }

    #endregion

    #region Utils

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        if (_particles.Length <= 0)
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

    public void ToggleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

    protected int ComponentPosition(Sprite item)
    {

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].sprite.Equals(item))
            {
                return i;
            }
        }

        return -1;
    }

    protected bool AllComponentsPlaced()
    {

        for (int i = 0; i < components.Length; i++)
        {
            if (!components[i].placed)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Coroutines

    protected virtual IEnumerator Actioned()
    {
        if (systemActions == null)
        {
            Debug.LogError("SystemActions not defined");
        }

        else
        {
            yield return new WaitForSeconds(activationTime);
            systemActions.DoSomething(this.gameObject, true);
        }
    }

    #endregion

}
