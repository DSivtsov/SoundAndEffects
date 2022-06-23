using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Used to count and check the deltaTime, and to compare difference at Moving the class MoveObstacles and MoveBackGround
/// </summary>
public class TraceObjectMove
{
    private static int countCycleUpdate = 0;
    private static int countCycleFixedUpdate = 0;

    private Transform transform;
    private string nameCycle;
    private float oldX;
    private double oldtime;
    private double newTime;

    [Conditional("TRACEON")]
    public void InterateUpdate() => countCycleUpdate++;
    [Conditional("TRACEON")]
    public void InterateFixedUpdate() => countCycleFixedUpdate++;

    public TraceObjectMove(string nameCycle, Transform transform)
    {
        this.nameCycle = nameCycle;
        this.transform = transform;
    }

    public enum TypeCycle
    {
        Update,
        FixedUpdate
    }
    
    [Conditional("TRACEON")]
    public void ShowDeltaXAndDeltaTime(TypeCycle type)
    {
        int countCycle = 0;
        switch (type)
        {
            case TypeCycle.Update:
                countCycle = countCycleUpdate;
                break;
            case TypeCycle.FixedUpdate:
                countCycle = countCycleFixedUpdate;
                break;
        }

        if (oldX == 0)
        {
            oldX = transform.position.x;
            oldtime = Time.timeAsDouble;
            UnityEngine.Debug.Log($"{nameCycle}({countCycle}) X={oldX:F4} time={oldtime:F4}");
        }
        else
        {
            newTime = Time.timeAsDouble;
            UnityEngine.Debug.Log($"{nameCycle}({countCycle}) X={transform.position.x:F4} deltaX={(oldX - transform.position.x ):F4} time={newTime:F4}" +
                    $" deltaTime={(newTime - oldtime):F4}");
            oldX = transform.position.x;
            oldtime = newTime;
        }
    }
}
