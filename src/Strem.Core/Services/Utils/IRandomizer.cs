namespace Strem.Core.Services.Utils;

public interface IRandomizer
{
    /// <summary>
    /// Generates a random integer between min/max (max is inclusive)
    /// </summary>
    /// <param name="min">The minimum number</param>
    /// <param name="max">The maximum number (Inclusive)</param>
    /// <returns>A random integer between the range</returns>
    int Random(int min, int max);
    
    /// <summary>
    /// Generates a random float between the min/max
    /// </summary>
    /// <param name="min">The minimum number</param>
    /// <param name="max">The maximum number</param>
    /// <returns>A number within the given range</returns>
    float Random(float min = 0, float max = 1.0f);
}