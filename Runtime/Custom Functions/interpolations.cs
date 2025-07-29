using UnityEngine;
using System;

namespace AltifoxTools
{
    public static class AudioConstants
    {
        public const float SEMITONE_TO_PITCH_FACTOR = 1.05946f;
    }


    public class Tones
    {
        public static float PitchToSemitones(float pitch)
        {
            return Mathf.Pow(AudioConstants.SEMITONE_TO_PITCH_FACTOR, 1 / pitch);
        }

        public static float SemitonesToPitch(float semitones)
        {
            return Mathf.Pow(AudioConstants.SEMITONE_TO_PITCH_FACTOR, semitones);
        }
    }

    [Serializable]
    public enum InterpolationType
    {
        Linear,
        Smooth,
        Exponential,
        EaseInExponential,
        EaseOutExponential,
        EaseInSine,
        EaseOutSine,
        RoundedScale,
    }

    public class Interpolations
    {
        public static System.Func<Vector2, Vector2, float, float> GetInterpolationFuncRef(InterpolationType interpolationType)
        {
            System.Func<Vector2, Vector2, float, float> interpolationFunction;
            switch (interpolationType)
            {
                case InterpolationType.Smooth:
                    interpolationFunction = Smooth;
                    break;
                case InterpolationType.Exponential:
                    interpolationFunction = Exponential;
                    break;
                case InterpolationType.EaseInExponential:
                    interpolationFunction = EaseInExponential;
                    break;
                case InterpolationType.EaseOutExponential:
                    interpolationFunction = EaseOutExponential;
                    break;
                case InterpolationType.EaseInSine:
                    interpolationFunction = EaseInSine;
                    break;
                case InterpolationType.EaseOutSine:
                    interpolationFunction = EaseOutSine;
                    break;
                case InterpolationType.RoundedScale:
                    interpolationFunction = RoundedScale;
                    break;
                case InterpolationType.Linear:
                default:
                    interpolationFunction = Linear;
                    break;
            }
            return interpolationFunction;
        }


        private static float GetNormalizedPosition(Vector2 start, Vector2 stop, float position)
        {
            // Prevent division by zero if start and stop x-values are the same.
            if (Mathf.Approximately(start.x, stop.x))
            {
                return 0f; // No duration, so normalized position is 0.
            }
            float t = (position - start.x) / (stop.x - start.x);
            return Mathf.Clamp01(t);
        }

        public static float RoundedScale(Vector2 Start, Vector2 Stop, float Position)
        {
            return Mathf.Round(Linear(Start, Stop, Position));
        }

        public static float Linear(Vector2 Start, Vector2 Stop, float Position)
        {
            float relativePosition = GetNormalizedPosition(Start, Stop, Position);
            return Mathf.Lerp(Start.y, Stop.y, relativePosition);
        }

        public static float Smooth(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            return Mathf.Lerp(Start.y, Stop.y, smoothT);
        }

        public static float Exponential(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);

            // Ensure values are positive for logarithmic interpolation
            if (Start.y <= 0f || Stop.y <= 0f)
            {
                Debug.LogWarning("Exponential interpolation requires positive start and stop values. Clamping to a small positive value.");
                // Handle cases where values might be zero or negative, e.g., clamp to a small positive epsilon.
                // For audio gain, a very small positive value (e.g., 0.00001f) represents silence.
                float clampedStartY = Mathf.Max(Start.y, 0.00001f);
                float clampedStopY = Mathf.Max(Stop.y, 0.00001f);
                return Mathf.Exp(Mathf.Lerp(Mathf.Log(clampedStartY), Mathf.Log(clampedStopY), t));
            }

            // Standard exponential interpolation using log/exp
            return Mathf.Exp(Mathf.Lerp(Mathf.Log(Start.y), Mathf.Log(Stop.y), t));
        }

        /// <summary>
        /// Starts extremely slow and accelerates very fast at the end.
        /// </summary>
        public static float EaseOutExponential(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);

            // Handle the edge case to ensure it starts exactly at the beginning.
            if (t == 0)
            {
                return Start.y;
            }

            // This formula creates a dramatic exponential ease-in curve.
            float curvedT = Mathf.Pow(2, 10 * (t - 1));

            return Mathf.Lerp(Start.y, Stop.y, curvedT);
        }

        /// <summary>
        /// Starts very fast and decelerates toward the end.
        /// </summary>
        public static float EaseInExponential(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);

            // Handle the edge case to ensure it ends exactly at the final value.
            if (t == 1)
            {
                return Stop.y;
            }

            // This formula creates a dramatic exponential ease-out curve.
            float curvedT = 1f - Mathf.Pow(2, -10 * t);

            return Mathf.Lerp(Start.y, Stop.y, curvedT);
        }

        public static float EaseOutSine(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);
            // This formula creates a curve that starts slow and ends fast.
            float curvedT = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            return Mathf.Lerp(Start.y, Stop.y, curvedT);
        }

        public static float EaseInSine(Vector2 Start, Vector2 Stop, float Position)
        {
            float t = GetNormalizedPosition(Start, Stop, Position);
            // This formula creates a curve that starts fast and ends slow.
            float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            return Mathf.Lerp(Start.y, Stop.y, curvedT);
        }


    }
}