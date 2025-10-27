using UnityEngine;

namespace UIParty
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UITuto _tutoWindow;
        [SerializeField] private UiPlay _playWindow;
        [SerializeField] private UIWin _winWindow;
        [SerializeField] private UILose _loseWindow;
        [SerializeField] private UITransitionScreen _transitionWindow;


        private Canvas _m_uiCanvas;
        public Canvas UICanvas => _m_uiCanvas;

        public void Init()
        {
            _tutoWindow.Init();
            _winWindow.Init();
            _loseWindow.Init();
            _playWindow.Init();

            _transitionWindow.OnFullSplashScreen += FullSplashScreen;
            _transitionWindow.OnClosedSplashScreen += ClosedSplashScreen;

            _transitionWindow.Init();
            _transitionWindow.OnClosedSplashScreen?.Invoke();

            _m_uiCanvas = GetComponent<Canvas>();
        }

        public void OpenMenu()
        {
            _tutoWindow.Open();

            if (!Root.GameManager.gameStart)
                _tutoWindow.Open();

        }

        public void GameStarted()
        {
            _tutoWindow.Close();
            _playWindow.Open();
        }

        public void OpenWinWindow()
        {
            _winWindow.Open();
            _playWindow.Close();
        }

        public void OpenLoseWindow()
        {
            _loseWindow.Open();
            _playWindow.Close();
        }
      
        public void FullSplashScreenOpeen()
        {
            _transitionWindow.Open();
        }

        private void FullSplashScreen()
        {
            Root.TransitionScreenFull();
        }

        private void ClosedSplashScreen()
        {
            Root.TransitionScreenClosed();
        }
    }
}

