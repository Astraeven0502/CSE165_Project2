using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    public CheckpointManager checkpointManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            checkpointManager.OnGroundTouched();
        }
    }
}
