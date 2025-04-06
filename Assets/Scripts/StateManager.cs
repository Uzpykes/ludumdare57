#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class StateManager : MonoBehaviour
{
    public StateData stateData;
    public RectTransform mainMenu;
    public RectTransform gameUI;
    public RectTransform endUI;

    public UnityEvent onGameEnd;
    public UnityEvent onGameStart;

    void Awake()
    {
        stateData.onStateChanged += HadleStateChange;
        stateData.onBallCaptured += HandleBalLCapture;
        stateData.SetGameState(GameState.MainMenu);
    }

    void OnDestroy()
    {
        stateData.onStateChanged -= HadleStateChange;
    }

    void HandleBalLCapture(DiceFaceSlot slot, int count)
    {
        if (stateData.capturedBalls.Keys.Count == 7)
        {
            stateData.SetGameState(GameState.GameWon);
        }
    }

    void HadleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                mainMenu.gameObject.SetActive(true);
                gameUI.gameObject.SetActive(false);
                endUI.gameObject.SetActive(false);
                break;
            case GameState.Game:
                stateData.ResetData();
                mainMenu.gameObject.SetActive(false);
                endUI.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(true);
                break;
            case GameState.GameOver:
            case GameState.GameWon:
                mainMenu.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                endUI.gameObject.SetActive(true);
                break;
        }
    }

    public void OnPlayClicked()
    {
        if (stateData.currentState == GameState.MainMenu || stateData.currentState == GameState.GameOver || stateData.currentState == GameState.GameWon)
        {
            stateData.ResetData();
            stateData.SetGameState(GameState.Game);
        }
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

}
