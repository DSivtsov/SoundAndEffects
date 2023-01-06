using GMTools.Menu.Elements;
using UnityEngine;
using UnityEngine.UI;

public enum DefaultTopListToggleEnum : byte
{
    Local,
    Global,
}

public class ToggleGroupBoolDefTopList : ToggleGroupBoolEnum<DefaultTopListToggleEnum> { }