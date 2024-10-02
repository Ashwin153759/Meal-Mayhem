using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    // the following code checks if the player is moving so we can play the correct animation for when the player
    // is idle or walking
    
    private const string IS_WALKING = "IsWalking";

    [SerializeField] private Player player;
    private Animator animator;
    
    // ASK WHAT THIS DOES!!!!!
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    // update animator for every frame
    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
