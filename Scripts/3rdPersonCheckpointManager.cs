using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ThirdPersonCheckpointManager : MonoBehaviour
{
    public Parse parser;
    public GameObject checkpointPrefab;
    public OVRCameraRig cam;

    private List<GameObject> checkpoints = new List<GameObject>();
    private int currentCheckpointIndex = 0;

    private LineRenderer guideLine;

    public Text countdownText;
    public Text stopwatchText;
    public Text pointsText;

    private int points = 0;

    public float countdownDuration = 3f;
    private float countdownRemaining;
    private float stopwatchTime = 0f;

    private bool countdownFinished = false;

    private HandDirectionMover movementScript;

    public AudioClip checkpointSound;
    public AudioSource audioSource;

    public GameObject TPOV;
    public Transform centerEye;

    void Start()
    {
        List<Vector3> checkpointPositions = parser.ParseFile();

        foreach (Vector3 pos in checkpointPositions)
        {
            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);

            // Add Collider if not present
            SphereCollider col = checkpoint.GetComponent<SphereCollider>();
            if (col == null)
            {
                col = checkpoint.AddComponent<SphereCollider>();
            }
            col.radius = 9.144f;
            col.isTrigger = true;

            // Add the trigger handler
            if (checkpoint.GetComponent<CheckpointTrigger>() == null)
            {
                CheckpointTrigger trigger = checkpoint.AddComponent<CheckpointTrigger>();
                trigger.manager = this;
                trigger.checkpointIndex = checkpoints.Count;
            }

            checkpoints.Add(checkpoint);
        }

        if (checkpointPositions.Count > 0)
        {
            ThirdPersonAircraftView plane_offset = TPOV.GetComponent<ThirdPersonAircraftView>();
            Debug.Log(plane_offset.distance);
            Vector3 thirdPerson_offset = centerEye.forward * plane_offset.distance - centerEye.up * plane_offset.heightOffset;
            cam.transform.position = checkpointPositions[0] - thirdPerson_offset;
            
        }

        guideLine = cam.gameObject.AddComponent<LineRenderer>();
        guideLine.material = new Material(Shader.Find("Sprites/Default"));
        guideLine.startColor = Color.blue;
        guideLine.endColor = Color.blue;
        guideLine.startWidth = 0.02f;
        guideLine.endWidth = 0.02f;
        guideLine.positionCount = 2;

        countdownRemaining = countdownDuration;
        countdownText.gameObject.SetActive(true);
        stopwatchText.gameObject.SetActive(false);

        movementScript = cam.GetComponentInChildren<HandDirectionMover>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }
    }

    void Update()
    {
        if (!countdownFinished)
        {
            countdownRemaining -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(countdownRemaining).ToString();

            if (countdownRemaining <= 0f)
            {
                countdownFinished = true;
                countdownText.gameObject.SetActive(false);
                stopwatchText.gameObject.SetActive(true);

                if (movementScript != null)
                {
                    movementScript.enabled = true;
                }
            }
            return;
        }

        if (currentCheckpointIndex >= checkpoints.Count)
        {
            guideLine.enabled = false;
            return;
        }

        stopwatchTime += Time.deltaTime;
        stopwatchText.text = FormatTime(stopwatchTime);

        GameObject currentCheckpoint = checkpoints[currentCheckpointIndex];
        if (currentCheckpoint != null)
        {
            Vector3 camPos = cam.centerEyeAnchor.position;
            Vector3 camPosOffset = new Vector3(0f, -0.3f, 0f);
            guideLine.SetPosition(0, camPos + camPosOffset);
            guideLine.SetPosition(1, currentCheckpoint.transform.position);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void OnCheckpointHit(CheckpointTrigger trigger)
    {
        if (trigger.checkpointIndex != currentCheckpointIndex) return;

        Destroy(trigger.gameObject);
        currentCheckpointIndex++;

        points++;
        pointsText.text = points + "/" + checkpoints.Count + " CheckPoints";

        if (checkpointSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }

        if (currentCheckpointIndex >= checkpoints.Count)
        {
            guideLine.enabled = false;
        }
    }
}
