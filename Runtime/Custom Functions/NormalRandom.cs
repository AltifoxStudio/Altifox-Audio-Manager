using System;

namespace AltifoxStudio.AltifoxAudioManager
{
    public static class NormalRandom
{
    private static readonly Random _random = new Random();

    public static double Generate(double mean, double stdDev)
    {

        double u1 = _random.NextDouble();
        double u2 = _random.NextDouble();

        while (u1 == 0.0)
        {
            u1 = _random.NextDouble();
        }


        double stdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

        double normalValue = mean + stdDev * stdNormal;

        return normalValue;
    }
}
}
