using ADoorInsideTheDark.Interaction;
using ADoorInsideTheDark.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Player
{
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private PlayerContext _context;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _interactDistance = 3f;
        [SerializeField] private LayerMask _interactLayers = ~0;
        [SerializeField] private InteractionPromptView _promptView;
        [SerializeField] private InspectionPanelView _inspectionPanel;

        private InputAction _interactAction;

        private void Awake()
        {
            if (_context == null)
            {
                _context = GetComponent<PlayerContext>();
            }

            if (_camera == null && _context != null && _context.CameraRoot != null)
            {
                _camera = _context.CameraRoot.GetComponent<Camera>();
            }

            if (_camera == null)
            {
                _camera = GetComponentInChildren<Camera>();
            }

            CacheInputActions();
        }

        private void Update()
        {
            UpdateInteractionPrompt();
            HandleInteractInput();
        }

        private void HandleInteractInput()
        {
            if (_interactAction == null || !_interactAction.WasPressedThisFrame())
            {
                return;
            }

            if (_inspectionPanel != null && _inspectionPanel.IsOpen)
            {
                _inspectionPanel.Hide();
                return;
            }

            TryInteract();
        }

        private void UpdateInteractionPrompt()
        {
            if (_promptView == null)
            {
                return;
            }

            if (_inspectionPanel != null && _inspectionPanel.IsOpen)
            {
                _promptView.Hide();
                return;
            }

            if (_camera == null)
            {
                _promptView.Hide();
                return;
            }

            if (!TryGetRaycastHit(out RaycastHit hit))
            {
                _promptView.Hide();
                return;
            }

            IInspectable inspectable = FindInspectable(hit.collider.transform);
            IInteractable interactable = FindInteractable(hit.collider.transform);

            if (inspectable == null && interactable == null)
            {
                _promptView.Hide();
                return;
            }

            string promptText = inspectable != null ? "E Inspect" : "E Interact";
            _promptView.SetPrompt(promptText);
        }

        private void TryInteract()
        {
            if (_camera == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerInteractor)} on '{gameObject.name}' has no Camera for raycasts.",
                    this);
                return;
            }

            if (_context == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerInteractor)} on '{gameObject.name}' has no {nameof(PlayerContext)}.",
                    this);
                return;
            }

            if (!TryGetRaycastHit(out RaycastHit hit))
            {
                return;
            }

            Transform hitTransform = hit.collider.transform;
            IInspectable inspectable = FindInspectable(hitTransform);
            IInteractable interactable = FindInteractable(hitTransform);

            if (inspectable != null)
            {
                if (_inspectionPanel != null)
                {
                    _inspectionPanel.Show(inspectable.InspectionTitle, inspectable.InspectionBody);
                }
                else
                {
                    Debug.LogWarning(
                        $"{nameof(PlayerInteractor)} on '{gameObject.name}' hit an {nameof(IInspectable)} " +
                        $"but no {nameof(InspectionPanelView)} is assigned; panel will not open.",
                        this);
                }

                if (interactable != null)
                {
                    interactable.Interact(_context);
                }

                return;
            }

            if (interactable != null)
            {
                interactable.Interact(_context);
            }
        }

        private bool TryGetRaycastHit(out RaycastHit hit)
        {
            Ray ray = new(_camera.transform.position, _camera.transform.forward);
            return Physics.Raycast(
                ray,
                out hit,
                _interactDistance,
                _interactLayers,
                QueryTriggerInteraction.Ignore);
        }

        private static IInspectable FindInspectable(Transform hitTransform)
        {
            return hitTransform.GetComponentInParent<IInspectable>();
        }

        private static IInteractable FindInteractable(Transform hitTransform)
        {
            return hitTransform.GetComponentInParent<IInteractable>();
        }

        private void CacheInputActions()
        {
            if (!PlayerInputActionsLoader.TryEnsureLoaded(ref _controls, this, nameof(PlayerInteractor)))
            {
                return;
            }

            InputActionMap playerMap = _controls.FindActionMap("Player", throwIfNotFound: false);
            if (playerMap == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerInteractor)} on '{gameObject.name}' could not find action map 'Player'.",
                    this);
                return;
            }

            _interactAction = playerMap.FindAction("Interact", throwIfNotFound: false);
            if (_interactAction == null)
            {
                Debug.LogWarning(
                    $"{nameof(PlayerInteractor)} on '{gameObject.name}' could not find action 'Interact'.",
                    this);
            }
        }
    }
}
