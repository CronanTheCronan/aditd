using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.Shadow;
using UnityEngine;

namespace ADoorInsideTheDark.Rooms
{
    /// <summary>
    /// Scene-local graybox controller for room.main_floor.weight_of_door.
    /// Ordinary E alone cannot finish the room; Shadow (Q) reveals bindings; E while bindings are visible settles the door.
    /// </summary>
    public sealed class WeightOfDoorController : MonoBehaviour
    {
        private const string RoomId = "room.main_floor.weight_of_door";

        private enum DebugRoomState
        {
            Unresolved,
            FirstDoorAttempt,
            WrongFormWarning,
            ShadowRelevant,
            BindingsRevealed,
            Stabilized,
            Completed
        }

        [Header("Scene References")]
        [SerializeField] private Light _overheadLight;
        [SerializeField] private Renderer[] _hallRenderers;
        [SerializeField] private Renderer _doorPanelRenderer;
        [SerializeField] private Transform _doorPanelTransform;
        [SerializeField] private GameObject _bindingsRoot;
        [SerializeField] private ShadowRevealable _bindingsRevealable;
        [SerializeField] private ShadowPerceptionController _shadowPerception;
        [SerializeField] private AudioSource _clarityAudioSource;
        [SerializeField] private Transform _thermosRoot;
        [SerializeField] private Vector3 _thermosCompletedLocalPosition = new(-1.15f, 0.06f, -3.45f);
        [SerializeField] private GameObject _snowOwlMarker;

        [Header("Feedback")]
        [SerializeField] private bool _showDebugOverlay = true;
        [SerializeField] private float _wrongFormStrainDuration = 1.45f;
        [SerializeField] private float _blockedShadowDuration = 0.85f;
        [SerializeField] private float _stabilizeBeatDuration = 1.35f;
        [SerializeField] private float _wrongFormCueVolume = 0.14f;
        [SerializeField] private float _blockedCueVolume = 0.12f;

        private static readonly Color HallUnresolved = new(0.22f, 0.22f, 0.24f, 1f);
        private static readonly Color HallStrained = new(0.34f, 0.22f, 0.22f, 1f);
        private static readonly Color HallCompleted = new(0.28f, 0.32f, 0.29f, 1f);
        private static readonly Color DoorDefault = new(0.38f, 0.36f, 0.34f, 1f);
        private static readonly Color DoorStrained = new(0.55f, 0.38f, 0.36f, 1f);
        private static readonly Color DoorCompleted = new(0.45f, 0.52f, 0.48f, 1f);
        private static readonly Color SnowOwlCompleted = new(0.86f, 0.88f, 0.90f, 1f);
        private int _doorForceCount;
        private float _strainTimer;
        private float _blockedShadowTimer;
        private float _stabilizeTimer;
        private bool _completed;
        private Quaternion _doorBaseRotation = Quaternion.identity;
        private Renderer _snowOwlRenderer;
        private GUIStyle _overlayStyle;
        private AudioClip _generatedWrongFormCue;
        private AudioClip _generatedBlockedCue;

        public void UseDoor(PlayerContext context)
        {
            _ = context;

            if (_completed)
            {
                return;
            }

            if (_stabilizeTimer > 0f)
            {
                return;
            }

            if (IsShadowPerceptionActive() && CanBindingsRespondToShadow())
            {
                BeginStabilizeSequence();
                return;
            }

            _doorForceCount = Mathf.Min(_doorForceCount + 1, 10);
            UpdateBindingPolicy();
            _strainTimer = _wrongFormStrainDuration;

            if (_doorForceCount >= 2)
            {
                PlayWrongFormCue();
            }

            Debug.Log($"[{RoomId}] Ordinary door attempt #{_doorForceCount}.", this);
        }

        private void Awake()
        {
            AutoAssignDependencies();
            AutoAssignClarityAudio();
            CacheDoorBaseRotation();

            ApplyPalette(_hallRenderers, HallUnresolved);
            if (_doorPanelRenderer != null)
            {
                _doorPanelRenderer.material.color = DoorDefault;
            }

            if (_snowOwlMarker != null)
            {
                _snowOwlRenderer = _snowOwlMarker.GetComponent<Renderer>();
            }

            UpdateBindingPolicy();
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
            DestroyGeneratedClip(ref _generatedWrongFormCue);
            DestroyGeneratedClip(ref _generatedBlockedCue);
        }

        private void Update()
        {
            if (_strainTimer > 0f)
            {
                _strainTimer = Mathf.Max(0f, _strainTimer - Time.deltaTime);
            }

            if (_blockedShadowTimer > 0f)
            {
                _blockedShadowTimer = Mathf.Max(0f, _blockedShadowTimer - Time.deltaTime);
            }

            if (_stabilizeTimer > 0f)
            {
                _stabilizeTimer = Mathf.Max(0f, _stabilizeTimer - Time.deltaTime);
                if (_stabilizeTimer <= 0f)
                {
                    CompleteRoom();
                }
            }

            ApplyVisualFeedback();
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

            Rect rect = new Rect(16f, 16f, 520f, 200f);
            GUI.Box(rect, "Weight of the Door (graybox)");

            DebugRoomState debugState = GetDebugRoomState();
            string body = GetOverlayBody(debugState);

            GUI.Label(new Rect(28f, 46f, 496f, 120f), body, _overlayStyle);
            GUI.Label(new Rect(28f, 168f, 496f, 24f), $"State: {debugState}", _overlayStyle);
        }

