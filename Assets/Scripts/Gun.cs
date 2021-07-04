using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{

    [SerializeField] protected float sinceLastShot;
    public float ammoLeft;
    public GameObject bulletImpactPrefab;

    protected new void Start()
    {
        base.Start();
        ammoLeft = ((GunInfo)itemInfo).ammo;
    }

    public abstract void Shoot();

    public override void Use()
    {
        if (drawingTimeLeft <= 0 && sinceLastShot >= ((GunInfo)itemInfo).timePerShot && ammoLeft > 0)
        {
            ammoLeft -= 1;
            sinceLastShot = 0f;
            Shoot();
            if (ammoLeft == 0)
            {
                Draw();
                ammoLeft = ((GunInfo)itemInfo).ammo;
            }
        }
    }
}
