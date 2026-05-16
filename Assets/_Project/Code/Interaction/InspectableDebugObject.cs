using ADoorInsideTheDark.Player;
using UnityEngine;

namespace ADoorInsideTheDark.Interaction
{
    /// <summary>
    /// Placeholder inspectable: opens the inspection panel via <see cref="IInspectable"/> and mirrors
    /// <see cref="DebugInteractable"/> feedback for manual testing.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class InspectableDebugObject : MonoBehaviour, IInteractable, IInspectable
    {
        [SerializeField] private string _inspectionTitle = "Debug inspectable";
        [SerializeField, TextArea(3, 8)]
        private string _inspectionBody =
            "Placeholder inspection copy for Wave 004. Not final lore.";

        [SerializeField] private Color _startupTint = new(0.86f, 0.42f, 0.62f, 1f);

        [SerializeField] private Color _interactedColor = new(0.2f, 0.85f, 0.35f, 1f);

        private Renderer _renderer;

        public string InspectionTitle => _inspectionTitle;
        public string InspectionBody => _inspectionBody;

        /// <summary>
        /// Re-applies <see cref="_startupTint"/> to the renderer material (callable from Editor setup helpers).
        /// </summary>
        public void RefreshStartupTint()
        {
            ApplyStartupTintToRenderer();
        }

        private void Awake()
        {
            ApplyStartupTintToRenderer();
        }

        public void Interact(PlayerContext context)
        {
            _renderer ??= GetComponent<Renderer>();
            Debug.Log($"[InspectableDebugObject] Interacted with '{gameObject.name}'.", this);

            if (_renderer != null)
            {
                _renderer.material.color = _interactedColor;
            }
        }

        private void ApplyStartupTintToRenderer()
        {
            _renderer ??= GetComponent<Renderer>();
            if (_renderer == null)
            {
                return;
            }

            _renderer.material.color = _startupTint;
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(InspectableDebugObject)} on '{gameObject.name}' requires a Collider for raycasts.",
                    this);
            }

            if (!Application.isPlaying)
            {
                ApplyStartupTintToRenderer();
            }
        }
    }
}
