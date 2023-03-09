using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
//[UIPanelPrefabAttr("Panel/Shop/PanelShop", "Popup")]
public class PanelRoot : TPRLSingleton<PanelRoot>
{
    Dictionary<System.Type, GameObject> prefabs = new Dictionary<System.Type, GameObject>();
    Dictionary<System.Type, PanelItem> panels = new Dictionary<System.Type, PanelItem>();
    Dictionary<string, PanelItem> showingPanels = new Dictionary<string, PanelItem>();
    Dictionary<string, MonoBehaviour> panelsActive = new Dictionary<string, MonoBehaviour>();

    [SerializeField]
    GameObject objLock, bgBlack, objPopup;
    //private bool lockEvent = false;

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    public static bool isPopupShowing()
    {
        return Instance.panelsActive.Count > 0;
    }

    class PanelItem
    {
        public readonly MonoBehaviour Panel;
        public readonly string Group;
        public readonly System.Type Type;

        public PanelItem(MonoBehaviour panel, string group, System.Type type)
        {
            this.Panel = panel;
            this.Group = group;
            this.Type = type;
        }
    }

    public static void PreloadPrefab<T>() where T : MonoBehaviour
    {

        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }

        System.Type panelType = typeof(T);
        GameObject prefab;
        if (Instance.prefabs.TryGetValue(panelType, out prefab) == true)
        {
            Debug.LogWarning("already reserved " + panelType);
            return;
        }

        var prefabAttributes = panelType.GetCustomAttributes(typeof(UIPanelPrefabAttr), false);
        if (prefabAttributes == null || prefabAttributes.Length <= 0)
        {
            Debug.LogError("Panel " + panelType + " has no valid attribute.");
            return;
        }

        UIPanelPrefabAttr attribute = (UIPanelPrefabAttr)prefabAttributes[0];
        //AddressableLoader.LoadAssetAsync<GameObject>(attribute.PrefabPath, (prf) =>
        //{
        //    prefab = prf;
        //    if (prefab == null)
        //    {
        //        Debug.LogError("cannot load " + attribute.PrefabPath);
        //        return;
        //    }
        //    Instance.prefabs[panelType] = prefab;
        //});
        prefab = PrefabsManager.Instance.GetAsset<GameObject>(attribute.PrefabPath);
        if (prefab == null)
        {
            Debug.LogError("cannot load " + attribute.PrefabPath);
            return;
        }

