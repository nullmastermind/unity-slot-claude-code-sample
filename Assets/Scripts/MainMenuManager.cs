using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;

    void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButton);
        }
    }

public void OnPlayButton()
    {
        SceneManager.LoadScene("DiceRoll");
    }
}
