using System;
using Genkit;
using XRL;
using XRL.Core;

public class RandomUtils
{
    private static readonly Random INSTANCE = GetSeededRandom("LaurusDepths");

    private static Random GetSeededRandom(string seed)
    {
        if (XRLCore.Core.Game == null)
        {
            return new Random();
        }

        int hash = Hash.String(XRLCore.Core.Game.GetWorldSeed(null) + seed);
        return new Random(hash);
    }

    /// <summary>
    /// Returns a random integer between min (inclusive) and max (inclusive).
    /// </summary>
    public static int NextInt(int min, int max)
    {
        return INSTANCE.Next(min, max + 1);
    }

    /// <summary>
    /// Returns a random integer between 0 (inclusive) and max (inclusive).
    /// </summary>
    public static int NextInt(int max)
    {
        return INSTANCE.Next(0, max + 1);
    }

    /// <summary>
    /// Returns a random integer between min (inclusive) and max (inclusive),
    /// with a bias towards values greater than or less than the biasPoint.
    /// </summary>
    public static int NextIntWeighted(int min, int max, int biasPoint)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max.");
        if (biasPoint < min || biasPoint > max)
            throw new ArgumentException("biasPoint must be within min and max.");

        double u1 = 1.0 - INSTANCE.NextDouble();
        double u2 = 1.0 - INSTANCE.NextDouble();

        // Box-Muller transform for normal distribution
        double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

        // Rescale to [0,1]
        double scaled = (normal + 1) / 2.0;

        // Adjust skew based on biasPoint
        double biasFactor = (double)(biasPoint - min) / (max - min);
        scaled = Math.Pow(scaled, biasFactor < 0.5 ? 1.0 - biasFactor : biasFactor);

        // Map to the range
        int result = (int)Math.Round(min + scaled * (max - min));

        return Math.Max(min, Math.Min(max, result));
    }

    /// <summary>
    /// Returns a random float between min (inclusive) and max (inclusive).
    /// </summary>
    public static float NextFloat(float min, float max)
    {
        return (float)(INSTANCE.NextDouble() * (max - min) + min);
    }

    /// <summary>
    /// Returns a random float between min (inclusive) and max (inclusive),
    /// with a bias towards values greater than or less than the biasPoint.
    /// </summary>
    public static float NextFloatWeighted(float min, float max, float biasPoint)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max.");
        if (biasPoint < min || biasPoint > max)
            throw new ArgumentException("biasPoint must be within min and max.");

        double u1 = 1.0 - INSTANCE.NextDouble();
        double u2 = 1.0 - INSTANCE.NextDouble();

        // Box-Muller transform for normal distribution
        double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

        // Rescale to [0,1]
        double scaled = (normal + 1) / 2.0;

        // Adjust skew based on biasPoint
        double biasFactor = (biasPoint - min) / (max - min);
        scaled = Math.Pow(scaled, biasFactor < 0.5 ? 1.0 - biasFactor : biasFactor);

        // Map to the range
        float result = min + (float)(scaled * (max - min));

        return Math.Max(min, Math.Min(max, result));
    }

    /// <summary>
    /// Returns a random boolean.
    /// </summary>
    public static bool NextBool()
    {
        return INSTANCE.Next(2) == 0;
    }

    /// <summary>
    /// Returns a random element from the given array.
    /// </summary>
    public static T PickRandom<T>(T[] array)
    {
        if (array == null || array.Length == 0)
            throw new ArgumentException("Array cannot be null or empty.");

        return array[INSTANCE.Next(array.Length)];
    }

    /// <summary>
    /// Returns a random element from the given list.
    /// </summary>
    public static T PickRandom<T>(System.Collections.Generic.List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List cannot be null or empty.");

        return list[INSTANCE.Next(list.Count)];
    }

    /// <summary>
    /// Returns a random float between 0.0 (inclusive) and 1.0 (inclusive).
    /// </summary>
    public static float NextFloat()
    {
        return (float)INSTANCE.NextDouble();
    }

    /// <summary>
    /// Returns a random sign (-1 or 1).
    /// </summary>
    public static int NextSign()
    {
        return INSTANCE.Next(2) * 2 - 1;
    }

    /// <summary>
    /// Returns a random Gaussian-distributed float with mean 0 and standard deviation 1.
    /// </summary>
    public static float NextGaussian()
    {
        double u1 = 1.0 - INSTANCE.NextDouble();
        double u2 = 1.0 - INSTANCE.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return (float)randStdNormal;
    }

    public static int GetSeedForTerrainNoise()
    {
        return NextInt(short.MaxValue);
    }
}
