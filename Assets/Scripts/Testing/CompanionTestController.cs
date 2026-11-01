
using UnityEngine;

public class CompanionTestController : MonoBehaviour
{
    [SerializeField] private CompanionAgent companion;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (companion != null)
            {
                companion.StartNavigation();
            }
            else
            {
                Debug.LogWarning("Companion is not assigned!");
            }
        }
    }
}
