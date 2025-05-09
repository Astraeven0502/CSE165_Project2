using UnityEngine;
using System.Collections.Generic;

public class Parse : MonoBehaviour
{
    public TextAsset file;

    public List<Vector3> ParseFile()
    {
        float scaleFactor = 1.0f / 39.37f;
        List<Vector3> positions = new List<Vector3>();
        string[] lines = file.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] coords = line.Trim().Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            if (coords.Length < 3) continue;

            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);
            float z = float.Parse(coords[2]);

            positions.Add(new Vector3(x, y, z) * scaleFactor);
        }

        return positions;
    }
}
