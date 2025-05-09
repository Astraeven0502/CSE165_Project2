using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.Hands;

public class CheckpointManager : MonoBehaviour
{
    public Parse parser;
    public GameObject checkpointPrefab;
    public OVRCameraRig cam;

    private List<GameObject> checkpoints = new List<GameObject>();
    private const float triggerDistance = 9.144f;
    private int currentCheckpointIndex = 0;

    private LineRenderer guideLine;

    public Text countdownText;
    public Text stopwatchText;
    public Text pointsText;

    private int points = 0;
    private float countdownRemaining;
    private float stopwatchTime = 0f;

    private bool countdownFinished = false;

    private HandDirectionMover movementScript;

    public AudioClip checkpointSound;
    public AudioClip checkpointMusic;
    public AudioSource audioSource;

    // Freeze-related
    private Vector3 lastCheckpointPosition;
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    public float freezeDuration = 3f;

    public OVRSkeleton handSkeleton;

    void Start()
    {
        List<Vector3> checkpointPositions = parser.ParseFile();

        foreach (Vector3 pos in checkpointPositions)
        {
            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);

            AudioSource cpAudio = checkpoint.AddComponent<AudioSource>();
            cpAudio.clip = checkpointMusic;
            cpAudio.spatialBlend = 1.0f; // Fully 3D sound
            cpAudio.loop = true;
            cpAudio.playOnAwake = false;
            cpAudio.minDistance = 1f;
            cpAudio.maxDistance = 200f;
            cpAudio.rolloffMode = AudioRolloffMode.Linear;
            cpAudio.dopplerLevel = 0f; 

            checkpoints.Add(checkpoint);
        }

        if (checkpointPositions.Count > 0)
        {
            cam.transform.position = checkpointPositions[0];
            lastCheckpointPosition = checkpointPositions[0];
        }

        guideLine = cam.gameObject.AddComponent<LineRenderer>();
        guideLine.material = new Material(Shader.Find("Sprites/Default"));
        guideLine.startColor = Color.blue;
        guideLine.endColor = Color.blue;
        guideLine.startWidth = 0.02f;
        guideLine.endWidth = 0.02f;
        guideLine.positionCount = 2;

        countdownRemaining = 3f;
        countdownText.gameObject.SetActive(true);
        stopwatchText.gameObject.SetActive(false);

        movementScript = cam.GetComponentInChildren<HandDirectionMover>();
        if (movementScript != null)
            movementScript.enabled = false;
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
                    movementScript.enabled = true;
            }
            return;
        }

        GameObject currentCheckpoint = checkpoints[currentCheckpointIndex];

        AudioSource cpAudio = currentCheckpoint.GetComponent<AudioSource>();

        if (!cpAudio.isPlaying)
        {
            cpAudio.Play();
        }

        if (currentCheckpoint == null) return;

        if (currentCheckpointIndex >= checkpoints.Count)
        {
            guideLine.enabled = false;
            return;
        }

        Vector3 camPos = cam.centerEyeAnchor.position;
        Vector3 camPos_offset = new Vector3(0f, -0.3f, 0f);
        float distance = Vector3.Distance(camPos, currentCheckpoint.transform.position);

        if (distance < triggerDistance)
        {
            lastCheckpointPosition = currentCheckpoint.transform.position;

            if (cpAudio != null)
                cpAudio.Stop();

            Destroy(currentCheckpoint);
            currentCheckpointIndex++;

            points++;
            pointsText.text = points + "/" + checkpoints.Count + " CheckPoints";

            if (checkpointSound != null && audioSource != null)
                audioSource.PlayOneShot(checkpointSound);

            return;
        }

        stopwatchTime += Time.deltaTime;
        stopwatchText.text = FormatTime(stopwatchTime);

        if (isFrozen)
        {
            cam.transform.position = lastCheckpointPosition;
            freezeTimer -= Time.deltaTime;
            countdownText.text = "Crash!!!\n" + "Stabilizing: " + Mathf.Ceil(freezeTimer).ToString();
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
                if (movementScript != null)
                    movementScript.enabled = true;
                countdownText.gameObject.SetActive(false);
            }
        }

        guideLine.SetPosition(0, camPos + camPos_offset);
        guideLine.SetPosition(1, currentCheckpoint.transform.position);

        if (handSkeleton.Bones != null && handSkeleton.Bones.Count > 0)
        {
            // Index finger tip bone
            var indexTipBone = handSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
            Vector3 tipWorldPosition = indexTipBone.Transform.position;
            Debug.Log("Index Finger Tip Position: " + tipWorldPosition);
            guideLine.SetPosition(0, tipWorldPosition);
        }

    }

    public void OnGroundTouched()
    {
        if (countdownFinished && !isFrozen)
        {
            isFrozen = true;
            freezeTimer = freezeDuration;

            if (movementScript != null)
                movementScript.enabled = false;

            countdownText.gameObject.SetActive(true);

        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
