using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Health : MonoBehaviourPun {
    public float maxHealth = 50f;
    public Image fillHealthBar;

    public int respawnTime = 6;

    [HideInInspector] public bool isDead, respawning;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public Controller controller;

    private void Awake() {
        currentHealth = maxHealth;
    }
    
    IEnumerator Countdown() {
        UiManager.single_UM.respawnUiHolder.SetActive(true);
        int timer = respawnTime;
        while (timer > 0) {
            if (Manager.single_M.dev) { break; }
            UiManager.single_UM.respawnAnim.SetTrigger("Respawn");
            UiManager.single_UM.respawnTimer.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }
        photonView.RPC("RPC_Respawn", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Respawn() {
        isDead = false;
        respawning = false;
        if (photonView.IsMine) {
            UiManager.single_UM.respawnUiHolder.SetActive(false);
            if (!Manager.single_M.dev) {
                controller.ResetAtStartPosition();
            }
        }
        photonView.RPC("RPC_ChangeHealth", RpcTarget.All, maxHealth, 1, "");
    }

    public void DoDamage(float value, string killer) {
        if (!isDead) {
            photonView.RPC("RPC_ChangeHealth", RpcTarget.All, value, 0, killer);
        }
    }

    [PunRPC]
    void RPC_ChangeHealth(float value, int add, string killer) {
        if (Tools.IntToBool(add)) {
            currentHealth += value;
        } else {
            currentHealth -= value;
        }
        if (currentHealth <= 0) {
            isDead = true;
            currentHealth = 0;
            if (photonView.IsMine) {
                string nickname = photonView.Owner.NickName;
                nickname = Tools.RemoveIdFromNickname(nickname);
                photonView.RPC("RPC_KillFeed", RpcTarget.All, nickname, killer);
                if (!respawning) {
                    respawning = true;
                    StartCoroutine(Countdown());
                }
            }
        } else {
            isDead = false;
            if(currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }
        UpdateUiHeath();
    }

    [PunRPC]
    void RPC_KillFeed(string victim, string killer) {
        Debug.LogWarning("Still need to implement weapons icons here!");
        UiManager.single_UM.AddKillToFeed(System.DateTime.Now.Second + killer, victim, null, null, true);
    }

    void UpdateUiHeath() { 
        float fill = currentHealth / maxHealth;
        if (fillHealthBar) {
            fillHealthBar.fillAmount = fill;
        }
        if (photonView.IsMine) {
            UiManager.single_UM.ingameHealthBar.fillAmount = fill;
        }
    }
}