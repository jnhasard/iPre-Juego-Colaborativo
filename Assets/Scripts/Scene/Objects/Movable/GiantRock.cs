
using UnityEngine;

public class GiantRock : MovableObject
{
    public string blockerToBeMoved;
    private int numberOfHits;


    #region Start

    protected override void Start()
    {
        base.Start();
        openedPrefab = "Ambientales/SueloRoca";
        numberOfHits = 0;
        
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (blockerToBeMoved != null)
        {
            if (collision.gameObject.name == blockerToBeMoved)
            {
                CrushWithBlocker();
            }
        }
    }
    #endregion

    private void CrushWithBlocker()
    {
        numberOfHits++;
        if (numberOfHits == 1 || numberOfHits == 3 || numberOfHits == 5)
        {
            LevelManager lManager = FindObjectOfType<LevelManager>();
            lManager.ActivateNPCFeedback("Algo está bloqueando la roca... debe haber un modo de abrirla");
        }
    }
}
