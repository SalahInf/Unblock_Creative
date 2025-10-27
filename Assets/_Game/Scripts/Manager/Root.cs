using UIParty;
using UnityEngine;

public class Root : MonoBehaviour
{
    private static Root _instance;
    public static GameManager GameManager => _instance._gameManager;
    public static UIManager UIManager => _instance._uIManager;
    public static Controller Controller => _instance._controller;

  

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uIManager;
    [SerializeField] private Controller _controller;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _uIManager.Init();
    }

    private void Reset()
    {
        GameManager.Reset();

    }

    internal static void StartLevel()
    {
        UIManager.GameStarted();
    }

    public static void EndWindowStartClose()
    {
    }

    internal static void LevelWon()
    {        
        UIManager.OpenWinWindow();
    }

    public static void LevelWonClosed()
    {
        UIManager.FullSplashScreenOpeen();
    }

    internal static void LevelLost()
    {
        UIManager.OpenLoseWindow();
    }

    public static void LevelLoseClosed()
    {
        UIManager.FullSplashScreenOpeen();
    }

    public static void TransitionScreenFull()
    {
        _instance.Reset();
    }

    public static void TransitionScreenClosed()
    {
        UIManager.OpenMenu();
    }
}
