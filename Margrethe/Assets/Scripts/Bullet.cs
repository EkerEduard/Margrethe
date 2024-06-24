using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);

        Destroy(gameObject);
    }

    private void CreateImpactFX(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

        Destroy(newImpactFX, 1.0f); // ’мм... чуть позже попробовать 0.5
    }
}
