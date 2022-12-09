using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ThrowableOnSteroids : Throwable
{
    protected void Start()
    {
        //To have an enabled checkbox
    }

    protected override void OnAttachedToHand(Hand hand)
    {
        if (enabled) base.OnAttachedToHand(hand);
    }

    protected override void OnHandHoverBegin(Hand hand)
    {
        if (enabled) base.OnHandHoverBegin(hand);
    }

    protected override void HandHoverUpdate(Hand hand)
    {
        if (enabled) base.HandHoverUpdate(hand);
    }
    protected override void HandAttachedUpdate(Hand hand)
    {
        if (enabled) base.HandAttachedUpdate(hand);
        else hand.DetachObject(gameObject);
    }
}
