using UnityEngine;
using LevelGenerate;
using GameUI;
using Player;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelSetup[] levels;

    [Header("Global Setup")]
    [SerializeField] private GameObject collectPointsObj;
    [SerializeField] private MeshFilter groundMesh;
    [Header("Texture References")]
    [SerializeField] private Material obstacleMaterial;
    [SerializeField] private Material collectObstacleMaterial;

    private LevelGenerator Level;
    private int currentLevel;

    private void OnEnable()
    {
        GameStatsManager.OnPlayerReloadLevel += LoadLevel;
        GameStatsManager.OnLevelWon += LevelWin;
        PlayerInteraction.onGameOver += ResetLevel;

        GameStatsManager.OnPlayerStartLevel += () => GameStatsManager.Instance.playerStagesLeft = levels[currentLevel].numOfStages;
    }

    private void OnDisable()
    {
        GameStatsManager.OnPlayerReloadLevel -= LoadLevel;
        GameStatsManager.OnLevelWon -= LevelWin;
        PlayerInteraction.onGameOver -= ResetLevel;

        GameStatsManager.OnPlayerStartLevel -= () => GameStatsManager.Instance.playerStagesLeft = levels[currentLevel].numOfStages;
    }

    private void Start()
    {
        Level = new();
        Level.Init(collectPointsObj, collectObstacleMaterial, obstacleMaterial, groundMesh);

        currentLevel = 0;
    }

    private void LevelWin()
    {
        currentLevel++;

        if (currentLevel >= levels.Length)
        {
            currentLevel = 0;
        }

        Level.ResetLevel();
    }

    private void LoadLevel()
    {
        Level.PrepareLevel(levels[currentLevel]);

        Level.GenerateLevel(levels[currentLevel]);
    }

    private void ResetLevel()
    {
        Level.ResetLevel();
    }
}
