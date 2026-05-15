using ADoorInsideTheDark.Player;
using UnityEngine;

namespace ADoorInsideTheDark.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class DebugInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Color _interactedColor = new(0.2f, 0.85f, 0.35f, 1f);

        private Renderer _renderer;
        public void Interact(PlayerContext context)
        {
            _renderer ??= GetComponent<Renderer>();
            Debug.Log($"[DebugInteractable] Interacted with '{gameObject.name}'.", this);

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
                    $"{nameof(DebugInteractable)} on '{gameObject.name}' requires a Collider for raycasts.",
                    this);
            }
        }
    }
}
