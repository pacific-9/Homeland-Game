
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadScene("GameLevel");
    }
    public void Quitgame()
    {
        Application.Quit();
    }

}
