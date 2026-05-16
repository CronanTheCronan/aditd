using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ADoorInsideTheDark.Player;

namespace ADoorInsideTheDark.Shadow
{
    public sealed class ShadowPerceptionController : MonoBehaviour
    {
        [Header("Wave 009A Input")]
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private string _playerActionMapName = "Player";
        [SerializeField] private string _shadowPerceptionActionName = "ShadowPerception";
        [SerializeField] private string _inputBindingNote = "Uses the ADITDControls Player/ShadowPerception action. Keep binding changes in the project Input Actions asset.";

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource _perceptionAudioSource;
        [SerializeField] private AudioClip _activationCue;
        [SerializeField] private AudioClip _deactivationCue;
        [SerializeField] private float _activationCueVolume = 0.16f;
        [SerializeField] private float _deactivationCueVolume = 0.12f;

        private readonly List<IShadowRevealable> _registeredRevealables = new();
        private InputAction _shadowPerceptionAction;
        private AudioClip _generatedActivationCue;
        private AudioClip _generatedDeactivationCue;

        public bool IsPerceptionActive { get; private set; }

        public event Action<bool> PerceptionStateChanged;

        private void Awake()
        {
            CacheInputAction();
            AutoAssignAudioSource();
        }

        private void OnEnable()
        {
            _shadowPerceptionAction?.Enable();
            RefreshPerceptionStateFromInput();
        }

        private void Update()
        {
            RefreshPerceptionStateFromInput();
        }

        private void RefreshPerceptionStateFromInput()
        {
            bool shouldBeActive = _shadowPerceptionAction != null && _shadowPerceptionAction.IsPressed();
            if (shouldBeActive == IsPerceptionActive)
            {
                return;
            }

            SetPerceptionActive(shouldBeActive);
        }

        private void OnDisable()
        {
            if (IsPerceptionActive)
            {
                SetPerceptionActive(false);
            }

            _shadowPerceptionAction?.Disable();
        }

        private void OnDestroy()
        {
            DestroyGeneratedClip(ref _generatedActivationCue);
            DestroyGeneratedClip(ref _generatedDeactivationCue);
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_inputBindingNote))
            {
                _inputBindingNote = "Uses the ADITDControls Player/ShadowPerception action. Keep binding changes in the project Input Actions asset.";
            }

            AutoAssignAudioSource();
        }

        public void RegisterRevealable(IShadowRevealable revealable)
        {
            if (revealable == null || _registeredRevealables.Contains(revealable))
            {
                return;
            }

            _registeredRevealables.Add(revealable);
            revealable.SetShadowPerceptionActive(IsPerceptionActive);
        }

        public void UnregisterRevealable(IShadowRevealable revealable)
        {
            if (revealable == null)
            {
                return;
            }

            _registeredRevealables.Remove(revealable);
        }

        private void SetPerceptionActive(bool isActive)
        {
            IsPerceptionActive = isActive;

            for (int i = 0; i < _registeredRevealables.Count; i++)
            {
                _registeredRevealables[i]?.SetShadowPerceptionActive(isActive);
            }

            PlayPerceptionCue(isActive);
            PerceptionStateChanged?.Invoke(isActive);
        }

        private void CacheInputAction()
        {
            if (!PlayerInputActionsLoader.TryEnsureLoaded(ref _controls, this, nameof(ShadowPerceptionController)))
            {
                return;
            }

            InputActionMap playerMap = _controls.FindActionMap(_playerActionMapName, throwIfNotFound: false);
            if (playerMap == null)
            {
                Debug.LogWarning(
                    $"{nameof(ShadowPerceptionController)} on '{gameObject.name}' could not find action map '{_playerActionMapName}'.",
                    this);
                return;
            }

            _shadowPerceptionAction = playerMap.FindAction(_shadowPerceptionActionName, throwIfNotFound: false);
            if (_shadowPerceptionAction == null)
            {
                Debug.LogWarning(
                    $"{nameof(ShadowPerceptionController)} on '{gameObject.name}' could not find action '{_shadowPerceptionActionName}'.",
                    this);
            }
        }

        private void AutoAssignAudioSource()
        {
            if (_perceptionAudioSource == null)
            {
                _perceptionAudioSource = GetComponent<AudioSource>();
            }
        }

        private void PlayPerceptionCue(bool isActive)
        {
            if (_perceptionAudioSource == null)
            {
                return;
            }

            AudioClip clip = isActive ? GetActivationCue() : GetDeactivationCue();
            if (clip == null)
            {
                return;
            }

            _perceptionAudioSource.PlayOneShot(
                clip,
                isActive ? _activationCueVolume : _deactivationCueVolume);
        }

        private AudioClip GetActivationCue()
        {
            if (_activationCue != null)
            {
                return _activationCue;
            }

            _generatedActivationCue ??= ShadowAudioClipFactory.CreatePerceptionEnterCue();
            return _generatedActivationCue;
        }

        private AudioClip GetDeactivationCue()
        {
            if (_deactivationCue != null)
            {
                return _deactivationCue;
            }

            _generatedDeactivationCue ??= ShadowAudioClipFactory.CreatePerceptionExitCue();
            return _generatedDeactivationCue;
        }

        private static void DestroyGeneratedClip(ref AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            Destroy(clip);
            clip = null;
        }
    }
}
