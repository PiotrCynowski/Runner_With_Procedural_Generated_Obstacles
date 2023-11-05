using UnityEngine;
using Player;
using System;
using GameUI;

namespace GameInput
{
    public class InputManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMovement movement;

        private PlayerInputActions controls;
        private PlayerInputActions.PlayerActions playerActions;

        private Vector2 mouseInput;

        public static Action onPlayerEscButton;

        private void Awake()
        {
            controls = new PlayerInputActions();
            playerActions = controls.Player;

            playerActions.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();

            playerActions.LMB.performed += _ => movement.ReceiveLMB(true);
            playerActions.LMB.canceled += _ => movement.ReceiveLMB(false);

            playerActions.PauseMenu.performed += _ => EscapeButPerformed();
            GameStatsManager.OnPlayerStartLevel += EnableControlls;
        }

        private void Update()
        {
            movement.ReceiveInput(mouseInput);
        }

        #region enable/disable
        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
        #endregion

        private void EscapeButPerformed()
        {
            onPlayerEscButton.Invoke();
            controls.Disable();
        }

        private void EnableControlls()
        {
            controls.Enable();
        }
    }
}