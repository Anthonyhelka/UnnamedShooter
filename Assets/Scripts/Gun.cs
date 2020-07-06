using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10.0f;
    public float range = 100.0f;
    public Camera camera;
    public ParticleSystem muzzleFlash;
    public float fireRate = 0.15f;
    public float canFire = -0.1f;
    public float recoilRotation;
    public Transform container;
    public float minRecoil = 0.4f;
    public float maxRecoil = 0.6f;
    public float recoil;
    public Animator animator;
    public AudioSource audioSource;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (canFire < Time.time)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        audioSource.Play();
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        Recoil();
        canFire = Time.time + fireRate;
    }

    void Recoil()
    {
        animator.SetTrigger("Firing");
        recoilRotation += Random.Range(minRecoil, maxRecoil);
        container.transform.localRotation = Quaternion.Euler(-recoilRotation, 0.0f, 0.0f);
    }
}
