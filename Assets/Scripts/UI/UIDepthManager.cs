using TMPro;
using UnityEngine;

public class UIDepthManager : MonoBehaviour
{
    [SerializeField]
    private StateData stateData;
    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        stateData.onDepthChanged += HandleDepthChange;
        HandleDepthChange(0, stateData.currentDepth);
    }

    void OnEnable()
    {
        HandleDepthChange(0, stateData.currentDepth);
    }

    void OnDestroy()
    {
        stateData.onDepthChanged -= HandleDepthChange;
    }


    private void HandleDepthChange(int delta, int fullScore)
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
