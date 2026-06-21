using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ByteConverter : MonoBehaviour
{
    public TMP_Text txt;
    public Slider fileSize;

    public static string FormatBytes(double bytes)
    {
        string[] units = {"B", "KB", "MB", "GB", "TB"};
        double size = bytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F2} {units[unitIndex]}";
    }

    void Start()
    {
        txt.text = "<= " + FormatBytes(fileSize.value);
    }

    public void OnValChange()
    {
        txt.text = "<= " + FormatBytes(fileSize.value);
    }
}
