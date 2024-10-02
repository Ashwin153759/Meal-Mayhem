using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerCounter : BaseCounter {

    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player) {
        // give the object from the containerCounter to the player
        // only give it if the player does not have anything in their hands and the counter doesn't have anything on it
        if (!player.HasKitchenObject() && !HasKitchenObject()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);

        } 
        // if the player has an item in hands and the counter doesn't have something on it, then place the object on counter
        else if (player.HasKitchenObject() && !HasKitchenObject()) {
            player.GetKitchenObject().SetKitchenObjectParent(this);
        } 
        // the counter has something on it, so the player then picks it up
        else {
            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }

}
