using UnityEngine;
using PoolSpawner;
using Player;
using ProceduralLevelGenerate;

namespace LevelGenerate
{
    public class LevelGenerator
    {
        //Settings Obstacles
        private const float horizontalGapSize = 0.2f;
        private float verticalGapSize;
        private float cubeSize;
        private Material obstaclesMat;
        private MeshFilter groundMesh;

        private int previousHighestObstLvl = 0;

        //Settings Layers
        private readonly int obstaclesLayerNumber = 6;
        private readonly int collectObstaclesLayerNumber = 7;

        //Math
        private Vector3 obstacleMainPos;
        private float obstMiddlePos;
        private int obstMiddlePosInt, rndStartPos, rndEndPos;
        private int collectObstIndex, collectPoitnsIndex;

        private ProceduralMeshGenerate generateLevelMesh;
        private SpawnWithPool spawner;
        private System.Random rnd;

        #region main commands
        public void Init(GameObject pointsPrefab, Material obstaclesCollectMat, Material _obstaclesMat, MeshFilter _groundMesh)
        {
            rnd = new System.Random();
            generateLevelMesh = new();

            (float _cubeSize, float _spaceBetweenCubes) = generateLevelMesh.getCubeData();
            cubeSize = _cubeSize;
            verticalGapSize = _spaceBetweenCubes;

            spawner = new();
            spawner.AddElementsContainer();

            PrepareSpawnerCollectObst(obstaclesCollectMat); //pool add obstacles for player stand

            PrepareSpawnerCollectPoints(pointsPrefab); //pool add points to collect

            obstaclesMat = _obstaclesMat;
            groundMesh = _groundMesh;
        }

        public void PrepareLevel(LevelSetup lvl)
        {
            PrepareSpawnData(lvl.distanceBetweenStages, lvl.horizontTotalCubesNumber);

            if(lvl.numOfStages > previousHighestObstLvl)
            {
                PrepareSpawnerObstacles(lvl.numOfStages, previousHighestObstLvl, obstaclesMat); //pool for obstacles

                previousHighestObstLvl = lvl.numOfStages;
            }
        }

        public void GenerateLevel(LevelSetup lvl)
        {
            int diffMax = 0; //every next obstacle should be higher, till the max possible height
            int diffMin;

            for (int i = 0; i <= lvl.numOfStages; i++)
            {
                diffMax = diffMax >= lvl.numOfStages ? lvl.numOfStages : diffMax + 1;
                diffMin = diffMax >= 3 ? diffMax - 3 : 0;

                obstacleMainPos = new Vector3(0, 0, 0 + lvl.distanceBetweenStages * i);

                if (i < lvl.numOfStages)
                {
                    GenerateCollectObst(obstacleMainPos, lvl.numOfCollectObstPerStage);
                    GenerateCollectPoints(obstacleMainPos, lvl.numOfCollectPointsPerStage);
                }

                if (i > 0)
                    GenerateObstacle(obstacleMainPos, diffMin, diffMax, lvl.horizontTotalCubesNumber);
            }

            groundMesh.sharedMesh = generateLevelMesh.GeneratePlane(Vector3.zero,
              new Vector3(0, 0, 0 + (lvl.distanceBetweenStages * (lvl.numOfStages + 1))),
              cubeSize * lvl.horizontTotalCubesNumber + (horizontalGapSize * lvl.horizontTotalCubesNumber));
        }

        public void ResetLevel()
        {
            spawner.ReleaseObjPools();

            spawner.ReleaseObstPools();
        }
        #endregion

        #region prepare spawner elements
        public void PrepareSpawnData(float distanceBetweenStages, int horizontTotalCubesNumber)
        {
            obstMiddlePos = 0.5f * (cubeSize + horizontalGapSize) * (horizontTotalCubesNumber - 1);
            obstMiddlePosInt = (int)obstMiddlePos;

            rndStartPos = (int)(0.25f * distanceBetweenStages);
            rndEndPos = (int)(0.75f * distanceBetweenStages);
        }

        private void PrepareSpawnerCollectObst(Material collectObstacleMaterial)
        {
            collectObstIndex = 1;
            Mesh[] meshArray = generateLevelMesh.GenerateArrayCubes();

            spawner.AddPoolForSingleCubeWithMesh(meshArray, collectObstacleMaterial, cubeSize * Vector3.one, collectObstaclesLayerNumber, collectObstIndex);

            PlayerObstaclesStand.detachObj = spawner.ReAttachObj;
        }
        private void PrepareSpawnerCollectPoints(GameObject collectPointsGameobject)
        {
            collectPoitnsIndex = 2;
            spawner.AddPoolForGameObject(collectPointsGameobject, collectPoitnsIndex);

            PlayerInteraction.PointRemove += RemoveCollectPointsObj;
        }

        private void RemoveCollectPointsObj(GameObject obj)
        {
            spawner.ThisObjReleased(obj, collectPoitnsIndex);
        }

        private void PrepareSpawnerObstacles(int stages, int lastGenerated, Material obstacleMaterial)
        {
            for (int i = lastGenerated; i < stages; i++)
            {
                Mesh mesh = generateLevelMesh.GenerateCubesStack(i);
                Vector3 colliderDim = new(cubeSize + horizontalGapSize, i * cubeSize + 0.5f * verticalGapSize, cubeSize);

                spawner.AddPoolForObstMesh(mesh, obstacleMaterial, colliderDim, obstaclesLayerNumber, i);
            }
        }
        #endregion

        #region generate level elements
        private void GenerateObstacle(Vector3 levelPos, int diffMin, int diffMax, int horizontTotalCubesNumber)
        {
            for (int i = 0; i < horizontTotalCubesNumber; i++)
            {
                spawner.SpawnObst(levelPos + new Vector3(i * (cubeSize + horizontalGapSize) - obstMiddlePos, 0, 0), rnd.Next(diffMin, diffMax));
            }
        }

        private void GenerateCollectObst(Vector3 levelPos, int numOfCollectObstPerStage)
        {
            int distFromStart = rnd.Next(rndStartPos, rndEndPos);
            int posX = rnd.Next(-obstMiddlePosInt, obstMiddlePosInt+1);

            for (int i = 0; i < numOfCollectObstPerStage; i++)
            {
                spawner.SpawnObj(levelPos + new Vector3(posX, 0.5f, distFromStart + (i * 1)), collectObstIndex);
            }
        }

        private void GenerateCollectPoints(Vector3 levelPos, int numOfCollectPointsPerStage)
        {
            for (int i = 0; i < numOfCollectPointsPerStage; i++)
            {
                spawner.SpawnObj(levelPos + new Vector3(rnd.Next(-obstMiddlePosInt, obstMiddlePosInt+1), 0.5f, rnd.Next(rndStartPos, rndEndPos)), collectPoitnsIndex);
            }
        }
        #endregion
    }
}