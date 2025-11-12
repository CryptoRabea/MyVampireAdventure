using UnityEngine;
using UnityEngine.SceneManagement;
using VampireSurvivor.Core;
using VampireSurvivor.Save;
using VampireSurvivor.Pooling;

namespace VampireSurvivor
{
    /// <summary>
    /// Central game manager - singleton that persists across scenes
    /// Manages game state, scene transitions, and core system initialization
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("Scene Names")]
        [SerializeField] private string hubSceneName = "Hub";
        [SerializeField] private string runSceneName = "Run";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Header("Systems")]
        [SerializeField] private ObjectPoolManager poolManager;
        [SerializeField] private SaveManager saveManager;

        private GameState previousState;

        public GameState CurrentState => currentState;
        public ObjectPoolManager PoolManager => poolManager;
        public SaveManager SaveManager => saveManager;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSystems();
        }

        private void OnEnable()
        {
            // Subscribe to game events
            GameEvents.OnRunStarted += HandleRunStarted;
            GameEvents.OnRunEnded += HandleRunEnded;
            GameEvents.OnReturnToHub += HandleReturnToHub;
        }

        private void OnDisable()
        {
            // Unsubscribe from game events
            GameEvents.OnRunStarted -= HandleRunStarted;
            GameEvents.OnRunEnded -= HandleRunEnded;
            GameEvents.OnReturnToHub -= HandleReturnToHub;
        }

        private void InitializeSystems()
        {
            // Initialize Object Pool Manager
            if (poolManager == null)
            {
                GameObject poolObj = new GameObject("ObjectPoolManager");
                poolObj.transform.SetParent(transform);
                poolManager = poolObj.AddComponent<ObjectPoolManager>();
            }

            // Initialize Save Manager
            if (saveManager == null)
            {
                GameObject saveObj = new GameObject("SaveManager");
                saveObj.transform.SetParent(transform);
                saveManager = saveObj.AddComponent<SaveManager>();
            }

            // Load persistent data
            saveManager.LoadGame();

            Debug.Log("[GameManager] Systems initialized");
        }

        /// <summary>
        /// Change the current game state
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;

            previousState = currentState;
            currentState = newState;

            GameEvents.GameStateChanged(previousState, currentState);
            Debug.Log($"[GameManager] State changed: {previousState} -> {currentState}");
        }

        /// <summary>
        /// Start a new run
        /// </summary>
        public void StartRun()
        {
            ChangeGameState(GameState.InRun);
            LoadScene(runSceneName);
        }

        /// <summary>
        /// Return to hub after a run
        /// </summary>
        public void ReturnToHub()
        {
            ChangeGameState(GameState.Hub);
            LoadScene(hubSceneName);
        }

        /// <summary>
        /// Load main menu
        /// </summary>
        public void LoadMainMenu()
        {
            ChangeGameState(GameState.MainMenu);
            LoadScene(mainMenuSceneName);
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.InRun)
            {
                ChangeGameState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeGameState(GameState.InRun);
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// Quit the application
        /// </summary>
        public void QuitGame()
        {
            saveManager.SaveGame();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void HandleRunStarted()
        {
            Debug.Log("[GameManager] Run started");
            Time.timeScale = 1f;
        }

        private void HandleRunEnded(bool victory)
        {
            Debug.Log($"[GameManager] Run ended - Victory: {victory}");
            saveManager.SaveGame();
        }

        private void HandleReturnToHub()
        {
            ReturnToHub();
        }

        private void OnApplicationQuit()
        {
            saveManager.SaveGame();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                saveManager.SaveGame();
            }
        }
    }
}
