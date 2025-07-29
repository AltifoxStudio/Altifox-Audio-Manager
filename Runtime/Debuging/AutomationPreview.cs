using UnityEngine;
using AltifoxTools; // Your namespace

public class AutomationPreviewer : MonoBehaviour
{
    [Header("Generation Source")]
    public InterpolationType sourceType;
    public Vector2 startPoint = new Vector2(0, 0);
    public Vector2 endPoint = new Vector2(1, 1);
    
    [Range(2, 100)]
    public int resolution = 20; // Number of keys to generate

    [Header("Output")]
    [Tooltip("The generated curve will be stored here.")]
    public AnimationCurve targetCurve;

    /// <summary>
    /// This method generates the keyframes based on the selected interpolation.
    /// </summary>
    public void GenerateCurve()
    {
        // Start with a fresh curve
        targetCurve = new AnimationCurve();

        // Generate the keys based on the resolution
        for (int i = 0; i <= resolution; i++)
        {
            float percentage = (float)i / resolution;
            // The 'position' on the x-axis for the interpolation function
            float positionX = Mathf.Lerp(startPoint.x, endPoint.x, percentage);

            float valueY = 0f;
            switch (sourceType)
            {
                case InterpolationType.Linear:
                    valueY = Interpolations.Linear(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.Smooth:
                    valueY = Interpolations.Smooth(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.Exponential:
                    valueY = Interpolations.Exponential(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.EaseInExponential:
                    valueY = Interpolations.EaseInExponential(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.EaseOutExponential:
                    valueY = Interpolations.EaseOutExponential(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.EaseInSine:
                    valueY = Interpolations.EaseInSine(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.RoundedScale:
                    valueY = Interpolations.RoundedScale(startPoint, endPoint, positionX);
                    break;
                case InterpolationType.EaseOutSine:
                    valueY = Interpolations.EaseOutSine(startPoint, endPoint, positionX);
                    break;
            }
            
            // The time for the keyframe is its position on the graph's x-axis
            float keyTime = positionX;
            var key = new Keyframe(keyTime, valueY);
            targetCurve.AddKey(key);
        }

        // Optional: Smooth out all the tangents for a cleaner look in the editor
        for (int i = 0; i < targetCurve.length; i++)
        {
            targetCurve.SmoothTangents(i, 0f);
        }
    }
}