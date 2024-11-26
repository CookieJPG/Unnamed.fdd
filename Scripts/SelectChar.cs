using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    [Header("References")]
    public GameObject[] chars;
    public GameObject activate;
    public Button actButton;
    public GameObject self;

    [Header("Start Script")]
    public SetChars setChars;

    [Header("Selected Character")]
    public int currentChar = -1;
    public string playerIdentity;

    private void Start()
    {
        actButton.onClick.AddListener(InitPlayer);
        PlayerPrefs.SetString($"{playerIdentity}String", playerIdentity);
    }

    public void InitPlayer()
    {
        self.SetActive(true);
        setChars.Chars.Add(new Character(playerIdentity, currentChar));
        SetupChar();
    }

    private void SetupChar()
    {
        currentChar = 0;
        PlayerPrefs.SetInt(playerIdentity, currentChar);
        activate.SetActive(false);
    }

    public void NextChar()
    {
        chars[currentChar].SetActive(false);
        currentChar = (currentChar + 1) % chars.Length;
        chars[currentChar].SetActive(true);
        UpdateChar(playerIdentity, currentChar);
        PlayerPrefs.SetInt(playerIdentity, currentChar);
    }

    public void PreviousChar()
    {
        chars[currentChar].SetActive(false);
        currentChar--;
        if (currentChar < 0)
        {
            currentChar = chars.Length;
        }
        chars[currentChar].SetActive(true);
        UpdateChar(playerIdentity, currentChar);
        PlayerPrefs.SetInt(playerIdentity, currentChar);
    }

    private void UpdateChar(string identity, int newIndex)
    {
        Character charToUpdt = setChars.Chars.Find(c => c.identity == identity);
        
        if (charToUpdt != null)
        {
            charToUpdt.index = newIndex;
        }
    }
}
