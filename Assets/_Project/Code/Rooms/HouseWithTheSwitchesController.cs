using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.Shadow;
using UnityEngine;

namespace ADoorInsideTheDark.Rooms
{
    public sealed class HouseWithTheSwitchesController : MonoBehaviour
    {
        private enum RoomState
        {
            Unresolved,
            Destabilized,
            ShadowRevealed,
            Completed
        }

        [Header("Scene References")]
        [SerializeField] private Light _overheadLight;
        [SerializeField] private Renderer[] _roomRenderers;
        [SerializeField] private Renderer _switchRenderer;
        [SerializeField] private Transform _switchHandle;
        [SerializeField] private GameObject _hiddenSeamRoot;
        [SerializeField] private ShadowRevealable _hiddenSeamRevealable;
        [SerializeField] private Renderer[] _hiddenSeamRenderers;
        [SerializeField] private GameObject _completionMarker;
        [SerializeField] private Renderer[] _completionRenderers;
        [SerializeField] private ShadowPerceptionController _shadowPerception;
        [SerializeField] private AudioSource _clarityAudioSource;

        [Header("Feedback")]
        [SerializeField] private bool _showDebugOverlay = true;
        [SerializeField] private float _wrongFormFeedbackDuration = 1.35f;
        [SerializeField] private float _switchHandlePressedAngle = 26f;
        [SerializeField] private string _shadowPerceptionControlLabel = "Hold Q to enter Shadow perception";
        [SerializeField] private AudioClip _shadowBlockedCue;
        [SerializeField] private AudioClip _destabilizedGuidanceCue;
        [SerializeField] private float _shadowBlockedCueVolume = 0.12f;
        [SerializeField] private float _destabilizedGuidanceCueVolume = 0.15f;
        [SerializeField] private float _blockedFeedbackDuration = 0.85f;
        [SerializeField] private float _destabilizedGuidanceDuration = 2.25f;

        private static readonly Color RoomUnresolvedColor = new(0.22f, 0.22f, 0.24f, 1f);
        private static readonly Color RoomDestabilizedColor = new(0.35f, 0.21f, 0.21f, 1f);
        private static readonly Color RoomCompletedColor = new(0.30f, 0.31f, 0.28f, 1f);
        private static readonly Color SwitchUnresolvedColor = new(0.72f, 0.72f, 0.74f, 1f);
        private static readonly Color SwitchDestabilizedColor = new(0.78f, 0.42f, 0.42f, 1f);
        private static readonly Color SwitchRevealColor = new(0.44f, 0.63f, 0.84f, 1f);
        private static readonly Color SwitchCompletedColor = new(0.59f, 0.81f, 0.54f, 1f);
        private static readonly Color SeamRevealColor = new(0.44f, 0.81f, 0.89f, 1f);
        private static readonly Color CompletionColor = new(0.72f, 0.91f, 0.67f, 1f);

        private RoomState _state = RoomState.Unresolved;
        private float _feedbackTimer;
        private float _blockedFeedbackTimer;
        private float _destabilizedGuidanceTimer;
        private bool _shadowRevealLearned;
        private Quaternion _switchHandleBaseRotation = Quaternion.identity;
        private GUIStyle _overlayStyle;
        private AudioClip _generatedBlockedCue;
        private AudioClip _generatedDestabilizedGuidanceCue;

        public void UseOrdinarySwitch(PlayerContext context)
        {
            _ = context;

            if (_state == RoomState.Completed)
            {
                return;
            }

            if (IsShadowPerceptionActive() && _shadowRevealLearned)
            {
                CompleteRoom();
                return;
            }

            TriggerWrongFormFeedback();
        }

