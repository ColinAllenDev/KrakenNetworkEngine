using UnityEngine;

public class Loadout : MonoBehaviour {

    #region Loadout
    [Header("Loadout")]
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public Weapon meleeWeapon;
    private Weapon currentWeapon;
    private int currentWeaponIndex = 1;
    #endregion

    #region Weapon Settings
    [Header("Weapon Settings")]
    public Transform weaponSocket;
    #endregion

    #region Getters
    public Weapon GetCurrentWeapon() {
        return currentWeapon;
    }
    #endregion

    void Update() {
        // Weapon Input Handling
        for(int i = 1; i <= weaponSocket.childCount; i++) {
            if(Input.GetKeyDown("" + i)) {
                currentWeaponIndex = i - 1;
                SwitchWeapon(currentWeaponIndex);
            }
        }
    }

    // TODO: Switch to assetbundles to manage resources
    // TODO: Maybe find a more practical way of doing this
    public void SetLoadout(string _primary, string _secondary, string _melee) {
        // Remove whitespace for path
        _primary = _primary.Replace(" ", string.Empty);
        _secondary = _secondary.Replace(" ", string.Empty);
        _melee = _melee.Replace(" ", string.Empty);

        // Load weapon resources
        Weapon _primaryAsset = Resources.Load($"Weapons/{_primary}", typeof(Weapon)) as Weapon;
        Weapon _secondaryAsset = Resources.Load($"Weapons/{_secondary}", typeof(Weapon)) as Weapon;
        Weapon _meleeAsset = Resources.Load($"Weapons/{_melee}", typeof(Weapon)) as Weapon;

        // Instantiate weapons
        primaryWeapon = Instantiate(_primaryAsset, weaponSocket.position, weaponSocket.rotation, weaponSocket);
        secondaryWeapon = Instantiate(_secondaryAsset, weaponSocket.position, weaponSocket.rotation, weaponSocket);
        meleeWeapon = Instantiate(_meleeAsset, weaponSocket.position, weaponSocket.rotation, weaponSocket);

        // Set active weapon
        currentWeapon = primaryWeapon;
        primaryWeapon.gameObject.SetActive(true);
        secondaryWeapon.gameObject.SetActive(false);
        meleeWeapon.gameObject.SetActive(false);
    }

    void SwitchWeapon(int _index) {
        for(int i = 0; i < weaponSocket.childCount; i++) {
            if(i == _index) {
                weaponSocket.GetChild(i).gameObject.SetActive(true);
                currentWeapon = weaponSocket.GetChild(i).GetComponent<Weapon>();
            }
            else {
                weaponSocket.GetChild(i).gameObject.SetActive(false);
            } 
        }
    }

}
