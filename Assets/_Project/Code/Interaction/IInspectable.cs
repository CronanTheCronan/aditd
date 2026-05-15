namespace ADoorInsideTheDark.Interaction
{
    /// <summary>
    /// Supplies UI text for the inspection panel. Kept separate from <see cref="IInteractable"/>
    /// so interact-only objects stay lightweight.
    /// </summary>
    public interface IInspectable
    {
        string InspectionTitle { get; }
        string InspectionBody { get; }
    }
}
