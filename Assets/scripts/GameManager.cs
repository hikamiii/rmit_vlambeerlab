using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    private MonoBehaviour[] cameraControllers;
    private int activeControllerIndex = 0;

    private void Start()
    {
        // Get all camera controller scripts attached to the camera
        cameraControllers = mainCamera.GetComponents<MonoBehaviour>();

        // Disable all except the first controller by default
        for (int i = 0; i < cameraControllers.Length; i++)
        {
            cameraControllers[i].enabled = (i == activeControllerIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Pathmaker.globalTileCount = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchCameraController();
        }
    }

    private void SwitchCameraController()
    {
        if (cameraControllers == null || cameraControllers.Length == 0)
        {
            Debug.LogError("No camera controllers found!");
            return;
        }

        cameraControllers[activeControllerIndex].enabled = false;

        activeControllerIndex = (activeControllerIndex + 1) % cameraControllers.Length;

        cameraControllers[activeControllerIndex].enabled = true;

        Debug.Log($"Switched to Camera Controller {activeControllerIndex + 1}");
    }
}