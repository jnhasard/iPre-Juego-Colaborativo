using UnityEngine;

public class ChangeSprite : MonoBehaviour {

    #region Attributes

    private SpriteRenderer thyRenderer;
	public Sprite newSprite;

    #endregion

    #region Start

    void Start () {
		thyRenderer = GetComponent<SpriteRenderer>();
	}

    #endregion

    #region Common

    public void SpriteChanger()
	{
		thyRenderer.sprite = newSprite;
	}

    #endregion

}
