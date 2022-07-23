using System;
using UnityEngine;

namespace GMTools.Manager
{
    [Serializable]
    public class CharacterData : MonoBehaviour
    {
        [SerializeField] protected int _health;
        [SerializeField] protected int _defence;

        public event Action<string> SetHealthTopMenu;

        public int Health
        {
            get => _health;
            set
            {
                if (value > 100)
                    _health = 100;
                else if (value < 0)
                    _health = 0;
                else
                    _health = value;
            }
        }

        private void Start()
        {
            SetHealthTopMenu?.Invoke(Health.ToString());
        }

        private void OnValidate()
        {
            Health = _health;
            SetHealthTopMenu?.Invoke(Health.ToString());
        }
    }
}