        private void Awake()
        {
            AutoAssignDependencies();
            AutoAssignClarityAudioSource();

            if (_switchHandle != null)
            {
                _switchHandleBaseRotation = _switchHandle.localRotation;
            }

            ApplyPalette(_roomRenderers, RoomUnresolvedColor);
            ApplyPalette(_hiddenSeamRenderers, SeamRevealColor);
            ApplyPalette(_completionRenderers, CompletionColor);

            if (_completionMarker != null)
            {
                _completionMarker.SetActive(false);
            }

            ApplyCurrentVisualState();
        }

        private void OnEnable()
        {
            if (_shadowPerception != null)
            {
                _shadowPerception.PerceptionStateChanged += HandleShadowPerceptionChanged;
                HandleShadowPerceptionChanged(_shadowPerception.IsPerceptionActive);
            }
        }

        private void OnDisable()
        {
            if (_shadowPerception != null)
            {
                _shadowPerception.PerceptionStateChanged -= HandleShadowPerceptionChanged;
            }
        }

        private void OnDestroy()
        {
            DestroyGeneratedClip(ref _generatedBlockedCue);
            DestroyGeneratedClip(ref _generatedDestabilizedGuidanceCue);
        }

        private void Update()
        {
            if (_feedbackTimer > 0f)
            {
                _feedbackTimer = Mathf.Max(0f, _feedbackTimer - Time.deltaTime);
            }

            if (_blockedFeedbackTimer > 0f)
            {
                _blockedFeedbackTimer = Mathf.Max(0f, _blockedFeedbackTimer - Time.deltaTime);
            }

            if (_destabilizedGuidanceTimer > 0f)
            {
                _destabilizedGuidanceTimer = Mathf.Max(0f, _destabilizedGuidanceTimer - Time.deltaTime);
            }

            ApplyCurrentVisualState();
        }

        private void OnGUI()
        {
            if (!_showDebugOverlay)
            {
                return;
            }

            if (_overlayStyle == null)
            {
                _overlayStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    wordWrap = true,
                    normal = { textColor = Color.white }
                };
            }

            Rect rect = new(16f, 16f, 460f, 120f);
            GUI.Box(rect, "House With the Switches");

            string instructions = _state switch
            {
                RoomState.Unresolved =>
                    _blockedFeedbackTimer > 0f
                        ? "Nothing answers yet. Press E on the ordinary switch first, then return to Shadow."
                        : "Press E on the ordinary switch first. Shadow reveals nothing while the room is still stable.",
                RoomState.Destabilized =>
                    _destabilizedGuidanceTimer > 0f
                        ? "Something loosened in the room. " + _shadowPerceptionControlLabel + " and watch the back wall."
                        : "The glare fights you back. " + _shadowPerceptionControlLabel + " and look for the hidden seam.",
                RoomState.ShadowRevealed =>
                    "The seam is visible. Keep Shadow perception active and press E on the switch to settle the room.",
                RoomState.Completed =>
                    "The room holds together. The light is stable and the way forward is open.",
                _ => string.Empty
            };

            GUI.Label(new Rect(28f, 46f, 436f, 64f), instructions, _overlayStyle);
            GUI.Label(new Rect(28f, 96f, 436f, 24f), $"State: {_state}", _overlayStyle);
        }

