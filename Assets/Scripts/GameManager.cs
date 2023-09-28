using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    public PhotoScoringManager photoScoringManager;
    public List<GameObject> objectsToCapture;
    [SerializeField] private TextMeshProUGUI tasksListText;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementObjectIndex()
    {
        currentObjectIndex++;
        RedrawTasks();
    }

    private void RedrawTasks()
    {
        if (currentObjectIndex <= 0)
        {
            tasksListText.text = "A Blue Mushroom\nA Cat";
        }
        if (currentObjectIndex == 1)
        {
            tasksListText.text = "A Cat";
        }
        if (currentObjectIndex > 1)
        {
            tasksListText.text = "Anything!";
        }
    }
}
