using System;
using System.Collections;
using UnityEngine;
using GameUI;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Layers")]
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask collectObstLayer;
        [SerializeField] private LayerMask collectPointsLayer;

        public static Action<GameObject> onCollectObst;
        public static Action<GameObject> onCollectPoint;
        public static Action<int> onCollideObstAmount;
        public static Action<int> onOutOfObstacle;

        public static Action onGameOver;

        [Header("RayCheck")]
        [SerializeField] private Transform rayCheckPos;
        [SerializeField] private Vector3 playerStandDimensions;
        private Vector3 leftRay, rightRay;
        private Vector3? standVectorAdd;
        private int collectedObst, hitObst;
        private int lCheck, rCheck;
        private bool isHitObstacle;

        public delegate void RemoveCollectPoint(GameObject obj);
        public static RemoveCollectPoint PointRemove;

        private void Start()
        {
            if(playerStandDimensions.y <= 0)
            {
                Debug.LogWarning("playerStandDimensions Y can't be 0");
                return;
            }

            standVectorAdd = new Vector3(0, playerStandDimensions.y, 0);
        }

        private void OnEnable()
        {
            GameStatsManager.OnPlayerReloadLevel += ResetPlayer;
        }

        private void OnDisable()
        {
            GameStatsManager.OnPlayerReloadLevel -= ResetPlayer;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (obstacleLayer == (obstacleLayer | (1 << collision.gameObject.layer)))
            {
                if (isHitObstacle)
                {
                    return;
                }

                isHitObstacle = true;
                StartCoroutine(CastRayCheck(rayCheckPos.position));
                return;
            }

            if (collectObstLayer == (collectObstLayer | (1 << collision.gameObject.layer)))
            {
                if (isHitObstacle)
                {
                    return;
                }

                collectedObst++;
                onCollectObst?.Invoke(collision.gameObject);
                return;
            }

            if (collectPointsLayer == (collectPointsLayer | (1 << collision.gameObject.layer)))
            {
                PointRemove.Invoke(collision.gameObject);
                GameStatsManager.Instance.currentPoints++;
                return;
            }
        }

        private IEnumerator CastRayCheck(Vector3 rayPosStart)
        {
            if (playerStandDimensions.x <= 0 || (playerStandDimensions.z <= 0) || !standVectorAdd.HasValue)
            {
                Debug.LogWarning("playerStandDimension can't be 0");
                yield break;
            }

            leftRay = new(rayPosStart.x - (playerStandDimensions.x * 0.5f), rayPosStart.y, rayPosStart.z);
            rightRay = new(rayPosStart.x + (playerStandDimensions.x * 0.5f), rayPosStart.y, rayPosStart.z);

            lCheck = 0;
            rCheck = 0;

            RaycastHit hit;

            while (Physics.Raycast(leftRay, transform.forward, out hit, 2, obstacleLayer))
            {
                leftRay += standVectorAdd.Value;
                lCheck++;
            }

            while (Physics.Raycast(rightRay, transform.forward, out hit, 2, obstacleLayer))
            {
                rightRay += standVectorAdd.Value;
                rCheck++;
            }

            hitObst = lCheck > rCheck ? lCheck : rCheck;
            collectedObst -= hitObst;

            if (collectedObst < 0)
            {
                onGameOver.Invoke();
                yield break;
            }

            onCollideObstAmount.Invoke(hitObst);

            yield return new WaitUntil(() => transform.position.z > (rayPosStart.z + (2 * playerStandDimensions.z)));

            isHitObstacle = false;
            onOutOfObstacle.Invoke(hitObst);
        }

        private void ResetPlayer()
        {
            isHitObstacle = false;
            hitObst = 0;
            collectedObst = 0;
        }
    }
}