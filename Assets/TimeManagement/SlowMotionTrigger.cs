using UnityEngine;

public static class SlowMotionTrigger
{

    public static void TriggerSlowMotion(float slowFactor, float slowDuration, AnimationCurve slowMotionCurve)
    {
        if (TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager instance not found in the scene.");
            return;
        }

        TimeManager.Instance.SlowDownTime(slowFactor, slowDuration, slowMotionCurve);
    }
}