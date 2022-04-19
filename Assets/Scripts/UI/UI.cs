using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Boss _boss;
    [SerializeField] private Player _player;
    [SerializeField] private LevelsLoader _levelsLoader;
    [SerializeField] private InputController _inputController;
    [Header("Canvas")]
    [SerializeField] private CanvasGroup _gamePanel;
    [SerializeField] private CanvasGroup _startPanel;
    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private CanvasGroup _losePanel;
    [Header("Buttons")]
    [SerializeField] private Button _retry;
    [SerializeField] private Button _restart;
    [SerializeField] private Button _fightTap;
    [SerializeField] private Button _tapToFight;

    private const float DURATION = 0.5f;

    public event Action FightClicked;
    public event Action TapToFightClicked;

    private void OnEnable()
    {
        _boss.Won += OnBossWon;
        _boss.Died += OnBossDied;
        _inputController.Clicked += StartGame;

        _retry.onClick.AddListener(Restart);
        _restart.onClick.AddListener(Restart);
        _fightTap.onClick.AddListener(OnFightCliked);
        _tapToFight.onClick.AddListener(OnTapToFightCliked);
    }

    private void OnDisable()
    {
        _boss.Won -= OnBossWon;
        _boss.Died -= OnBossDied;
        _inputController.Clicked -= StartGame;

        _retry.onClick.RemoveListener(Restart);
        _restart.onClick.RemoveListener(Restart);
        _fightTap.onClick.RemoveListener(OnFightCliked);
        _tapToFight.onClick.RemoveListener(OnTapToFightCliked);
    }

    private void StartGame()
    {
        _player.EnableMove();
        EnableCanvas(_gamePanel);
        DisableCanvas(_startPanel);
    }

    private void Restart()
    {
        _levelsLoader.Restart();
    }

    private void OnTapToFightCliked()
    {
        _inputController.enabled = false;
        TapToFightClicked?.Invoke();
    }

    private void OnFightCliked()
    {
        FightClicked?.Invoke();
    }

    private void OnBossDied()
    {
        _fightTap.enabled = false;
        EnableCanvas(_winPanel);
    }

    private void OnBossWon()
    {
        _fightTap.enabled = false;
        EnableCanvas(_losePanel);        
    }

    private void EnableCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.DOFade(1, DURATION);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void DisableCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.DOFade(0, DURATION);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
