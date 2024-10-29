using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab.MultiplayerModels;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefab;
    public Transform spawnPoint;
    public string playerIdentity;

    [Header("Display")]
    public GameObject display;

    private string playerName;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt(playerIdentity);
        Debug.Log($"{playerIdentity}: {selectedCharacter}");
        if (!(selectedCharacter == -1))
        {
            GameObject prefab = characterPrefab[selectedCharacter];
            GameObject character = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            PlayerMovement charScript = character.GetComponent<PlayerMovement>();
            charScript.SetRespawn(spawnPoint);

            UpdateStats updateStats = display.GetComponent<UpdateStats>();
            Debug.Log(character);
            updateStats.SetPlayer(character);

            if (playerIdentity == "Player1" && PlayerPrefs.GetString("PlayerName") != null)
            {
                playerName = PlayerPrefs.GetString("PlayerName");
            }
            Image img = display.GetComponentInChildren<Image>();
            img.sprite = characterPrefab[selectedCharacter].GetComponent<SpriteRenderer>().sprite;
            img.color = characterPrefab[selectedCharacter].GetComponent<SpriteRenderer>().color;
        }
        else
        {
            display.SetActive(false);
        }
    }
}
