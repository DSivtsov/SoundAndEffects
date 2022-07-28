using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ManageTopMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjectHealth;
    [SerializeField] private TextMeshProUGUI strScore;
    [SerializeField] private TextMeshProUGUI strLevel;

    private GameParametersManager _gameParametersManager;
    private CharacterDataController _characterDataCtrl;

    private void Awake()
    {
        //Debug.Log($"{this} Awake()");
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        _characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
    }

    //private void Start()
    //{

    //    strLevel.text = $"{9:D2}";
    //    SetLevel(9);
    //    strScore.text = $"{9:000 000}";
    //    SetScore(9);
    //    gameObjectHealth[0].SetActive(false);
    //    SetHealth(3);
    //}

    private void OnEnable()
    {
        _gameParametersManager.LevelChanged += SetLevel;
        _characterDataCtrl.HealthChanged += SetHealth;
        _characterDataCtrl.ScoreChanged += SetScore;
    }

    private void OnDisable()
    {
        _gameParametersManager.LevelChanged -= SetLevel;
        _characterDataCtrl.HealthChanged -= SetHealth;
        _characterDataCtrl.ScoreChanged -= SetScore;
    }
    /// <summary>
    /// Set the Game Score
    /// </summary>
    /// <param name="score">Range[000 000:999 999]</param>
    public void SetScore(int score)
    {
        if (!(score < 0 || score > 999999))
        {
            strScore.text = $"{score:000 000}"; 
        }
    }
    /// <summary>
    /// Set the Game Level
    /// </summary>
    /// <param name="level">Range[01:99]</param>
    public void SetLevel(int level)
    {
        if (!(level < 1 || level > 99))
        {
            strLevel.text = $"{level:D2}";
        }
    }
    /// <summary>
    /// Fill the Bar of Lives in inverse order
    /// </summary>
    /// <param name="livesLeft">Range[0:3]</param>
    public void SetHealth(int livesLeft)
    {
        int maximumLives = gameObjectHealth.Length;
        for (int i = maximumLives; i >= 1; i--)
        {
            gameObjectHealth[maximumLives - i].SetActive(i <= livesLeft);
        }
    }
}
