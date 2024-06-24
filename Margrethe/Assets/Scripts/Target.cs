using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Автоматически добавляет к объекту rigidbody
public class Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
