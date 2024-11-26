using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] TMP_Text text;

    void Start()
    {
        int charIndex = PlayerPrefs.GetInt(PlayerPrefs.GetString($"{PlayerPrefs.GetString("Victory")}String"));
        players[charIndex].SetActive(true);
        text.text = $"{PlayerPrefs.GetString("Victory")}\nhas won!";
        Invoke(nameof(BackToMenu), 3);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
