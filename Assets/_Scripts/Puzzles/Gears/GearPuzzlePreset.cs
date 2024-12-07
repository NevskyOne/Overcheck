using System.Collections.Generic;
using UnityEngine;

public class GearPuzzlePreset : MonoBehaviour
{
    [SerializeField] private List<Node> _nodes = new();
    [SerializeField] private List<Gear> _gears = new();

    public void Initialize(Canvas canvas)
    {
        foreach (var gear in _gears)
        {
            gear.Init(canvas);
        }
    }

    public bool IsCorrect()
    {
        bool result = true, allSigned = true;
        
        foreach (var gear in _gears)
        {
            if (gear.CurrentNode == null)
            {
                result = false;
                allSigned = false;
                break;
            }

            if (gear.CurrentNode.Size != gear.Size)
                result = false;
        }

        if (allSigned && !result)
        {
            foreach (var gear in _gears) gear.ResetPos();
        }
        return result;
    }

    public void ResetGears()
    {
        foreach (var gear in _gears)
        {
            gear.ResetPos();
        }
    }
}