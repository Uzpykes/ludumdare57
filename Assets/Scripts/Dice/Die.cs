using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Die : MonoBehaviour
{
    DiceConfiguration config;
    [SerializeField]
    private DieFaceRenderer faceRenderer;
    public Rigidbody rb;
    private BoxCollider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        // col.excludeLayers = LayerMask.NameToLayer("PoolBall");
    }

    public void SetConfiguration(DiceConfiguration diceConfiguration, Dictionary<DiceFaceSlot, ABaseEye> eyeMapping)
    {
        config = diceConfiguration;
        UpdateFaces(eyeMapping);
    }

    private void UpdateFaces(Dictionary<DiceFaceSlot, ABaseEye> eyeMapping)
    {
        faceRenderer.transform.localRotation = Quaternion.Euler(config.randomizedRotation);
        faceRenderer.UpdateFaces(config, eyeMapping);
    }

    public DieFace GetTopFace()
    {
        var maxY = float.NegativeInfinity;
        var maxIndex = -1;
        var index = -1;
        foreach (var face in faceRenderer.faces)
        {
            index++;
            if (face.facePlane.transform.position.y > maxY)
            {
                maxY = face.facePlane.transform.position.y;
                maxIndex = index;
            }
        }

        return faceRenderer.faces[maxIndex];
    }

    public void EnableDifficulty()
    {

    }
}
