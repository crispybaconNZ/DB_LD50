using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour {
    [SerializeField, Tooltip("Reference to TMPro text field for version number")] private TextMeshProUGUI versionText;
    private void Start() {
        versionText.text = "v" + Application.version;
    }
    public void StartGame() {
        SceneManager.LoadScene("Game");
    }

    public void Instructions() {
        SceneManager.LoadScene("Instructions");
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
