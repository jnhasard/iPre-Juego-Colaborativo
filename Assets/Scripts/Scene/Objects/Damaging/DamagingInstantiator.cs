using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingInstantiator : MonoBehaviour
{

    public Vector2 initialPosition;
    public Vector2 targetPosition;
    public float instantiationRate;
    public float moveSpeed;
    public bool startsAtBegginning;
    public bool needed;
    public bool needsParticles;


    public string objectName;

    public bool isWorking;
    public bool playerHasReturned;

    // Use this for initialization
    void Start()
    {

        initialPosition = gameObject.transform.position;
        CheckParameters();
        if (startsAtBegginning)
        {
            needed = true;
            StartCoroutine(InstantiateDamaging());
        }

    }

    public IEnumerator InstantiateDamaging()
    {
        while (true)
        {
            if (!needed)
            {
                yield break;
            }
            GameObject damagingObject = (GameObject)Instantiate(Resources.Load("Prefabs/Damaging/" + objectName));

            if (needsParticles)
            {
                GameObject parasiteMageParticle = (GameObject)Instantiate(Resources.Load("Prefabs/Damaging/MageArrowParticle"));
                GameObject parasiteWarriorParticle = (GameObject)Instantiate(Resources.Load("Prefabs/Damaging/WarriorArrowParticle"));
                GameObject parasiteEngineerParticle = (GameObject)Instantiate(Resources.Load("Prefabs/Damaging/EngineerArrowParticle"));
                GameObject[] parasitesForDamaging = new GameObject[3] { parasiteMageParticle, parasiteWarriorParticle, parasiteEngineerParticle };

                if (damagingObject != null)
                {
                    damagingObject.transform.position = initialPosition;
                    CheckAndSetPowerableData(damagingObject, parasitesForDamaging);
                    CheckAndSetMovingData(damagingObject, parasitesForDamaging);
                    yield return new WaitForSeconds(instantiationRate);
                }
            }

            else if (!needsParticles)
            {
                if (damagingObject != null)
                {
                    damagingObject.transform.position = initialPosition;
                    CheckAndSetMovingData(damagingObject);
                    yield return new WaitForSeconds(instantiationRate);
                }
            }
        }
    }

    private void CheckAndSetPowerableData(GameObject damagingObject, GameObject[] parasiteParticles)
    {
        if (damagingObject.GetComponent<PowerableObject>())
        {
            PowerableObject powerableObject = damagingObject.GetComponent<PowerableObject>();

            for (int i = 0; i < powerableObject.powers.Length; i++)
            {
                if (i <= 1)
                {
                    powerableObject.powers[i].particles[0] = parasiteParticles[0];
                }
                if (i <= 3 && i >= 2)
                {
                    powerableObject.powers[i].particles[0] = parasiteParticles[1];
                }
                if (i <= 5 && i >= 4)
                {
                    powerableObject.powers[i].particles[0] = parasiteParticles[2];
                }
            }
        }
    }

    private void CheckAndSetMovingData(GameObject damagingObject, GameObject[] parasitesForDamaging)
    {
        if (damagingObject.GetComponent<OneTimeMovingObject>())
        {
            OneTimeMovingObject objectMovement = damagingObject.GetComponent<OneTimeMovingObject>();
            objectMovement.SetParasiteParticles(parasitesForDamaging);
            objectMovement.target = targetPosition;
            objectMovement.moveSpeed = moveSpeed;
            objectMovement.needsParticles = true;
            objectMovement.move = true;
            objectMovement.diesAtTheEnd = true;


            if (targetPosition.y > transform.position.y + 1)
            {
                Quaternion _Q = objectMovement.transform.rotation;
                objectMovement.transform.rotation = _Q * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
            }
            else if (targetPosition.y < transform.position.y - 1)
            {
                Quaternion _Q = objectMovement.transform.rotation;
                objectMovement.transform.rotation = _Q * Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
            }

            if (targetPosition.x > transform.position.x)
            {
                objectMovement.transform.localScale *= -1;
            }

        }

    }

    private void CheckAndSetMovingData(GameObject damagingObject)
    {
        if (damagingObject.GetComponent<OneTimeMovingObject>())
        {
            OneTimeMovingObject objectMovement = damagingObject.GetComponent<OneTimeMovingObject>();
            objectMovement.target = targetPosition;
            objectMovement.moveSpeed = moveSpeed;
            objectMovement.needsParticles = false;
            objectMovement.move = true;
            objectMovement.diesAtTheEnd = true;


            if (targetPosition.y > transform.position.y + 1)
            {
                Quaternion _Q = objectMovement.transform.rotation;
                objectMovement.transform.rotation = _Q * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
            }
            else if (targetPosition.y < transform.position.y - 1)
            {
                Quaternion _Q = objectMovement.transform.rotation;
                objectMovement.transform.rotation = _Q * Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
            }

            if (targetPosition.x > transform.position.x)
            {
                objectMovement.transform.localScale *= -1;
            }

        }

    }
    private void CheckParameters()
    {
        needed = false;
        if (initialPosition == new Vector2(0f, 0f))
        {
            Debug.LogError("DamagingInstantiator: " + gameObject.name + " needs an Initial Position");
        }

        if (targetPosition == new Vector2(0f, 0f))
        {
            Debug.LogError("DamagingInstantiator: " + gameObject.name + " needs a Target Position");
        }

        if (instantiationRate == 0f)
        {
            Debug.LogError("DamagingInstantiator: " + gameObject.name + " needs an Instantiatio Rate");
        }

        if (moveSpeed == 0f)
        {
            Debug.LogError("DamagingInstantiator: " + gameObject.name + " needs a MoveSpeed");
        }
    }
}
