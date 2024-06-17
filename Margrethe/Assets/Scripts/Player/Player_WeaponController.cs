using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bulletPrefab; // Префаб пули
    [SerializeField] private float bulletSpeed; // Скорость пули
    [SerializeField] private Transform gunPoint; // Точка создания пули

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();

        player.controlls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 5);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
        {
            direction.y = 0;
        }

        weaponHolder.LookAt(aim);
        gunPoint.LookAt(aim); 

        return direction;
    }

    public Transform GunPoint() => gunPoint;

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    //}
}
