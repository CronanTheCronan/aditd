using ADoorInsideTheDark.Player;
using UnityEngine;

namespace ADoorInsideTheDark.Hearth
{
    /// <summary>
    /// Scene-local controller for the first Hearth return loop:
    /// claim the Green Thermos, carry it as a local boolean, and place it on one shelf slot.
    /// </summary>
    public sealed class HearthMinimalReturnController : MonoBehaviour
    {
        private const string RoomId = "room.hearth.minimal_return";
        private const string CarryStatus = "Carrying: Green Thermos";
        private const string ConfirmationLine = "An ordinary capacity to endure, held in place.";
        [Header("Gate")]
        [SerializeField] private bool _weightOfDoorCompleted = true;

        [Header("Scene References")]
        [SerializeField] private GameObject _pickupThermosRoot;
        [SerializeField] private GameObject _placedThermosRoot;
        [SerializeField] private Light _hearthFireLight;
        [SerializeField] private Renderer _hearthFireRenderer;
        [SerializeField] private GameObject _nextDoorCueRoot;
        [SerializeField] private Transform _nextDoorVisual;
        [SerializeField] private Light _nextDoorCueLight;

        [Header("Fire Feedback")]
        [SerializeField] private float _restingFireIntensity = 1.1f;
        [SerializeField] private float _placedFireIntensity = 1.7f;
        [SerializeField] private Color _restingFireColor = new(0.92f, 0.47f, 0.18f, 1f);
        [SerializeField] private Color _placedFireColor = new(1f, 0.72f, 0.34f, 1f);

        [Header("Exit Stub Timing")]
        [SerializeField] private float _confirmationHoldSeconds = 1.6f;
        [SerializeField] private float _nextDoorAcknowledgeSeconds = 0.9f;

        [Header("Exit Cue Feedback")]
        [SerializeField] private float _nextDoorCueRestingIntensity = 0f;
        [SerializeField] private float _nextDoorCueAvailableIntensity = 0.65f;
        [SerializeField] private float _nextDoorCueAcknowledgeIntensity = 1.1f;
        [SerializeField] private float _nextDoorVisualRestingYaw = 0f;
        [SerializeField] private float _nextDoorVisualAcknowledgeYaw = -6f;

        [Header("Debug Overlay")]
        [SerializeField] private bool _showDebugOverlay = false;

        private bool _isCarryingGreenThermos;
        private bool _greenThermosPlaced;
        private bool _nextDoorAvailable;
        private float _confirmationTimer;
        private float _nextDoorAcknowledgeTimer;
        private GUIStyle _overlayStyle;

        public bool WeightOfDoorCompleted => _weightOfDoorCompleted;
        public bool IsCarryingGreenThermos => _isCarryingGreenThermos;
        public bool GreenThermosPlaced => _greenThermosPlaced;
        public bool CanUseNextDoor => _weightOfDoorCompleted && _greenThermosPlaced && _nextDoorAvailable;
        public string CarryStatusText => CarryStatus;
        public string ConfirmationText => ConfirmationLine;

        public bool CanClaimGreenThermos =>
            _weightOfDoorCompleted &&
            !_isCarryingGreenThermos &&
            !_greenThermosPlaced;

        public bool CanPlaceGreenThermos =>
            _weightOfDoorCompleted &&
            _isCarryingGreenThermos &&
            !_greenThermosPlaced;

        public void ClaimGreenThermos(PlayerContext context)
        {
            _ = context;

            if (!CanClaimGreenThermos)
            {
                return;
            }

            _isCarryingGreenThermos = true;
            RefreshSceneState();

            Debug.Log($"[{RoomId}] Green Thermos claimed for local return.", this);
        }

        public void PlaceGreenThermos(PlayerContext context)
        {
            _ = context;

            if (!CanPlaceGreenThermos)
            {
                return;
            }

            _isCarryingGreenThermos = false;
            _greenThermosPlaced = true;
            _nextDoorAvailable = false;
            _confirmationTimer = _confirmationHoldSeconds;
            _nextDoorAcknowledgeTimer = 0f;
            RefreshSceneState();

            Debug.Log($"[{RoomId}] {ConfirmationLine}", this);
        }

        public void AcknowledgeNextDoor(PlayerContext context)
        {
            _ = context;

            if (!CanUseNextDoor)
            {
                return;
            }

            _nextDoorAcknowledgeTimer = _nextDoorAcknowledgeSeconds;
            ApplyNextDoorFeedback();

            Debug.Log($"[{RoomId}] Next door acknowledged locally; no transition triggered.", this);
        }

        private void Awake()
        {
            ResetRuntimeState();
            RefreshSceneState();
        }

        private void Update()
        {
            TickTimers();
            ApplyFireFeedback();
            ApplyNextDoorFeedback();
        }

