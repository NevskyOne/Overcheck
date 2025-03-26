using UnityEngine;
using Zenject;

public class RandomEventContainer : MonoBehaviour
{
    private QuizControl _quizControl;
    private EventBus _eventBus;
    
    [Inject]
    private void Initialize(EventBus eventBus, QuizControl quizControl)
    {
        _eventBus = eventBus;
        _quizControl = quizControl;
    }

    public void MathQuiz()
    {
        print("math");
        SceneMusic.State = MusicState.Math;
        _quizControl.StartQuiz(0);
        _eventBus.Invoke(new EventHasBeenInvoked(MathQuiz));
    }
    
    public void WiresQuiz()
    {
        print("wires");
        SceneMusic.State = MusicState.Wires;
        _quizControl.StartQuiz(1);
        _eventBus.Invoke(new EventHasBeenInvoked(WiresQuiz));
    }
    
    public void MorseQuiz()
    {
        print("morse");
        SceneMusic.State = MusicState.Morse;
        _quizControl.StartQuiz(2);
        _eventBus.Invoke(new EventHasBeenInvoked(MorseQuiz));
    }
}