using UnityEngine;

public class MoveTowardsAndDie : MonoBehaviour
{

    public GameObject ignoredObject;
    public Vector3 target;


    private GameObject[] particles;
    private bool moving;
    public float speed;

    void Start()
    {

        if (target.Equals(default(Vector3)))
        {
            Debug.LogError("No target for moveTowardAndDie");
        }

    }

    void Update()
    {

        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed);
            if (transform.position == target)
            {
                ToggleParticles(false);
                Destroy(gameObject, .1f);
            }
        }
    }

    public void StartMoving(GameObject[] _particles)
    {
        if (ignoredObject)
        {
            IgnoreCollisionWithObject(ignoredObject);
        }
        moving = true;
        particles = _particles;

    }

    protected void ToggleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

    private void IgnoreCollisionWithObject(GameObject ignoredObject)
    {
        Physics2D.IgnoreCollision(ignoredObject.GetComponent<Collider2D>(), this.gameObject.GetComponent<Collider2D>(), true);
    }

}
