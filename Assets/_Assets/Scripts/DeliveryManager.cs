using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFail;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;

        // if the spawn timer is below 0 seconds, its time to add a new recipe to the waiting recipes
        if (spawnRecipeTimer <= 0f) { 
            spawnRecipeTimer = spawnRecipeTimerMax;

            // check there is less than the maximum number of orders out right now
            if (waitingRecipeSOList.Count < waitingRecipesMax) {
                // grab a random recipe from the possible recipes and add it to the waiting recipes list
                RecipeSO waitingRecipSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // check if the plate you delivered and the order matches by first checking if the number of ingredients in both are the same
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {

                bool plateContentsMatchesRecipe = true;

                // now cycle through each ingredient in the order recipe
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // now cycle through all the ingredients on the plate
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // check if the ingredient in the plate matches the ingredient in the recipe 
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }

                    // if the ingredient was not found of the plate
                    if (!ingredientFound) {
                        plateContentsMatchesRecipe = false;
                    }
                }

                // if player delivered the correct recipe, then remove the order from the waitingRecipeSOList
                if (plateContentsMatchesRecipe) {
                    waitingRecipeSOList.RemoveAt(i);

                    //add to counter to keep track of how many successful deliveries the player made
                    successfulRecipesAmount++;

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    // event to play audio of successfull delivery
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    return;
                }
            }
        }
        // if the code gets here, then player did not deliver a correct recipe
        OnRecipeFail?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() { 
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() { 
        return successfulRecipesAmount;
    }
}