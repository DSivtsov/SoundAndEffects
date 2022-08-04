using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTools.Menu
{

    public class CanvasManager : SingletonController<CanvasManager>
    {
        [SerializeField] private Transform groupsMenu;
        [SerializeField] private CanvasName startMenu;

        private List<CanvasObject> canvasControllerList;
        private CanvasObject lastActiveCanvas;

        protected override void Awake()
        {
            base.Awake();
            //ButtonActions.InitButtonActions(this);
            canvasControllerList = groupsMenu.GetComponentsInChildren<CanvasObject>(includeInactive: true).ToList();
            canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
            SwitchCanvas(startMenu);
        }

        public void SwitchCanvas(CanvasName _type)
        {
            if (lastActiveCanvas != null)
            {
                lastActiveCanvas.gameObject.SetActive(false);
            }

            CanvasObject desiredCanvas = canvasControllerList.Find(x => x.canvasName == _type);
            if (desiredCanvas != null)
            {
                GameObject gameObjectCanvas = desiredCanvas.gameObject;
                gameObjectCanvas.SetActive(true);
                SectionManager sectionManager = gameObjectCanvas.GetComponent<SectionManager>();
                //Check the desiredCanvas that it has the SectionManager
                if (sectionManager)
                {
                    //Debug.Log($"{sectionManager.name} is made Active");
                    sectionManager.SetActiveSectionManager();
                }
                lastActiveCanvas = desiredCanvas;
            }
            else { Debug.LogWarning("The desired canvas was not found!"); }
        }
    }

}