using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.Player;
using UnityEngine;

namespace ADoorInsideTheDark.Hearth
{
    [RequireComponent(typeof(Collider))]
    public sealed class GreenThermosReturnInteractable : MonoBehaviour, IInteractable, IInteractionPromptProvider
    {
        [SerializeField] private HearthMinimalReturnController _controller;

        public void Interact(PlayerContext context)
        {
            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(GreenThermosReturnInteractable)} on '{gameObject.name}' has no {nameof(HearthMinimalReturnController)} assigned.",
                    this);
                return;
            }

            _controller.ClaimGreenThermos(context);
        }

        public string GetInteractionPrompt(PlayerContext context)
        {
            _ = context;
            return _controller != null && _controller.CanClaimGreenThermos
                ? "E - Claim Thermos"
                : string.Empty;
        }

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponentInParent<HearthMinimalReturnController>();
            }

            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(GreenThermosReturnInteractable)} on '{gameObject.name}' could not find a {nameof(HearthMinimalReturnController)} during {nameof(Awake)}.",
                    this);
            }
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(GreenThermosReturnInteractable)} on '{gameObject.name}' requires a Collider for player interaction.",
                    this);
            }
        }
    }
}
