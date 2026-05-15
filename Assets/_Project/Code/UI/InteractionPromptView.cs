using UnityEngine;
using UnityEngine.UI;

namespace ADoorInsideTheDark.UI
{
    /// <summary>
    /// Shows or hides a single line of interaction prompt text (e.g. "E Inspect").
    /// </summary>
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Text _promptText;

        private void Awake()
        {
            Hide();
        }

        public void SetPrompt(string prompt)
        {
            if (_root == null)
            {
                return;
            }

            if (_promptText != null)
            {
                _promptText.text = prompt ?? string.Empty;
            }

            _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root == null)
            {
                return;
            }

            _root.SetActive(false);
        }

        private void OnValidate()
        {
            if (_root == null)
            {
                Debug.LogWarning(
                    $"{nameof(InteractionPromptView)} on '{gameObject.name}' should assign a prompt root object.",
                    this);
            }
        }
    }
}
