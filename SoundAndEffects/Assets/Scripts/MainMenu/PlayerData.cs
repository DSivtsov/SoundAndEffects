using UnityEngine;

[System.Serializable]
public class PlayerData : System.IComparable<PlayerData>
{
    [SerializeField] private string _userName;
    /// <summary>
    /// distance in meters
    /// </summary>
    [SerializeField] private int _summaryDistance;
    [SerializeField] private int _summaryScore;

    public PlayerData(string userName, int summaryDistance, int summaryScore)
    {
        _userName = userName;
        _summaryDistance = summaryDistance;
        _summaryScore = summaryScore;
    }
    //Sorting in descending order by Score
    public int CompareTo(PlayerData other) => other._summaryScore - _summaryScore;

    public (string userName, int summaryDistance, int summaryScore) GetValues() => (_userName, _summaryDistance, _summaryScore);

    public int GetScoreValue() => _summaryScore;

    public override string ToString() => $"GameResult Name=\"{_userName}\" Distance={_summaryDistance} meters Score={_summaryScore}";
}

