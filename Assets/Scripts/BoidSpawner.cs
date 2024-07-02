using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] BoidEntity boidPrefab;
    [SerializeField] Transform minLeftPoint;
    [SerializeField] Transform maxRightPoint;
    [SerializeField] BoidsDataSettings boidsData;
    [SerializeField] float threshHold;
    [SerializeField] int boidNumber;
    [SerializeField] Camera mainCam;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 direction;

    void Start()
    {
        SpawnBoids();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = GetMousePosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPos = GetMousePosition();
            direction = endPos - startPos;
            float angle = Vector3.Angle(Vector3.forward, direction);
            if (endPos.x < startPos.x)
            {
                angle *= -1;
            }
            startPos.y = 0f;
            BoidEntity boid = Instantiate(boidPrefab, startPos, Quaternion.Euler(Vector3.up * angle));
            boid.Init(minLeftPoint.position, maxRightPoint.position, boidsData);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCam.transform.position.y;
        return mainCam.ScreenToWorldPoint(mousePos);
    }

    void SpawnBoids()
    {
        StartCoroutine(CorSpawnBoids());
    }

    private IEnumerator CorSpawnBoids()
    {
        for (int i = 0; i < boidNumber; i++)
        {
            BoidEntity boid = Instantiate(boidPrefab, transform.position, Quaternion.identity);
            boid.Init(minLeftPoint.position, maxRightPoint.position, boidsData);
            boid.transform.position = GetSpawnPosition();
            boid.transform.eulerAngles = GetAngle(boid.transform.position);
            yield return new WaitForSeconds(Random.Range(0.02f, 0.07f));
        }
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        int x = Random.Range(0, 2);
        int y = Random.Range(0, 2);
        Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(0, y, mainCam.transform.position.y + 5));
        spawnPosition = new Vector3(Random.Range(minLeftPoint.position.x, maxRightPoint.position.x), 0f, worldPos.z);
        return spawnPosition;
    }

    private Vector3 GetAngle(Vector3 boidPosition)
    {
        Vector3 eulerAngle = Vector3.zero;
        Vector3 endPos = Random.insideUnitSphere * 10f;
        eulerAngle.y = Vector3.Angle(boidPosition, endPos);
        return eulerAngle;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(minLeftPoint.position, new Vector3(minLeftPoint.position.x, 0f, maxRightPoint.position.z));
        Gizmos.DrawLine(minLeftPoint.position, new Vector3(maxRightPoint.position.x, 0f, minLeftPoint.position.z));
        Gizmos.DrawLine(maxRightPoint.position, new Vector3(minLeftPoint.position.x, 0f, maxRightPoint.position.z));
        Gizmos.DrawLine(maxRightPoint.position, new Vector3(maxRightPoint.position.x, 0f, minLeftPoint.position.z));
    }
#endif
}
