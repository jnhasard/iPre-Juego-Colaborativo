using UnityEngine;

public class ChozaChanger : MonoBehaviour
{

    #region Attributes

    private SpriteRenderer theRenderer;
    public Sprite insideHouse;
    private Sprite nobodyIn;

    #endregion

    #region Start

    void Start()
    {
        theRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        nobodyIn = theRenderer.sprite;
    }

    #endregion

    #region Events

    private void OnTriggerrEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>().localPlayer)
        {
            theRenderer.sprite = insideHouse;
            theRenderer.sortingLayerName = "Suelo";
            theRenderer.sortingOrder = -3;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>().localPlayer)
        {
            theRenderer.sprite = nobodyIn;
            theRenderer.sortingOrder = -1;
        }
    }

    #endregion

}
