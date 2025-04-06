using TMPro;
using UnityEngine;

public class UIEndGameText : MonoBehaviour
{
    public StateData stateData;
    public TextMeshProUGUI text;

    void OnEnable()
    {
        if (stateData.currentState == GameState.GameOver)
            text.text = "you lost";
        else if (stateData.currentState == GameState.GameWon)
            text.text = "you won";
    }
}
