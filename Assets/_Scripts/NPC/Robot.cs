
using System.Threading.Tasks;
using Random = System.Random;
using UnityEngine;
using UnityEngine.InputSystem;

public class Robot : MonoBehaviour
{
    private Random _rnd = new Random();
    private DialogSystem _dialogSystem => FindFirstObjectByType<DialogSystem>();
    private PlayerInput _input => FindFirstObjectByType<PlayerInput>();
    private RandomEvents _events => FindFirstObjectByType<RandomEvents>();
    
    public async void OnTriggerEnter(Collider sbj)
    {
        if (sbj.CompareTag("Player"))
        {
            sbj.transform.LookAt(transform);
            _input.enabled = false;
            
            var fragment = new DialogFragment
                { Text = RandomParamSt.RobotsReplics[_rnd.Next(RandomParamSt.RobotsReplics.Count)]};
            _dialogSystem.FragmentsStack = new() { fragment };
            _dialogSystem.PlayNext();
            
            await Task.Delay(3000);
            _dialogSystem.EndChat();
            _events.Lose();
            _input.enabled = true;
        }
    }
}
