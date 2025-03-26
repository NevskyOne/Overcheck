using System.Collections;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.2f;
    [SerializeField] private float rotateDuration = 0.1f;
    [SerializeField] private Vector3 initialLocalPosition = new Vector3(0,0.66f,0);
    [SerializeField] private Transform _rotateEmpty;
    private Vector3 initialLocalEulerAngles;
    private Vector3 positionVelocity;
    private float rotationVelocityX, rotationVelocityY, rotationVelocityZ;
    private float currentLocalXRotation;

    private Coroutine _currentCoroutine, _currentRotateRoutine;
    private Player _player;
    
    [Inject]
    private void Initialize(Player player)
    {
        initialLocalEulerAngles = transform.localEulerAngles;
        _player = player;
    }

    public void MoveToTarget(Vector3 targetPosition, Vector3 targetLocalEulerAngles, Vector3 hitPos)
    {
        _rotateEmpty.LookAt(hitPos);
        initialLocalEulerAngles = new Vector3(_rotateEmpty.localEulerAngles.x, 0,0);
        Player.Movement.CamRot = initialLocalEulerAngles;
        
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        if (_currentRotateRoutine != null) StopCoroutine(_currentRotateRoutine);
        if(targetPosition != Vector3.zero)
            _currentCoroutine = StartCoroutine(SmoothMove(targetPosition));
        _currentRotateRoutine = StartCoroutine(SmoothRotate(targetLocalEulerAngles));
    }

    public void ResetCamera()
    {

        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        if (_currentRotateRoutine != null) StopCoroutine(_currentRotateRoutine);
        _currentCoroutine = StartCoroutine(LocalSmoothMove(initialLocalPosition));
        _currentRotateRoutine = StartCoroutine(SmoothRotate(initialLocalEulerAngles));
    }

    private IEnumerator LocalSmoothMove(Vector3 targetPosition)
    {
        StopCoroutine(nameof(SmoothMove));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 0.5f)
        {
            elapsedTime += Time.fixedDeltaTime;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref positionVelocity, moveDuration);
            yield return new WaitForFixedUpdate();
        }

        transform.localPosition = targetPosition;
        Player.Interactions.Focus();
        Player.State = PlayerState.Movement;
    }
    
    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        StopCoroutine(nameof(LocalSmoothMove));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 0.5f)
        {
            elapsedTime += Time.fixedDeltaTime;

            transform.position =
                Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, moveDuration);
            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPosition;
    }

    private IEnumerator SmoothRotate(Vector3 targetEulerAngles)
    {
        StopCoroutine(nameof(SmoothRotate));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 0.5f)
        {
            elapsedTime += Time.fixedDeltaTime;

            Vector3 smoothedRotation = new Vector3(
                Mathf.SmoothDampAngle(transform.localEulerAngles.x, targetEulerAngles.x, ref rotationVelocityX, rotateDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetEulerAngles.y, ref rotationVelocityY, rotateDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetEulerAngles.z, ref rotationVelocityZ, rotateDuration)
            );

            transform.localEulerAngles = smoothedRotation;

            yield return new WaitForFixedUpdate();
        }

        transform.localEulerAngles = targetEulerAngles;
    }
}
