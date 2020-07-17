using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private static int nextSpawnerId = 1;
    
    public int spawnerId;
    public bool hasItem = false;
    public Item currentItem;

    private void Start() {
        hasItem = false;
        spawnerId = nextSpawnerId;
        nextSpawnerId++;
        spawners.Add(spawnerId, this);

        StartCoroutine(SpawnItem(1f));
    }

    private void OnTriggerEnter(Collider col) {
        if(hasItem && col.CompareTag("Player")) {
            Player _player = col.GetComponent<Player>();
            if(_player.AttemptPickupItem()) {
                ItemPickedUp(_player.id);
            }
        }
    }

    private void ItemPickedUp(int _byPlayer) {
        hasItem = false;
        ServerSend.ItemPickedUp(spawnerId, _byPlayer);
        StartCoroutine(SpawnItem(10f));
    }

    private IEnumerator SpawnItem(float _time) {
        yield return new WaitForSeconds(_time);
        hasItem = true;
        ServerSend.ItemSpawned(spawnerId);
    }
}
