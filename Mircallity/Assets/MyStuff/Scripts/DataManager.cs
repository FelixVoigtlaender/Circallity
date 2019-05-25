using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataManager {
    private static string colorInfoFileName = "/colorInfo.dat";
    public static void DeletAll()
    {

    }

    public static void SaveColors(ColorInfo info)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath +colorInfoFileName);

        bf.Serialize(file, ConvertColorInfoToColorData(info));
        file.Close();
    }

    public static ColorInfo LoadColors()
    {
        if(File.Exists(Application.persistentDataPath + colorInfoFileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + colorInfoFileName, FileMode.Open);
            ColorData data = (ColorData)bf.Deserialize(file);
            file.Close();

            return ConvertColorDataToColorInfo(data);
        }
        return null;
    }
    public static ColorInfo ConvertColorDataToColorInfo(ColorData colorData)
    {
        ColorInfo ci = new ColorInfo();
        ci.currentColor = colorData.currentColor.GetColor();
        ci.allColors = new Color[colorData.allColors.Length];
        for(int i = 0; i < ci.allColors.Length; i++)
        {
            ci.allColors[i] = colorData.allColors[i].GetColor();
        }
        return ci;
    }
    public static ColorData ConvertColorInfoToColorData(ColorInfo colorInfo)
    {
        ColorData cd = new ColorData();
        cd.currentColor = new SerColor(colorInfo.currentColor);
        cd.allColors = new SerColor[colorInfo.allColors.Length];
        for (int i = 0; i < cd.allColors.Length; i++)
        {
            cd.allColors[i] = new SerColor(colorInfo.allColors[i]);
        }
        return cd;
    }
}
public class ColorInfo
{
    public Color currentColor;
    public Color[] allColors;
}

[Serializable]
public class ColorData
{
    public SerColor currentColor;
    public SerColor[] allColors;
}

[Serializable]
public class SerColor
{
    float[] color;
    public SerColor(Color color)
    {
        SetColor(color);
    }
    public Color GetColor()
    {
        return new Color(color[0], color[1], color[2], color[3]);
    }

    public void SetColor(Color color)
    {
        this.color = new float[] { color.r, color.g, color.b, color.a };
    }

}