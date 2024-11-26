using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetChars : MonoBehaviour
{

    [SerializeField] Button self;

    public List<Character> Chars { get; set; }

    void Start()
    {
        self.onClick.AddListener(SetChar);
        Chars = new List<Character>();
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetInt($"Player{i + 1}", -1);
        }
    }

    private void SetChar()
    {
        try
        {
            foreach (Character c in Chars)
            {
                Debug.Log($"{PlayerPrefs.GetString($"{c.identity}String")}: {PlayerPrefs.GetInt(PlayerPrefs.GetString($"{c.identity}String"))}");
            }
        }
        catch
        {
            Debug.Log("No character selected.");
        }
        if (PlayerPrefs.GetString("Scene") == "Practice Mode")
        {
            PlayerPrefs.SetInt("Player2", 2);
        }
        PlayerPrefs.SetInt("CharsCount", Chars.Count);
        SceneManager.LoadScene(PlayerPrefs.GetString("Scene"));
    }
}
