using System;
using System.Collections;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private Camera cam;
    private Quaternion defaultRotation;
    private Vector3 defaultPosition;
    private float defaultFov;
    private float minFov = float.Epsilon * 10f;

    [SerializeField]
    private float zoomTime = .5f;

    private Coroutine transitionCoroutine;

    void Awake()
    {
        cam = GetComponent<Camera>();
        defaultRotation = transform.rotation;
        defaultFov = cam.fieldOfView;
        defaultPosition = transform.position;
    }

    public void PlayDiceZoomInTransition(Transform lookTarget, Action onZoomIn)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionCoroutine(lookTarget, onZoomIn));
    }

    private IEnumerator TransitionCoroutine(Transform target, Action onZoomIn)
    {
        var t = 0f;
        var lookRot = Quaternion.FromToRotation(transform.forward, (target.position - transform.position).normalized) * transform.rotation;

        while (t < zoomTime)
        {
            cam.fieldOfView = Mathf.Lerp(defaultFov, minFov, t / zoomTime);
            cam.transform.rotation = Quaternion.Slerp(defaultRotation, lookRot, t / zoomTime);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        onZoomIn.Invoke();

        t = 0f;
        while (t < zoomTime)
        {
            cam.fieldOfView = Mathf.Lerp(180, defaultFov, t / zoomTime);
            cam.transform.rotation = Quaternion.Slerp(lookRot, defaultRotation, t / zoomTime);
            t += Time.deltaTime;
            yield return null;
        }


        cam.fieldOfView = defaultFov;
        cam.transform.rotation = defaultRotation;
        yield return new WaitForSeconds(0.2f);
        yield return null;
        transitionCoroutine = null;
    }

    public void PlayDiceZoomOutTransition(Transform lookTarget, Action onZoomOut)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionOutCoroutine(lookTarget, onZoomOut));
    }

    private IEnumerator TransitionOutCoroutine(Transform target, Action onZoomOut)
    {
        var t = 0f;
        var lookRot = Quaternion.FromToRotation(transform.forward, (target.position - transform.position).normalized) * transform.rotation;

        while (t < zoomTime)
        {
            cam.fieldOfView = Mathf.Lerp(defaultFov, 180, t / zoomTime);
            // cam.transform.rotation = Quaternion.Slerp(defaultRotation, lookRot, t / zoomTime);
            t += Time.deltaTime;
            yield return null;
        }
        onZoomOut.Invoke();

        t = 0f;

        while (t < zoomTime)
        {
            cam.fieldOfView = Mathf.Lerp(minFov, defaultFov, t / zoomTime);
            // cam.transform.rotation = Quaternion.Slerp(lookRot, defaultRotation, t / zoomTime);
            t += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = defaultFov;
        cam.transform.rotation = defaultRotation;
        yield return new WaitForSeconds(0.2f);

        yield return null;
        transitionCoroutine = null;
    }

    // public IEnumerator RecenterCamera(Vector3 offset)
    // {
    //     var t = 0f;
    //     var recenterTime = 0.5f;
    //     cam.transform.position = defaultPosition + offset;

    //     while (t < recenterTime)
    //     {
    //         cam.transform.position = Vector3.Slerp(defaultPosition + offset, defaultPosition, t / recenterTime);
    //         t += Time.deltaTime;
    //         yield return null;
    //     }

    //     cam.transform.position = defaultPosition;
    // }
}
