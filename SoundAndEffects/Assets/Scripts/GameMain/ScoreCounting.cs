using System;
using System.Collections.Generic;

public static class ScoreCounting
{
    private readonly struct PassedObstacleWScore
    {
        public readonly int IDObstacle;
        public readonly int ScoreObstacle;

        public PassedObstacleWScore(int IDObstacle, int ScoreObstacle)
        {
            this.IDObstacle = IDObstacle;
            this.ScoreObstacle = ScoreObstacle;
        }
    }
    public static event Action<int> ScoreChanged;
    private static Stack<PassedObstacleWScore> passedObstaclesWScore = new Stack<PassedObstacleWScore>(10);
    private static GameParametersManager _gameParametersManager;
    public static int AllSum { get; private set; }

    public static void InitScoreCounting(GameParametersManager gameParametersManager)
    {
        _gameParametersManager = gameParametersManager;
    }

    public static void AddObstacleToStack(int IDObstacle, int ScoreObstacle)
    {
        passedObstaclesWScore.Push(new PassedObstacleWScore(IDObstacle,ScoreObstacle));
    }

    public static void JumpFinished()
    {
        //CountFrame.DebugLogFixedUpdate("-------------Jump Finished--------------");
        int sum = 0;
        foreach (PassedObstacleWScore item in passedObstaclesWScore)
        {
            //Debug.Log($"{item.Key} {item.Value}");
            sum += item.ScoreObstacle;
        }
        //CountFrame.DebugLogFixedUpdate($"[Jump Finished] Count={scorePassedObstacles.Count} sum={sum}");
        AllSum += sum * ((passedObstaclesWScore.Count > 1)? _gameParametersManager.Level + passedObstaclesWScore.Count : 1);
        ScoreChanged.Invoke(AllSum);
        passedObstaclesWScore.Clear();
    }

    public static void Reset()
    {
        AllSum = 0;
        ScoreChanged.Invoke(0);
    }

    public static void CollisionOccur(int IDObstacleCollided)
    {
        //UnityEngine.Debug.Log($"count={passedObstaclesWScore.Count}");
        if (passedObstaclesWScore.Count > 0)
        {
            //UnityEngine.Debug.Log($"before AllSum={AllSum}");
            PassedObstacleWScore lastPassedObstacle = passedObstaclesWScore.Peek();
            if (lastPassedObstacle.IDObstacle == IDObstacleCollided)
                passedObstaclesWScore.Pop();
            if (passedObstaclesWScore.Count > 0)
            {
                JumpFinished();
                //UnityEngine.Debug.Log($"after JumpFinished  AllSum={AllSum}");
            }
        }
    }
}

