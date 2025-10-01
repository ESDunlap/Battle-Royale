using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Data;

public class GameUI : MonoBehaviourPun
{
    public Slider healthBar;
    public Slider shieldBar;
    public Image healthBarColor;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI winText;
    public Image winBackground;
    private PlayerController player;

    // instance
    public static GameUI instance;
    void Awake()
    {
        instance = this;
    }

    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHp;
        healthBar.value = player.curHp;
        shieldBar.maxValue = player.maxShieldTime;
        shieldBar.value = player.curShieldTime;
        UpdatePlayerInfoText();
        UpdateAmmoText();
    }

    public void UpdateHealthBar()
    {
        shieldBar.value = player.curShieldTime;
        healthBar.value = player.curHp;
    }

    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n</b> Kills:</b> " + player.kills;
    }

    public void UpdateAmmoText()
    {
        ammoText.text = player.weapon.curAmmo + " / " + player.weapon.maxAmmo;
    }

    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
}
