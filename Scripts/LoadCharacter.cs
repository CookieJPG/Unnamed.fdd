using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Users;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefab;
    public Transform spawnPoint;
    public string playerIdentity;

    [Header("Input System")]
    private UserInput uInput;

    [Header("Display")]
    public GameObject display;

    [Header("Select Character")]
    public PlayerInputManager manager;

    private string playerName;

    internal GameObject character;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt(playerIdentity);
        Debug.Log($"{PlayerPrefs.GetString(playerIdentity)}: {selectedCharacter}");
        if (selectedCharacter != -1)
        {
            SetupChars();
        }
        else
        {
            display.SetActive(false);
        }
    }

    private void SetupChars()
    {
        GameObject prefab = characterPrefab[PlayerPrefs.GetInt(playerIdentity)];
        
        manager.playerPrefab = prefab;

        character = manager.JoinPlayer().gameObject;
        //GameObject character = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        PlayerMovement charScript = character.GetComponent<PlayerMovement>();

        PlayerInput input = character.GetComponent<PlayerInput>();

        uInput = character.GetComponent<UserInput>();
        uInput.SetControlScheme(playerIdentity);

        AttackBehaviour charAtk = character.GetComponent<AttackBehaviour>();
        charScript.SetRespawn(spawnPoint);

        PlayerMovement pm = character.GetComponent<PlayerMovement>();

        UpdateStats updateStats = display.GetComponent<UpdateStats>();
        updateStats.SetPlayer(pm, playerIdentity);

        if (playerIdentity == "Player1" && PlayerPrefs.GetString("PlayerName") != null)
        {
            playerName = PlayerPrefs.GetString("PlayerName");
        }
        else if (playerIdentity == "Dummy")
        {
            pm.dummy = true;
            playerName = "Dummy";
        }
        Image img = display.GetComponentInChildren<Image>();
        img.sprite = characterPrefab[PlayerPrefs.GetInt(playerIdentity)].GetComponent<SpriteRenderer>().sprite;
        img.color = characterPrefab[PlayerPrefs.GetInt(playerIdentity)].GetComponent<SpriteRenderer>().color;
    }
}
