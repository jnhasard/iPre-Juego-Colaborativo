using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    #region Attributes

    public float angle;
    public float asynchronyTime;
    public float rotationTime;
    public float timeToWait;
    public bool playerHasReturned;
    public bool isWorking;

    #endregion

    #region Start

    protected virtual void Start()
    {
        if (angle == 0)
        {
            Debug.LogError("Object: " + gameObject.name + " needs an angle to rotate");
        }

        StartRotation();
    }

    #endregion

    #region Common

    protected void StartRotation()
    {
        //StartCoroutine(StartDelay());
        if (playerHasReturned)
        {
            SendRotatingInstantiatorData();
            playerHasReturned = false;
        }
        StartCoroutine(RotateObject(angle, new Vector3(0, 0, 1), rotationTime));
    }

    protected virtual IEnumerator RotateObject(float angle, Vector3 axis, float inTime)
    {
        // calculate rotation speed
        float rotationSpeed = angle / inTime;

        while (true)
        {
            // save starting rotation position
            Quaternion startRotation = transform.rotation;

            float deltaAngle = 0;

            // rotate until reaching angle

            while (deltaAngle < angle)
            {
                deltaAngle += rotationSpeed * Time.deltaTime;
                deltaAngle = Mathf.Min(deltaAngle, angle);

                transform.rotation = startRotation * Quaternion.AngleAxis(deltaAngle, axis);

                yield return null;
            }
            // delay here
            yield return new WaitForSeconds(timeToWait);
        }
    }


    #endregion

    protected virtual IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(asynchronyTime);
        Debug.Log("Rotator waited Delay");
    }

    private void SendRotatingInstantiatorData()
    {
        float zRotation = transform.rotation.z;
        string message = "CoordinateRotators/" + name + "/" + zRotation;

        SendMessageToServer(message, true);
    }

    public void HandleRotatingInstantiatorData(string[] msg)
    {
        float rotationAngle = float.Parse(msg[2]);
        Quaternion _Q = transform.rotation;
        transform.rotation = _Q * Quaternion.AngleAxis(rotationAngle, new Vector3(0, 0, 1));
        StartRotation();
    }

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }
}   