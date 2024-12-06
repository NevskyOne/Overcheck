using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    private RandomEvents _randomEvents;
    protected bool _isPuzzleSolved = false;

    private void Start()
    {
        _randomEvents = FindFirstObjectByType<RandomEvents>();
        StartPuzzle();
    }
    
    public abstract void StartPuzzle();
    protected abstract void LosePuzzle();

    protected virtual void SolvePuzzle()
    {
        //_isPuzzleSolved = true;
        //_randomEvents.OnEventEnd();
        Debug.Log("Головоломка решена!");
    }
}