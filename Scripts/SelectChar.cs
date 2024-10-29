using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    [Header("References")]
    public GameObject[] chars;
    public GameObject activate;
    public GameObject self;

    [Header("Selected Character")]
    public int currentChar = -1;
    public string playerIdentity;

    private void Start()
    {
        Button actButton = activate.GetComponent<Button>();
    }

    public void InitPlayer()
    {
        activate.SetActive(false);
        self.SetActive(true);
        currentChar = 0;
    }

    public void NextChar()
    {
        chars[currentChar].SetActive(false);
        currentChar = (currentChar + 1) % chars.Length;
        chars[currentChar].SetActive(true);
    }
    public void PreviousChar()
    {
        chars[currentChar].SetActive(false);
        currentChar--;
        if (currentChar < 0)
        {
            currentChar += chars.Length;
        }
        chars[currentChar].SetActive(true);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt(playerIdentity, currentChar);
        SceneManager.LoadScene(PlayerPrefs.GetString("Scene"), LoadSceneMode.Single);
    }
}
