﻿using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Health : MonoBehaviourPun {
    public int maxHealth = 100;

    public int respawnTime = 2;

    public Image fillHealthBar;

    [HideInInspector] public bool isDead, respawning;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public Controller controller;
    CanvasComponents cc;
    HealthBarScript healthBar;

    private void Awake() {
        currentHealth = maxHealth;
    }

    private void Start() {
        cc = CanvasComponents.single_CC;
        healthBar = cc.healthBar;
    }

    private void Update() {
        if (Input.GetButtonDown("1") && controller.IsMineAndAlive()) {
            if (PhotonNetwork.IsConnected) {
                DoDamage(20, "Darth Max");
            } else {
                currentHealth -= 1;
                if (currentHealth <= 0) {
                    currentHealth = maxHealth;
                }
                healthBar.ChangeHealth(currentHealth, maxHealth);
            }
        }
    }
    
    public void DoDamage(int value, string killer) {
        if (!isDead && !respawning) {
            int killerTeam = TeamManager.single_TM.GetTeamIndex(killer);
            int myTeam = TeamManager.single_TM.GetTeamIndex(photonView.Owner.NickName);
            if (killerTeam != myTeam) {
                if (killerTeam < ScoreScript.single_ss.scoreListings.Length) {
                    photonView.RPC(nameof(RPC_ChangeHealth), RpcTarget.All, value, 0, 1, killer, killerTeam);
                }
                print($"Killer: {killer}, Team: {killerTeam} | Victim: {photonView.Owner.NickName}, Team: {myTeam}");
            }
        }
    }

    [PunRPC]
    void RPC_ChangeHealth(int value, int add, int instant, string killer, int killerTeam) {
        if (!Tools.IntToBool(add)) {
            value *= -1;
        }

        if (Manager.single_M.IsDev() && value < 0) { return; }

        currentHealth += value;

        if (currentHealth < 1) {
            if (!respawning) {
                respawning = true;
                isDead = true;
                currentHealth = 0;
                controller.animator.enabled = false;

                if (photonView.IsMine) {
                    controller.rigid.velocity = Vector3.zero;
                    if (killerTeam > -1) {
                        ScoreScript.single_ss.IncreaseScore(killerTeam, 1);
                    }
                    string nickname = photonView.Owner.NickName;
                    nickname = Tools.RemoveIdFromNickname(nickname);
                    photonView.RPC(nameof(RPC_KillFeed), RpcTarget.All, nickname, killer);
                    StartCoroutine(Countdown());
                }
            }
        } else {
            isDead = false;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }

        bool _Instant = Tools.IntToBool(instant);
        if (_Instant) {
            UpdateUiHeath(_Instant);
        }
    }

    IEnumerator Countdown() {
        cc.respawnUiHolder.SetActive(true);
        int timer = respawnTime;
        while (timer > 0) {
            if (Manager.single_M.IsDev()) { break; }
            cc.respawnAnim.SetTrigger("Respawn");
            cc.respawnTimer.text = timer.ToString();
            yield return new WaitForSeconds(respawnTime);
            timer--;
        }
        photonView.RPC(nameof(RPC_Respawn), RpcTarget.All);
    }

    public void StopRespawning() {
        photonView.RPC(nameof(RPC_StopRespawning), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StopRespawning() {
        respawning = false;
    }

    [PunRPC]
    void RPC_Respawn() {
        UpdateUiHeath(false);
        isDead = false;
        controller.animator.enabled = true;
        if (photonView.IsMine) {
            cc.respawnUiHolder.SetActive(false);
            if (!Manager.single_M.IsDev()) {
                controller.ResetAtStartPosition();
            }
        }
        photonView.RPC(nameof(RPC_ChangeHealth), RpcTarget.All, maxHealth, 1, 0, "", -1);
    }

    [PunRPC]
    void RPC_KillFeed(string victim, string killer) {
        Debug.LogWarning("Still need to implement weapons icons here!");
        UiManager.single_UM.AddKillToFeed(killer, victim, null, null, true);
    }

    void UpdateUiHeath(bool instant) {
        float fill = currentHealth / maxHealth;
        fillHealthBar.fillAmount = fill;
        if (photonView.IsMine) {
            if (instant) {
                healthBar.ChangeHealth(currentHealth, maxHealth);
            } else {
                StartCoroutine(healthBar.ShowPartsOverTime(this));
            }
        }
    }
}