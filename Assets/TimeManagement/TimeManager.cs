using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    [SerializeField] public float slowMotionFactor = 0.1f;  // Factor by which time slows
    [SerializeField] public float slowMotionDuration = 1f; // Duration of slow motion
    private static TimeManager _instance;  // Private field storing the instance

    public static TimeManager Instance     // Public property for access
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TimeManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TimeManager");
                    _instance = go.AddComponent<TimeManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SlowDownTime(float factor, float duration, AnimationCurve slowMotionCurve)
    {
        slowMotionFactor = factor;
        slowMotionDuration = duration;

        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Update physics tick rate


        // Start the fall-off coroutine after the initial slow-motion duration
        StartCoroutine(SlowMotionFallOff(duration, slowMotionCurve));
    }

    private IEnumerator SlowMotionFallOff(float fallOffDuration, AnimationCurve slowMotionCurve)
    {
        float elapsed = 0.0f;

        while (elapsed < fallOffDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Track real-world time
            float curveValue = slowMotionCurve.Evaluate(elapsed / fallOffDuration); // Evaluate the curve

            // Smoothly transition time scale
            Time.timeScale = slowMotionFactor * curveValue;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Update physics tick rate

            yield return null;
        }

        // Ensure time scale resets to normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}

