using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DieEyeLookup", menuName = "Scriptable Objects/DieEyeLookup")]
public class DieEyeLookup : ScriptableObject
{
    public List<EyeMapping> eyeMappings;
}

[System.Serializable]
public struct EyeMapping
{
    public DiceFaceSlot diceFaceSlot;
    public ABaseEye eyePrefab;
}