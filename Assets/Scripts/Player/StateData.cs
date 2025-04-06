using System;
using System.Collections.Generic;
using UnityEngine;

public class StateData : MonoBehaviour
{
    public GameState currentState { get; private set; }

    public int currentScore { get; private set; }
    public int currentDepth { get; private set; }
    public int currentRerolls { get; private set; }
    public int currentFreeHits { get; private set; }
    public Dictionary<DiceFaceSlot, int> capturedBalls { get; private set; } = new();

    public Action<DiceFaceSlot, int> onBallCaptured;
    public Action<int, int> onScoreChanged;

    public void SetGameState(GameState newState)
    {
        currentState = newState;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        onScoreChanged(score, currentScore);
    }

    public void AddDepth(int depth)
    {
        currentDepth += depth;
    }

    public void AddReroll(int reroll)
    {
        currentRerolls += reroll;
    }

    public void AddFreeHits(int hitChances)
    {
        currentFreeHits += hitChances;
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

        onBallCaptured(ball, capturedBalls[ball]);
    }
}

public enum GameState
{
    MainMenu,
    Game,
    GameOver,
    GameWon
}
