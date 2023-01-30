using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTools.Menu
{

    public class CanvasManager : SingletonController<CanvasManager>
    {
        [SerializeField] private Transform _groupsMenu;
        [SerializeField] private CanvasName _startMenu;

        private Dictionary<CanvasName, CanvasObject> _dictCanvasControllerList;
        private CanvasObject lastActiveCanvas;

        protected override void Awake()
        {
            base.Awake();
            List<CanvasObject> _canvasControllerList = _groupsMenu.GetComponentsInChildren<CanvasObject>(includeInactive: true).ToList();
            _dictCanvasControllerList =  new Dictionary<CanvasName, CanvasObject>(_canvasControllerList.Count);
            for (int i = 0; i < _canvasControllerList.Count; i++)
            {
                CanvasObject canvasObject = _canvasControllerList[i];
                _dictCanvasControllerList.Add(canvasObject.CanvasName, canvasObject);
                canvasObject.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            SwitchCanvas(_startMenu);
        }

        public void SwitchCanvas(CanvasName _type)
        {
            if (lastActiveCanvas != null)
            {
                lastActiveCanvas.gameObject.SetActive(false);
            }
            if (_dictCanvasControllerList.TryGetValue(_type, out CanvasObject desiredCanvas))
            {
                desiredCanvas.gameObject.SetActive(true);
                //SectionManager sectionManager = desiredCanvas.SectionManager;
                ////Check the desiredCanvas that it has the SectionManager
                //if (sectionManager)
                //{
                //    sectionManager.SetActiveSectionManager();
                //}
                SwitchCanvasCallSpecificActions(lastActiveCanvas, desiredCanvas);
                lastActiveCanvas = desiredCanvas;
            }
            else { Debug.LogWarning("The desired canvas was not found!"); }
        }

        protected virtual void SwitchCanvasCallSpecificActions(CanvasObject prevCanvasObject, CanvasObject nextCanvasObject) { }
    }
}