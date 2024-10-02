using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {


    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() { 
        OnAnyCut = null;
    }


    public event EventHandler <IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;


    [SerializeField] private CuttingRecipeSO[] cutKitchenObjectSOArray;

    private int cuttingProgress;

    public override void Interact(Player player) {
        // There is no KitchenObject here
        if (!HasKitchenObject()) {
            // if player is carrying something, set it on the counter
            if (player.HasKitchenObject()) {

                // this code works only if the ingredient in their hands CAN be cut
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    cuttingProgress = 0;

                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    // get the cutting progress for the bar

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    });
                } 
                // this code runs if the ingredient CANNOT be cut
                else if (!HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                }
            } 
            // if player has nothing in hands, do nothing
            else { 
                
            }
        }
        // There is a KitchenObject here        
        else {
            // if the player is carrying something, do nothing, unless it is a plate, then pick up the object onto the plate
            if (player.HasKitchenObject()) {
                // if the player is holding a plate
                // get the plate and then add the object to the plate (aka the plate list containing all the obejcts on the plate)
                // if successfully added to the plate, then destory the object from the counter
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            // the player is not carrying something, then pick up the KitchenObject on counter
            else {
                
                // make the progress bar disappear (this is code I added)
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });

                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        
        // If there is a KitchenObject on the counter AND the kitchenObject can be cut, do the cutting action
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            // add 1 to the cutting progress every time you press F
            cuttingProgress += 1;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);   

            // get the output kitchenObject (the cut ingredient)
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            // get the cutting progress for the bar
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            // if the amount of times you press F to cut exceeds the max, then return the cut object
            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                //get rid of the old kithenObject (non cut ingredient)
                GetKitchenObject().DestroySelf();

                // spawn the new kitchenObject
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

            }
        }
    }

    // return the cuttingRecipeSO for the coressponding ingredient
    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    // return the corressponding cut version of the ingredient
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;

        } else {
            return null;
        }
    }

    // check if the kitchenObject is cuttable
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cutKitchenObjectSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO;
            }
        }

        return null;   
    }

}
