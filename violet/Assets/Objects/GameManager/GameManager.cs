using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateGameState(GameState.Init);
    }

    public void UpdateGameState(GameState state)
    {
        OnGameStateChange?.Invoke(state);
        Debug.Log($"Game State: {state}");
    }
}

public enum GameState
{
    Init,
    GameStart,
    FadeInView,
    LoadEnvironment,
    LoadScoreboard,
    FadeOutView,
    Gameplay,
    EndState
}
