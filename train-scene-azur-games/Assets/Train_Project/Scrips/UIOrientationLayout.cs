using System;
using UnityEngine;

public class UIOrientationLayout : MonoBehaviour
{
    [Serializable]
    public class LayoutItem
    {
        public RectTransform target;

        [Header("Portrait")]
        public Vector2 portraitAnchoredPos = Vector2.zero;
        public Vector3 portraitScale = Vector3.one;
        public Vector2 portraitSizeDelta = Vector2.zero;
        public bool portraitOverrideSizeDelta = false;

        [Header("Landscape")]
        public Vector2 landscapeAnchoredPos = Vector2.zero;
        public Vector3 landscapeScale = Vector3.one;
        public Vector2 landscapeSizeDelta = Vector2.zero;
        public bool landscapeOverrideSizeDelta = false;
    }

    [Tooltip("If true, landscape is chosen when aspect >= threshold (width/height). 1.0 means square.")]
    public float landscapeAspectThreshold = 1.0f;

    public LayoutItem[] items;

    private bool _isLandscapeApplied;
    private int _lastW;
    private int _lastH;

    private void Start()
    {
        ApplyForCurrentAspect(force: true);
    }

    private void Update()
    {
        // In Editor/GameView resolution can change without orientation events.
        if (Screen.width != _lastW || Screen.height != _lastH)
        {
            ApplyForCurrentAspect(force: true);
        }
    }

    private void ApplyForCurrentAspect(bool force)
    {
        _lastW = Screen.width;
        _lastH = Screen.height;

        float aspect = (Screen.height == 0) ? 0f : (float)Screen.width / Screen.height;
        bool isLandscape = aspect >= landscapeAspectThreshold;

        if (!force && isLandscape == _isLandscapeApplied) return;

        _isLandscapeApplied = isLandscape;

        foreach (var item in items)
        {
            if (item == null || item.target == null) continue;

            if (isLandscape)
            {
                item.target.anchoredPosition = item.landscapeAnchoredPos;
                item.target.localScale = item.landscapeScale;

                if (item.landscapeOverrideSizeDelta)
                    item.target.sizeDelta = item.landscapeSizeDelta;
            }
            else
            {
                item.target.anchoredPosition = item.portraitAnchoredPos;
                item.target.localScale = item.portraitScale;

                if (item.portraitOverrideSizeDelta)
                    item.target.sizeDelta = item.portraitSizeDelta;
            }
        }
    }
}