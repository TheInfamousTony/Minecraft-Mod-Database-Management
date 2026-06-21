using TMPro;
using UnityEngine;

public class TagSearch : MonoBehaviour
{
    public TMP_Text txt;
    public string curTag;

    [HideInInspector]
    public SQL_Script sqlscript;

    void Start()
    {
        sqlscript = GameObject.FindGameObjectWithTag("SQLMANAGER").GetComponent<SQL_Script>();
    }

    public void TagInfo(string s)
    {
        txt.text = s;
        curTag = s;
    }

    public void DeleteTag()
    {
        sqlscript.DeleteTagFromSet(curTag);
    }
}
