using System;
using UnityEngine;

[Serializable]
public class CharacterData : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    //[SerializeField] private int _maxDefence;

    public event Action<int> HealthChanged;
    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            if (value > _maxHealth)
                _health = _maxHealth;
            else if (value < 0)
                _health = 0;
            else
                _health = value;
        }
    }

    private void Start()
    {
        if (SingletonGame.Instance.GameSceneManager().GameMainManagerOff)
        {
            ResetHealth(); 
        }
    }

    public void ResetHealth()
    {
        Health = _maxHealth;
        HealthChanged?.Invoke(Health);
    }

    //private void OnValidate()
    //{
    //    Health = _health;
    //    HealthChanged?.Invoke(_health);
    //}


    /// <summary>
    /// Change health
    /// </summary>
    /// <param name="decreaseAmmount">value with sign</param>
    public void ChangeHealth(int decreaseAmmount)
    {
        Health += decreaseAmmount;
        HealthChanged?.Invoke(Health);
        Debug.Log($"HealthChanged to {Health}");
    }
}

