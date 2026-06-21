using UnityEngine;

public class ClearChildren : MonoBehaviour
{
    public void Clear()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
