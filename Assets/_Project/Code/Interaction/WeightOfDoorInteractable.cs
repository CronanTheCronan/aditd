using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.Rooms;
using UnityEngine;

namespace ADoorInsideTheDark.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeightOfDoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private WeightOfDoorController _controller;

        public void Interact(PlayerContext context)
        {
            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorInteractable)} on '{gameObject.name}' has no {nameof(WeightOfDoorController)} assigned.",
                    this);
                return;
            }

            _controller.UseDoor(context);
        }

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponentInParent<WeightOfDoorController>();
            }

            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorInteractable)} on '{gameObject.name}' could not find a {nameof(WeightOfDoorController)} during {nameof(Awake)}.",
                    this);
            }
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(WeightOfDoorInteractable)} on '{gameObject.name}' requires a Collider for player interaction.",
                    this);
            }
        }
    }
}
