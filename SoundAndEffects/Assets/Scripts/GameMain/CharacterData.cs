using System;
using UnityEngine;

[Serializable]
public class CharacterData : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _maxDefence;

    public event Action<string> HealthChanged;
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
        HealthChanged?.Invoke(Health.ToString());
    }

    private void OnValidate()
    {
        Health = _health;
        HealthChanged?.Invoke(Health.ToString());
    }
}

