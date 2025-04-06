using UnityEngine;

public abstract class ABaseEye : MonoBehaviour
{
    public int faceIndex;
    public int eyeIndex;
    public bool isInteractable = false;

    public DiceConfiguration configuration;

    public abstract void SetInteractable();
    public abstract void SetNonInteractable();
}