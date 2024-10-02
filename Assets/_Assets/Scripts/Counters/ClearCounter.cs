using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        // There is no KitchenObject on the counter
        if (!HasKitchenObject()) {

            // if player is carrying something, set it on the counter
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            // if player has nothing in hands, do nothing
            else {

            }
        }
        // There is a KitchenObject here        
        else {
            // if the player is carrying something, do nothing, unless its a plate, then put the item on the plate
            if (player.HasKitchenObject()) {
                // if the player is holding a plate
                // get the plate and then add the object to the plate (aka the plate list containing all the obejcts on the plate)
                // if successfully added to the plate, then destory the object from the counter
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                } 
                // player is not carrying a plate, but something else
                else {
                    // if counter has a plate on it
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        // try to add the object the player is holding onto the plate
                        // if successful, destory the object from the player hands
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            // the player is not carrying something, then pick up the KitchenObject on counter
            else {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }
}
