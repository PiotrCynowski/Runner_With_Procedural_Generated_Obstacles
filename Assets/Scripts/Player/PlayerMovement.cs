using UnityEngine;
using GameUI;
using GameInput;

namespace Player {

    public class PlayerMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float forwardSpeed = 5f;
        [SerializeField] private float sideMoveSensitivity = 0.01f;
        [SerializeField] private float sideLimit = 5f;

        [Header("Reference")]
        [SerializeField] private Animator playerAnim;

        private Vector3 startPos;
        private Vector3 mousePos;
        private float newPosX, clampedPosX;
        private float xNegLim, xPosLim;

        private bool isLMB;
        private bool isMoving = false;

        private PlayerAnim playerAnimation;


        private void Awake()
        {
            xNegLim = transform.position.x - sideLimit;
            xPosLim = transform.position.x + sideLimit;

            startPos = transform.position;
        }

        private void Start()
        {
            playerAnimation = new();
        }

        private void OnEnable()
        {
            PlayerInteraction.onGameOver += () => MovingSwitch(false); 

            InputManager.onPlayerEscButton += () => MovingSwitch(false);

            GameStatsManager.OnPlayerStartLevel += () => MovingSwitch(true);
            GameStatsManager.OnPlayerReloadLevel += PlayerPosReset;

            GameStatsManager.OnLevelWon += () => MovingSwitch(false);
        }

        private void OnDisable()
        {
            PlayerInteraction.onGameOver -= () => MovingSwitch(false);

            InputManager.onPlayerEscButton -= () => MovingSwitch(false);

            GameStatsManager.OnPlayerStartLevel -= () => MovingSwitch(true);
            GameStatsManager.OnPlayerReloadLevel -= PlayerPosReset;

            GameStatsManager.OnLevelWon -= () => MovingSwitch(false);
        }

        private void Update()
        {
            if (!isMoving)
            {
                return;
            }

            #region side movement
            if (isLMB)
            {
                newPosX = transform.position.x + (mousePos.x * sideMoveSensitivity);
                clampedPosX = Mathf.Clamp(newPosX, xNegLim, xPosLim);
                transform.position = new Vector3(clampedPosX, transform.position.y, transform.position.z);
            }
            #endregion

            #region animation
            playerAnim.SetFloat("Blend", playerAnimation.AnimUpdate(mousePos.x, isLMB));
            #endregion

            #region forward movement
            transform.Translate(forwardSpeed * Time.deltaTime * Vector3.forward);
            #endregion
        }

        private void MovingSwitch(bool _isMoving)
        {
            isMoving = _isMoving;

            playerAnim.SetBool("Anim", _isMoving);
        }

        private void PlayerPosReset()
        {
            transform.position = startPos;
        }

        #region receive input
        public void ReceiveInput(Vector2 _mousePos)
        {
            mousePos = _mousePos;
        }

        public void ReceiveLMB(bool isClicked)
        {
            isLMB = isClicked;
        }
        #endregion
    }
}