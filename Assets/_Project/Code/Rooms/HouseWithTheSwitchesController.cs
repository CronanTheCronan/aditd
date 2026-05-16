using ADoorInsideTheDark.Player;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private Renderer[] _hiddenSeamRenderers;
        [SerializeField] private GameObject _completionMarker;
        [SerializeField] private Renderer[] _completionRenderers;

        [Header("Feedback")]
        [SerializeField] private bool _showDebugOverlay = true;
        [SerializeField] private float _wrongFormFeedbackDuration = 1.35f;
        [SerializeField] private float _switchHandlePressedAngle = 26f;
        [SerializeField] private string _shadowPlaceholderControlLabel = "Hold Q to enter Shadow perception";

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
        private bool _shadowPerceptionActive;
        private bool _shadowRevealLearned;
        private Quaternion _switchHandleBaseRotation = Quaternion.identity;
        private GUIStyle _overlayStyle;

        public void UseOrdinarySwitch(PlayerContext context)
        {
            _ = context;

            if (_state == RoomState.Completed)
            {
                return;
            }

            if (_shadowPerceptionActive && _shadowRevealLearned)
            {
                CompleteRoom();
                return;
            }

            TriggerWrongFormFeedback();
        }

        private void Awake()
        {
            if (_switchHandle != null)
            {
                _switchHandleBaseRotation = _switchHandle.localRotation;
            }

            ApplyPalette(_roomRenderers, RoomUnresolvedColor);
            ApplyPalette(_hiddenSeamRenderers, SeamRevealColor);
            ApplyPalette(_completionRenderers, CompletionColor);

            if (_hiddenSeamRoot != null)
            {
                _hiddenSeamRoot.SetActive(false);
            }

            if (_completionMarker != null)
            {
                _completionMarker.SetActive(false);
            }

            ApplyCurrentVisualState();
        }

        private void Update()
        {
            UpdateShadowPlaceholderInput();

            if (_feedbackTimer > 0f)
            {
                _feedbackTimer = Mathf.Max(0f, _feedbackTimer - Time.deltaTime);
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
                    "Use the ordinary switch with E. It will not settle the room by force alone.",
                RoomState.Destabilized =>
                    "The glare fights you back. " + _shadowPlaceholderControlLabel + " and look for the hidden seam.",
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
        }

        private void UpdateShadowPlaceholderInput()
        {
            bool shouldEnableShadow = Keyboard.current != null && Keyboard.current.qKey.isPressed;
            if (_shadowPerceptionActive == shouldEnableShadow)
            {
                return;
            }

            _shadowPerceptionActive = shouldEnableShadow;

            if (_state == RoomState.Completed)
            {
                return;
            }

            if (_shadowPerceptionActive && _state == RoomState.Destabilized)
            {
                _state = RoomState.ShadowRevealed;
                _shadowRevealLearned = true;
                Debug.Log("[HouseWithTheSwitches] Shadow perception revealed the hidden seam.", this);
            }
            else if (!_shadowPerceptionActive && _state == RoomState.ShadowRevealed)
            {
                _state = RoomState.Destabilized;
            }
        }

        private void TriggerWrongFormFeedback()
        {
            _state = RoomState.Destabilized;
            _feedbackTimer = _wrongFormFeedbackDuration;
            Debug.Log("[HouseWithTheSwitches] Ego-only switch use destabilized the room.", this);
        }

        private void CompleteRoom()
        {
            _state = RoomState.Completed;
            _feedbackTimer = 0f;
            _shadowPerceptionActive = false;
            Debug.Log("[HouseWithTheSwitches] Integrated Ego + Shadow action completed the room.", this);
        }

        private void ApplyCurrentVisualState()
        {
            bool revealActive = _state == RoomState.Completed ||
                (_shadowPerceptionActive && (_state == RoomState.Destabilized || _state == RoomState.ShadowRevealed));

            if (_hiddenSeamRoot != null && _hiddenSeamRoot.activeSelf != revealActive)
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

            if (_overheadLight != null)
            {
                switch (_state)
                {
                    case RoomState.Unresolved:
                        _overheadLight.enabled = true;
                        _overheadLight.intensity = 0.18f;
                        _overheadLight.color = new Color(0.92f, 0.91f, 0.88f);
                        break;
                    case RoomState.Destabilized:
                    case RoomState.ShadowRevealed:
                        _overheadLight.enabled = true;
                        _overheadLight.intensity = 0.35f + (feedbackPulse * 1.9f);
                        _overheadLight.color = Color.Lerp(
                            new Color(0.94f, 0.84f, 0.80f),
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
                    RoomState.Unresolved => SwitchUnresolvedColor,
                    RoomState.Destabilized => SwitchDestabilizedColor,
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
