using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;

            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount ++;

                // fire the spawn a plate visual event
                OnPlateSpawned.Invoke(this, EventArgs.Empty);

            }
        }
    }

    public override void Interact(Player player) {
        //player does not have anything in hand
        if (!player.HasKitchenObject()) {
            // there is an available plate on the counter
            if (platesSpawnedAmount > 0) {
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                // fire the remove a plate event
                OnPlateRemoved.Invoke(this, EventArgs.Empty);
            }
        }
    }

}
