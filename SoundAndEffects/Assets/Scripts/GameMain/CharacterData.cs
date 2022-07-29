using UnityEngine;

[System.Serializable]
public class CharacterData : System.IComparable<CharacterData>
{
    [SerializeField] private string _userName;
    /// <summary>
    /// distance in meters
    /// </summary>
    [SerializeField] private int _summaryDistance;
    [SerializeField] private int _summaryScore;

    public CharacterData(string userName, int summaryDistance, int summaryScore)
    {
        _userName = userName;
        _summaryDistance = summaryDistance;
        _summaryScore = summaryScore;
    }
    //Sorting in descending order by Score
    public int CompareTo(CharacterData other) => other._summaryScore - _summaryScore;

    public (string userName, int summaryDistance, int summaryScore) GetValues() => (_userName, _summaryDistance, _summaryScore);


    public override string ToString() => $"GameResult Name=\"{_userName}\" Distance={_summaryDistance} meters Score={_summaryScore}";
}

