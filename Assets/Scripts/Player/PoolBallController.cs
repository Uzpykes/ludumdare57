using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PoolBallController : MonoBehaviour
{
    private Plane plane;

    public bool isInputActive = false;
    private Rigidbody rb;

    private Vector3 aimDirection;
    private bool canHit = false;
    [SerializeField]
    private LineRenderer hitLine;
    private StateData stateData;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hitLine.gameObject.SetActive(false);
        stateData = FindAnyObjectByType<StateData>();
        DieManager.Instance.onDieThrow += HandleThrow;
    }

    private void HandleThrow()
    {
        gameObject.SetActive(false);
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
            trackingCoroutine = null;
        }
    }

    void OnEnable()
    {
        // rb.excludeLayers = 1 << 0;
        isInputActive = true;

        plane.SetNormalAndPosition(Vector3.up, transform.position);

        InputSystem.actions["Fire"].performed += HandleHit;
        rb.isKinematic = false;
    }

    void OnDisable()
    {
        isInputActive = false;
        rb.isKinematic = true;
        InputSystem.actions["Fire"].performed -= HandleHit;
    }

    void OnDestroy()
    {
        InputSystem.actions["Fire"].performed -= HandleHit;
        DieManager.Instance.onDieThrow -= HandleThrow;
    }

    void Update()
    {
        if (isInputActive)
        {
            HandlePlacement();
        }
    }

    private void HandlePlacement()
    {
        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(r, out float enter))
        {
            var pos = r.GetPoint(enter);
            aimDirection = transform.position - pos;
            canHit = aimDirection.sqrMagnitude > 0.03f;
            if (hitLine.gameObject.activeSelf != canHit)
            {
                hitLine.gameObject.SetActive(canHit);
                hitLine.SetPosition(0, transform.position);
            }

            hitLine.SetPosition(1, aimDirection + transform.position);

#if UNITY_EDITOR
            if (canHit)
                Debug.DrawRay(transform.position, aimDirection, Color.red);
#endif
        }
    }

    private void HandleHit(InputAction.CallbackContext context)
    {
        if (isInputActive)
        {
            HitBall();
        }
    }

    private void HitBall()
    {
        if (canHit)
        {
            if (stateData.currentFreeHits > 0)
                stateData.AddFreeHits(-1);
            else stateData.AddScore(-stateData.currentDepth);
            hitLine.gameObject.SetActive(false);
            isInputActive = false;
            rb.isKinematic = false;
            rb.AddForce(aimDirection, ForceMode.Impulse);
        }
    }

    private Coroutine trackingCoroutine;

    private void FixedUpdate()
    {
        if (trackingCoroutine == null && !rb.isKinematic)
        {
            if (rb.linearVelocity.sqrMagnitude < 0.1f && rb.angularVelocity.sqrMagnitude < 0.1f)
            {
                trackingCoroutine = StartCoroutine(TrackDie());
            }
        }
    }

    // Checks if die is at rest state
    private IEnumerator TrackDie()
    {
        var t = 0f;
        var restCooldown = 0.5f;

        while (t < restCooldown)
        {
            if (rb.linearVelocity.sqrMagnitude > 0.1f || rb.angularVelocity.sqrMagnitude > 0.1f)
            {
                trackingCoroutine = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        if (gameObject.activeSelf)
        {
            isInputActive = true;
            rb.isKinematic = true;
        }
        trackingCoroutine = null;

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isInputActive = false;
            gameObject.SetActive(false);
            DieManager.Instance.RequestDiceZoomOutTransition(transform, () => { });
        }
    }

}
