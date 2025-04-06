using System;
using System.Collections.Generic;
using UnityEngine;

public class DieFaceRenderer : MonoBehaviour
{
    public List<DieFace> faces;

    public void UpdateFaces(DiceConfiguration configuration, Dictionary<DiceFaceSlot, ABaseEye> eyeMapping)
    {
        foreach (var c in configuration.faces)
        {
            var i = c.EyeCount - 1;
            var face = faces[i];

            var trs = face.facePlane.localToWorldMatrix;
            foreach (var ins in face.instances)
            {
                if (ins != null)
                    Destroy(ins.gameObject);
            }
            face.instances.Clear();

            for (int e = 0; e < c.EyeCount; e++)
            {
                if (c.EyeType[e] == DiceFaceSlot.None) continue;

                var o = eyeMapping[c.EyeType[e]];
                var instance = Instantiate(o, trs.MultiplyPoint(face.dieEyes[e].planeOffset + Vector3.up * 0.0375f), face.facePlane.rotation, transform);
                instance.eyeIndex = e;
                instance.faceIndex = i;
                instance.configuration = configuration;
                face.instances.Add(instance);
            }
        }
    }
}

[System.Serializable]
public class DieFace
{
    public List<DieEye> dieEyes;
    public Transform facePlane;
    [NonSerialized]
    public List<ABaseEye> instances = new();
}

[System.Serializable]
public struct DieEye
{
    public Vector3 planeOffset;
}