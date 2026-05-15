using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Player
{
    public sealed class PlayerLook : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float _lookSensitivity = 0.12f;
        [SerializeField] private float _minPitch = -85f;
        [SerializeField] private float _maxPitch = 85f;
        [SerializeField] private bool _lockCursorOnStart = true;

        private InputAction _lookAction;
        private float _pitch;

        private void Awake()
        {
            CacheInputActions();

            if (_cameraPivot == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerLook)} on '{gameObject.name}' is missing {nameof(_cameraPivot)}.",
                    this);
            }
        }

        private void OnEnable()
        {
            if (_lockCursorOnStart)
            {
                ApplyPlayModeCursor();
            }
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            if (!_lockCursorOnStart)
            {
                return;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (_lookAction == null)
            {
                return;
            }

            Vector2 lookInput = _lookAction.ReadValue<Vector2>();
            float yaw = lookInput.x * _lookSensitivity;
            float pitchDelta = -lookInput.y * _lookSensitivity;

            transform.Rotate(Vector3.up, yaw, Space.World);

            _pitch = Mathf.Clamp(_pitch + pitchDelta, _minPitch, _maxPitch);
            if (_cameraPivot != null)
            {
                _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void ApplyPlayModeCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void CacheInputActions()
        {
            if (!PlayerInputActionsLoader.TryEnsureLoaded(ref _controls, this, nameof(PlayerLook)))
            {
                return;
            }

            InputActionMap playerMap = _controls.FindActionMap("Player", throwIfNotFound: false);
            if (playerMap == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerLook)} on '{gameObject.name}' could not find action map 'Player'.",
                    this);
                return;
            }

            _lookAction = playerMap.FindAction("Look", throwIfNotFound: false);
            if (_lookAction == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerLook)} on '{gameObject.name}' could not find action 'Look'.",
                    this);
            }
        }
    }
}