        Instance.prefabs[panelType] = prefab;
    }

    public static void Register<T>(T panel, bool clearPrefab = false, string groupName = null) where T : MonoBehaviour
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }
        System.Type panelType = typeof(T);
        //if (instance.panels.ContainsKey(panelType) == true)
        //{
        //    Debug.Log("replacing panel " + panel.name);
        //}

        if (string.IsNullOrEmpty(groupName) == true)
        {
            Instance.panels[panelType] = new PanelItem(panel, "", panelType);
        }
        else
        {
            Instance.panels[panelType] = new PanelItem(panel, groupName.ToLower(), panelType);
        }

        var autoUnregister = panel.gameObject.AddComponent<PanelAutoUnRegister>();
        autoUnregister.SetPanelType(panelType, clearPrefab);
    }

    public static void Unregister(System.Type panelType, bool clearPrefab)
    {
        if (Instance == null)
            return;

        PanelItem item;
        string groupName = "";
        foreach (var panelitem in Instance.showingPanels)
        {
            if (panelitem.Value.Type == panelType)
            {
                groupName = panelitem.Value.Group;
            }
        }
        if (string.IsNullOrEmpty(groupName) == false)
        {
            Instance.showingPanels.Remove(groupName);
        }

        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            Debug.LogWarning("no item to Unregister " + panelType);
            return;
        }
        Instance.panels.Remove(panelType);

        if (clearPrefab == true)
        {
            if (Instance.prefabs.ContainsKey(panelType))
            {
                Instance.prefabs.Remove(panelType);
            }
        }

        //Debug.Log( "unregister " + panelType );
    }

    public static bool Contains(string PanelName)
    {
        if (Instance == null)
        {
            return false;
        }

        System.Type panelType = System.Type.GetType(PanelName);
        return Instance.panels.ContainsKey(panelType);
    }

    public static bool Contains<T>() where T : MonoBehaviour
    {
        if (Instance == null)
        {
            return false;
        }
        System.Type panelType = typeof(T);
        return Instance.panels.ContainsKey(panelType);
    }

    public static MonoBehaviour Get(MonoBehaviour mono)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }
        PanelItem item;
        System.Type panelType = mono.GetType();
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            var script = Instance.IETryCreate(mono).GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
                Debug.Log("not registered panel " + panelType);
                return null;
            }
            return script;
        }

        return item.Panel;
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }

        PanelItem item;
        System.Type panelType = typeof(T);
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            T script = Instance.IETryCreate<T>().GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
                Debug.Log("not registered panel " + panelType);
                return null;
            }
            return script;
        }

        return (T)item.Panel;
    }

    private IEnumerable<MonoBehaviour> IETryCreate(MonoBehaviour mono)
    {
        System.Type panelType = mono.GetType();
        var prefabAttributes = panelType.GetCustomAttributes(typeof(UIPanelPrefabAttr), false);
        if (prefabAttributes == null || prefabAttributes.Length <= 0)
        {
            Debug.LogError("Panel " + panelType + " has no valid attribute.");
            yield return null;
        }

        UIPanelPrefabAttr attribute = (UIPanelPrefabAttr)prefabAttributes[0];
        GameObject prefab;
        if (Instance.prefabs.TryGetValue(panelType, out prefab) == false)
        {
            //bool is_done = false;
            //AddressableLoader.LoadAssetAsync<GameObject>(attribute.PrefabPath, (prf)=> {
            //    prefab = prf;
            //    is_done = true;
            //});
            //while(is_done == false)
            //    yield return null;
            prefab = PrefabsManager.Instance.GetAsset<GameObject>(attribute.PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("cannot load " + attribute.PrefabPath);
                yield return null;
            }

            Instance.prefabs[panelType] = prefab;
        }

        GameObject anchor = GameObject.Find(attribute.AnchorName);
        if (anchor == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("cannot find anchor " + attribute.AnchorName);
#endif
            anchor = GameObject.Find("Anchor");
            if (anchor == null)
            {
                UIAnchor anchorObject = GameObject.FindObjectOfType(typeof(UIAnchor)) as UIAnchor;
                if (anchorObject != null)
                {
                    anchor = anchorObject.gameObject;
                }
            }
            if (anchor == null)
            {
#if UNITY_EDITOR
                Debug.LogError("cannot find any anchor in this scene");
#endif
            }
        }

        GameObject panel = AddChild(anchor, prefab);
        panel.transform.localPosition = prefab.transform.localPosition;
        var script = panel.GetComponent<MonoBehaviour>();
        if (script == null)
        {
            Debug.LogError("panel " + panelType + " has no script in prefab");
        }
        yield return script;
    }

    private IEnumerable<T> IETryCreate<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);
        var prefabAttributes = panelType.GetCustomAttributes(typeof(UIPanelPrefabAttr), false);
        if (prefabAttributes == null || prefabAttributes.Length <= 0)
        {
            Debug.LogError("Panel " + panelType + " has no valid attribute.");
            yield return null;
        }

        UIPanelPrefabAttr attribute = (UIPanelPrefabAttr)prefabAttributes[0];

        GameObject prefab;
        if (Instance.prefabs.TryGetValue(panelType, out prefab) == false)
        {
            //bool is_done = false;
            //AddressableLoader.LoadAssetAsync<GameObject>(attribute.PrefabPath, (prf) => {
            //    prefab = prf;
            //    is_done = true;
            //});
            //while (is_done == false)
            //    yield return null;
            prefab = PrefabsManager.Instance.GetAsset<GameObject>(attribute.PrefabPath);
            if (prefab == null)
            {
                Debug.LogError("cannot load " + attribute.PrefabPath);
                yield return null;
            }

            Instance.prefabs[panelType] = prefab;
        }

        GameObject anchor = GameObject.Find(attribute.AnchorName);
        if (anchor == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("cannot find anchor " + attribute.AnchorName);
#endif
            anchor = GameObject.Find("Anchor");
            if (anchor == null)
            {
                UIAnchor anchorObject = GameObject.FindObjectOfType(typeof(UIAnchor)) as UIAnchor;
                if (anchorObject != null)
                {
                    anchor = anchorObject.gameObject;
                }
            }
            if (anchor == null)
            {
#if UNITY_EDITOR
                Debug.LogError("cannot find any anchor in this scene");
#endif
            }
        }

        GameObject panel = AddChild(anchor, prefab);
        panel.transform.localPosition = prefab.transform.localPosition;
        var script = panel.GetComponent<T>();
        if (script == null)
        {
            Debug.LogError("panel " + panelType + " has no script in prefab");
        }
        yield return script;
    }


    public static MonoBehaviour Show(MonoBehaviour mono, ShowHidePanel.onEventHandler onShowBegin, ShowHidePanel.onEventHandler onShowFinished)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }
        PanelItem item;
        System.Type panelType = mono.GetType();
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            var script = Instance.IETryCreate(mono).GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
                return null;
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
                Debug.Log("not registered panel " + panelType);
                return null;
            }
            return Show(script, onShowBegin, onShowFinished);
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                hidePanel(oldPanelItem.Panel);
            }
            Instance.showingPanels[item.Group] = new PanelItem(item.Panel, item.Group, item.Type);
        }

        showPanel(item.Panel, onShowBegin, onShowFinished);
        return item.Panel;
    }

    public static T Show<T>(ShowHidePanel.onEventHandler onShowBegin, ShowHidePanel.onEventHandler onShowFinished) where T : MonoBehaviour
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }

        PanelItem item;
        System.Type panelType = typeof(T);
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            T script = Instance.IETryCreate<T>().GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
                return null;
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
                Debug.Log("not registered panel " + panelType);
                return null;
            }
            return Show<T>(onShowBegin, onShowFinished);
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                hidePanel(oldPanelItem.Panel);
            }
            Instance.showingPanels[item.Group] = new PanelItem(item.Panel, item.Group, item.Type);
        }

        showPanel(item.Panel, onShowBegin, onShowFinished);
        return (T)item.Panel;
    }

    public static MonoBehaviour Show(MonoBehaviour mono)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }

        PanelItem item;
        System.Type panelType = mono.GetType();
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            var script = Instance.IETryCreate(mono).GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
                return null;
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning("not registered panel " + panelType + ".  need register to PanelRoot from Awake()");
#else
				Debug.Log( "not registered panel " + panelType );
