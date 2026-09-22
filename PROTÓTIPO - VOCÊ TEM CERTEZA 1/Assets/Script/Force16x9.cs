using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Force16x9 : MonoBehaviour
{
    private Camera cam;
    private int lastWidth;
    private int lastHeight;

    private const float TargetAspect = 16f / 9f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyAspect();
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            ApplyAspect();
        }
    }

    private void ApplyAspect()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        if (lastHeight <= 0)
            return;

        float windowAspect = (float)lastWidth / lastHeight;
        float scaleHeight = windowAspect / TargetAspect;

        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (scaleHeight < 1f)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }

        cam.rect = rect;
    }
}