using UnityEngine;

public class ItemToShowLight : MonoBehaviour
{
    [SerializeField] GameObject[] itemToLights;
    public void SetStatusButton(bool status)
    {
        if (itemToLights == null) return;
        int length = itemToLights.Length;
        for (int i = 0; i < length; i++)
        {
            if (itemToLights[i] != null)
                itemToLights[i].SetActive(status);
        }
    }
}