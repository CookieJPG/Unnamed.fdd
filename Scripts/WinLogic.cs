using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLogic : MonoBehaviour
{
    private struct PlayerData
    {
        internal GameObject player;
        internal int index;
        internal bool deadPlayer;

        internal PlayerData(GameObject player, int index)
        {
            this.player = player;
            this.index = index;
            deadPlayer = false;
        }
    }

    private PlayerData[] pd;
    private int playerCount;
    private int currentPlayers = -1;

    internal bool playerWon = false;

    void Start()
    {
        Invoke(nameof(FindPlayers), 1f);
    }

    void FindPlayers()
    {
        GameObject[] plyr = GameObject.FindGameObjectsWithTag("Player");
        playerCount = plyr.Length;
        pd = new PlayerData[playerCount];
        for (int i = 0; i < plyr.Length; i++)
        {
            pd[i] = new PlayerData(plyr[i], i + 1);
        }
        currentPlayers = playerCount;
    }

    void Update()
    {
        if (PlayerPrefs.GetString("Scene") == "ObstRace")
        {
            if (currentPlayers == 0)
            {
                Debug.Log("All players are dead. Game over!");
                SceneManager.LoadScene("Menu");
            }
            return;
        }
        if (playerWon)
        {
            return;
        }
        for (int i = 0; i < playerCount; i++)
        {
            if (currentPlayers <= 1)
            {
                Debug.Log("Player " + pd[i].index + " wins!");
                PlayerPrefs.SetString("Victory", $"Player{i + 1}");
                playerWon = true;
                SceneManager.LoadScene("WinScreen");
                break;
            }
            if (!pd[i].player.activeSelf && !pd[i].deadPlayer)
            {
                pd[i].deadPlayer = true;
                currentPlayers--;
            }
        }
    }
}
