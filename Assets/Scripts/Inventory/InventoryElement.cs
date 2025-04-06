using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryElement : LayoutElement
{
    private GameObject targetObject;

    public void SetObject(GameObject o)
    {
        targetObject = o;
        UpdateObject();
    }

    public void UpdateObject()
    {
        targetObject.transform.localScale = new Vector3(1 / transform.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z) / 3.2f;
        targetObject.SetActive(false);

        targetObject.transform.SetParent(transform, false);
        StartCoroutine(UpdatePosition());
    }

    private IEnumerator UpdatePosition()
    {
        yield return null;
        targetObject.transform.localPosition = Vector3.zero;
        targetObject.SetActive(true);
    }
}
