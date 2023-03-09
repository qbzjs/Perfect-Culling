using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ParagonInfoShowUp : MonoBehaviour
{
    [SerializeField] private TMP_Text tx_ParagonNumber;
    [SerializeField] private TMP_Text tx_ParagonName;
    [SerializeField] private TMP_Text tx_ParagonPower;
    [SerializeField] private TMP_Text tx_ParagonPrice;
    [SerializeField] private Transform tf_ParagonShowPosition;

    private GameObject ob_SaveCurrentParagon;
    public int shelf_Index;

    private void SetTextForParagonInfoField(TMP_Text tx_text, string content)
    {
        if (tx_text != null)
            tx_text.text = content;
    }
    public void SetParagonInfoToShowingUp(string number, string name, string power, string price)
    {
        SetTextForParagonInfoField(tx_ParagonNumber, number);
        SetTextForParagonInfoField(tx_ParagonName, name);
        SetTextForParagonInfoField(tx_ParagonPower, power);
        SetTextForParagonInfoField(tx_ParagonPrice, price);
    }
    public Vector3 PositionToShowParagon()
    {
        if (tf_ParagonShowPosition != null)
            return tf_ParagonShowPosition.position;
        return Vector3.zero;
    }


    public void CreateNewParagon()
    {
        GameObject new_paragon = RandomSpawnParagon();
        ob_SaveCurrentParagon = new_paragon;
        Paragon _paragon = new_paragon.GetComponent<Paragon>();
        FakeUpdateParagonToShopUp(_paragon);
    }

    public void DestroyCurrentParagon()
    {
        if (ob_SaveCurrentParagon != null)
        {
            Debug.Log(ob_SaveCurrentParagon.name);
            Destroy(ob_SaveCurrentParagon);
        }
    }
    private GameObject RandomSpawnParagon()
    {
        int int_dex = Random.Range(1, 18);
        GameObject paragon_asset_bundle = Instantiate(PrefabsManager.Instance.GetAsset<GameObject>(int_dex.ToString()));
        return paragon_asset_bundle;
    }

    private void FakeUpdateParagonToShopUp(Paragon paragon)
    {
        if (paragon != null)
        {
            float number_price = Random.Range(1, 100);
            string id = "ID : #" + Substring(paragon.gameObject.name);
            string name = "Paragon " + Substring(paragon.gameObject.name);
            string power = "Power : " + Substring(paragon.gameObject.name);
            string price = "Price : " + (number_price * 13).ToString();
            SetParagonInfoToShowingUp(id, name, power, price);
            paragon.transform.SetParent(gameObject.transform);
            paragon.transform.position = PositionToShowParagon();
        }
    }
    private string Substring(string origin_string)
    {
        int length_name_paragon = origin_string.Length - origin_string.IndexOf('(');
        string name_paragon = origin_string.Substring(0, origin_string.Length - length_name_paragon);
        return name_paragon;
    }
}
