using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMenuAnim : MonoBehaviour {

    public Animator door, bot, mine;

    [Header("Door")]
    public bool open, close;
    bool hasBeenOpen, hasBeenClosed;

    [Header("Bot")]
    public bool dropFromTop, dropFromDoor, land, walk;
    bool wasDroppedFromTop, wasDroppenFromTop, hasLanded, hasWalked;

    private void Update() {
        if (open && !hasBeenOpen) {
            door.SetTrigger("Open");
            open = false;
        } else if(!open && hasBeenOpen) {
            hasBeenOpen = false;
        }
    }
}