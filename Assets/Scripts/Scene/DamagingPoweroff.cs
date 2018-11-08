using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingPoweroff : MonoBehaviour {


    public GameObject[] deactivableDamagings;
    public int hitsBeforDeactivating;

    private int touched; 


    // Use this for initialization
    void Start () {

        if (deactivableDamagings.Length<= 0)
        {
            Debug.LogError("the DamagingPowerOff named : " + gameObject.name + "needs damagings to turn off");
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.GetComponent<DamagingArrow>())
        {
            touched++;
            if (touched == hitsBeforDeactivating)
            {
                for (int i = 0; i<deactivableDamagings.Length; i++)
                {
                    DamagingInstantiator dArrow = deactivableDamagings[i].GetComponent<DamagingInstantiator>();
                    dArrow.needed = false;
                }
            }
        }
    }
}
