using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour {
    public void StartGame() {
        SceneManager.LoadScene("Game");
    }
}
