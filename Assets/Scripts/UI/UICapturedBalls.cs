using System.Collections.Generic;
using UnityEngine;

public class UICapturedBalls : MonoBehaviour
{
    [SerializeField]
    private StateData stateData;
    [SerializeField]
    List<UIBallItem> Balls = new();
    private Dictionary<DiceFaceSlot, UIBallItem> mapping = new();
    void Start()
    {
        foreach (var ball in Balls)
        {
            mapping[ball.type] = ball;
        }

        stateData.onBallCaptured += HandleBallCapture;
    }

    void OnEnable()
    {
        foreach (var ball in Balls)
        {
            ball.HideCaptured();
            if (stateData.capturedBalls.ContainsKey(ball.type) && stateData.capturedBalls[ball.type] > 0)
                ball.ShowCaptured();
        }

    }

    void OnDestroy()
    {
        stateData.onBallCaptured -= HandleBallCapture;
    }

    void HandleBallCapture(DiceFaceSlot type, int count)
    {
        mapping[type].ShowCaptured();
    }
}