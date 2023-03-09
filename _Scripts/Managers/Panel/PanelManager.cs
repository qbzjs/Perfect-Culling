using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[UIPanelPrefabAttr("Panel/Shop/PanelShop", "Popup")]
public class PanelManager : TPRLSingleton<PanelManager>
{
    Dictionary<System.Type, PanelItem> panels = new Dictionary<System.Type, PanelItem>();
    Dictionary<string, PanelItem> showingPanels = new Dictionary<string, PanelItem>();

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
            Unregister(this.panelType);
        }
    }

    public static void Unregister(System.Type panelType)
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
    }

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    private static void Register<T>(T panel, bool clearPrefab = false, string groupName = null) where T : MonoBehaviour
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
            if (!Instance.panels.ContainsKey(panelType))
                Instance.panels.Add(panelType, new PanelItem(panel, "", panelType));
        }
        else
        {
            Instance.panels[panelType] = new PanelItem(panel, groupName.ToLower(), panelType);
        }

        var autoUnregister = panel.gameObject.AddComponent<PanelAutoUnRegister>();
        autoUnregister.SetPanelType(panelType, clearPrefab);
    }

    public static T Show<T>() where T : MonoBehaviour
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
            var prefabAttributes = panelType.GetCustomAttributes(typeof(UIPanelPrefabAttr), false);
            if (prefabAttributes == null || prefabAttributes.Length <= 0)
            {
                Debug.LogError("Panel " + panelType + " has no valid attribute.");
                return null;
            }

            UIPanelPrefabAttr attribute = (UIPanelPrefabAttr)prefabAttributes[0];
            GameObject prefab = PrefabsManager.Instance.GetAsset<GameObject>(attribute.PrefabPath); ;
            //bool is_done = false;
            //AddressableLoader.LoadAssetAsync<GameObject>(attribute.PrefabPath, (prf) =>
            //{
            //    prefab = prf;
            //    is_done = true;
            //});
            //yield return new WaitUntil(() => is_done);
            if (prefab == null)
            {
                Debug.LogError("cannot load " + attribute.PrefabPath);
                return null;
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
            Register(script);
            return Show<T>();
        }
        if (item.Panel == null)
        {
            if (Instance.panels.ContainsKey(panelType))
                Instance.panels.Remove(panelType);
            return null;
        }
        showPanel(item);
        return (T)item.Panel;
    }

    public static void ShowAsync<T>(Action<T> action) where T : MonoBehaviour
    {
        if (Instance == null)
        {
            Debug.LogError("PanelRoot did not initialized");
            return;
        }
        Instance.StartCoroutine(Instance.IEShowAsync<T>(action));
    }

    private IEnumerator IEShowAsync<T>(Action<T> action) where T : MonoBehaviour
    {
        PanelItem item;
        System.Type panelType = typeof(T);
        if (Instance.panels.TryGetValue(panelType, out item) == false)
        {
            var prefabAttributes = panelType.GetCustomAttributes(typeof(UIPanelPrefabAttr), false);
            if (prefabAttributes == null || prefabAttributes.Length <= 0)
            {
                Debug.LogError("Panel " + panelType + " has no valid attribute.");
                yield return null;
            }

            UIPanelPrefabAttr attribute = (UIPanelPrefabAttr)prefabAttributes[0];
            GameObject prefab = null;
            //bool is_done = false;
            //AddressableLoader.LoadAssetAsync<GameObject>(attribute.PrefabPath, (prf) =>
            //{
            //    prefab = prf;
            //    is_done = true;
            //});
            //yield return new WaitUntil(() => is_done);
            if (prefab == null)
            {
                Debug.LogError("cannot load " + attribute.PrefabPath);
                yield return null;
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
            Register(script);
            ShowAsync<T>(action);
            yield break;
        }
        if (item.Panel == null)
        {
            if (Instance.panels.ContainsKey(panelType))
                Instance.panels.Remove(panelType);
            yield break;
        }
        showPanel(item);
        action?.Invoke((T)item.Panel);
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

    static void showPanel(PanelItem panel, bool is_active_bg_black = true)
    {
        //SetActiveLock(true, is_active_bg_black);
        string panelName = panel.GetType().ToString();
        if (!Instance.showingPanels.ContainsKey(panelName))
            Instance.showingPanels.Add(panelName, panel);
        else
            Instance.showingPanels[panelName] = panel;
        ShowHidePanel script = null;
        if (panel.Panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            panel.Panel.gameObject.SetActive(true);
            panel.Panel.transform.SetAsLastSibling();
            return;
        }

        script.Show();
    }
    public static void Hide<T>(bool is_destroy = false) where T : MonoBehaviour
    {
        Hide(typeof(T), is_destroy);
    }

    public static void Hide(MonoBehaviour panelScript, bool is_destroy = false)
    {
        Hide(panelScript.GetType(), is_destroy);
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

    static void hidePanel(MonoBehaviour panel, bool is_destroy = false)
    {
        string panelName = panel.GetType().ToString();
        if (Instance.showingPanels.ContainsKey(panelName))
            Instance.showingPanels.Remove(panelName);
        //bool is_all_panel_deactive = Instance.showingPanels.Count == 0;
        ShowHidePanel script = null;
        if (panel.TryGetComponent<ShowHidePanel>(out script) == false)
        {
            if (is_destroy)
            {
                System.Type panelType = panel.GetType();
                if (Instance.panels.ContainsKey(panelType))
                    Instance.panels.Remove(panelType);
                if (Instance.showingPanels.ContainsKey(panelType.ToString()))
                    Instance.showingPanels.Remove(panelType.ToString());
                Destroy(panel.gameObject);
            }
            else
                panel.gameObject.SetActive(false);
            //if (is_all_panel_deactive)
            //    SetActiveLock(false);
            return;
        }

        script.Hide();
        //if (Instance.showingPanels.Count == 0)
        //    SetActiveLock(false);
    }
}
