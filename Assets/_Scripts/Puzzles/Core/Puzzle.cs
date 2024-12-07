using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    private RandomEvents _randomEvents;
    protected bool _isPuzzleSolved;

    private void OnEnable()
    {
        _randomEvents = FindFirstObjectByType<RandomEvents>();
        StartPuzzle();
    }
    
    public abstract void StartPuzzle();

    protected virtual void LosePuzzle()
    {
        _isPuzzleSolved = false;
        _randomEvents.Lose();
    }

    protected virtual void SolvePuzzle()
    {
        _isPuzzleSolved = true;
        _randomEvents.OnEventEnd();
    }
}