        private void OnValidate()
        {
            if (_overheadLight == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorController)} on '{gameObject.name}' should assign {nameof(_overheadLight)}.",
                    this);
            }

            if (_bindingsRevealable == null && _bindingsRoot != null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorController)} on '{gameObject.name}' should assign {nameof(_bindingsRevealable)}.",
                    this);
            }

            if (_doorPanelRenderer == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorController)} on '{gameObject.name}' should assign {nameof(_doorPanelRenderer)}.",
                    this);
            }

            if (_shadowPerception == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorController)} on '{gameObject.name}' should assign {nameof(_shadowPerception)}.",
                    this);
            }
        }

        private void AutoAssignDependencies()
        {
            if (_bindingsRevealable == null && _bindingsRoot != null)
            {
                _bindingsRevealable = _bindingsRoot.GetComponent<ShadowRevealable>();
            }

            if (_shadowPerception == null)
            {
                _shadowPerception = FindAnyObjectByType<ShadowPerceptionController>();
            }
        }

        private void AutoAssignClarityAudio()
        {
            if (_clarityAudioSource == null)
            {
                _clarityAudioSource = GetComponent<AudioSource>();
            }
        }

        private void CacheDoorBaseRotation()
        {
            if (_doorPanelTransform != null)
            {
                _doorBaseRotation = _doorPanelTransform.localRotation;
            }
        }

        private void HandleShadowPerceptionChanged(bool isActive)
        {
            if (_completed)
            {
                return;
            }

            if (isActive && _doorForceCount == 0)
            {
                _blockedShadowTimer = _blockedShadowDuration;
                PlayBlockedCue();
                Debug.Log($"[{RoomId}] Shadow perception held before the door was tried with E.", this);
            }

            UpdateBindingPolicy();
        }

        private void UpdateBindingPolicy()
        {
            if (_bindingsRevealable == null)
            {
                return;
            }

            bool allowed = _doorForceCount >= 1 && !_completed;
            _bindingsRevealable.SetRevealAllowed(allowed);
            if (_completed)
            {
                _bindingsRevealable.SetForceReveal(false);
            }
        }

        private bool CanBindingsRespondToShadow()
        {
            return _doorForceCount >= 1;
        }

        private void BeginStabilizeSequence()
        {
            _stabilizeTimer = _stabilizeBeatDuration;

            if (_bindingsRevealable != null)
            {
                _bindingsRevealable.SetForceReveal(true);
            }

            Debug.Log($"[{RoomId}] Settling beat: ordinary E while Shadow showed bindings.", this);
        }

        private void CompleteRoom()
        {
            _completed = true;
            _strainTimer = 0f;
            _blockedShadowTimer = 0f;
            _stabilizeTimer = 0f;

            if (_bindingsRevealable != null)
            {
                _bindingsRevealable.SetForceReveal(false);
                _bindingsRevealable.SetRevealAllowed(false);
            }

            if (_thermosRoot != null)
            {
                _thermosRoot.localPosition = _thermosCompletedLocalPosition;
            }

            if (_doorPanelTransform != null)
            {
                _doorPanelTransform.localRotation =
                    _doorBaseRotation * Quaternion.Euler(0f, -12f, 0f);
            }

            if (_snowOwlRenderer != null)
            {
                _snowOwlRenderer.material.color = SnowOwlCompleted;
            }

            Debug.Log($"[{RoomId}] Room reached calm completion (graybox).", this);
        }

        private DebugRoomState GetDebugRoomState()
        {
            if (_completed)
            {
                return DebugRoomState.Completed;
            }

            if (_stabilizeTimer > 0f)
            {
                return DebugRoomState.Stabilized;
            }

            if (IsShadowPerceptionActive() && CanBindingsRespondToShadow())
            {
                return DebugRoomState.BindingsRevealed;
            }

            if (_doorForceCount == 0)
            {
                return DebugRoomState.Unresolved;
            }

            if (_doorForceCount == 1)
            {
                return DebugRoomState.FirstDoorAttempt;
            }

            if (_doorForceCount == 2)
            {
                return DebugRoomState.WrongFormWarning;
            }

            return DebugRoomState.ShadowRelevant;
        }

        private string GetOverlayBody(DebugRoomState state)
        {
            if (_blockedShadowTimer > 0f && _doorForceCount == 0)
            {
                return "Nothing strains yet. Press E on the door first, then use Shadow near the frame.";
            }

            return state switch
            {
                DebugRoomState.Unresolved =>
                    "The front door feels wrong. Press E and notice what pushes back.",
                DebugRoomState.FirstDoorAttempt =>
                    "It is not just stuck. Forcing it makes the hall tighten.",
                DebugRoomState.WrongFormWarning =>
                    "More force is making this heavier. Stop trying to win the door.",
                DebugRoomState.ShadowRelevant =>
                    "Hold Q near the frame and look for what the strain is hiding.",
                DebugRoomState.BindingsRevealed =>
                    "Shadow reveals bindings along the frame.\n\n" +
                    "Keep steady. While the bindings are revealed, press E to stop forcing and let the door settle.",
                DebugRoomState.Stabilized =>
                    "Keep steady. While the bindings are revealed, press E to stop forcing and let the door settle.",
                DebugRoomState.Completed =>
                    "The weight lifts. The door can be opened without a fight.",
                _ => string.Empty
            };
        }

        private void ApplyVisualFeedback()
        {
            float strainPulse = _strainTimer > 0f
                ? Mathf.Abs(Mathf.Sin(Time.time * 22f)) * Mathf.Clamp01(_strainTimer / _wrongFormStrainDuration)
                : 0f;
            float blockedPulse = _blockedShadowTimer > 0f
                ? Mathf.Abs(Mathf.Sin(Time.time * 11f)) * Mathf.Clamp01(_blockedShadowTimer / _blockedShadowDuration)
                : 0f;
            float settlePulse = _stabilizeTimer > 0f
                ? Mathf.Clamp01(_stabilizeTimer / _stabilizeBeatDuration)
                : 0f;

            bool revealHeld = !_completed && IsShadowPerceptionActive() && CanBindingsRespondToShadow();

            if (_overheadLight != null)
            {
                if (_completed)
                {
                    _overheadLight.enabled = true;
                    _overheadLight.intensity = 2.0f;
                    _overheadLight.color = new Color(1f, 0.97f, 0.90f, 1f);
                }
                else
                {
                    _overheadLight.enabled = true;
                    float baseIntensity = 0.22f;
                    if (revealHeld)
                    {
                        baseIntensity = 0.38f + settlePulse * 0.15f;
                    }

                    _overheadLight.intensity = baseIntensity + strainPulse * 2.1f + blockedPulse * 0.2f;
                    Color cool = Color.Lerp(
                        new Color(0.94f, 0.84f, 0.82f),
                        new Color(0.72f, 0.88f, 0.96f),
                        revealHeld ? 0.55f : 0f);
                    _overheadLight.color = Color.Lerp(
                        cool,
                        new Color(1f, 0.42f, 0.42f),
                        strainPulse);
                }
            }

            if (_hallRenderers != null && _hallRenderers.Length > 0)
            {
                Color hallColor;
                if (_completed)
                {
                    hallColor = HallCompleted;
                }
                else if (_doorForceCount >= 2 || strainPulse > 0.01f)
                {
                    hallColor = Color.Lerp(HallUnresolved, HallStrained, 0.35f + strainPulse * 0.5f);
                }
                else
                {
                    hallColor = HallUnresolved;
                }

                ApplyPalette(_hallRenderers, hallColor);
            }

            if (_doorPanelRenderer != null)
            {
                if (_completed)
                {
                    _doorPanelRenderer.material.color = DoorCompleted;
                }
                else
                {
                    Color tone = Color.Lerp(DoorDefault, DoorStrained, strainPulse * 0.85f);
                    if (revealHeld)
                    {
                        tone = Color.Lerp(tone, new Color(0.50f, 0.62f, 0.68f, 1f), 0.35f);
                    }

                    _doorPanelRenderer.material.color = tone;
                }
            }

            if (_doorPanelTransform != null && !_completed && _stabilizeTimer <= 0f)
            {
                float wobble = strainPulse * 1.6f + blockedPulse * 0.35f;
                float yaw = Mathf.Sin(Time.time * 19f) * wobble;
                _doorPanelTransform.localRotation = _doorBaseRotation * Quaternion.Euler(0f, yaw, 0f);
            }

            if (_snowOwlRenderer != null)
            {
                if (_completed)
                {
                    _snowOwlRenderer.material.color = SnowOwlCompleted;
                }
                else
                {
                    float pulse = 0.65f + Mathf.Abs(Mathf.Sin(Time.time * 2.1f)) * 0.08f;
                    _snowOwlRenderer.material.color = new Color(pulse, pulse, pulse + 0.03f, 1f);
                }
            }
        }

        private bool IsShadowPerceptionActive()
        {
            return _shadowPerception != null && _shadowPerception.IsPerceptionActive;
        }

        private void PlayWrongFormCue()
        {
            if (_clarityAudioSource == null)
            {
                return;
            }

            _generatedWrongFormCue ??= ShadowAudioClipFactory.CreateDestabilizedGuidanceCue();
            _clarityAudioSource.PlayOneShot(_generatedWrongFormCue, _wrongFormCueVolume);
        }

        private void PlayBlockedCue()
        {
            if (_clarityAudioSource == null)
            {
                return;
            }

            _generatedBlockedCue ??= ShadowAudioClipFactory.CreatePerceptionBlockedCue();
            _clarityAudioSource.PlayOneShot(_generatedBlockedCue, _blockedCueVolume);
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
