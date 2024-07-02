using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidEntity : MonoBehaviour
{
    public Vector3 Direction;
    [SerializeField] List<GameObject> fishes;


    [SerializeField] List<BoidEntity> nearByBoids = new();
    [SerializeField] List<BoidEntity> closeByBoids = new();

    private Vector3 centerMass;
    private Vector3 cohesionVector;
    private Vector3 seperationVector;
    private Vector3 alignmentVector;
    private BoidsDataSettings boidsData;
    private int nearByBoidsCount;
    private int closeByBoidsCount;
    public float speed;
    private Vector3 minLeft;
    private Vector3 maxRight;

    void Start()
    {
        Direction = transform.forward;
        speed = Random.Range(boidsData.minSpeed, boidsData.maxSpeed);
        GameObject chosenOne = fishes[Random.Range(0, fishes.Count)];
        chosenOne.SetActive(true);
    }

    public void Init(Vector3 minLeft, Vector3 maxRight, BoidsDataSettings boidsData)
    {
        this.minLeft = minLeft;
        this.maxRight = maxRight;
        this.boidsData = boidsData;
    }

    private Vector3 GetCohesionVector()
    {
        centerMass = Vector3.zero;
        cohesionVector = Vector3.zero;
        for (int i = 0; i < nearByBoidsCount; i++)
        {
            centerMass += nearByBoids[i].transform.position;
        }

        centerMass /= nearByBoidsCount;
        cohesionVector = centerMass - transform.position;
        return cohesionVector.normalized * boidsData.cohesionFactor;
    }

    private Vector3 GetSeperationVector()
    {
        Vector3 seperationVector = Vector3.zero;
        for (int i = 0; i < closeByBoidsCount; i++)
        {
            if (Vector3.Distance(closeByBoids[i].transform.position, transform.position) <= boidsData.sperationRadius)
            {
                float distance = Vector3.Distance(closeByBoids[i].transform.position, transform.position);
                float ratio = distance / boidsData.sperationRadius;
                // ratio = 1 - ratio;
                ratio = Mathf.Clamp01(ratio);

                ratio = 1f;
                seperationVector += (transform.position - closeByBoids[i].transform.position) * ratio;
            }
        }

        return seperationVector.normalized * boidsData.seperationFactor;
    }

    private Vector3 GetAlignmentVector()
    {
        Vector3 alignmentVector = Vector3.zero;
        for (int i = 0; i < nearByBoidsCount; i++)
        {
            alignmentVector += nearByBoids[i].transform.forward;
        }

        alignmentVector /= nearByBoidsCount;
        return alignmentVector.normalized * boidsData.alignmentFactor;
    }

    private Vector3 GetFinalVector()
    {
        cohesionVector = GetCohesionVector();
        seperationVector = GetSeperationVector();
        alignmentVector = GetAlignmentVector();
        Vector3 FinalVector =
        cohesionVector
        +
        seperationVector
        +
        alignmentVector
        +
        transform.forward
        ;
        return FinalVector.normalized;
    }

    void Update()
    {
        GetNearByBoids();
        Movement();
        CheckBoundary();
    }

    private void Movement()
    {
        Direction = GetFinalVector();

        transform.forward = Vector3.RotateTowards(transform.forward, Direction, boidsData.rotateSpeed * Time.deltaTime, 0f);
        transform.position += Direction * speed * Time.deltaTime;
    }

    public void GetNearByBoids()
    {
        var Colliders = Physics.OverlapSphere(transform.position + transform.forward * 2, boidsData.checkRadius);
        nearByBoids.Clear();
        for (int i = 0; i < Colliders.Length; i++)
        {
            // if (Colliders[i].gameObject == this.gameObject)
            //     continue;
            if (Colliders[i].TryGetComponent<BoidEntity>(out var boidEntity))
                nearByBoids.Add(boidEntity);
        }

        var Colliders1 = Physics.OverlapSphere(transform.position + transform.forward * 2, boidsData.sperationRadius);
        closeByBoids.Clear();
        for (int i = 0; i < Colliders1.Length; i++)
        {
            if (Colliders1[i].gameObject == this.gameObject)
                continue;
            if (Colliders1[i].TryGetComponent<BoidEntity>(out var boidEntity))
                closeByBoids.Add(boidEntity);
        }

        nearByBoidsCount = nearByBoids.Count;
        closeByBoidsCount = closeByBoids.Count;
    }

    private void CheckBoundary()
    {
        Vector3 pos = transform.position;
        if (pos.x < minLeft.x)
            transform.position = new Vector3(maxRight.x, 0f, transform.position.z);
        if (pos.x > maxRight.x)
            transform.position = new Vector3(minLeft.x, 0f, transform.position.z);
        if (pos.z < minLeft.z)
            transform.position = new Vector3(transform.position.x, 0f, maxRight.z);
        if (pos.z > maxRight.z)
            transform.position = new Vector3(transform.position.x, 0f, minLeft.z);

    }


#if UNITY_EDITOR    
    void OnDrawGizmos()
    {
        if (boidsData.showDirection)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Direction);
        }

        if (boidsData.showCohesion)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, cohesionVector);
        }

        if (boidsData.showSeperation)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, seperationVector);
        }

        if (boidsData.showAlignment)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, alignmentVector);
        }

        if (boidsData.showRanges)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, boidsData.sperationRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, boidsData.checkRadius);
        }

        if (boidsData.showCenterMask)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(centerMass, 1f);
        }
    }
#endif
}