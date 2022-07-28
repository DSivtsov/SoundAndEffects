using UnityEngine;

public static class UnitSystem
{
    public enum Distance
    {
        m,
        ft
    }

    public static Distance Current { get; private set; } = Distance.ft;

    public const float OneFootInMeter = 0.3048f;
    /// <summary>
    /// Convert from Meter to Foot
    /// </summary>
    /// <param name="distanceM">distance in meters</param>
    /// <returns></returns>
    public static int Convert(int distanceM) => (Current == Distance.ft) ? Mathf.RoundToInt(distanceM / OneFootInMeter) : distanceM;
}
