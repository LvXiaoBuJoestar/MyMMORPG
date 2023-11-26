using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideController : MonoBehaviour
{
    [SerializeField] Transform mountPoint;
    [SerializeField] EntityController rider;
    [SerializeField] Vector3 offset;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(this.mountPoint == null || this.rider == null) return;
        this.rider.SetRidePosition(mountPoint.position + mountPoint.TransformDirection(offset));
    }

    public void SetRider(EntityController rider)
    {
        this.rider = rider;
    }

    internal void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}
