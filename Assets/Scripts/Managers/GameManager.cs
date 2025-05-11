using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int collectedFragments = 0;
    public int currentLevel = 1;
    public Vector3 checkpointPosition;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("CollectedFragments", collectedFragments);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetFloat("CheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat("CheckpointY", checkpointPosition.y);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        collectedFragments = PlayerPrefs.GetInt("CollectedFragments", 0);
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        float x = PlayerPrefs.GetFloat("CheckpointX", 0);
        float y = PlayerPrefs.GetFloat("CheckpointY", 0);
        checkpointPosition = new Vector3(x, y, 0);
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        SaveGame();
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        SceneManager.LoadScene("Level" + levelNumber);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CollectFragment()
    {
        collectedFragments++;
        SaveGame();
    }
}