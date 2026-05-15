using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ADoorInsideTheDark.UI
{
    /// <summary>
    /// Simple full-screen or centered panel with title and body. Closes on <b>Escape</b> (handled here)
    /// or via <see cref="Hide"/> (e.g. when the interactor consumes Interact while open).
    /// </summary>
    public sealed class InspectionPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _bodyText;

        private bool _isOpen;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            Hide();
        }

        private void Update()
        {
            if (!_isOpen)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Hide();
            }
        }

        public void Show(string title, string body)
        {
            if (_panelRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(InspectionPanelView)} on '{gameObject.name}' has no panel root assigned.",
                    this);
                return;
            }

            if (_titleText != null)
            {
                _titleText.text = title ?? string.Empty;
            }

            if (_bodyText != null)
            {
                _bodyText.text = body ?? string.Empty;
            }

            _panelRoot.SetActive(true);
            _isOpen = true;
        }

        public void Hide()
        {
            if (_panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }

            _isOpen = false;
        }

        private void OnValidate()
        {
            if (_panelRoot == null)
            {
                Debug.LogWarning(
                    $"{nameof(InspectionPanelView)} on '{gameObject.name}' should assign a panel root object.",
                    this);
            }
        }
    }
}
