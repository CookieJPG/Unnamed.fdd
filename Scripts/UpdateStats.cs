using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStats : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text stats;
    public Slider dashCd;

    private GameObject player;
    private PlayerMovement pm;
    private bool dashStart = false;

    public void Start()
    {
        if (PlayerPrefs.GetString("PlayerName") != null)
        {
            playerName.text = PlayerPrefs.GetString("PlayerName");
            Debug.Log(PlayerPrefs.GetString("PlayerName"));
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (player != null)
        {
            pm = player.GetComponent<PlayerMovement>();
            if (pm == null)
            {
                Debug.LogError("PlayerMovement component not found on player.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject is not set.");
        }
    }

    void Update()
    {
        stats.text = pm.percent.ToString();
        if (pm.isDashing && !dashStart)
        {
            dashStart = true;
            StartCoroutine(UpdateDashCd(pm.dashingCooldown));
        }
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
