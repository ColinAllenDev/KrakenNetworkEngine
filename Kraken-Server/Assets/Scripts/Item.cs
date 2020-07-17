using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {
    
    #region Networking Stuff
    public int id;
    public static Dictionary<int, Item> items = new Dictionary<int, Item>();
    private static int nextItemId = 1;
    #endregion

    #region Item Type
    public enum ItemType {
        Weapon,
        HealthPickup,
        AmmoPickup,
        Throwable
    }
    [Header("Item Type")]
    public ItemType itemType;
    #endregion

    #region Pickup Options
    public bool canBePickedUp;
    public bool canBeDropped;
    #endregion

    void Awake() {
        id = nextItemId;
        nextItemId++;
        items.Add(id, this);
    }

    void OnDestroy() {
        items.Remove(id);
        nextItemId--;
    }

}