#endif
                return null;
            }
            return Show(mono);
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                hidePanel(oldPanelItem.Panel);
            }
            Instance.showingPanels[item.Group] = new PanelItem(item.Panel, item.Group, item.Type);
        }
#if _DEV
        Debug.Log("popup item : " + item.GetType() + " - " + item.Group + " - " + item.Panel.name + " - " + item.Type + " - " + item.ToString());
#endif
        showPanel(item.Panel);
        return item.Panel;
    }

    public static T Show<T>(bool is_active_bg_black = true) where T : MonoBehaviour
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return null;
        }

        PanelItem item;
        System.Type panelType = typeof(T);
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            T script = Instance.IETryCreate<T>().GetEnumerator().Current;
            if (script == null)
            {
                Debug.Log("cannot create " + panelType);
                return null;
            }
            if (Instance.panels.ContainsKey(panelType) == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning("not registered panel " + panelType + ".  need register to PanelRoot from Awake()");
#else
				Debug.Log( "not registered panel " + panelType );
#endif
                return null;
            }
            return Show<T>(is_active_bg_black);
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                hidePanel(oldPanelItem.Panel);
            }
            Instance.showingPanels[item.Group] = new PanelItem(item.Panel, item.Group, item.Type);
        }
#if _DEV
        Debug.Log("popup item : " + item.GetType() + " - " + item.Group + " - " + item.Panel.name + " - " + item.Type + " - " + item.ToString());
