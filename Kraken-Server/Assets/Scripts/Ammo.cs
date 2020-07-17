using UnityEngine;

public class Ammo : Item, IPickupItem {
    #region Ammo Type
    public enum AmmoType {All, Pistols, Rifles, Melee, SMGs, Heavy}
    [Header("Ammo Attributes")]
    public AmmoType ammoType;
    #endregion
    
    #region Ammo Attributes
    public int ammoId;
    public string ammoName;
    public int maxAmmo;
    #endregion
    
    void OnTriggerEnter(Collider col) {
        if(col.CompareTag("Player")) {
            Pickup();
        }
        
    }

    public void Pickup() {
        Debug.Log("Player picked up ammo!");
    }
}