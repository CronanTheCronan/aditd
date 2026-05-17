using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.Player;
using UnityEngine;

namespace ADoorInsideTheDark.Hearth
{
    [RequireComponent(typeof(Collider))]
    public sealed class HearthAnchorPlacementSlot : MonoBehaviour, IInteractable, IInteractionPromptProvider
    {
        [SerializeField] private HearthMinimalReturnController _controller;
        [SerializeField] private GameObject _highlightRoot;
        [SerializeField] private Renderer[] _highlightRenderers;
        [SerializeField] private Color _highlightColor = new(0.91f, 0.79f, 0.47f, 1f);
        [SerializeField] private float _pulseSpeed = 3.4f;

        private Color[] _baseColors;

        public void Interact(PlayerContext context)
        {
            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthAnchorPlacementSlot)} on '{gameObject.name}' has no {nameof(HearthMinimalReturnController)} assigned.",
                    this);
                return;
            }

            _controller.PlaceGreenThermos(context);
        }

        public string GetInteractionPrompt(PlayerContext context)
        {
            _ = context;
            return _controller != null && _controller.CanPlaceGreenThermos
                ? "E - Place Anchor"
                : string.Empty;
        }

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponentInParent<HearthMinimalReturnController>();
            }

            CacheBaseColors();
            ApplyHighlightState();

            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthAnchorPlacementSlot)} on '{gameObject.name}' could not find a {nameof(HearthMinimalReturnController)} during {nameof(Awake)}.",
                    this);
            }
        }

        private void Update()
        {
            ApplyHighlightState();
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(HearthAnchorPlacementSlot)} on '{gameObject.name}' requires a Collider for player interaction.",
                    this);
            }
        }

        private void CacheBaseColors()
        {
            if (_highlightRenderers == null || _highlightRenderers.Length == 0)
            {
                _baseColors = System.Array.Empty<Color>();
                return;
            }

            _baseColors = new Color[_highlightRenderers.Length];
            for (int i = 0; i < _highlightRenderers.Length; i++)
            {
                Renderer renderer = _highlightRenderers[i];
                _baseColors[i] = renderer != null ? renderer.material.color : Color.white;
            }
        }

        private void ApplyHighlightState()
        {
            bool active = _controller != null && _controller.CanPlaceGreenThermos;

            if (_highlightRoot != null && _highlightRoot.activeSelf != active)
            {
                _highlightRoot.SetActive(active);
            }

            if (_highlightRenderers == null)
            {
                return;
            }

            float pulse = active ? 0.5f + Mathf.Abs(Mathf.Sin(Time.time * _pulseSpeed)) * 0.5f : 0f;

            for (int i = 0; i < _highlightRenderers.Length; i++)
            {
                Renderer renderer = _highlightRenderers[i];
                if (renderer == null)
                {
                    continue;
                }

                Color baseColor = _baseColors != null && i < _baseColors.Length ? _baseColors[i] : Color.white;
                renderer.material.color = active
                    ? Color.Lerp(baseColor, _highlightColor, pulse)
                    : baseColor;
            }
        }
    }
}
