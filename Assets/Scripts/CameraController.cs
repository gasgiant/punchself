using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Transform cameraTransform;
    public float dampTime = 0.1f;
    public float zCoord = -10;
    public float yCoord = 0;

    public AnimationCurve randomShakeMoveCurve;
    public AnimationCurve randomShakeDurationeCurve;
    public AnimationCurve inOutShakeCurve;

    public static CameraController instance;
    Transform tr;

    Vector3 current_center;
    Vector3 vel;

    Vector3 shakeOffsest;
    Vector3 directionalOffsest;
    Quaternion shakeRotation = Quaternion.Euler(0, 0, 0);
    Coroutine shakeRoutine;
    Coroutine directionalshakeRoutine;
    int currentShakeOrder;


    void Awake()
    {
        instance = this;
        tr = transform;
        current_center = transform.position;
    }

    void Update()
    {
        Vector3 desired_center = new Vector3(target.position.x, yCoord, zCoord);
        desired_center = desired_center + desired_center.z * Vector3.forward;
        current_center = Vector3.SmoothDamp(current_center, desired_center, ref vel, dampTime);
        tr.position = current_center;
        cameraTransform.localRotation = shakeRotation;
        cameraTransform.position = tr.position + shakeOffsest + directionalOffsest;
    }

    public void SnapPosition()
    {
        Vector3 desired_center = new Vector3(target.position.x, yCoord, zCoord);
        current_center = desired_center;
        vel = Vector3.zero;
        tr.position = desired_center;
    }
    public static void Shake(ScreenShakeParams shakeParams, Vector2 direction)
    {
        if (shakeParams.magnitude > 0)
            RandomShake(shakeParams.magnitude, shakeParams.duration, shakeParams.count, shakeParams.rotationMagnitude, shakeParams.order);
        if (shakeParams.dirMagnitude > 0 && direction.sqrMagnitude > 0)
            DirectionalShake(direction, shakeParams.dirMagnitude, shakeParams.dirDuration);
    }

    public static void RandomShake(float magnitude, float duration, int count, float rotationMagnitude, int order)
    {
        if (instance.shakeRoutine == null)
        {
            instance.currentShakeOrder = order;
            instance.shakeRoutine = instance.StartCoroutine(instance.RandomShakeRoutine(magnitude, duration, count, rotationMagnitude));
        }
        else
        {
            if (order < instance.currentShakeOrder)
            {
                instance.StopCoroutine(instance.shakeRoutine);
                instance.currentShakeOrder = order;
                instance.shakeRoutine = instance.StartCoroutine(instance.RandomShakeRoutine(magnitude, duration, count, rotationMagnitude));
            }
        }
    }


    IEnumerator RandomShakeRoutine(float magnitude, float duration, int count, float rotationMagnitude)
    {
        Vector3 startPosition = shakeOffsest;
        Vector3 newPosition;

        Quaternion startRotation = Quaternion.Euler(0, 0, 0);
        Quaternion newRotation;

        float t;

        float percent;
        float randomDuration;
        float magMultiplier;

        for (int i = 0; i < count; i++)
        {
            t = 0;

            if (count > 1)
            {
                magMultiplier = inOutShakeCurve.Evaluate(i * 1.0f / (count - 1));
                randomDuration = duration * Random.Range(0.9f, 1.1f) * randomShakeDurationeCurve.Evaluate(i * 1.0f / (count - 1));
            }
            else
            {
                magMultiplier = 1;
                randomDuration = duration;
            }
            newPosition = Random.insideUnitCircle.normalized * magnitude * magMultiplier;
            int sign = Random.value > 0.5 ? 1 : -1;
            newRotation = Quaternion.AngleAxis(rotationMagnitude * magMultiplier * sign * Random.Range(0.7f, 1f), Vector3.forward);

            while (t < 1)
            {
                t += Time.unscaledDeltaTime / randomDuration;
                percent = randomShakeMoveCurve.Evaluate(t);
                shakeOffsest = Vector3.Lerp(startPosition, newPosition, t);
                shakeRotation = Quaternion.Lerp(startRotation, newRotation, t);
                yield return null;
            }
            startPosition = newPosition;
            startRotation = newRotation;
        }

        t = 0;
        newPosition = Vector3.zero;
        newRotation = Quaternion.Euler(0, 0, 0);
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            percent = randomShakeMoveCurve.Evaluate(t);
            shakeOffsest = Vector3.Lerp(startPosition, newPosition, percent);
            shakeRotation = Quaternion.Lerp(startRotation, newRotation, percent);
            yield return null;
        }
        shakeOffsest = Vector3.zero;
        shakeRotation = Quaternion.Euler(0, 0, 0);

        shakeRoutine = null;
    }

    public static void DirectionalShake(Vector2 direction, float magnitude, float duration)
    {
        if (instance.directionalshakeRoutine == null)
            instance.directionalshakeRoutine = instance.StartCoroutine(instance.DirectionalShakeRoutine(direction, magnitude, duration));
    }

    IEnumerator DirectionalShakeRoutine(Vector2 direction, float magnitude, float duration)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 newPosition;

        float t;

        t = 0;

        newPosition = direction * magnitude;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            directionalOffsest = Vector3.Lerp(startPosition, newPosition, randomShakeMoveCurve.Evaluate(t));
            yield return null;
        }
        startPosition = newPosition;

        t = 0;
        newPosition = Vector3.zero;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            directionalOffsest = Vector3.Lerp(startPosition, newPosition, randomShakeMoveCurve.Evaluate(t));
            yield return null;
        }
        directionalOffsest = Vector3.zero;
        directionalshakeRoutine = null;
    }
}

[System.Serializable]
public class ScreenShakeParams
{
    [Header("Directional Shake")]
    public float dirMagnitude;
    public float dirDuration;
    [Header("Random Shake")]
    public float magnitude;
    public float duration;
    public int count;
    public float rotationMagnitude;
    public int order;
}
