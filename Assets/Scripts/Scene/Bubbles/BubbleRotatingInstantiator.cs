using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleRotatingInstantiator : MonoBehaviour
{

    [Serializable]

    public class BubbleTargets
    {
        public Vector2[] bubbleTargets;
    }

    public BubbleTargets[] bubbleThrowings;
    public string[] bubbleNames;
	public float timeToKillBubble;

	private int gearsActivated;
	public int gearsNeeded;

    public float angle;
    public float rotationTime;
    public float timeToWait;
    public bool rotatesInStart;
    public bool playerHasReturned;
    public bool isWorking;
    public bool activatesFeedback;
    public GameObject feedbackToggle;


    // Variable Auxiliar, para hacer un swap tenís que tener una variable auxiliar. 

    public float speed;
    private LevelManager levelManager;

    // Use this for initialization

    private void Start()
    {
		isWorking = false; 

		if (rotatesInStart) 
		{ 
			isWorking = true; 
			StartRotation ();
		}

		if (gearsNeeded == 0 && !rotatesInStart) 
		{
			Debug.LogError ("Gear System " + gameObject.name + " has no Number of Gears Needed Setted");
		}

		gearsActivated = 0;
		levelManager = FindObjectOfType<LevelManager>();

		if(angle == 0)
		{
			Debug.LogError("Object: " + gameObject.name + " needs an angle to rotate");
		}

        if (speed <= 0.5)
        {
            Debug.LogError("THIS " + gameObject.name + " BUBBLEINSTANTIATOR HAS NO SPEED!");
        }
    }

    protected IEnumerator RotateObject(float angle, Vector3 axis, float inTime)
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

            InstantiateObjects();
            // delay here
            yield return new WaitForSeconds(timeToWait);
        }
    }

    protected void StartRotation()
    {
        if (activatesFeedback)
        {
            FeedbackToggle(true);
        }
        StartCoroutine(RotateObject(angle, new Vector3(0, 0, 1), rotationTime));
    }

    private void InstantiateObjects()
    {
		if (playerHasReturned) 
		{
			SendBubbleInstantiatorData ();
			playerHasReturned = false; 
		}

        for (int i = 0; i < bubbleThrowings.Length; i++)
        {
			levelManager.InstantiateBubbleWithTargets(bubbleNames[i], bubbleThrowings[i].bubbleTargets[0], bubbleThrowings[i].bubbleTargets, speed, timeToWait, timeToKillBubble);
        }
        ChangeBubbleNamesOrder();
    }

	public void GearActivation()
	{
		gearsActivated++; 
		Debug.LogError ("Number of gears who activated is: " + gearsActivated); 
		if (gearsActivated >= gearsNeeded) 
		{
			isWorking = true; 
			StartRotation ();
			gearsActivated = 0;
		}
	}
    private void ChangeBubbleNamesOrder()
    {
        int j = bubbleNames.Length -1;
        string lostName = bubbleNames[j];

        for (int i = bubbleNames.Length-1; i > -1; i--)
        {
            j = i - 1;
            if (j >= 0)
            {
                bubbleNames[i] = bubbleNames[j];
            }
            else if (j < 0)
            {
                bubbleNames[i] = lostName;
            }
        }
    }
		
	protected void SendBubbleInstantiatorData()
	{
		float zRotation = transform.rotation.z;


		string message = "InstantiateBubble/" + name + "/" + zRotation + "/BubbleNames/"; 

		for (int i = 0; i < bubbleNames.Length; i++) 
		{
			message += "/" + bubbleNames [i];
		}
			
		SendMessageToServer (message, true);
	}

	public void HandleBubbleInstantiatorData(string[] msg)
	{
		for (int i = 4; i < msg.Length; i++)
		{ 
			bubbleNames [i] = msg [i];
		}
			
		float rotationAngle = float.Parse (msg [2]);
		Quaternion _Q = transform.rotation;
		transform.rotation = _Q * Quaternion.AngleAxis (rotationAngle, new Vector3 (0, 0, 1));
		StartRotation ();
 		
	}
	private void SendMessageToServer(string message, bool secure)
	{
		if (Client.instance)
		{
			Client.instance.SendMessageToServer(message, secure);
		}
	}

    private void FeedbackToggle(bool active)
    {
        feedbackToggle.GetComponent<FeedbackToggle>().ToggleFeedbacks(active);
    }
}
