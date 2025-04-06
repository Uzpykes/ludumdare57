using TMPro;
using UnityEngine;

public class UIScoreManager : MonoBehaviour
{
    [SerializeField]
    private StateData stateData;
    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        stateData.onScoreChanged += HandleScoreChange;
        HandleScoreChange(0, 0);
    }

    void OnDestroy()
    {
        stateData.onScoreChanged -= HandleScoreChange;
    }


    private void HandleScoreChange(int delta, int fullScore)
    {
        var score = "";
        var fullScoreAsString = fullScore.ToString();
        foreach (var c in fullScoreAsString)
        {
            score += $"<sprite=\"numbers\" name=\"{c}\">";
        }
        text.text = score;
    }
}
