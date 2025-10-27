using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    private bool _gameStarted = false;
    private int _level;
    public bool gameStart => _gameStarted;
    public Level[] levelList;
    
    public Material[] colorsWagon;
    public Material[] colorsCELL;
    [SerializeField] CinemachineVirtualCamera _cam;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Init();
    }

    public void Reset()
    {
        Init();
        CameraController();
        _gameStarted = false;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Root.LevelWon();
        }
    }
    public void Init()
    {
        _level = _level < levelList.Length ? _level : 0;
        GridSpowner.instance.Init(_level);
        CameraController();
    }
    void CameraController()
    {
        _cam.m_Lens.FieldOfView = Mathf.Clamp(GridSpowner.instance.rows * 10,40 , 60);
    }
    public void StartGame()
    {
        _gameStarted = true;
        Root.StartLevel();
    }

    public void GameWin()
    {
        if (_gameStarted)
        {
            _level++;
            _gameStarted = false;
            Root.LevelWon();
        }
    }

    public void LoseGame()
    {
        if (_gameStarted)
        {
            _gameStarted = false;
            Debug.Log("lose Game");
            Root.LevelLost();
        }
    }
}
