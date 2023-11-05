using UnityEngine;

[CreateAssetMenu(fileName = "LVL", menuName = "MissionGenerator/Add Level")]
public class LevelSetup : ScriptableObject
{
    [Header("Settings Map Generate")]
    public float distanceBetweenStages;
    public int numOfStages;
    public int horizontTotalCubesNumber;

    [Header("Settings Collectibles Generate")]
    public int numOfCollectObstPerStage;
    public int numOfCollectPointsPerStage;
}