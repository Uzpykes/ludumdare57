using UnityEditor.Rendering;
using UnityEngine;

public class BallSpawnEye : ABaseEye
{
    private Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void SetInteractable()
    {
        isInteractable = true;
        rb.isKinematic = false;
    }

    public override void SetNonInteractable()
    {
        isInteractable = false;
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            DieManager.Instance.ReportBallClearance(faceIndex, eyeIndex);
            Destroy(this.gameObject);
            DieManager.Instance.EvaluateFaceClearance();
            Debug.Log("TODO: Add score, or send one level above");
        }
    }
}
