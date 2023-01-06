using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class AphorismText 
{
    private static bool _init = false;
    private static string[] _arrAphorismTex;
    private static System.Random random = new System.Random();
    private const string pathAphorismText = "Text/AphorismEng";

    public static string GetStrRandomAphorismText()
    {
        if (!_init)
            if (InitAphorismText())
                _init = true;
            else
            {
                Debug.LogError($"Class [AphorismText] can't be initialized");
                return null;
            }
        return _arrAphorismTex[random.Next(0, _arrAphorismTex.Length)]; ;
    }

    private static bool InitAphorismText()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(pathAphorismText);
        //string filename = @"C:\Unity\Git_SoundAndEffects\SoundAndEffects\Assets\DataFiles\Text\AphorismEng.txt";
        List<string> tmpArray = new List<string>(50);
        //using (StreamReader reader = File.OpenText(filename))
        using (StringReader reader = new StringReader(textAsset.text))
        {
            string text;
            text = reader.ReadLine();
            while (text != null)
            {
                tmpArray.Add(text);
                text = reader.ReadLine();
            }
        }
        if (tmpArray.Count != 0)
        {
            _arrAphorismTex = tmpArray.ToArray();
            return true;
        }
        else
            return false;
    }
}