#endif
        if (item.Panel == null)
        {
            if (Instance.panels.ContainsKey(panelType))
                Instance.panels.Remove(panelType);
            if (Instance.panelsActive.ContainsKey(panelType.ToString()))
                Instance.panelsActive.Remove(panelType.ToString());
            return Show<T>(is_active_bg_black);
        }
        showPanel(item.Panel, is_active_bg_black);

        return (T)item.Panel;
    }

    public static void Hide<T>(ShowHidePanel.onEventHandler onHideBegin, ShowHidePanel.onEventHandler onHideFinished) where T : MonoBehaviour
    {
        Hide(typeof(T), onHideBegin, onHideFinished);
    }

    public static void Hide<T>() where T : MonoBehaviour
    {
        Hide(typeof(T));
    }

    public static void Hide(System.Type type, ShowHidePanel.onEventHandler onHideBegin, ShowHidePanel.onEventHandler onHideFinished)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }

        PanelItem item;
        if (Instance.panels.TryGetValue(type, out item) == false)
        {
            Debug.LogWarning("Panel " + type + " is not registered");
            //Debug.LogError( "Panel " + type + " is not registered" );
            return;
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                Instance.showingPanels.Remove(item.Group);
                //if ( item.Panel != oldPanel )
                {
                    //if ( item.Panel.GetType() != oldPanel.GetType() )
                    {
                        Debug.LogWarning("try hide : old panel " + oldPanelItem.Type + " new Panel " + item.Panel.GetType());
                    }
                }
            }
        }

        hidePanel(item.Panel, onHideBegin, onHideFinished);
    }

    public static void Hide(System.Type type, bool is_destroy = false)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }

        PanelItem item;
        if (Instance.panels.TryGetValue(type, out item) == false)
        {
            Debug.LogWarning("Panel " + type + " is not registered");
            //Debug.LogError( "Panel " + type + " is not registered" );
            return;
        }

        bool hasGroup = (string.IsNullOrEmpty(item.Group) == false);
        if (hasGroup == true)
        {
            PanelItem oldPanelItem;
            if (Instance.showingPanels.TryGetValue(item.Group, out oldPanelItem) == true)
            {
                Instance.showingPanels.Remove(item.Group);
                //if ( item.Panel != oldPanel )
                {
                    //if ( item.Panel.GetType() != oldPanel.GetType() )
                    {
                        Debug.LogWarning("try hide : old panel " + oldPanelItem.Type + " new Panel " + item.Panel.GetType());
                    }
                }
            }
        }

        hidePanel(item.Panel, is_destroy);
    }

    public static void Hide(MonoBehaviour panelScript, bool is_destroy = false)
    {
        Hide(panelScript.GetType(), is_destroy);
    }

    public static void HidePanelGroup(string groupName)
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }
        if (string.IsNullOrEmpty(groupName) == true)
        {
            return;
        }

        PanelItem oldPanelItem;
        if (Instance.showingPanels.TryGetValue(groupName.ToLower(), out oldPanelItem) == false)
        {
            return;
        }

        Instance.showingPanels.Remove(groupName);
        hidePanel(oldPanelItem.Panel);
    }

    public static void HideAllPanel(string except_panel)
    {
        foreach (var panelitem in Instance.panels)
        {
            if (panelitem.Value.Panel.name.Equals(except_panel)) continue;
            Hide(panelitem.Value.Panel);
        }
    }
    public static void HideAllPanel()
    {
        foreach (var panelitem in Instance.panels)
        {
            Hide(panelitem.Value.Panel);
        }
    }


    static void SetActiveLock(bool active, bool is_active_bg_black = true)
    {
        //if (active == false)
        //{
        //    foreach (Transform child in instance.objPopup.transform)
        //    {
        //        if (child.gameObject.activeInHierarchy) return;
        //    }
        //}
        //if (instance.objLock != null)
        //    instance.objLock.SetActive(active);
        //if (instance.bgBlack != null && is_active_bg_black)
        //    instance.bgBlack.SetActive(active);
    }

    static void hidePanel(MonoBehaviour panel, ShowHidePanel.onEventHandler onHideBegin, ShowHidePanel.onEventHandler onHideFinished)
    {

        string panelName = panel.GetType().ToString();
        if (Instance.panelsActive.ContainsKey(panelName))
            Instance.panelsActive.Remove(panelName);
        bool is_all_panel_deactive = Instance.panelsActive.Count == 0;
        ShowHidePanel script = null;
        if (panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            panel.gameObject.SetActive(false);
            if (is_all_panel_deactive)
                SetActiveLock(false);
            return;
        }

        script.onHideBegin = onHideBegin;
        script.onHideFinished = onHideFinished;
        script.Hide();
        if (Instance.panelsActive.Count == 0)
            SetActiveLock(false);
    }

    static void hidePanel(MonoBehaviour panel, bool is_destroy = false)
    {
        string panelName = panel.GetType().ToString();
        if (Instance.panelsActive.ContainsKey(panelName))
            Instance.panelsActive.Remove(panelName);
        bool is_all_panel_deactive = Instance.panelsActive.Count == 0;
        ShowHidePanel script = null;
        if (panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            if (is_destroy)
            {
                System.Type panelType = panel.GetType();
                if (Instance.panels.ContainsKey(panelType))
                    Instance.panels.Remove(panelType);
                if (Instance.panelsActive.ContainsKey(panelType.ToString()))
                    Instance.panelsActive.Remove(panelType.ToString());
                Destroy(panel.gameObject);
            }
            else
                panel.gameObject.SetActive(false);
            if (is_all_panel_deactive)
                SetActiveLock(false);
            return;
        }

        script.Hide();
        if (Instance.panelsActive.Count == 0)
            SetActiveLock(false);
    }

    static void showPanel(MonoBehaviour panel, ShowHidePanel.onEventHandler onShowBegin, ShowHidePanel.onEventHandler onShowFinished)
    {
        SetActiveLock(true);
        string panelName = panel.GetType().ToString();
        if (!Instance.panelsActive.ContainsKey(panelName))
            Instance.panelsActive.Add(panelName, panel);
        else
            Instance.panelsActive[panelName] = panel;
        ShowHidePanel script = null;
        if (panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            panel.gameObject.SetActive(true);
            panel.transform.SetAsLastSibling();
            return;
        }

        script.onShowBegin = onShowBegin;
        script.onShowFinished = onShowFinished;
        script.Show();
    }

    static void showPanel(MonoBehaviour panel, bool is_active_bg_black = true)
    {
        SetActiveLock(true, is_active_bg_black);
        string panelName = panel.GetType().ToString();
        if (!Instance.panelsActive.ContainsKey(panelName))
            Instance.panelsActive.Add(panelName, panel);
        else
            Instance.panelsActive[panelName] = panel;
        ShowHidePanel script = null;
        if (panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            panel.gameObject.SetActive(true);
            panel.transform.SetAsLastSibling();
            return;
        }

        script.Show();
    }

    static public GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }

    protected override void OnDestroy()
    {
        panels.Clear();
        showingPanels.Clear();
        prefabs.Clear();
        panelsActive.Clear();
        base.OnDestroy();
    }

    public class PanelAutoUnRegister : MonoBehaviour
    {
        System.Type panelType = null;

        bool clearPrefab = false;

        public void SetPanelType(System.Type type, bool clearPrefab)
        {
            this.panelType = type;

            this.clearPrefab = clearPrefab;
        }

        void OnDestroy()
        {
            Unregister(this.panelType, clearPrefab);
        }
    }

}

public class UIPanelPrefabAttr : System.Attribute
{
    public readonly string PrefabPath = "";
    public readonly string AnchorName = "";

    public UIPanelPrefabAttr(string prefabPath, string anchorName)
    {
        this.PrefabPath = prefabPath;
        this.AnchorName = anchorName;
    }
}

