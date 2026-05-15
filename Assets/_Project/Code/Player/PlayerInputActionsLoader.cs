using UnityEngine;
using UnityEngine.InputSystem;

namespace ADoorInsideTheDark.Player
{
    internal static class PlayerInputActionsLoader
    {
        private const string ResourceName = "ADITDControls";

        internal static bool TryEnsureLoaded(ref InputActionAsset controls, MonoBehaviour host, string owningTypeName)
        {
            if (controls != null)
            {
                return true;
            }

            controls = Resources.Load<InputActionAsset>(ResourceName);
            if (controls == null)
            {
                Debug.LogWarning(
                    $"{owningTypeName} on '{host.gameObject.name}': assignment {nameof(InputActionAsset)} " +
                    $"is missing and could not Resources.Load<InputActionAsset>(\"{ResourceName}\"). Ensure the asset " +
                    $"lives under a folder named Resources and is saved as `{ResourceName}.inputactions`.",
                    host);
                return false;
            }

            return true;
        }
    }
}
