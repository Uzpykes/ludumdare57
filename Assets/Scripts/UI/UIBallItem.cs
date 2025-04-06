using UnityEngine;
using UnityEngine.UI;

public class UIBallItem : MonoBehaviour
{
    public DiceFaceSlot type;
    public Image mainImage;
    public Image checkImage;

    void Awake()
    {
        checkImage.enabled = false;
    }

    public void ShowCaptured()
    {
        checkImage.enabled = true;
    }

    public void HideCaptured()
    {
        checkImage.enabled = false;
    }
}
