using System.Collections;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1f;

    private Vector3 initialLocalPosition = new Vector3(0,0.7f,0);
    private Vector3 initialLocalEulerAngles;
    private Vector3 positionVelocity;
    private Vector3 rotationVelocity;
    private float currentLocalXRotation;

    private Coroutine _currentCoroutine, _currentRotateRoutine;
    private PlayerInteractions _playerInter;
    
    private void Start()
    {
        initialLocalEulerAngles = transform.localEulerAngles;
        _playerInter = Player.Interactions;
    }

    public void MoveToTarget(Vector3 targetPosition, Vector3 targetLocalEulerAngles )
    {
        initialLocalEulerAngles = transform.localEulerAngles;
        initialLocalEulerAngles.z = 0;
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

        while (elapsedTime < moveDuration + 0.4f)
        {
            elapsedTime += Time.deltaTime;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref positionVelocity, moveDuration);
            yield return null;
        }

        transform.localPosition = targetPosition;
        _playerInter.Focus();
    }
    
    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        StopCoroutine(nameof(LocalSmoothMove));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 1.5f)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, moveDuration);
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator SmoothRotate(Vector3 targetEulerAngles)
    {
        StopCoroutine(nameof(SmoothRotate));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 1.5f)
        {
            elapsedTime += Time.deltaTime;

            Vector3 smoothedRotation = new Vector3(
                Mathf.SmoothDampAngle(transform.localEulerAngles.x, targetEulerAngles.x, ref rotationVelocity.x, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetEulerAngles.y, ref rotationVelocity.y, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetEulerAngles.z, ref rotationVelocity.z, moveDuration)
            );

            transform.localEulerAngles = smoothedRotation;

            yield return null;
        }

        transform.localEulerAngles = targetEulerAngles;
    }
}
