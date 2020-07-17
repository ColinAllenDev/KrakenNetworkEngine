using UnityEngine;

public abstract class Weapon : Item, IPickupItem, IDropItem {

    #region Networking Stuff
    public int ownerId;
    #endregion

    #region Weapon Enums   
    public enum WeaponState {Dequipped, Equipped, Dropped, Firing, Reloading}
    public enum WeaponType {Rifles, Pistols, Melee, SMGs, Heavy}
    public enum FireRate {FullAuto, SemiAuto, Burst}
    public enum FireType {Hitscan, Projectile}
    [Header("Weapon Type")]
    public WeaponState weaponState;
    public WeaponType weaponType;
    public FireRate fireRate;
    public FireType fireType;
    #endregion

    #region Weapon Attributes
    [Header("Weapon Attributes")]    
    public int weaponId;
    public string weaponName;
    public float maxDamage;
    public int maxAmmo;
    public int magSize;
    #endregion

    public void Pickup() {
        weaponState = WeaponState.Equipped;
    }

    public void Drop() {
        weaponState = WeaponState.Dequipped;
    }
    
}