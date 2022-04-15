using UnityEngine;
using IJunior.TypedScenes;

public class LevelsLoader : MonoBehaviour
{
    [SerializeField] private Boss _boss;
    [SerializeField] private int _levelsNumber;
    [SerializeField] private bool _isStartApp = false;
    [SerializeField] private bool _isTestLevel = false;

    private readonly int _levelsAmount = 5;
    private float _spentTime;

    public int Level => _levelsNumber;

    public void LoadCurrentLevelOnStartApp()
    {
        string currentLevel = AmplitudeHandler.LEVEL;
        int level = PlayerPrefs.GetInt(currentLevel);

        Load(level);
    }

    public void Next()
    {
        int nextLevel = _levelsNumber + 1;
        Load(nextLevel);
    }

    public void Restart()
    {
        Load(_levelsNumber);

        AmplitudeHandler.SetRestartLevel(_levelsNumber);
    }

    private void OnEnable()
    {
        if(_isTestLevel)
            Debug.Log("TEST LEVEL");

        if (_isStartApp || _isTestLevel)
            return;

        string currentLevelName = AmplitudeHandler.LEVEL;
        int currentLevel = PlayerPrefs.GetInt(currentLevelName);
        _levelsNumber = currentLevel;

        AmplitudeHandler.SetLevelStart(_levelsNumber);

        _boss.BossWon += OnLevelDone;
    }

    private void OnDisable()
    {
        if (_isStartApp)
            return;

        _boss.BossWon -= OnLevelDone;
    }

    private void OnLevelDone()
    {
        string level = AmplitudeHandler.LEVEL;

        if (_isTestLevel == false)
        {
            int nextLevel = _levelsNumber + 1;
            PlayerPrefs.SetInt(level, nextLevel);
        }
        else
        {
            Debug.Log("Test Level Done");
        }

        AmplitudeHandler.SetLevelComplete(_levelsNumber, (int)_spentTime);
    }


    private void RandomLevel()
    {
        int randomLevel = Random.Range(3, _levelsAmount + 1);
        Load(randomLevel);
    }

    private void Load(int number)
    {
        switch (number)
        {
            case 0:
                Visual.Load();
                break;
            default:
                RandomLevel();
                break;
        }
    }

    private void Update()
    {
        _spentTime += Time.deltaTime;
    }
}