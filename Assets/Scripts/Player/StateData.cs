using System;
using System.Collections.Generic;
using UnityEngine;

public class StateData : MonoBehaviour
{
    public GameState currentState { get; private set; }

    public int currentScore { get; private set; }
    public int currentDepth { get; private set; }
    public int currentRerolls { get; private set; } = 50;
    public int currentFreeHits { get; private set; }
    public Dictionary<DiceFaceSlot, int> capturedBalls { get; private set; } = new();

    public Action<DiceFaceSlot, int> onBallCaptured;
    public Action<int, int> onScoreChanged;
    public Action<int, int> onDepthChanged;
    public Action<int, int> onFreeHitsChanged;
    public Action<int, int> onRerollsChanged;
    public Action<GameState> onStateChanged;

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        onStateChanged?.Invoke(newState);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        onScoreChanged.Invoke(score, currentScore);
    }

    public void AddDepth(int depth)
    {
        currentDepth += depth;
        onDepthChanged?.Invoke(depth, currentDepth);
    }

    public void AddReroll(int reroll)
    {
        currentRerolls += reroll;
        onRerollsChanged?.Invoke(reroll, currentRerolls);
    }

    public void AddFreeHits(int hitChances)
    {
        currentFreeHits += hitChances;
        onFreeHitsChanged?.Invoke(hitChances, currentFreeHits);
    }

    public void AddBall(DiceFaceSlot ball)
    {
        if (capturedBalls.ContainsKey(ball))
        {
            capturedBalls[ball] += 1;
        }
        else
        {
            capturedBalls[ball] = 1;
        }

        onBallCaptured?.Invoke(ball, capturedBalls[ball]);
    }

    public void ResetData()
    {
        currentScore = 0;
        currentDepth = 0;
        currentRerolls = 50;
        currentFreeHits = 0;
        capturedBalls.Clear();
    }
}

public enum GameState
{
    MainMenu,
    Game,
    GameOver,
    GameWon
}
