using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackToggle : MonoBehaviour {

    public GameObject[] feedbacks;

	// Use this for initialization
	void Start ()
    {
        CheckParameters();
        ToggleFeedbacks(false);
    }
	
    public void ToggleFeedbacks(bool active)
    {
        for (int i = 0; i < feedbacks.Length; i++)
        {
            Collider2D[] colliders = feedbacks[i].GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (collider.isTrigger)
                {
                    collider.enabled = active;
                }
            }
        }
    }

    private void CheckParameters()
    {
        if (feedbacks.Length == 0)
        {
            Debug.LogError("Feedback Toggler named : " + gameObject.name + "needs gameObjects to toggle");
        }
    }
}
