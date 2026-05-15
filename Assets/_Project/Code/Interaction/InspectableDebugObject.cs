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

        [SerializeField] private Color _interactedColor = new(0.2f, 0.85f, 0.35f, 1f);

        private Renderer _renderer;

        public string InspectionTitle => _inspectionTitle;
        public string InspectionBody => _inspectionBody;

        public void Interact(PlayerContext context)
        {
            _renderer ??= GetComponent<Renderer>();
            Debug.Log($"[InspectableDebugObject] Interacted with '{gameObject.name}'.", this);

            if (_renderer != null)
            {
                _renderer.material.color = _interactedColor;
            }
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(InspectableDebugObject)} on '{gameObject.name}' requires a Collider for raycasts.",
                    this);
            }
        }
    }
}
