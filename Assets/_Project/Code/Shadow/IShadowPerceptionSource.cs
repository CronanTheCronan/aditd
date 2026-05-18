using System;

namespace ADoorInsideTheDark.Shadow
{
    public interface IShadowPerceptionSource
    {
        bool IsPerceptionActive { get; }
        event Action<bool> PerceptionStateChanged;
    }
}
