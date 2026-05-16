using ADoorInsideTheDark.Player;
using ADoorInsideTheDark.Rooms;
using UnityEngine;

namespace ADoorInsideTheDark.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class HouseSwitchInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private HouseWithTheSwitchesController _controller;

        public void Interact(PlayerContext context)
        {
            if (_controller == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseSwitchInteractable)} on '{gameObject.name}' has no {nameof(HouseWithTheSwitchesController)} assigned.",
                    this);
                return;
            }

            _controller.UseOrdinarySwitch(context);
        }

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponentInParent<HouseWithTheSwitchesController>();
            }
        }

        private void OnValidate()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.LogWarning(
                    $"{nameof(HouseSwitchInteractable)} on '{gameObject.name}' requires a Collider for player interaction.",
                    this);
            }
        }
    }
}
