using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyEye : ABaseEye, IPointerDownHandler
{
    public bool isCompleted = false;
    [SerializeField]
    private GameObject colliders;
    public void OnPointerDown(PointerEventData eventData)
    {
        // if (!isInteractable || eventData.button != PointerEventData.InputButton.Left) return;
        // DieManager.Instance.RequestDiceTransition(this, OnTransitionDone);
    }

    public override void SetInteractable()
    {
        isInteractable = true;
    }

    private void OnTransitionDone()
    {
        isInteractable = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBall"))
        {
            colliders.SetActive(true);
            other.attachedRigidbody.excludeLayers = ~0;
            other.attachedRigidbody.includeLayers = 1 << LayerMask.NameToLayer("Hole") | 1 << LayerMask.NameToLayer("DieEye");
            isInteractable = false;
            if (transition == null)
                StartCoroutine(TransitionCoroutine(other));
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("PoolBall"))
        {
            var dist = other.transform.position - transform.position;
            dist.y = 0;
            other.attachedRigidbody.AddForce(dist * 10);
        }
    }

    private Coroutine transition;
    private IEnumerator TransitionCoroutine(Collider other)
    {
        other.attachedRigidbody.linearVelocity = other.attachedRigidbody.linearVelocity.normalized / 2f;
        yield return new WaitForSeconds(.2f);
        other.attachedRigidbody.excludeLayers = 0;
        other.attachedRigidbody.includeLayers = 0;
        other.gameObject.SetActive(false);
        DieManager.Instance.RequestDiceTransition(this, OnTransitionDone);
        transition = null;
    }

    public override void SetNonInteractable()
    {
        isInteractable = false;
    }
}
