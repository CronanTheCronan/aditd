using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _gravity = -20f;

        private CharacterController _characterController;
        private InputActionMap _playerMap;
        private InputAction _moveAction;
        private float _verticalVelocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            CacheInputActions();
        }

        private void OnEnable()
        {
            if (_playerMap != null)
            {
                _playerMap.Enable();
            }
        }

        private void OnDisable()
        {
            if (_playerMap != null)
            {
                _playerMap.Disable();
            }
        }

        private void Update()
        {
            if (_characterController == null || _moveAction == null)
            {
                return;
            }

            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            Vector3 move = (transform.right * moveInput.x) + (transform.forward * moveInput.y);
            move = Vector3.ClampMagnitude(move, 1f) * _moveSpeed;

            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += _gravity * Time.deltaTime;
            move.y = _verticalVelocity;

            _characterController.Move(move * Time.deltaTime);
        }

        private void CacheInputActions()
        {
            if (!PlayerInputActionsLoader.TryEnsureLoaded(ref _controls, this, nameof(PlayerController)))
            {
                return;
            }

            _playerMap = _controls.FindActionMap("Player", throwIfNotFound: false);
            if (_playerMap == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerController)} on '{gameObject.name}' could not find action map 'Player'.",
                    this);
                return;
            }

            _moveAction = _playerMap.FindAction("Move", throwIfNotFound: false);
            if (_moveAction == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerController)} on '{gameObject.name}' could not find action 'Move'.",
                    this);
            }
        }
    }
}
