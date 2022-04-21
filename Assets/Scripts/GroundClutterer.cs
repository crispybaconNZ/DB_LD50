using System.Collections.Generic;
using UnityEngine;

public class GroundClutterer : MonoBehaviour {
    public Sprite[] clutterSprites;
    public GameObject cluttererPrefab;
    
    void Start() {
        if (clutterSprites.Length == 0) { return; }  // no prefabs so exit

        int numClutter = Random.Range(10, 30);
        List<Vector3> positions = new List<Vector3>(numClutter);   // array to store positions for clutter

        for (int i = 0; i < numClutter; i++) {
            // find a random location to place the clutter
            Vector3 pos = new Vector3(Random.Range(0f, 30f) - 5, Random.Range(0f, 10f) - 5f);
            while (!ClearSpace(pos, positions, 2f)) {
                pos = new Vector3(Random.Range(0f, 30f) - 5f, Random.Range(0f, 10f) - 5f);
            }

            // get a random clutter
            int clutterIndex = Random.Range(0, clutterSprites.Length);
            GameObject clutter = Instantiate(cluttererPrefab, pos, Quaternion.identity, transform);
            clutter.GetComponent<SpriteRenderer>().sprite = clutterSprites[clutterIndex];
            positions.Add(pos);
        }
    }

    /// <summary>Check if a specified <c>Vector3</c> is close to any position in a list of <c>Vector3</c>s.</summary>
    /// <param name="newPos">The <c>Vector3</c> to check for.</param>
    /// <param name="existingPositions">The <c>List</c> of <c>Vector3</c>s to check against.</param>
    /// <param name="tolerance">Maximum distance between the positions allowed; defaults to <c>0.5</c></param>
    /// <returns><c>true</c> if <paramref name="newPos"/> is at least <paramref name="tolerance"/> distance from 
    /// any member of <paramref name="existingPositions"/>; <c>false</c> otherwise. Also returns <c>true</c> if 
    /// <paramref name="existingPositions"/> is <c>null</c> or is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="newPos"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="tolerance"/> is less than 0.</exception>
    private bool ClearSpace(Vector3 newPos, List<Vector3> existingPositions, float tolerance=0.5f) {
        if (newPos == null) { throw new System.ArgumentNullException(nameof(newPos)); }
        if (tolerance < 0) { throw new System.ArgumentOutOfRangeException(nameof(tolerance), "tolerance must be greater than 0"); }
        if (existingPositions == null || existingPositions.Count == 0) { return true; }

        foreach (Vector3 position in existingPositions) {
            if (Vector3.Distance(newPos, position) < tolerance)
                return false;
        }
        return true;
    }
}
