using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    private float damage;
    private Rigidbody rb;


    // Method to handle creation of a bullet and pass required information
    public void Create(Transform startTransform, Vector3 direction, float speed, float _damage, float bulletLife)
    {
        // Assigning any required stats for this bullet
        damage = _damage;

        // Setting the start position and rotation of the bullet
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        rb = GetComponent<Rigidbody>();
        // Assign start velocity
        rb.velocity = transform.forward * speed;

        // Start bullet deletion process
        Invoke(nameof(DeleteBullet), bulletLife);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Dealing damage when required
        DamageHandler damageHandler = other.gameObject.GetComponent<DamageHandler>();
        if (damageHandler) { damageHandler.DealDamage(damage); }

        // Deleting object when dealing damage
        Destroy(gameObject);
    }

    public void DeleteBullet()
    {
        Destroy(gameObject);
    }
}
