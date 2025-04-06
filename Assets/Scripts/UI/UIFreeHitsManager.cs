using TMPro;
using UnityEngine;

public class UIFreeHitsManager : MonoBehaviour
{
    [SerializeField]
    private StateData stateData;
    [SerializeField]
    private TextMeshProUGUI text;

    void Start()
    {
        stateData.onFreeHitsChanged += HandleFreeHitsChange;
        HandleFreeHitsChange(0, 0);
    }

    void OnEnable()
    {
        HandleFreeHitsChange(0, stateData.currentFreeHits);
    }

    void OnDestroy()
    {
        stateData.onFreeHitsChanged -= HandleFreeHitsChange;
    }


    private void HandleFreeHitsChange(int delta, int fullScore)
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
