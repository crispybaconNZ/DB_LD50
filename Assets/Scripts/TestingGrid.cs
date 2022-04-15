using UnityEngine;

public class TestingGrid : MonoBehaviour {
    private DBGrid<int> grid;

    public void Start() {
        grid = new DBGrid<int>(4, 2, 10f, Vector3.zero, () => { return 0; });
    }

    public void Update() {
    }
}
