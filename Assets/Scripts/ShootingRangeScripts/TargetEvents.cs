using System;

// Broadcasts target actions so the score and timer can respond independently.
public static class TargetEvents
{
    public static event Action OnScoreTargetHit;
    public static event Action OnPenaltyTargetHit;
    public static event Action<bool> OnTargetMissed; // bool = isPenaltyTarget
    public static event Action<float> OnTimePenaltyTriggered; // float = seconds to deduct

    // Publish each event to any listeners registered for it.
    public static void TriggerScoreHit() => OnScoreTargetHit?.Invoke();
    public static void TriggerPenaltyHit() => OnPenaltyTargetHit?.Invoke();
    public static void TriggerTargetMissed(bool isPenaltyTarget) => OnTargetMissed?.Invoke(isPenaltyTarget);
    public static void TriggerTimePenalty(float seconds) => OnTimePenaltyTriggered?.Invoke(seconds);
}