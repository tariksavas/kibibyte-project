using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject stoppedMenu = null;

    //Bu scriptteki metotlar gerekli buttonlara editör üzerinden tanımlanmıştır.
    public void Continue()
    {
        Time.timeScale = 1;
        stoppedMenu.SetActive(false);
    }
    public void NextLevel()
    {
        Debug.Log("Next level not ready yet!");
    }
    public void MainMenuB()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Help()
    {
        SceneManager.LoadScene("HelpScene");
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Application.Quit();
    }
}