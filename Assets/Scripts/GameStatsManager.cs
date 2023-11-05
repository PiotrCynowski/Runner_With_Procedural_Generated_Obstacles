using UnityEngine;
using UnityEngine.UI;
using GameInput;
using System;
using Player;

namespace GameUI
{
    public class GameStatsManager : MonoBehaviour
    {
        public static GameStatsManager Instance;

        public static Action OnPlayerStartLevel;
        public static Action OnPlayerReloadLevel;
        public static Action OnLevelWon;

        [Header("References Gameplay")]
        [SerializeField] private Text gamePoints;
        [SerializeField] private GameObject panelPause;
       
        [Header("References Menu")]
        [SerializeField] private Text menuCurrentPoints;
        [SerializeField] private Text menuBestPoints;
        [SerializeField] private Button buttonStart;
        [SerializeField] private Button buttonNext;
        [SerializeField] private Button buttonRestart;
        [SerializeField] private Button buttonUnPause;

        private enum PlayerStatus {gameWon, gamePause, gameLost};

        public int playerStagesLeft;
        private int _currentPoints;
        public int currentPoints
        {
            get { return _currentPoints; }
            set
            {
                _currentPoints = value;
                gamePoints.text = _currentPoints.ToString();
            }
        }

        private int recordPoints;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            buttonStart.onClick.AddListener(() => StartGame());
            buttonRestart.onClick.AddListener(() => RestartGame());
            buttonUnPause.onClick.AddListener(() => UnPauseGame());

            buttonNext.onClick.AddListener(() => StartGame());

            if (PlayerPrefs.HasKey("Record"))
            {
                recordPoints = PlayerPrefs.GetInt("Record");
                menuBestPoints.text = "Record: " + recordPoints.ToString();
            }
        }

        private void OnEnable()
        {
            InputManager.onPlayerEscButton += PauseGame;
            PlayerInteraction.onOutOfObstacle += PlayerStageComplete;
            PlayerInteraction.onGameOver += () => ActivGameMenu(PlayerStatus.gameLost);
        }

        private void OnDisable()
        {
            InputManager.onPlayerEscButton -= PauseGame;
            PlayerInteraction.onOutOfObstacle -= PlayerStageComplete;
            PlayerInteraction.onGameOver -= () => ActivGameMenu(PlayerStatus.gameLost);
        }

        private void PauseGame()
        {
            ActivGameMenu(PlayerStatus.gamePause);
        }

        #region buttons on click
        private void UnPauseGame()
        {
            DeActivMainMenu();

            OnPlayerStartLevel.Invoke();
        }

        private void StartGame()
        {
            OnPlayerReloadLevel.Invoke();
            OnPlayerStartLevel.Invoke();
            DeActivMainMenu();
        }

        private void RestartGame()
        {
            OnPlayerReloadLevel.Invoke();
            OnPlayerStartLevel.Invoke();
            DeActivMainMenu();
        }
        #endregion

        private void PlayerStageComplete(int heightOfObstacle)
        {
            playerStagesLeft--;

            if(playerStagesLeft <= 0)
            {
                OnLevelWon.Invoke(); 
                ActivGameMenu(PlayerStatus.gameWon);
            }
        }

        private void ActivGameMenu(PlayerStatus status)
        {
            panelPause.SetActive(true);
            menuCurrentPoints.text = "current points: " + _currentPoints.ToString();

            switch (status)
            {
                case PlayerStatus.gamePause:          
                    buttonUnPause.gameObject.SetActive(true);
                    break;

                case PlayerStatus.gameLost:
                    UpdatePlayerPoints();
                    buttonRestart.gameObject.SetActive(true);
                    break;

                case PlayerStatus.gameWon:
                    UpdatePlayerPoints();
                    buttonNext.gameObject.SetActive(true);
                    break;

                default:
                    Debug.LogWarning("Incorrect status");
                    break;
            }
        }

        public void DeActivMainMenu()
        { 
            buttonStart.gameObject.SetActive(false);
            buttonNext.gameObject.SetActive(false);
            buttonRestart.gameObject.SetActive(false);
            buttonUnPause.gameObject.SetActive(false);

            panelPause.SetActive(false);
        }

        private void UpdatePlayerPoints()
        {
            if(currentPoints > recordPoints)
            {
                menuBestPoints.text = "Record: " + currentPoints.ToString();
                PlayerPrefs.SetInt("Record", currentPoints);
            } 
        }
    }
}