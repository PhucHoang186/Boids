using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Boids Data")]
public class BoidsDataSettings : ScriptableObject
{
    [Header("Movement Params")]
    public float rotateSpeed;
    public float minSpeed;
    public float maxSpeed;

    [Header("Boids Tunable Value")]
    [Range(0f, 5f)]
    public float seperationFactor;
    [Range(0f, 5f)]
    public float cohesionFactor;
    [Range(0f, 5f)]
    public float alignmentFactor;
    public float sperationRadius = 10f;
    public float checkRadius = 10f;

    [Header("Debugging")]
    public bool showCohesion;
    public bool showSeperation;
    public bool showAlignment;
    public bool showDirection;
    public bool showRanges;
    public bool showCenterMask;
}
