public enum WeaponType
{
    Pistol,
    Revolver,
    Tommygun,
    AutoRifle,
    Machinegun,
    Shotgun,
    Rifle
}

[System.Serializable] // ��� ����������� � ����������
public class Weapon
{
    public WeaponType weaponType;
    public int ammo;
    public int maxAmmo;
}
