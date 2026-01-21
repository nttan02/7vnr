using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    Paused
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }
    public GameObject playerPrefab;
    public Transform UIHolder;
    public Transform EffectHolder;
    public Transform EnemiesHolder;
    public GameObject gameplayPrefab;
    public HealthBar healthBar;
    public Transform mapRoot;
    public Transform enemyCanvas;
    private GameScreen currentScreen;
    private GamePlay gamePlay;
    private float saveTimer;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SwitchToGameplay()
    {
        gamePlay ??= new GamePlay(this);
        SwitchScreen(gamePlay);
    }
    private void SwitchScreen(GameScreen newScreen)
    {
        currentScreen?.Hide();
        currentScreen = newScreen;
        currentScreen?.Show();
    }

    public void CreateNewCharacter()
    {
        gamePlay ??= new GamePlay(this);
        CharacterData defaultData = CharacterSaveSystem.LoadCharacter();
        if (defaultData == null)
        {
            defaultData = CreateDefaultCharacterData();
        }
        gamePlay.CreateCharacter(defaultData);
    }
    private CharacterData CreateDefaultCharacterData()
    {
        return new CharacterData
        {
            name = "Player",
            level = 10000000,
            hpMax = 100000,
            hp = 100000,
            mpMax = 50000,
            mp = 50000,
            moveSpeed = 10f,
            x = 0,
            y = 0,
            SkillIds = new List<int> { 1, 2, 3 }
        };
    }

    private void Start()
    {
        SwitchToGameplay();
        CreateNewCharacter();
        SetState(GameState.Playing);
    }
    private void Update()
    {
        currentScreen?.Update();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        saveTimer += Time.deltaTime;
        if (saveTimer >= 30f)
        {
            CharacterSaveSystem.SaveCharacter(Character.Instance);
            saveTimer = 0f;
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;
        Debug.Log($"Game State changed to: {newState}");
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            SetState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            SetState(GameState.Playing);
    }

    public static long GetCurrentMilisecond()
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (DateTime.UtcNow.Ticks - dateTime.Ticks) / 10000;

    }
}
