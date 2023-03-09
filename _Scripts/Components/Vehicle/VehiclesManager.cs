using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VehiclesManager : MonoBehaviour
{

    private GameObject ob_Player;
    private GameObject _currentVehicle;
    public GameObject currentVehicle => _currentVehicle;
    private ObscuredString currentVehicleId = "";
    private RecordItemInventory recordItemInventory;

    private void Start()
    {


    }
    private void Awake()
    {

    }

    private void OnDestroy()
    {

    }

    public void EquipVehicle(RecordItemEquip record, Vector3 vehicle_spawn_pos, Quaternion vehicle_rotation, UnityAction<RecordItemInventory> on_done)
    {
        RecordItemEquip current_vehicle = UserDatas.current_vehicle;
        if (_currentVehicle == null || !current_vehicle.itemID.Equals(record.itemID))
        {
            string file_name = "ItemVehicleInfo";
            string item_id = record.itemID;
            recordItemInventory = DataController.Instance.itemInventoryVO.GetDataByName<RecordItemInventory>(file_name, item_id);
            GameObject prefab = PrefabsManager.Instance.GetAsset<GameObject>(recordItemInventory.prefab.name);
            Debug.LogError("SpawnVehicle : " + (prefab != null));
            if (prefab == null)
            {
                return;
            }
            if (_currentVehicle != null)
                Destroy(_currentVehicle);
            _currentVehicle = CreateController.instance.CreateObject(prefab, vehicle_spawn_pos, transform);
        }
        else
        {
            _currentVehicle.transform.position = vehicle_spawn_pos;
        }
        _currentVehicle.transform.rotation = vehicle_rotation;
        _currentVehicle.SetActive(true);
        UserDatas.current_vehicle = record;
        on_done?.Invoke(recordItemInventory);
    }

    public void UnEquipVehicle()
    {
        if (_currentVehicle != null)
        {
            Destroy(_currentVehicle);
            //_currentVehicle.SetActive(false);
        }
    }
}


