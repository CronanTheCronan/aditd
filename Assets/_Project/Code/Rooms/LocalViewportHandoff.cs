using System;
using System.Collections;
using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.Shadow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Rooms
{
    public sealed class LocalViewportHandoff : MonoBehaviour, IShadowPerceptionSource
    {
        private enum HandoffState
        {
            EgoActive,
            TransitioningToShadow,
            ShadowActive,
            TransitioningToEgo
        }

        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private GameObject _egoProxyRoot;
        [SerializeField] private GameObject _shadowProxyRoot;
        [SerializeField] private Transform _initialShadowAnchor;
        [SerializeField] private AudioSource _fractureAudioSource;
        [SerializeField] private AudioClip _fractureCue;
        [SerializeField] private float _transitionDelaySeconds = 0.2f;
        [SerializeField] private float _fractureCueVolume = 0.18f;

        private HandoffState _state = HandoffState.EgoActive;
        private PlayerContext _player;
        private PlayerController _playerController;
        private CharacterController _characterController;
        private InputAction _shadowPerceptionAction;
        private Vector3 _egoAnchorPosition;
        private Quaternion _egoAnchorRotation;
        private Vector3 _shadowAnchorPosition;
        private Quaternion _shadowAnchorRotation;
        private AudioClip _generatedFractureCue;
        private Coroutine _transitionCoroutine;

        public bool IsPerceptionActive => _state is HandoffState.ShadowActive or HandoffState.TransitioningToShadow;

        public event Action<bool> PerceptionStateChanged;

        private void Awake()
        {
            CachePlayerReferences();
            CacheInputAction();
            CacheShadowAnchorFromInitial();
            EnsureFractureCue();
        }

        private void Start()
        {
            ApplyStableStateVisuals();
        }

        private void OnEnable()
        {
            _shadowPerceptionAction?.Enable();
        }

        private void OnDisable()
        {
            _shadowPerceptionAction?.Disable();

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            if (_playerController != null && !_playerController.enabled)
            {
                _playerController.enabled = true;
            }

            if (_characterController != null && !_characterController.enabled)
            {
                _characterController.enabled = true;
            }
        }

        private void OnDestroy()
        {
            if (_generatedFractureCue != null)
            {
                Destroy(_generatedFractureCue);
                _generatedFractureCue = null;
            }
        }

        private void Update()
        {
            if (_shadowPerceptionAction == null || _player == null)
            {
                return;
            }

            if (_state == HandoffState.EgoActive && _shadowPerceptionAction.WasPressedThisFrame())
            {
                BeginTransitionToShadow();
            }
            else if (_state == HandoffState.ShadowActive && _shadowPerceptionAction.WasPressedThisFrame())
            {
                BeginTransitionToEgo();
            }
        }

        private void BeginTransitionToShadow()
        {
            if (_transitionCoroutine != null)
            {
                return;
            }

            _state = HandoffState.TransitioningToShadow;
            PerceptionStateChanged?.Invoke(true);
            LockPlayer();

            _egoAnchorPosition = _player.transform.position;
            _egoAnchorRotation = _player.transform.rotation;

            SetProxyActive(_egoProxyRoot, true, _egoAnchorPosition, _egoAnchorRotation);
            SetProxyActive(_shadowProxyRoot, false, _shadowAnchorPosition, _shadowAnchorRotation);

            TeleportPlayer(_shadowAnchorPosition, _shadowAnchorRotation);
            PlayFractureCue();

            _transitionCoroutine = StartCoroutine(CompleteTransitionAfterDelay(HandoffState.ShadowActive));
        }

        private void BeginTransitionToEgo()
        {
            if (_transitionCoroutine != null)
            {
                return;
            }

            _state = HandoffState.TransitioningToEgo;
            PerceptionStateChanged?.Invoke(false);
            LockPlayer();

            _shadowAnchorPosition = _player.transform.position;
            _shadowAnchorRotation = _player.transform.rotation;

            SetProxyActive(_shadowProxyRoot, true, _shadowAnchorPosition, _shadowAnchorRotation);
            SetProxyActive(_egoProxyRoot, false, _egoAnchorPosition, _egoAnchorRotation);

            TeleportPlayer(_egoAnchorPosition, _egoAnchorRotation);
            PlayFractureCue();

            _transitionCoroutine = StartCoroutine(CompleteTransitionAfterDelay(HandoffState.EgoActive));
        }

        private IEnumerator CompleteTransitionAfterDelay(HandoffState nextState)
        {
            yield return new WaitForSeconds(_transitionDelaySeconds);

            _state = nextState;
            UnlockPlayer();
            ApplyStableStateVisuals();
            _transitionCoroutine = null;
        }

        private void ApplyStableStateVisuals()
        {
            switch (_state)
            {
                case HandoffState.EgoActive:
                    SetProxyActive(_egoProxyRoot, false, _egoAnchorPosition, _egoAnchorRotation);
                    SetProxyActive(_shadowProxyRoot, true, _shadowAnchorPosition, _shadowAnchorRotation);
                    break;
                case HandoffState.ShadowActive:
                    SetProxyActive(_egoProxyRoot, true, _egoAnchorPosition, _egoAnchorRotation);
                    SetProxyActive(_shadowProxyRoot, false, _shadowAnchorPosition, _shadowAnchorRotation);
                    break;
            }
        }

        private void CachePlayerReferences()
        {
            _player = FindAnyObjectByType<PlayerContext>();
            if (_player == null)
            {
                Debug.LogWarning($"{nameof(LocalViewportHandoff)} on '{gameObject.name}' could not find {nameof(PlayerContext)}.", this);
                return;
            }

            _playerController = _player.GetComponent<PlayerController>();
            if (_playerController == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' could not find {nameof(PlayerController)} on the player.",
                    this);
            }

            _characterController = _player.GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' could not find {nameof(CharacterController)} on the player.",
                    this);
            }
        }

        private void CacheInputAction()
        {
            if (_controls == null)
            {
                _controls = Resources.Load<InputActionAsset>("ADITDControls");
                if (_controls == null)
                {
                    Debug.LogWarning(
                        $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' is missing {nameof(InputActionAsset)} assignment and could not load 'ADITDControls' from Resources.",
                        this);
                    return;
                }
            }

            InputActionMap playerMap = _controls.FindActionMap("Player", throwIfNotFound: false);
            if (playerMap == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' could not find action map 'Player'.",
                    this);
                return;
            }

            _shadowPerceptionAction = playerMap.FindAction("ShadowPerception", throwIfNotFound: false);
            if (_shadowPerceptionAction == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' could not find action 'ShadowPerception'.",
                    this);
            }
        }

        private void CacheShadowAnchorFromInitial()
        {
            if (_initialShadowAnchor == null)
            {
                Debug.LogWarning(
                    $"{nameof(LocalViewportHandoff)} on '{gameObject.name}' is missing {nameof(_initialShadowAnchor)}.",
                    this);
                return;
            }

            _shadowAnchorPosition = _initialShadowAnchor.position;
            _shadowAnchorRotation = _initialShadowAnchor.rotation;
        }

        private void EnsureFractureCue()
        {
            if (_fractureCue != null)
            {
                return;
            }

            _generatedFractureCue = ShadowAudioClipFactory.CreateDestabilizedGuidanceCue();
            _fractureCue = _generatedFractureCue;
        }

        private void LockPlayer()
        {
            if (_playerController != null)
            {
                _playerController.enabled = false;
            }

            if (_characterController != null)
            {
                _characterController.enabled = false;
            }
        }

        private void UnlockPlayer()
        {
            if (_characterController != null)
            {
                _characterController.enabled = true;
            }

            if (_playerController != null)
            {
                _playerController.enabled = true;
            }
        }

        private void TeleportPlayer(Vector3 position, Quaternion rotation)
        {
            if (_player == null)
            {
                return;
            }

            if (_characterController != null)
            {
                _characterController.enabled = false;
            }

            _player.transform.SetPositionAndRotation(position, rotation);
        }

        private static void SetProxyActive(GameObject proxyRoot, bool isActive, Vector3 position, Quaternion rotation)
        {
            if (proxyRoot == null)
            {
                return;
            }

            proxyRoot.transform.SetPositionAndRotation(position, rotation);
            proxyRoot.SetActive(isActive);
        }

        private void PlayFractureCue()
        {
            if (_fractureAudioSource == null || _fractureCue == null)
            {
                return;
            }

            _fractureAudioSource.PlayOneShot(_fractureCue, _fractureCueVolume);
        }
    }
}
