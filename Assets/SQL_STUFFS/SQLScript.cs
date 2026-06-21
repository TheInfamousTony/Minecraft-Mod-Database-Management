//reset auto increment: UPDATE sqlite_sequence SET seq=0 WHERE name="mods"

using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class SQL_Script : MonoBehaviour
{
    public GameObject viewContent;
    public GameObject tagView;
    public GameObject addModPanel;
    public GameObject curImage;
    public GameObject noResultText;
    public TMP_InputField searchBar;
    public TMP_InputField nameField;
    public TMP_InputField verField;
    public Slider sizeSlider;
    public TMP_Dropdown modLoaderDropdown;
    public TMP_Dropdown tagDropDown;
    public TMP_Dropdown tagSearchDropDown;
    public TMP_Dropdown sortDropDown;
    public TMP_Dropdown modLoaderSearchDropdown;
    public Slider sizeCondition;
    public Card cardPrefab;
    public TagSearch tagSearchPrefab;

    SortedSet<string> tagSet = new SortedSet<string>();
    string path;
    string readcode = "SELECT * FROM mods";
    byte[] binary;
    string[] modLoaderList = {
        "Forge",
        "Fabric",
        "Neoforge",
        "All"
    };
    string[] tagList = {
        "Miscellanous",
        "Quality-of-life",
        "Technology",
        "Optimization",
        "Horror",
        "World Generation",
        "Cosmetic",
        "Items"
    };

    string[] sortList = {
        "",
        " ORDER BY downloads DESC",
        " ORDER BY downloads ASC",
        " ORDER BY name"
    };

    void Start()
    {
        path = "URI=file:" + Application.persistentDataPath + "/MCMODSDB.db";
        CopyDatabaseIfNeeded();
        ReadData();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ApplySearch();
        }
    }

    void CopyDatabaseIfNeeded()
    {
        string sourcePath = Application.streamingAssetsPath + "/MCMODSDB.db";
        string persistentDbPath = Application.persistentDataPath + "/MCMODSDB.db";

        if (!File.Exists(persistentDbPath))
        {
            // Debug.Log("Copying DB to persistentDataPath...");
            File.Copy(sourcePath, persistentDbPath);
        }
        else
        {
            // Debug.Log("DB already exists at: " + persistentDbPath);
        }
    }

    public void PickImage()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };
        var imgpath = StandaloneFileBrowser.OpenFilePanel("Select Image for the banner", "", extensions, false);
        if(path.Length > 0)
        {
            if(imgpath[0] == "") return;
            string chosenimgpath = imgpath[0];
            binary = File.ReadAllBytes(chosenimgpath);
            Texture2D curimg = LoadImg(binary);
            curImage.GetComponent<Image>().sprite = Sprite.Create(curimg, new Rect(0f, 0f, curimg.width, curimg.height), new Vector2(0.5f, 0.5f));
        }
    }

    public static DateTime GmtToIndo(DateTime dateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    }

    public void ReadData()
    {
        IDbConnection conn;
        conn = new SqliteConnection(path);
        conn.Open();
        IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = readcode + sortList[sortDropDown.value];
        IDataReader reader = cmd.ExecuteReader();

        viewContent.GetComponent<ClearChildren>().Clear();
        bool f = false;
        while(reader.Read())
        {
            f = true;
            // int id = reader.GetInt32(0);
            int downloads = reader.GetInt32(3);
            int size = reader.GetInt32(6);
            string name = reader.GetString(1);
            string release_date = reader.GetString(2);
            string latest_ver = reader.GetString(4);
            string mod_type = reader.GetString(5);
            string tag = reader.GetString(8);
            byte[] image = (byte[])reader["image"];


            Card curCard = Instantiate(cardPrefab, viewContent.transform);
            curCard.SetInfo(name, downloads, release_date, latest_ver, size, mod_type, image, tag);

            // Debug.Log("ID: " + id + " | NAME: " + name + " | RELEASE DATE: " + release_date + " | DOWNLOADS: " + downloads + " | LATEST VERSION: " + latest_ver + " | MOD TYPE: " + mod_type + " | SIZE: " + size + " ");
        }

        if(f == false)  noResultText.SetActive(true);
        else noResultText.SetActive(false);

        reader.Close();
        reader = null;
        cmd.Dispose();
        cmd = null;
        conn.Close();
        conn = null;
    }

    public void ShowAddModPanel()
    {
        addModPanel.SetActive(true);
    }
    public void HideAddModPanel()
    {
        addModPanel.SetActive(false);
    }

    public void Apply()
    {
        DateTime dateTime = DateTime.UtcNow.Date;

        string addDataCMD = "INSERT INTO mods (id, name, downloads, release_date, latest_ver, mod_type, size, image, tag) VALUES (NULL" + ", " + "\"" + nameField.text + "\"" + ", "  + "0" + ", " + "\"" + GmtToIndo(dateTime).ToString("dd/MM/yyyy") + "\"" + ", " + "\"" + verField.text + "\"" + ", " + "\"" + modLoaderList[modLoaderDropdown.value] + "\"" + ", " + sizeSlider.value.ToString() + ", " + "@image" + ", " + "\"" + tagList[tagDropDown.value] + "\"" + ")";
        // Debug.Log(addDataCMD);

        IDbConnection conn;
        conn = new SqliteConnection(path);
        conn.Open();
        IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = addDataCMD;
        cmd.Parameters.Add(new SqliteParameter("@image", binary));
        cmd.ExecuteReader();

        ReadData();

        addModPanel.SetActive(false);
    }

    Texture2D LoadImg(byte[] binery)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(binery);
        return tex;
    }

    public void AddTag()
    {
        // Debug.Log(tagDropDown.value);
        tagSet.Add(tagList[tagSearchDropDown.value]);

        tagView.GetComponent<ClearChildren>().Clear();
        foreach(string name in tagSet)
        {
            TagSearch curTagSearch = Instantiate(tagSearchPrefab, tagView.transform);
            curTagSearch.TagInfo(name);
        }
    }

    public void DeleteTagFromSet(string tag)
    {
        tagSet.Remove(tag);

        tagView.GetComponent<ClearChildren>().Clear();
        foreach(string name in tagSet)
        {
            TagSearch curTagSearch = Instantiate(tagSearchPrefab, tagView.transform);
            curTagSearch.TagInfo(name);
        }
    }

    public void ApplySearch()
    {
        //chỉ lấy những cái có cột "downloads" <= 0
        readcode = "SELECT * FROM mods WHERE size <= " + sizeCondition.value.ToString();

        //nếu modloader là 3 thì không cần lọc Mod Loader
        if(modLoaderSearchDropdown.value != 3)
        {
            //chỉ lấy những cái có "mod_type" người dùng chọn
            readcode += " AND mod_type = \"" + modLoaderList[modLoaderSearchDropdown.value] + "\"";
        }

        //nếu không muốn search thì để trống search bar
        if(searchBar.text != "")
        {
            //chỉ lấy những cái có "name" chính xác
            readcode += " AND name = \"" + searchBar.text + "\"";
        }

        //người dùng có thể cho tag để search vào trong tag set, khi search thì 
        //kết quả chỉ ra những kết quả có chứa những tag mà người dùng chọn, dùng phép OR
        if(tagSet.Count > 0)
        {
            readcode += " AND (";
            foreach(string name in tagSet)
            {
                readcode += "tag = " + "\"" + name + "\"" + " OR ";
            }
            string trimmed = readcode.Substring(0, readcode.Length - 4);
            readcode = trimmed;
            readcode += ")";
        }

        ReadData();
    }

    public void IncrementDownloadInSQL(string name, int curDownload)
    {
        int curCurDownload = curDownload+1;
        //khi có người dùng download 1 mod thì cột "downloads" trong mod đó sẽ tăng +1
        string incCode = "UPDATE mods SET downloads = " + curCurDownload.ToString() + " WHERE name = \"" + name + "\"";

        IDbConnection conn;
        conn = new SqliteConnection(path);
        conn.Open();
        IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = incCode;
        cmd.ExecuteReader();

        ReadData();
    }

    // tạo mod cho những người khác tải về và sử dụng
    // search mod cần tìm --> tải mod
}
