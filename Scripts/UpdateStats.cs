using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStats : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text stats;
    public Slider dashCd;

    private PlayerMovement pm;
    private bool dashStart = false;
    private string playerIdentity;
    public GameObject[] lifes;
    private bool coroutineStarted = false;

    public void SetPlayer(PlayerMovement player, string identity)
    {
        pm = player;
        this.playerIdentity = identity;
        if (PlayerPrefs.GetString("PlayerName") != null && PlayerPrefs.GetString($"{playerIdentity}String") == "Player1")
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
            Debug.Log(PlayerPrefs.GetString("PlayerName"));
        }
    }

    void Update()
    {
        stats.text = $"{pm.percent}%";
        if (pm.dead && !coroutineStarted)
        {
            StartCoroutine(SetLifes());
        }
        if (pm.isDashing && !dashStart)
        {
            dashStart = true;
            StartCoroutine(UpdateDashCd(pm.dashingCooldown));
        }
    }

    IEnumerator SetLifes()
    {
        coroutineStarted = true;
        yield return new WaitForSeconds(0.5f);
        lifes[pm.lifes].SetActive(false);
        coroutineStarted = false;
    }

    public IEnumerator UpdateDashCd(float cd)
    {
        dashCd.value = 0;
        yield return new WaitForSeconds(cd);
        for (float i = 0; i < cd; i += 0.1f)
        {
            dashCd.value += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        dashStart = false;
    }
}
