
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Settings")]
    [SerializeField] private GameObject bridgeVisual;

    private bool isActivated = false;

    private void Start()
    {
        if (bridgeVisual != null)
        {
            bridgeVisual.SetActive(false);
        }
    }

    public void ActivateBridge()
    {
        if (isActivated)
            return;

        isActivated = true;

        if (bridgeVisual != null)
        {
            bridgeVisual.SetActive(true);
        }

        Debug.Log("Bridge activated!");
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
