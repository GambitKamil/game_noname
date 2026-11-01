
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BridgeController bridgeController;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated)
            return;

        CompanionAgent agent =
            other.GetComponent<CompanionAgent>();

        if (agent == null)
            return;

        ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        isActivated = true;

        if (bridgeController != null)
        {
            bridgeController.ActivateBridge();
        }
        else
        {
            Debug.LogWarning(
                "BridgeController is not assigned!"
            );
        }

        Debug.Log("Switch activated by CompanionAgent!");
    }
}
