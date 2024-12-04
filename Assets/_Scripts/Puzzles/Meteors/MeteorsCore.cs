using UnityEngine;

public class MeteorsCore : MonoBehaviour
{
    private RandomEvents _randomEvents;
    private bool _isPuzzleSolved = false;

    private void Start()
    {
        _randomEvents = FindFirstObjectByType<RandomEvents>();
    }

    public void StartPuzzle()
    {
        
    }
}