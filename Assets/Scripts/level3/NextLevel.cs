using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string nextScene = "";
            if (currentScene == "Level03-1")
            {
                nextScene = "Level03-2";
            }
            else if (currentScene == "Level03-2")
            {
                nextScene = "Level03-3";
            }
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}
