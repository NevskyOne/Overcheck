using UnityEngine;
using Zenject;

public class RandomEventContainer : MonoBehaviour
{
    [SerializeField] private GameObject _mathQuiz;
    [SerializeField] private GameObject _wiresQuiz;
    [SerializeField] private GameObject _morzeQuiz;
    
    private EventBus _eventBus;
    
    [Inject]
    private void Initialize(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void MathQuiz()
    {
        _mathQuiz.SetActive(true);
        _eventBus.Invoke(new EventHasBeenInvoked(MathQuiz));
    }
    
    public void WiresQuiz()
    {
        _wiresQuiz.SetActive(true);
        _eventBus.Invoke(new EventHasBeenInvoked(WiresQuiz));
    }
    
    public void MorzeQuiz()
    {
        _morzeQuiz.SetActive(true);
        _eventBus.Invoke(new EventHasBeenInvoked(MorzeQuiz));
    }
}