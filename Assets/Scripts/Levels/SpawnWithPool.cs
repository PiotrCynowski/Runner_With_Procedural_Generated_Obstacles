using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolSpawner
{
    public class SpawnWithPool
    {
        private readonly bool collectionChecks = true;
        private readonly int maxPoolSize = 25;

        private Transform elementsContainer;

        private Vector3 startPosition;

        private readonly Dictionary<int, ObjectPool<GameObject>> poolObjList = new();
        private readonly Dictionary<int, ObjectPool<GameObject>> poolObstList = new();

        private Dictionary<GameObject, int> activeObjects = new();
        private Dictionary<GameObject, int> activeObstacles = new();

        #region add obj to pool
        public void AddPoolForGameObject(GameObject toSpawn, int id)
        {
            ObjectPool<GameObject> pool = new(() =>
            {
                var obj = GameObject.Instantiate(toSpawn, Vector3.zero, Quaternion.identity, elementsContainer);
                return obj;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObjList.Add(id, pool);
        }

        public void AddPoolForSingleCubeWithMesh(Mesh[] toSpawn, Material mat, Vector3 collSize, int addLayerToObst, int id)
        {
            ObjectPool<GameObject> pool = new(() =>
            {
                var obj = new GameObject();
                obj.transform.parent = elementsContainer;

                obj.AddComponent<MeshRenderer>().material = mat;
                obj.AddComponent<MeshFilter>().sharedMesh = toSpawn[Random.Range(0, toSpawn.Length)];

                BoxCollider collider = obj.AddComponent<BoxCollider>();
                collider.size = collSize;
                obj.layer = addLayerToObst;

                obj.name = "CubeObst";

                return obj;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObjList.Add(id, pool);
        }

        public void AddPoolForObstMesh(Mesh toSpawn, Material mat, Vector3 collSize, int addLayerToObst, int id)
        {
            ObjectPool<GameObject> pool = new(() =>
            {
                var obj = new GameObject();
                obj.transform.parent = elementsContainer;

                obj.AddComponent<MeshRenderer>().material = mat;
                obj.AddComponent<MeshFilter>().sharedMesh = toSpawn;

                BoxCollider collider = obj.AddComponent<BoxCollider>();
                collider.size = collSize;
                obj.layer = addLayerToObst;

                obj.name = "Obst";

                return obj;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObstList.Add(id, pool);
        }

        public void AddElementsContainer()
        {
            if (elementsContainer == null)
            {
                elementsContainer = new GameObject().GetComponent<Transform>();
                elementsContainer.name = "PoolElementsContainer";
            }
        }
        #endregion

        #region release pools
        public void ReleaseObjPools()
        {
            foreach (GameObject obj in activeObjects.Keys)
            {
                activeObjects.TryGetValue(obj, out int id);
                poolObjList[id].Release(obj);
                obj.transform.parent = elementsContainer;
            }

            activeObjects.Clear();
        }

        public void ReleaseObstPools()
        {
            foreach (GameObject obj in activeObstacles.Keys)
            {
                activeObstacles.TryGetValue(obj, out int id);
                poolObstList[id].Release(obj);
            }

            activeObstacles.Clear();
        }
        #endregion

        #region level operations
        public void SpawnObj(Vector3 pos, int id)
        {
            startPosition = pos;
            activeObjects.Add(poolObjList[id].Get(), id);
        }

        public void SpawnObst(Vector3 pos, int id)
        {
            startPosition = pos;
            activeObstacles.Add(poolObstList[id].Get(), id);
        }

        public void ReAttachObj(GameObject obj)
        {
            obj.transform.parent = elementsContainer.transform;
        }

        public void ThisObjReleased(GameObject obj, int id)
        {
            poolObjList[id].Release(obj);
            activeObjects.Remove(obj);
        }
        public void ThisObstReleased(GameObject obj, int id)
        {
            poolObstList[id].Release(obj);
            activeObstacles.Remove(obj);
        }
        #endregion

        #region poolOperations
        private void OnReturnedToPool(GameObject system)
        {         
            system.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(GameObject system)
        {
            system.transform.position = startPosition;
            system.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(GameObject system)
        {
            GameObject.Destroy(system.gameObject);
        }
        #endregion  
    }
}