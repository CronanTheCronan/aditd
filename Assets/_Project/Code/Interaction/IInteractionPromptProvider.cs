using ADoorInsideTheDark.Player;

namespace ADoorInsideTheDark.Interaction
{
    /// <summary>
    /// Allows interactables to override the default prompt text without teaching
    /// the player interactor about puzzle-specific behavior.
    /// </summary>
    public interface IInteractionPromptProvider
    {
        string GetInteractionPrompt(PlayerContext context);
    }
}
