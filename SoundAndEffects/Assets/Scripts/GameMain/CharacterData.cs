[System.Serializable]
public class CharacterData
{
    private string _userName;
    /// <summary>
    /// distance in meters
    /// </summary>
    private int _summaryDistance;
    private int _summaryScore;

    public CharacterData(string userName, int summaryDistance, int summaryScore)
    {
        _userName = userName;
        _summaryDistance = summaryDistance;
        _summaryScore = summaryScore;
    }

    public (string userName, int summaryDistance, int summaryScore) GetValues() => (_userName, _summaryDistance, _summaryScore);


    public override string ToString() => $"GameResult Name=\"{_userName}\" Distance={_summaryDistance} meters Score={_summaryScore}";
}

