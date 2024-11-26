using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    [Header("Scenes")]
    public string MainMenu = "Menu";
    public string PracticeMode = "Practice Mode";
    public string FreeForAll = "GroupFight";
    public string PlayerVsPlayer = "1v1Fight";
    public string CourseMode = "ObstRace";

    [Header("Buttons")]
    public Button PracticeButton;
    public Button FreeForAllButton;
    public Button PvPButton;
    public Button RaceButton;

    [Header("Cursor")]
    public Texture2D CursorTexture;

    [Header("Audio")]
    public AudioSource BgMusic;

    private void Start()
    {
        StartCoroutine(StartMusic());
        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        PracticeButton.onClick.AddListener(PlayPractice);
        FreeForAllButton.onClick.AddListener(PlayF4All);
        PvPButton.onClick.AddListener(Play1v1);
        RaceButton.onClick.AddListener(PlayCourse);
    }

    private IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(1.5F);
        BgMusic.Play();
    }

    public void PlayPractice()
    {
        PlayerPrefs.SetString("Scene", PracticeMode);
        SceneManager.LoadScene("SelectObstacle");
    }

    public void Play1v1()
    {
        PlayerPrefs.SetString("Scene", PlayerVsPlayer);
        SceneManager.LoadScene("Select1v1");
    }

    public void PlayF4All()
    {   
        PlayerPrefs.SetString("Scene", FreeForAll);
        SceneManager.LoadScene("SelectChar");
    }

    public void PlayCourse()
    {
        PlayerPrefs.SetString("Scene", CourseMode);
        SceneManager.LoadScene("SelectObstacle");
    }
}
