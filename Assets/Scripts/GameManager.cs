using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    public PhotoScoringManager photoScoringManager;
    public List<GameObject> objectsToCapture;
    [SerializeField] private TextMeshProUGUI mushroomTaskText;
    public GameObject mushroomTaskUntick;
    public GameObject mushroomTaskTick;
    public GameObject mushroomTaskStrike;
    [SerializeField] private TextMeshProUGUI catTaskText;
    public GameObject catTaskUntick;
    public GameObject catTaskTick;
    public GameObject catTaskStrike;
    public GameObject anythingTask;
    public static GameManager Instance;

    public int currentObjectIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        RedrawTasks();
    }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void IncrementObjectIndex()
    {
        currentObjectIndex++;
        RedrawTasks();
    }

    private void RedrawTasks()
    {
        switch (currentObjectIndex)
        {
            case <= 0:
                mushroomTaskText.color = Color.black;
                catTaskText.color = Color.black;
                mushroomTaskUntick.SetActive(true);
                catTaskUntick.SetActive(true);
                mushroomTaskTick.SetActive(false);
                catTaskTick.SetActive(false);
                mushroomTaskStrike.SetActive(false);
                catTaskStrike.SetActive(false);
                anythingTask.SetActive(false);
                break;
            case 1:
                mushroomTaskText.color = Color.grey;
                catTaskText.color = Color.black;
                mushroomTaskUntick.SetActive(false);
                catTaskUntick.SetActive(true);
                mushroomTaskTick.SetActive(true);
                catTaskTick.SetActive(false);
                mushroomTaskStrike.SetActive(true);
                catTaskStrike.SetActive(false);
                anythingTask.SetActive(false);
                break;
            case > 1:
                mushroomTaskText.color = Color.grey;
                catTaskText.color = Color.grey;
                mushroomTaskUntick.SetActive(false);
                catTaskUntick.SetActive(false);
                mushroomTaskTick.SetActive(true);
                catTaskTick.SetActive(true);
                mushroomTaskStrike.SetActive(true);
                catTaskStrike.SetActive(true);
                anythingTask.SetActive(true);
                break;
        }
    }
}
