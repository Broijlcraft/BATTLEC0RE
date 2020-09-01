using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun {
    public float maxHealth = 50f;
    public Image fillHealthBar;

    [Header("HideInInspector")]
    public bool isDead;
    public float currentHealth;

    private void Awake() {
        currentHealth = maxHealth;
    }

    public void DoDamage(float value) {
        if (!isDead) {
            photonView.RPC("RPC_DoDamage", RpcTarget.All, value);
        }
    }

    [PunRPC]
    void RPC_DoDamage(float value) {
        currentHealth -= value;
        if (currentHealth <= 0) {
            isDead = true;
            currentHealth = 0;
        }
        UpdateUiHeath();
    }

    void UpdateUiHeath() { 
        if (fillHealthBar) {
            fillHealthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}