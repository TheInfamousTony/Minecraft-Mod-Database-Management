using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [HideInInspector]
    public int dbID;
    [HideInInspector]
    public int dbDownloads;
    [HideInInspector]
    public double dbSize;
    [HideInInspector]
    public string dbName;
    [HideInInspector]
    public string dbRelease_date;
    [HideInInspector]
    public string dbLatest_ver;
    [HideInInspector]
    public string dbMod_type;

    public TMP_Text nameText;
    public TMP_Text DownloadText;
    public TMP_Text RelDateText;
    public TMP_Text LatestVerText;
    public TMP_Text SizeText;
    public TMP_Text ModTypeText;
    public TMP_Text TagText;
    public GameObject banner;

    [HideInInspector]
    public SQL_Script sqlscript;

    int curDownloads;

    void Start()
    {
        sqlscript = GameObject.FindGameObjectWithTag("SQLMANAGER").GetComponent<SQL_Script>();
    }

    Texture2D LoadImg(byte[] binery)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(binery);
        return tex;
    }

    public static string FormatBytes(double bytes)
    {
        string[] units = {"B", "KB", "MB", "GB", "TB"};
        double size = bytes;
        int unitIndex = 0;

        while (size >= 1000 && unitIndex < units.Length - 1)
        {
            size /= 1000;
            unitIndex++;
        }

        return $"{size:F2} {units[unitIndex]}";
    }

    public void SetInfo(string name, int downloads, string release_date, string latest_ver, int size, string mod_type, byte[] image, string tag)
    {
        nameText.text = name;
        DownloadText.text = downloads.ToString();
        curDownloads = downloads;
        RelDateText.text = "Date: " + release_date;
        LatestVerText.text = "Version: " + latest_ver;
        SizeText.text = FormatBytes(size);
        ModTypeText.text = mod_type;
        TagText.text = tag;

        Texture2D textu = LoadImg(image);
        banner.GetComponent<Image>().sprite  = Sprite.Create(textu, new Rect(0f, 0f, textu.width, textu.height), new Vector2(0.5f, 0.5f));
    }

    public void IncrementDownload()
    {
        sqlscript.IncrementDownloadInSQL(nameText.text, curDownloads);
    }
}
