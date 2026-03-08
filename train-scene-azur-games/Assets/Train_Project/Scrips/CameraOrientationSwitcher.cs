using UnityEngine;

public class CameraOrientationSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera portraitCamera;
    public Camera landscapeCamera;

    [Header("Aspect Settings")]
    public float landscapeAspectThreshold = 1.0f;

    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.Space;
    public bool allowManualToggle = true;
    public bool autoSwitchByAspect = true;

    [Header("Orientation")]
    public bool forceScreenOrientation = true;

    [Tooltip("When true, lock to portrait/landscape on toggle. Useful for mobile builds.")]
    public bool lockOrientationOnToggle = true;

    [Header("Debug")]
    public bool logSwitches = false;

    private bool _manualOverride = false;
    private bool _manualLandscape = false;
    private int _lastW;
    private int _lastH;

    private void Start()
    {
        _lastW = Screen.width;
        _lastH = Screen.height;
        ApplyByAspect(force: true);
    }

    private void Update()
    {
        if (allowManualToggle && Input.GetKeyDown(toggleKey))
        {
            _manualOverride = true;
            _manualLandscape = !_manualLandscape;
            Apply(_manualLandscape, setOrientation: true);

            if (logSwitches)
                Debug.Log($"[CameraOrientationSwitcher] Manual toggle -> {(_manualLandscape ? "Landscape" : "Portrait")}");
        }

        if (!autoSwitchByAspect || _manualOverride) return;

        if (Screen.width != _lastW || Screen.height != _lastH)
        {
            _lastW = Screen.width;
            _lastH = Screen.height;
            ApplyByAspect(force: true);
        }
    }

    public void ClearManualOverride()
    {
        _manualOverride = false;
        ApplyByAspect(force: true);
        if (logSwitches) Debug.Log("[CameraOrientationSwitcher] Manual override cleared");
    }

    private void ApplyByAspect(bool force)
    {
        float aspect = (Screen.height == 0) ? 0f : (float)Screen.width / Screen.height;
        bool wantLandscape = aspect >= landscapeAspectThreshold;
        Apply(wantLandscape, setOrientation: false);

        if (logSwitches)
            Debug.Log($"[CameraOrientationSwitcher] Auto switch by aspect {aspect:F2} -> {(wantLandscape ? "Landscape" : "Portrait")}");
    }

    private void Apply(bool useLandscape, bool setOrientation)
    {
        if (portraitCamera == null || landscapeCamera == null)
        {
            Debug.LogWarning("[CameraOrientationSwitcher] Assign both cameras in the inspector.");
            return;
        }

        portraitCamera.enabled = !useLandscape;
        landscapeCamera.enabled = useLandscape;

        // Ensure only one AudioListener
        var pListener = portraitCamera.GetComponent<AudioListener>();
        var lListener = landscapeCamera.GetComponent<AudioListener>();
        if (pListener != null) pListener.enabled = !useLandscape;
        if (lListener != null) lListener.enabled = useLandscape;

        if (!forceScreenOrientation || !setOrientation) return;

        if (lockOrientationOnToggle)
        {
            Screen.orientation = useLandscape ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = !useLandscape;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = useLandscape;
            Screen.autorotateToLandscapeRight = useLandscape;
        }
    }
}