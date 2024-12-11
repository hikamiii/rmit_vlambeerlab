using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCameraControl : MonoBehaviour
{
    public static FreeCameraControl instance;
    public Transform followTransform;

    [SerializeField] Transform cameraTransform;
    [SerializeField] bool moveWithMouseDrag;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 50f;

    Vector3 newPosition;
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;


    private void Start()
    {
        instance = this;

        newPosition = transform.position;
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleZoomInput();
    }

    private void HandleZoomInput()
    {
        // Get the scroll input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (cameraTransform.GetComponent<Camera>().orthographic)
        {
            Camera camera = cameraTransform.GetComponent<Camera>();
            camera.orthographicSize -= scrollInput * zoomSpeed;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            // Adjust field of view for perspective cameras
            Camera camera = cameraTransform.GetComponent<Camera>();
            camera.fieldOfView -= scrollInput * zoomSpeed;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minZoom, maxZoom);
        }
    }


    void HandleCameraMovement()
    {
        if (moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }
    }



    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Create a plane at the camera's height for detecting drag start position
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                // Calculate the drag movement difference and directly update position
                Vector3 dragDifference = dragStartPosition - dragCurrentPosition;

                transform.position += dragDifference;
            }
        }
    }
}