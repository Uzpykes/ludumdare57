using TMPro;
using UnityEngine;

public class UIRerollManager : MonoBehaviour
{
    [SerializeField]
    private StateData stateData;
    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        stateData.onRerollsChanged += HandleRerollsChange;
        HandleRerollsChange(0, 0);
    }

    void OnEnable()
    {
        HandleRerollsChange(0, stateData.currentRerolls);
    }

    void OnDestroy()
    {
        stateData.onRerollsChanged -= HandleRerollsChange;
    }


    private void HandleRerollsChange(int delta, int fullScore)
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
