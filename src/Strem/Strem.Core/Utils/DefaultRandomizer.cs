namespace Strem.Core.Utils;

public class DefaultRandomizer : IRandomizer
{
    public Random NativeRandomizer { get; }

    public DefaultRandomizer(Random random)
    { NativeRandomizer = random; }
    
    public DefaultRandomizer(int seed)
    { NativeRandomizer = new Random(seed); }

    public int Random(int min, int max)
    { return NativeRandomizer.Next(min, max + 1); }

    public float Random(float min, float max)
    { return (float)NativeRandomizer.NextDouble() * (max - min) + min; }
}