using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    [SerializeField] protected bool wasDrawn = false; 
    [SerializeField] protected Vector3 initialPosition;
    [SerializeField] protected Quaternion initialRotation;
    [SerializeField] protected Vector3 animatedPosition;
    [SerializeField] protected Quaternion animatedRotation;
    [SerializeField] protected Vector3 animatedPositionDifference;
    [SerializeField] protected Vector3 animatedRotationDifference;
    [SerializeField] protected float goingAwayTime;
    [SerializeField] float lerpAway;
    [SerializeField] float lerpBack;
    protected float goingAwayTimeLeft = 0;
    [SerializeField] protected float drawTime;
    [SerializeField] protected float lerpDraw;
    protected float drawingTimeLeft = 0;

    public abstract void Use();
    public abstract void Draw();

    protected void Start()
    {
        if(itemInfo.name != "Rifle")
        {
            initialPosition = itemGameObject.transform.localPosition;
            animatedPosition = initialPosition + animatedPositionDifference;
            initialRotation = itemGameObject.transform.localRotation;
            animatedRotation = initialRotation * Quaternion.Euler(animatedRotationDifference);
            Debug.Log("Saved initial pos+rot " + itemInfo.name);
        }
    }

    private void FixedUpdate()
    {
        if(drawingTimeLeft > 0)
        {
            drawingTimeLeft -= Time.deltaTime;
            itemGameObject.transform.localPosition = Vector3.Lerp(itemGameObject.transform.localPosition, initialPosition, lerpDraw);
            itemGameObject.transform.localRotation = Quaternion.Lerp(itemGameObject.transform.localRotation, initialRotation, lerpDraw);
        }
        else
        {
            if(goingAwayTimeLeft>0)
            {
                goingAwayTimeLeft -= Time.deltaTime;
                itemGameObject.transform.localPosition = Vector3.Lerp(itemGameObject.transform.localPosition, animatedPosition, lerpAway);
                itemGameObject.transform.localRotation = Quaternion.Lerp(itemGameObject.transform.localRotation, animatedRotation, lerpAway);
            }
            else
            {
                itemGameObject.transform.localPosition = Vector3.Lerp(itemGameObject.transform.localPosition, initialPosition, lerpBack);
                itemGameObject.transform.localRotation = Quaternion.Lerp(itemGameObject.transform.localRotation, initialRotation, lerpBack);
            }
        }
    }
}
