using ADoorInsideTheDark.Rooms;
using UnityEngine;

namespace ADoorInsideTheDark.Shadow
{
    public sealed class ShadowRevealable : MonoBehaviour, IShadowRevealable
    {
        [SerializeField] private MonoBehaviour _perceptionSource;
        [SerializeField] private GameObject[] _objectsToToggle;
        [SerializeField] private Renderer[] _renderersToToggleVisibility;
        [SerializeField] private Renderer[] _renderersToTint;
        [SerializeField] private Color _activeTint = new(0.44f, 0.81f, 0.89f, 1f);

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource _revealAudioSource;
        [SerializeField] private AudioClip _revealLoopClip;
        [SerializeField] private float _revealLoopVolume = 0.14f;
        [SerializeField] private float _revealFadeSpeed = 2.75f;

        private Color[] _cachedInactiveColors;
        private bool _shadowPerceptionActive;
        private bool _forceReveal;
        private bool _revealAllowed = true;
        private bool _hasLoggedMissingControllerWarning;
        private bool _isCurrentlyRevealed;
        private AudioClip _generatedRevealLoopClip;

        private IShadowPerceptionSource PerceptionSource => _perceptionSource as IShadowPerceptionSource;

        private void Awake()
        {
            AutoAssignController(logWarningIfMissing: true);
            AutoAssignRevealAudioSource();
            CacheInactiveColors();
            RefreshVisualState();
        }

        private void OnEnable()
        {
            AutoAssignController(logWarningIfMissing: true);
            AutoAssignRevealAudioSource();

            if (PerceptionSource != null)
            {
                PerceptionSource.PerceptionStateChanged += SetShadowPerceptionActive;
                SetShadowPerceptionActive(PerceptionSource.IsPerceptionActive);
            }

            RefreshVisualState();
        }

        private void Update()
        {
            UpdateRevealAudioFade();
        }

        private void OnDisable()
        {
            if (PerceptionSource != null)
            {
                PerceptionSource.PerceptionStateChanged -= SetShadowPerceptionActive;
            }

            StopRevealAudioImmediately();
        }

        private void OnDestroy()
        {
            if (_generatedRevealLoopClip != null)
            {
                Destroy(_generatedRevealLoopClip);
                _generatedRevealLoopClip = null;
            }
        }

        private void OnValidate()
        {
            AutoAssignController(logWarningIfMissing: false);
            AutoAssignRevealAudioSource();
        }

        public void SetShadowPerceptionActive(bool isActive)
        {
            if (_shadowPerceptionActive == isActive)
            {
                return;
            }

            _shadowPerceptionActive = isActive;
            RefreshVisualState();
        }

        public void SetForceReveal(bool forceReveal)
        {
            if (_forceReveal == forceReveal)
            {
                return;
            }

            _forceReveal = forceReveal;
            RefreshVisualState();
        }

        public void SetRevealAllowed(bool revealAllowed)
        {
            if (_revealAllowed == revealAllowed)
            {
                return;
            }

            _revealAllowed = revealAllowed;
            RefreshVisualState();
        }

        private void AutoAssignController(bool logWarningIfMissing)
        {
            if (_perceptionSource != null && PerceptionSource != null)
            {
                _hasLoggedMissingControllerWarning = false;
                return;
            }

            LocalViewportHandoff handoff = FindAnyObjectByType<LocalViewportHandoff>();
            if (handoff != null)
            {
                _perceptionSource = handoff;
                _hasLoggedMissingControllerWarning = false;
                return;
            }

            ShadowPerceptionController holdController = FindAnyObjectByType<ShadowPerceptionController>();
            if (holdController != null)
            {
                _perceptionSource = holdController;
                _hasLoggedMissingControllerWarning = false;
                return;
            }

            if (!logWarningIfMissing || _hasLoggedMissingControllerWarning)
            {
                return;
            }

            Debug.LogWarning(
                $"[ShadowRevealable] '{name}' could not find an {nameof(IShadowPerceptionSource)} through the inspector or scene fallback. It will not receive Shadow perception state changes until one is assigned.",
                this);
            _hasLoggedMissingControllerWarning = true;
        }

