using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public float padding = 5f;
    public float smoothSpeed = 0.1f;

    private Vector3 targetPosition;
    private float currentFieldOfView = 30f;

    void LateUpdate()
    {
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor");
        if (floorTiles.Length == 0)
            return;

        // Calculate the average position of all floor tiles
        Vector3 averagePosition = Vector3.zero;
        foreach (GameObject tile in floorTiles)
        {
            averagePosition += tile.transform.position;
        }
        averagePosition /= floorTiles.Length;

        // Move the camera to the center
        targetPosition = new Vector3(averagePosition.x, transform.position.y, averagePosition.z + 38);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Dynamically adjust the field of view
        AdjustFieldOfView(floorTiles, averagePosition);
    }

    private void AdjustFieldOfView(GameObject[] floorTiles, Vector3 centerPosition)
    {
        float maxDistance = 0f;

        foreach (GameObject tile in floorTiles)
        {
            float distance = Vector3.Distance(centerPosition, tile.transform.position);
            maxDistance = Mathf.Max(maxDistance, distance);
        }

        float targetFieldOfView = Mathf.Clamp(10 + maxDistance * 2, 10, 90);
        currentFieldOfView = Mathf.Lerp(currentFieldOfView, targetFieldOfView, smoothSpeed);

        mainCamera.fieldOfView = currentFieldOfView;
    }
}