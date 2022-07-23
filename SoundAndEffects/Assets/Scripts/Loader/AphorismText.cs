using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AphorismText 
{
    public static string[] GetArrAphorismTex()
    {
        string filename = @"H:\_Unity\Git_SoundAndEffects\SoundAndEffects\Assets\DataFiles\Text\AphorismEng.txt";
        List<string> tmpArray = new List<string>(50);
        using (StreamReader reader = File.OpenText(filename))
        {
            string text;
            text = reader.ReadLine();
            while (text != null)
            {
                tmpArray.Add(text);
                text = reader.ReadLine(); 
            } 
        }
        if (tmpArray.Count!=0)
        {
            return tmpArray.ToArray();
        }
        else
            return null;
    }

}