        private void AutoAssignRevealAudioSource()
        {
            if (_revealAudioSource == null)
            {
                _revealAudioSource = GetComponent<AudioSource>();
            }
        }

        private void CacheInactiveColors()
        {
            if (_renderersToTint == null)
            {
                _cachedInactiveColors = System.Array.Empty<Color>();
                return;
            }

            _cachedInactiveColors = new Color[_renderersToTint.Length];
            for (int i = 0; i < _renderersToTint.Length; i++)
            {
                Renderer renderer = _renderersToTint[i];
                _cachedInactiveColors[i] = renderer != null && renderer.sharedMaterial != null
                    ? renderer.material.color
                    : Color.white;
            }
        }

        private void RefreshVisualState()
        {
            bool shouldReveal = _forceReveal || (_revealAllowed && _shadowPerceptionActive);
            UpdateRevealAudioState(shouldReveal);

            if (_objectsToToggle != null)
            {
                for (int i = 0; i < _objectsToToggle.Length; i++)
                {
                    GameObject toggleTarget = _objectsToToggle[i];
                    if (toggleTarget != null && toggleTarget.activeSelf != shouldReveal)
                    {
                        toggleTarget.SetActive(shouldReveal);
                    }
                }
            }

            if (_renderersToToggleVisibility != null)
            {
                for (int i = 0; i < _renderersToToggleVisibility.Length; i++)
                {
                    Renderer renderer = _renderersToToggleVisibility[i];
                    if (renderer != null)
                    {
                        renderer.enabled = shouldReveal;
                    }
                }
            }

            if (_renderersToTint == null || _cachedInactiveColors == null)
            {
                return;
            }

            for (int i = 0; i < _renderersToTint.Length; i++)
            {
                Renderer renderer = _renderersToTint[i];
                if (renderer == null)
                {
                    continue;
                }

                Color inactiveColor = i < _cachedInactiveColors.Length ? _cachedInactiveColors[i] : Color.white;
                renderer.material.color = shouldReveal ? _activeTint : inactiveColor;
            }
        }

        private void UpdateRevealAudioState(bool shouldReveal)
        {
            if (_isCurrentlyRevealed == shouldReveal)
            {
                return;
            }

            _isCurrentlyRevealed = shouldReveal;
            if (!shouldReveal || _revealAudioSource == null)
            {
                return;
            }

            AudioClip loopClip = GetRevealLoopClip();
            if (loopClip == null)
            {
                return;
            }

            if (_revealAudioSource.clip != loopClip)
            {
                _revealAudioSource.clip = loopClip;
            }

            if (!_revealAudioSource.isPlaying)
            {
                _revealAudioSource.volume = 0f;
                _revealAudioSource.loop = true;
                _revealAudioSource.Play();
            }
        }

        private void UpdateRevealAudioFade()
        {
            if (_revealAudioSource == null)
            {
                return;
            }

            float targetVolume = _isCurrentlyRevealed ? _revealLoopVolume : 0f;
            _revealAudioSource.volume = Mathf.MoveTowards(
                _revealAudioSource.volume,
                targetVolume,
                _revealFadeSpeed * Time.deltaTime);

            if (!_isCurrentlyRevealed && _revealAudioSource.isPlaying && _revealAudioSource.volume <= 0.001f)
            {
                _revealAudioSource.Stop();
            }
        }

        private void StopRevealAudioImmediately()
        {
            if (_revealAudioSource == null)
            {
                return;
            }

            _revealAudioSource.Stop();
            _revealAudioSource.volume = 0f;
        }

        private AudioClip GetRevealLoopClip()
        {
            if (_revealLoopClip != null)
            {
                return _revealLoopClip;
            }

            _generatedRevealLoopClip ??= ShadowAudioClipFactory.CreateRevealLoop();
            return _generatedRevealLoopClip;
        }
    }
}