        private void OnValidate()
        {
            if (_overheadLight == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_overheadLight)}.",
                    this);
            }

            if (_hiddenSeamRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_hiddenSeamRoot)}.",
                    this);
            }

            if (_hiddenSeamRevealable == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_hiddenSeamRevealable)}.",
                    this);
            }

            if (_switchRenderer == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_switchRenderer)}.",
                    this);
            }

            if (_switchHandle == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_switchHandle)}.",
                    this);
            }

            if (_completionMarker == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_completionMarker)}.",
                    this);
            }

            if (_shadowPerception == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseWithTheSwitchesController)} on '{gameObject.name}' should assign {nameof(_shadowPerception)}.",
                    this);
            }
        }

        private void AutoAssignDependencies()
        {
            if (_hiddenSeamRevealable == null && _hiddenSeamRoot != null)
            {
                _hiddenSeamRevealable = _hiddenSeamRoot.GetComponent<ShadowRevealable>();
            }

            if (_shadowPerception == null)
            {
                _shadowPerception = FindAnyObjectByType<ShadowPerceptionController>();
            }
        }

        private void AutoAssignClarityAudioSource()
        {
            if (_clarityAudioSource == null)
            {
                _clarityAudioSource = GetComponent<AudioSource>();
            }
        }

        private void HandleShadowPerceptionChanged(bool isActive)
        {
            if (_state == RoomState.Completed)
            {
                return;
            }

            if (isActive && _state == RoomState.Unresolved)
            {
                TriggerBlockedShadowFeedback();
                return;
            }

            if (isActive && _state == RoomState.Destabilized)
            {
                _state = RoomState.ShadowRevealed;
                _shadowRevealLearned = true;
                _destabilizedGuidanceTimer = 0f;
                Debug.Log("[HouseWithTheSwitches] Shadow perception revealed the hidden seam.", this);
            }
            else if (!isActive && _state == RoomState.ShadowRevealed)
            {
                _state = RoomState.Destabilized;
            }
        }

        private void TriggerWrongFormFeedback()
        {
            _state = RoomState.Destabilized;
            _feedbackTimer = _wrongFormFeedbackDuration;
            _blockedFeedbackTimer = 0f;
            _destabilizedGuidanceTimer = _destabilizedGuidanceDuration;
            PlayClarityCue(GetDestabilizedGuidanceCue(), _destabilizedGuidanceCueVolume);

            if (IsShadowPerceptionActive())
            {
                HandleShadowPerceptionChanged(true);
            }

            Debug.Log("[HouseWithTheSwitches] Ego-only switch use destabilized the room.", this);
        }

        private void CompleteRoom()
        {
            _state = RoomState.Completed;
            _feedbackTimer = 0f;
            _blockedFeedbackTimer = 0f;
            _destabilizedGuidanceTimer = 0f;
            Debug.Log("[HouseWithTheSwitches] Integrated Ego + Shadow action completed the room.", this);
        }

        private void ApplyCurrentVisualState()
        {
            bool revealActive = _state == RoomState.Completed ||
                (IsShadowPerceptionActive() && (_state == RoomState.Destabilized || _state == RoomState.ShadowRevealed));

            if (_hiddenSeamRevealable != null)
            {
                _hiddenSeamRevealable.SetRevealAllowed(_state == RoomState.Destabilized || _state == RoomState.ShadowRevealed);
                _hiddenSeamRevealable.SetForceReveal(_state == RoomState.Completed);
            }
            else if (_hiddenSeamRoot != null && _hiddenSeamRoot.activeSelf != revealActive)
            {
                _hiddenSeamRoot.SetActive(revealActive);
            }

            if (_completionMarker != null)
            {
                _completionMarker.SetActive(_state == RoomState.Completed);
            }

            float feedbackPulse = _feedbackTimer > 0f
                ? Mathf.Abs(Mathf.Sin(Time.time * 24f)) * Mathf.Clamp01(_feedbackTimer / _wrongFormFeedbackDuration)
                : 0f;
            float blockedPulse = _blockedFeedbackTimer > 0f
                ? Mathf.Abs(Mathf.Sin(Time.time * 11f)) * Mathf.Clamp01(_blockedFeedbackTimer / _blockedFeedbackDuration)
                : 0f;
            float guidancePulse = _destabilizedGuidanceTimer > 0f
                ? Mathf.Abs(Mathf.Sin(Time.time * 7f)) * Mathf.Clamp01(_destabilizedGuidanceTimer / _destabilizedGuidanceDuration)
                : 0f;

            if (_overheadLight != null)
            {
                switch (_state)
                {
                    case RoomState.Unresolved:
                        _overheadLight.enabled = true;
                        _overheadLight.intensity = 0.18f + (blockedPulse * 0.18f);
                        _overheadLight.color = Color.Lerp(
                            new Color(0.92f, 0.91f, 0.88f),
                            new Color(0.76f, 0.85f, 0.98f),
                            blockedPulse * 0.55f);
                        break;
                    case RoomState.Destabilized:
                    case RoomState.ShadowRevealed:
                        _overheadLight.enabled = true;
                        _overheadLight.intensity = 0.35f + (feedbackPulse * 1.9f) + (guidancePulse * 0.22f);
                        _overheadLight.color = Color.Lerp(
                            new Color(0.94f, 0.84f, 0.80f),
                            new Color(0.70f, 0.86f, 0.95f),
                            guidancePulse * 0.55f);
                        _overheadLight.color = Color.Lerp(
                            _overheadLight.color,
                            new Color(1f, 0.45f, 0.45f),
                            feedbackPulse);
                        break;
                    case RoomState.Completed:
                        _overheadLight.enabled = true;
                        _overheadLight.intensity = 2.2f;
                        _overheadLight.color = new Color(1f, 0.97f, 0.90f);
                        break;
                }
            }

            ApplyPalette(
                _roomRenderers,
                _state switch
                {
                    RoomState.Unresolved => RoomUnresolvedColor,
                    RoomState.Completed => RoomCompletedColor,
                    _ => Color.Lerp(RoomDestabilizedColor, RoomUnresolvedColor, revealActive ? 0.35f : 0f)
                });

            if (_switchRenderer != null)
            {
                _switchRenderer.material.color = _state switch
                {
                    RoomState.Unresolved => Color.Lerp(SwitchUnresolvedColor, SeamRevealColor, blockedPulse * 0.25f),
                    RoomState.Destabilized => Color.Lerp(SwitchDestabilizedColor, SeamRevealColor, guidancePulse * 0.30f),
                    RoomState.ShadowRevealed => SwitchRevealColor,
                    RoomState.Completed => SwitchCompletedColor,
                    _ => SwitchUnresolvedColor
                };
            }

            if (_switchHandle != null)
            {
                float angle = (_state == RoomState.Destabilized || _state == RoomState.ShadowRevealed)
                    ? Mathf.Lerp(0f, _switchHandlePressedAngle, feedbackPulse)
                    : (_state == RoomState.Completed ? _switchHandlePressedAngle : 0f);

                _switchHandle.localRotation = _switchHandleBaseRotation * Quaternion.Euler(angle, 0f, 0f);
            }
        }

        private void TriggerBlockedShadowFeedback()
        {
            _blockedFeedbackTimer = _blockedFeedbackDuration;
            PlayClarityCue(GetBlockedShadowCue(), _shadowBlockedCueVolume);
            Debug.Log("[HouseWithTheSwitches] Shadow perception found nothing before the room was destabilized.", this);
        }

        private void PlayClarityCue(AudioClip clip, float volume)
        {
            if (_clarityAudioSource == null || clip == null)
            {
                return;
            }

            _clarityAudioSource.PlayOneShot(clip, volume);
        }

        private AudioClip GetBlockedShadowCue()
        {
            if (_shadowBlockedCue != null)
            {
                return _shadowBlockedCue;
            }

            _generatedBlockedCue ??= ShadowAudioClipFactory.CreatePerceptionBlockedCue();
            return _generatedBlockedCue;
        }

        private AudioClip GetDestabilizedGuidanceCue()
        {
            if (_destabilizedGuidanceCue != null)
            {
                return _destabilizedGuidanceCue;
            }

            _generatedDestabilizedGuidanceCue ??= ShadowAudioClipFactory.CreateDestabilizedGuidanceCue();
            return _generatedDestabilizedGuidanceCue;
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

        private bool IsShadowPerceptionActive()
        {
            return _shadowPerception != null && _shadowPerception.IsPerceptionActive;
        }

        private static void ApplyPalette(Renderer[] renderers, Color color)
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                renderer.material.color = color;
            }
        }
    }
}
