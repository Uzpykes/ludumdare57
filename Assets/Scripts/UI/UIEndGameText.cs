using UnityEngine;
using UnityEngine.UI;

public class UIEndGameText : MonoBehaviour
{
    public StateData stateData;
    public Image image;
    public Sprite winSprite;
    public Sprite loseSprite;

    void OnEnable()
    {
        if (stateData.currentState == GameState.GameOver)
            image.sprite = loseSprite;
        else if (stateData.currentState == GameState.GameWon)
            image.sprite = winSprite;
    }
}