        private void OnGUI()
        {
            if (!_showDebugOverlay)
            {
                return;
            }

            string primaryLine = _isCarryingGreenThermos
                ? CarryStatus
                : (_greenThermosPlaced && !_nextDoorAvailable ? ConfirmationLine : string.Empty);
            string stateLabel = GetDebugStateLabel();

            if (string.IsNullOrEmpty(primaryLine) &&
                _weightOfDoorCompleted &&
                stateLabel != "Next Door Available")
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

            Rect rect = new Rect(16f, 16f, 520f, 108f);
            GUI.Box(rect, "Hearth Minimal Return");

            if (!_weightOfDoorCompleted)
            {
                GUI.Label(
                    new Rect(28f, 46f, 496f, 40f),
                    "The Hearth return stays quiet until Weight of the Door is complete.",
                    _overlayStyle);
                GUI.Label(new Rect(28f, 78f, 496f, 24f), "State: Gated", _overlayStyle);
                return;
            }

            GUI.Label(new Rect(28f, 46f, 496f, 40f), primaryLine, _overlayStyle);
            GUI.Label(
                new Rect(28f, 78f, 496f, 24f),
                $"State: {stateLabel}",
                _overlayStyle);
        }

        private void OnValidate()
        {
            if (_pickupThermosRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_pickupThermosRoot)}.",
                    this);
            }

            if (_placedThermosRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_placedThermosRoot)}.",
                    this);
            }

            if (_hearthFireLight == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_hearthFireLight)}.",
                    this);
            }

            if (_hearthFireRenderer == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_hearthFireRenderer)}.",
                    this);
            }

            if (_nextDoorCueRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_nextDoorCueRoot)}.",
                    this);
            }

            if (_nextDoorVisual == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_nextDoorVisual)}.",
                    this);
            }

            if (_nextDoorCueLight == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthMinimalReturnController)} on '{gameObject.name}' should assign {nameof(_nextDoorCueLight)}.",
                    this);
            }
        }

        private void RefreshSceneState()
        {
            if (_pickupThermosRoot != null)
            {
                _pickupThermosRoot.SetActive(!_isCarryingGreenThermos && !_greenThermosPlaced);
            }

            if (_placedThermosRoot != null)
            {
                _placedThermosRoot.SetActive(_greenThermosPlaced);
            }

            ApplyFireFeedback();
            ApplyNextDoorFeedback();
        }

        private void ResetRuntimeState()
        {
            _isCarryingGreenThermos = false;
            _greenThermosPlaced = false;
            _nextDoorAvailable = false;
            _confirmationTimer = 0f;
            _nextDoorAcknowledgeTimer = 0f;
        }

        private void ApplyFireFeedback()
        {
            if (_hearthFireLight != null)
            {
                _hearthFireLight.enabled = true;
                _hearthFireLight.intensity = _greenThermosPlaced ? _placedFireIntensity : _restingFireIntensity;
                _hearthFireLight.color = _greenThermosPlaced ? _placedFireColor : _restingFireColor;
            }

            if (_hearthFireRenderer != null)
            {
                _hearthFireRenderer.material.color = _greenThermosPlaced ? _placedFireColor : _restingFireColor;
            }
        }

        private void TickTimers()
        {
            if (_confirmationTimer > 0f)
            {
                _confirmationTimer = Mathf.Max(0f, _confirmationTimer - Time.deltaTime);
                if (_confirmationTimer <= 0f && _greenThermosPlaced)
                {
                    _nextDoorAvailable = true;
                    RefreshSceneState();
                }
            }

            if (_nextDoorAcknowledgeTimer > 0f)
            {
                _nextDoorAcknowledgeTimer = Mathf.Max(0f, _nextDoorAcknowledgeTimer - Time.deltaTime);
            }
        }

        private void ApplyNextDoorFeedback()
        {
            bool cueActive = _nextDoorAvailable;
            bool acknowledgeActive = _nextDoorAcknowledgeTimer > 0f;

            if (_nextDoorCueRoot != null && _nextDoorCueRoot.activeSelf != cueActive)
            {
                _nextDoorCueRoot.SetActive(cueActive);
            }

            if (_nextDoorCueLight != null)
            {
                _nextDoorCueLight.enabled = cueActive;
                _nextDoorCueLight.intensity = !cueActive
                    ? _nextDoorCueRestingIntensity
                    : (acknowledgeActive ? _nextDoorCueAcknowledgeIntensity : _nextDoorCueAvailableIntensity);
            }

            if (_nextDoorVisual != null)
            {
                float targetYaw = acknowledgeActive ? _nextDoorVisualAcknowledgeYaw : _nextDoorVisualRestingYaw;
                Vector3 currentEuler = _nextDoorVisual.localEulerAngles;
                currentEuler.y = targetYaw;
                _nextDoorVisual.localEulerAngles = currentEuler;
            }
        }

        private string GetDebugStateLabel()
        {
            if (_nextDoorAvailable)
            {
                return "Next Door Available";
            }

            if (_greenThermosPlaced)
            {
                return "Placed";
            }

            if (_isCarryingGreenThermos)
            {
                return "Carrying";
            }

            return "Awaiting Claim";
        }
    }
}
