using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1f;

    private Vector3 initialLocalPosition = new Vector3(0,0.7f,0);
    private Vector3 initialLocalEulerAngles;
    private Vector3 positionVelocity;
    private Vector3 rotationVelocity;
    private float currentLocalXRotation;

    private Coroutine currentCoroutine;
    private PlayerInteractions _playerInter;

    private void Start()
    {
        initialLocalEulerAngles = transform.localEulerAngles;
        _playerInter = FindFirstObjectByType<PlayerInteractions>();
    }

    public void MoveToTarget(Vector3 targetPosition, Vector3 targetLocalEulerAngles)
    {
        initialLocalEulerAngles = transform.localEulerAngles;

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(
            SmoothMove(targetPosition, targetLocalEulerAngles));
    }

    public void ResetCamera()
    {

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(LocalSmoothMove(initialLocalPosition, initialLocalEulerAngles));
    }

    private IEnumerator LocalSmoothMove(Vector3 targetPosition, Vector3 targetEulerAngles)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 0.4f)
        {
            elapsedTime += Time.deltaTime;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref positionVelocity, moveDuration);

            Vector3 smoothedRotation = new Vector3(
                Mathf.SmoothDampAngle(transform.localEulerAngles.x, targetEulerAngles.x, ref rotationVelocity.x, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetEulerAngles.y, ref rotationVelocity.y, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetEulerAngles.z, ref rotationVelocity.z, moveDuration)
            );

            transform.localEulerAngles = smoothedRotation;

            yield return null;
        }

        transform.localPosition = targetPosition;
        transform.localEulerAngles = targetEulerAngles;
        
        _playerInter.Focus();
    }
    
    private IEnumerator SmoothMove(Vector3 targetPosition, Vector3 targetEulerAngles)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration + 1.5f)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, moveDuration);

            Vector3 smoothedRotation = new Vector3(
                Mathf.SmoothDampAngle(transform.localEulerAngles.x, targetEulerAngles.x, ref rotationVelocity.x, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.y, targetEulerAngles.y, ref rotationVelocity.y, moveDuration),
                Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetEulerAngles.z, ref rotationVelocity.z, moveDuration)
            );

            transform.localEulerAngles = smoothedRotation;

            yield return null;
        }

        transform.position = targetPosition;
        transform.localEulerAngles = targetEulerAngles;
    }
    
}
