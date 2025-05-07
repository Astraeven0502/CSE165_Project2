//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CheckpointManager : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
//---------------------------
//using UnityEngine;
//using System.Collections.Generic;

//public class CheckpointManager : MonoBehaviour
//{
//    public Parse parser;
//    public GameObject checkpointPrefab;
//    public OVRCameraRig cam;
//    private List<Vector3> checkpointPositions;

//    private List<GameObject> checkpoints = new List<GameObject>();
//    private const float triggerDistance = 9.144f; // 30 feet in meters

//    void Start()
//    {
//        checkpointPositions = parser.ParseFile();
//        foreach (Vector3 pos in checkpointPositions)
//        {
//            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);
//            checkpoints.Add(checkpoint);
//        }

//        cam.transform.position = checkpointPositions[0];
//    }

//    void Update()
//    {
//        Vector3 camPos = cam.centerEyeAnchor.position;

//        for (int i = checkpoints.Count - 1; i >= 0; i--)
//        {
//            if (checkpoints[i] == null) continue;

//            float distance = Vector3.Distance(camPos, checkpoints[i].transform.position);
//            if (distance < triggerDistance && )
//            {
//                Destroy(checkpoints[i]);
//                checkpoints.RemoveAt(i);
//            }
//        }
//    }
//}
//----------------------------

//using UnityEngine;
//using System.Collections.Generic;

//public class CheckpointManager : MonoBehaviour
//{
//    public Parse parser;
//    public GameObject checkpointPrefab;
//    public OVRCameraRig cam;

//    private List<GameObject> checkpoints = new List<GameObject>();
//    private const float triggerDistance = 9.144f; // 30 feet
//    private int currentCheckpointIndex = 0;

//    void Start()
//    {
//        List<Vector3> checkpointPositions = parser.ParseFile();

//        foreach (Vector3 pos in checkpointPositions)
//        {
//            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);
//            checkpoints.Add(checkpoint);
//        }

//        cam.transform.position = checkpointPositions[0]; // Optional initial placement
//    }

//    void Update()
//    {
//        if (currentCheckpointIndex >= checkpoints.Count) return;

//        GameObject currentCheckpoint = checkpoints[currentCheckpointIndex];
//        if (currentCheckpoint == null) return;

//        Vector3 camPos = cam.centerEyeAnchor.position;
//        float distance = Vector3.Distance(camPos, currentCheckpoint.transform.position);

//        if (distance < triggerDistance)
//        {
//            Destroy(currentCheckpoint);
//            currentCheckpointIndex++;
//        }
//    }
//}
//-----------------------------
//using UnityEngine;
//using System.Collections.Generic;

//public class CheckpointManager : MonoBehaviour
//{
//    public Parse parser;
//    public GameObject checkpointPrefab;
//    public OVRCameraRig cam;

//    private List<GameObject> checkpoints = new List<GameObject>();
//    private const float triggerDistance = 9.144f; // 30 feet
//    private int currentCheckpointIndex = 0;

//    private LineRenderer guideLine;

//    void Start()
//    {
//        List<Vector3> checkpointPositions = parser.ParseFile();

//        foreach (Vector3 pos in checkpointPositions)
//        {
//            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);
//            checkpoints.Add(checkpoint);
//        }

//        cam.transform.position = checkpointPositions[0];

//        // Create a guiding line
//        guideLine = cam.gameObject.AddComponent<LineRenderer>();
//        guideLine.material = new Material(Shader.Find("Sprites/Default"));
//        guideLine.startColor = Color.blue;
//        guideLine.endColor = Color.blue;
//        guideLine.startWidth = 0.02f;
//        guideLine.endWidth = 0.02f;
//        guideLine.positionCount = 2;
//    }

//    void Update()
//    {
//        if (currentCheckpointIndex >= checkpoints.Count)
//        {
//            guideLine.enabled = false;
//            return;
//        }

//        GameObject currentCheckpoint = checkpoints[currentCheckpointIndex];
//        if (currentCheckpoint == null) return;

//        Vector3 camPos = cam.centerEyeAnchor.position;
//        Vector3 camPos_offset = new Vector3(0f, -0.3f, 0f);
//        float distance = Vector3.Distance(camPos, currentCheckpoint.transform.position);

//        if (distance < triggerDistance)
//        {
//            Destroy(currentCheckpoint);
//            currentCheckpointIndex++;
//            return;
//        }

//        // Update the guiding line
//        guideLine.SetPosition(0, camPos + camPos_offset);
//        guideLine.SetPosition(1, currentCheckpoint.transform.position);
//    }
//}
//---------------------

using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public Parse parser;
    public GameObject checkpointPrefab;
    public OVRCameraRig cam;

    private List<GameObject> checkpoints = new List<GameObject>();
    private const float triggerDistance = 9.144f; // 30 feet
    private int currentCheckpointIndex = 0;

    private LineRenderer guideLine;

    public Text countdownText;
    public Text stopwatchText;

    public float countdownDuration = 3f; // seconds
    private float countdownRemaining;
    private float stopwatchTime = 0f;

    private bool countdownFinished = false;

    private HandDirectionMover movementScript; // Replace with your actual script name

    void Start()
    {
        List<Vector3> checkpointPositions = parser.ParseFile();

        foreach (Vector3 pos in checkpointPositions)
        {
            GameObject checkpoint = Instantiate(checkpointPrefab, pos, Quaternion.identity);
            checkpoints.Add(checkpoint);
        }

        cam.transform.position = checkpointPositions[0];

        // Create a guiding line
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
            movementScript.enabled = false; // Disable movement during countdown
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
                    movementScript.enabled = true; // Enable movement after countdown
                }
            }
            return; // Don't proceed until countdown finishes
        }

        if (currentCheckpointIndex >= checkpoints.Count)
        {
            guideLine.enabled = false;
            return;
        }
       
        //---------------Check point-------------------
        GameObject currentCheckpoint = checkpoints[currentCheckpointIndex];
        if (currentCheckpoint == null) return;

        Vector3 camPos = cam.centerEyeAnchor.position;
        Vector3 camPos_offset = new Vector3(0f, -0.3f, 0f);
        float distance = Vector3.Distance(camPos, currentCheckpoint.transform.position);

        if (distance < triggerDistance)
        {
            Destroy(currentCheckpoint);
            currentCheckpointIndex++;
            return;
        }
        //---------------Check point-----------------

        //--------------------Stopwatch--------------------
        stopwatchTime += Time.deltaTime;
        stopwatchText.text = FormatTime(stopwatchTime);
        //--------------------Stopwatch--------------------

        // Update the guiding line
        guideLine.SetPosition(0, camPos + camPos_offset);
        guideLine.SetPosition(1, currentCheckpoint.transform.position);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}

