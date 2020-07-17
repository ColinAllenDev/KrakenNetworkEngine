using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllItems : MonoBehaviour {

    Item[] itemsInScene;
    Weapon[] weaponsInScene;
    Ammo[] ammoInScene;

    void Start() {
        itemsInScene = FindObjectsOfType<Item>();
        Debug.Log($"Items in scene: {itemsInScene.Length}");
        foreach(var item in itemsInScene) {
            var _itemType = item.itemType;
            Debug.Log($"Item Type: {_itemType}");
        }

        weaponsInScene = FindObjectsOfType<Weapon>();
        Debug.Log($"Weapons in scene: {weaponsInScene.Length}");
        foreach(var weapon in weaponsInScene) {
            Debug.Log($"Weapon ID: {weapon.weaponId}");
            Debug.Log($"Weapon Type:  {weapon.weaponType}");
            Debug.Log($"Fire Type:  {weapon.fireType}");        
            Debug.Log($"Weapon Name:  {weapon.name}");
            Debug.Log($"Max Damage:  {weapon.maxDamage}");
        }

        ammoInScene = FindObjectsOfType<Ammo>();
        Debug.Log($"Ammo in scene: {ammoInScene.Length}");
        foreach(var ammo in ammoInScene) {
            var _ammoId = ammo.ammoId;
        }

    }
}
