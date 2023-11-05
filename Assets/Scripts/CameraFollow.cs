using UnityEngine;
using Player;

namespace GameplayCamera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private float smoothSpeed = 1;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 addObstacle;

        private bool keepTarget = true;

        private Vector3 desiredPosition;
        private Vector3 smoothedPosition;

        private void Start()
        {
            if(target == null)
            {
                keepTarget = false;
                Debug.LogWarning("camera target is null");
            }
        }

        private void OnEnable()
        {
            PlayerInteraction.onCollectObst += ObstacleCollected;
            PlayerInteraction.onOutOfObstacle += ObstaclesDroped;
        }

        private void OnDisable()
        {
            PlayerInteraction.onCollectObst -= ObstacleCollected;
            PlayerInteraction.onOutOfObstacle += ObstaclesDroped;
        }

        private void LateUpdate()
        {
            if (keepTarget)
            {
                desiredPosition = target.position + offset;
                smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = new Vector3(offset.x, offset.y, smoothedPosition.z);     
            }
        }

        private void ObstacleCollected(GameObject obj = null)
        {
            offset += addObstacle;
        }

        private void ObstaclesDroped(int amount)
        {
            offset -= amount * addObstacle;
        }
    }
}
