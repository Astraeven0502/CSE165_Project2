using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public ThirdPersonCheckpointManager manager;
    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aircraft")) // Tag your airplane model with "Aircraft"
        {
            manager.OnCheckpointHit(this);
        }
    }
}
