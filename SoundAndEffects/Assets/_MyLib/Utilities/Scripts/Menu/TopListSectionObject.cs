using UnityEngine;

namespace GMTools.Menu
{
    public class TopListSectionObject : SectionObject
    {
        [SerializeField] private TopListController _topListController;

        public TopListController SectionTopListController => _topListController;
    }
}

