using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    /// <summary>
    /// <para>Shows a random graphic of <see cref="_ComboGraphics"/> on combo.</para>
    /// <para> Combo is: Scoring more than <see cref="_MinComboCount"/> less than <see cref="_ComboInterval"/> seconds.</para> 
    /// <para> Call <see cref="DidScore"/> when scored.</para>
    /// </summary>
    public class ComboController: MonoBehaviour
    {
        [SerializeField, Required] private GameObject[] _ComboGraphics;
        [SerializeField, Required] private float _ComboInterval = 1f;
        [SerializeField, Required] private int _MinComboCount = 2;
        [SerializeField, Required] private float _MinShowInterval = .5f;
        
        private float _lastScoreTime;
        private float _lastComboShowTime;
        [ReadOnly, ShowInInspector] private int _comboCount;

        private void Start()
        {
            foreach (var comboGraphic in _ComboGraphics)
            {
                comboGraphic.SetActive(false);
            }
        }

        [Button]
        public void DidScore()
        {
            if (_lastScoreTime + _ComboInterval > Time.time)
            {
                DidCombo();
            }
            else
            {
                _comboCount = 1;
            }

            _lastScoreTime = Time.time;
        }

        private void DidCombo()
        {
            _comboCount++;
            if (_lastComboShowTime + _MinShowInterval < Time.time && _comboCount >= _MinComboCount)
            {
                _lastComboShowTime = Time.time;
                ShowRandomGraphics();
            }
        }

        private void ShowRandomGraphics()
        {
            var rnd = Random.Range(0, _ComboGraphics.Length);
            for (var index = 0; index < _ComboGraphics.Length; index++)
            {
                var comboGraphic = _ComboGraphics[index];
                comboGraphic.SetActive(index == rnd);
            }
        }
    }
}