using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometHitEventArgs : EventArgs
{
    public GameObject HitObject { get; }
    public float HitStrength { get; }
    public Vector3 HitLocation { get; }

    public CometHitEventArgs(GameObject hitObject, float hitStrength, Vector3 hitLocation)
    {
        HitObject = hitObject;
        HitStrength = hitStrength;
        HitLocation = hitLocation;
    }
}
