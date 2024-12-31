using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject pathmaker;
    [SerializeField] GameObject generateClip;
    public GameObject clicked;
    public GameObject unClicked;
    public AudioClip unlockedAudioClip;
    public AudioClip lockedAudioClip;
    public AudioClip restartClip;
    private AudioSource audioSource;
    private bool hasClicked = false;
    private MonoBehaviour[] cameraControllers;
    private int activeControllerIndex = 0;

    private void Start()
    {
        cameraControllers = mainCamera.GetComponents<MonoBehaviour>();
        audioSource = gameObject.GetComponent<AudioSource>();

        for (int i = 0; i < cameraControllers.Length; i++)
        {
            cameraControllers[i].enabled = (i == activeControllerIndex);
        }

        UpdateGameObjectState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            audioSource.clip = restartClip;
            audioSource.Play();
            pathmaker.SetActive(false);
            generateClip.SetActive(false);

            StartCoroutine(restart());
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchCameraController();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SwitchCameraController()
    {
        if (cameraControllers == null || cameraControllers.Length == 0)
        {
            return;
        }
        cameraControllers[activeControllerIndex].enabled = false;
        activeControllerIndex = (activeControllerIndex + 1) % cameraControllers.Length;
        cameraControllers[activeControllerIndex].enabled = true;

        hasClicked = !hasClicked;
        UpdateGameObjectState();
        PlayAudioClip();
    }

    private void UpdateGameObjectState()
    {
        if (clicked != null && unClicked != null)
        {
            clicked.SetActive(hasClicked);
            unClicked.SetActive(!hasClicked);
        }
    }

    private void PlayAudioClip()
    {
        if (audioSource != null)
        {
            audioSource.clip = hasClicked ? unlockedAudioClip : lockedAudioClip;
            audioSource.Play();
        }
    }
    IEnumerator restart()
    {
        yield return new WaitForSeconds(1);
        Pathmaker.globalTileCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}