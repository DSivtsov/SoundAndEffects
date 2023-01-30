using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTools.Menu
{
    public class CanvasObject : MonoBehaviour
    {
        [SerializeField] private CanvasName _canvasName;
        private SectionManager _sectionManager;

        public CanvasName CanvasName => _canvasName;
        public SectionManager SectionManager => _sectionManager;

        private void Awake() => _sectionManager = gameObject.GetComponent<SectionManager>();
    }
}
