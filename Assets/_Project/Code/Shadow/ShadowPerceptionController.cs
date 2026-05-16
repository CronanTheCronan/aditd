using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Shadow
{
    public sealed class ShadowPerceptionController : MonoBehaviour
    {
        [Header("Temporary Wave 008A Input")]
        [SerializeField] private Key _temporaryActivationKey = Key.Q;
        [SerializeField] private string _temporaryBindingNote = "Temporary local binding for Wave 008A. Route Shadow perception through Input Actions later.";

        private readonly List<IShadowRevealable> _registeredRevealables = new();

        public bool IsPerceptionActive { get; private set; }

        public event Action<bool> PerceptionStateChanged;

        private void Update()
        {
            // Wave 008A is key-state only, so there is no extra per-frame raycast/query cost here yet.
            bool shouldBeActive = Keyboard.current != null && Keyboard.current[_temporaryActivationKey].isPressed;
            if (shouldBeActive == IsPerceptionActive)
            {
                return;
            }

            SetPerceptionActive(shouldBeActive);
        }

        private void OnDisable()
        {
            if (IsPerceptionActive)
            {
                SetPerceptionActive(false);
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_temporaryBindingNote))
            {
                _temporaryBindingNote = "Temporary local binding for Wave 008A. Route Shadow perception through Input Actions later.";
            }
        }

        public void RegisterRevealable(IShadowRevealable revealable)
        {
            if (revealable == null || _registeredRevealables.Contains(revealable))
            {
                return;
            }

            _registeredRevealables.Add(revealable);
            revealable.SetShadowPerceptionActive(IsPerceptionActive);
        }

        public void UnregisterRevealable(IShadowRevealable revealable)
        {
            if (revealable == null)
            {
                return;
            }

            _registeredRevealables.Remove(revealable);
        }

        private void SetPerceptionActive(bool isActive)
        {
            IsPerceptionActive = isActive;

            for (int i = 0; i < _registeredRevealables.Count; i++)
            {
                _registeredRevealables[i]?.SetShadowPerceptionActive(isActive);
            }

            PerceptionStateChanged?.Invoke(isActive);
        }
    }
}
