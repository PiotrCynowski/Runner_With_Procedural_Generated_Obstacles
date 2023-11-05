using System;
using System.Collections.Generic;
using UnityEngine;
using GameUI;

namespace Player
{
    public class PlayerObstaclesStand : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        private Stack<GameObject> objAttached = new();

        [Header("Settings")]
        [SerializeField] private float spaceBetObst;
        private Vector3 addingObstTransform;

        public static Action<GameObject> detachObj;

        private void OnEnable()
        {
            PlayerInteraction.onCollectObst += ObstacleCollected;
            PlayerInteraction.onCollideObstAmount += ObstacleLeft;
            PlayerInteraction.onOutOfObstacle += PlayerStandDrop;

            GameStatsManager.OnPlayerReloadLevel += ResetPlayerStand;
        }

        private void OnDisable()
        {
            PlayerInteraction.onCollectObst -= ObstacleCollected;
            PlayerInteraction.onCollideObstAmount -= ObstacleLeft;
            PlayerInteraction.onOutOfObstacle -= PlayerStandDrop;

            GameStatsManager.OnPlayerReloadLevel -= ResetPlayerStand;
        }

        private void Awake()
        {
            addingObstTransform = new Vector3(0,spaceBetObst,0);
        }

        private void ObstacleCollected(GameObject obj)
        {      
            obj.transform.position = transform.position;
            playerTransform.position += addingObstTransform;
            obj.transform.parent = playerTransform.gameObject.transform;
           
            objAttached.Push(obj);
        }

        private void ObstacleLeft(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                detachObj(objAttached.Pop());
            }
        }

        private void PlayerStandDrop(int howMany)
        {
            playerTransform.position -= howMany * addingObstTransform;
        }

        private void ResetPlayerStand()
        {
            objAttached.Clear();
            playerTransform.transform.localPosition = Vector3.zero;
        }
    }
}