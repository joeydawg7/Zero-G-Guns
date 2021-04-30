using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene Finder - Locates objects and components within a scene or across all scenes in the project
/// When searching all scenes, those scenes will have to be loaded... Options to unload those scenes in the dropdown
/// next to the search bar.
/// Developer: Dibbie
/// Email: mailto:strongstrenth@hotmail.com [for questions/help and inquires]
/// Website: http://www.simpleminded.x10host.com
/// Discord: https://discord.gg/33PFeMv
/// </summary>
public class SceneFinder : EditorWindow
{
    [SerializeField]
    static string objectLookup = "";
    [SerializeField]
    static SceneSearchFilter filter;
    [SerializeField]
    static bool searchAllScenes;

    [SerializeField]
    static string[] scenesPaths;

    List<GameObject> searchResults = new List<GameObject>();
    List<bool> fold = new List<bool>();
    public enum SceneSearchFilter { ObjectsAndComponents, ObjectsOnly, ComponentsOnly, ByTag, ByLayer };

    bool saveScene, cancelSave;

    static EditorWindow window;
    static Vector2 scrollPos;
    static string sceneObjName;

    static string lastSearchInstance = "";

    public static void SearchFromHistory(SceneSearchHistory.SearchHistory search)
    {
        objectLookup = search.search;
        filter = search.searchFilter;
        searchAllScenes = search.allScenes;

        GUI.SetNextControlName("SearchField");
        GUI.FocusControl("SearchField");
    }

    [MenuItem("Scene Manager/Search Scene _&s")]
    static void Init()
    {
        window = GetWindow(typeof(SceneFinder), false, "Search In Scene");
        window.Show();
        PreOpenScenes();
    }

    void OnGUI()
    {
        if (window == null) { window = GetWindow(typeof(SceneFinder)); PreOpenScenes(); }

        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUI.SetNextControlName("SearchField");
        objectLookup = GUILayout.TextField(objectLookup, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(window.position.width - (string.IsNullOrEmpty(objectLookup) ? 21f : 37f)));

        //search clear button
        if (!string.IsNullOrEmpty(objectLookup))
        {
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                Unfocus();
            }
        }

        GUIStyle style = new GUIStyle();

        if (GUILayout.Button("", EditorStyles.toolbarDropDown, GUILayout.Width(15f)))
        {
            // create custom dropdown menu on button click
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Objects And Components"), filter == SceneSearchFilter.ObjectsAndComponents, OnSearchFilterChanged, SceneSearchFilter.ObjectsAndComponents);
            menu.AddItem(new GUIContent("Objects Only"), filter == SceneSearchFilter.ObjectsOnly, OnSearchFilterChanged, SceneSearchFilter.ObjectsOnly);
            menu.AddItem(new GUIContent("Components Only"), filter == SceneSearchFilter.ComponentsOnly, OnSearchFilterChanged, SceneSearchFilter.ComponentsOnly);
            menu.AddItem(new GUIContent("By Tag Name"), filter == SceneSearchFilter.ByTag, OnSearchFilterChanged, SceneSearchFilter.ByTag);
            menu.AddItem(new GUIContent("By Layer Name"), filter == SceneSearchFilter.ByLayer, OnSearchFilterChanged, SceneSearchFilter.ByLayer);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Search All Scenes"), searchAllScenes, OnSearchFilterChanged, !searchAllScenes);
            menu.AddItem(new GUIContent("Open Search History"), false, OnSearchHistoryOpened, null);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Unload All Opened Scenes", "Unloads any scenes opened by this tool"), false, UnloadAllScenes, null);

            menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();
        if (searchResults.Count > 0) { GUILayout.Label(searchResults.Count + ((searchResults.Count == 1) ? " Result Found" : " Results Found"), EditorStyles.boldLabel); }
        GUILayout.Label("Active Scene: " + GetSceneName(EditorSceneManager.GetActiveScene().name), EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        //generate search results (find objects/components)
        if (!string.IsNullOrEmpty(objectLookup))
        {
            if (searchResults.Count > 0)
            {
                searchResults.Clear();
                searchResults = new List<GameObject>();
            }

            List<GameObject> sceneObjs = new List<GameObject>();

            if (searchAllScenes)
            {
                var scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
                string[] scenesPaths = scenesGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();
                for (int i = 0; i < scenesPaths.Length; i++)
                {
                    sceneObjs.AddRange(EditorSceneManager.GetSceneByPath(scenesPaths[i]).GetRootGameObjects());
                }
            }
            else { sceneObjs.AddRange(EditorSceneManager.GetActiveScene().GetRootGameObjects()); }

            for (int i = 0; i < sceneObjs.Count; i++)
            {
                //BY OBJECT OR COMPONENT
                if (filter == SceneSearchFilter.ObjectsAndComponents)
                {
                    if (sceneObjs[i].name.ToLower().Contains(objectLookup.ToLower()) && !searchResults.Contains(sceneObjs[i])) { searchResults.Add(sceneObjs[i]); }

                    Component[] objComponents = sceneObjs[i].GetComponents<Component>();
                    foreach (Component component in objComponents)
                    {
                        if (component.GetType().Name.ToLower().Contains(objectLookup.ToLower()) && !searchResults.Contains(sceneObjs[i])) { searchResults.Add(sceneObjs[i]); break; }
                    }

                    //RECCURSIVE SEARCH (CHILD OBJECTS)
                    SearchChildObjects(sceneObjs[i].transform);
                }

                //BY OBJECT NAME
                else if (filter == SceneSearchFilter.ObjectsOnly)
                {
                    if (sceneObjs[i].name.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(sceneObjs[i]); }
                }

                //BY OBJECT COMPONENT
                else if (filter == SceneSearchFilter.ComponentsOnly)
                {
                    Component[] objComponents = sceneObjs[i].GetComponents<Component>();
                    foreach (Component component in objComponents)
                    {
                        if (component.GetType().Name.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(sceneObjs[i]); break; }
                    }
                }

                //BY OBJECT TAG
                else if (filter == SceneSearchFilter.ByTag)
                {
                    if (sceneObjs[i].tag.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(sceneObjs[i]); }
                }

                //BY OBJECT LAYER
                else if (filter == SceneSearchFilter.ByLayer)
                {
                    if (LayerMask.LayerToName(sceneObjs[i].layer).ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(sceneObjs[i]); }
                }

                //RECCURSIVE SEARCH (CHILD OBJECTS)
                SearchChildObjects(sceneObjs[i].transform);
            }

            //DISPLAY SEARCH RESULTS (FILTER-BASED)
            for (int i = 0; i < searchResults.Count; i++)
            {
                //GUILayout.Label(searchResults[i].name);
                if (fold.Count - 1 < i) { fold.Add(false); }
                fold[i] = EditorGUILayout.InspectorTitlebar(fold[i], searchResults[i]);

                if (fold[i])
                {
                    GameObject obj = searchResults[i];
                    int componentIndex = 1;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Tag", GUILayout.Width(30f));
                    obj.tag = EditorGUILayout.TagField("", obj.tag, GUILayout.Width(window.position.width / 2.5f));
                    GUILayout.Label("Layer", GUILayout.Width(40f));
                    obj.layer = EditorGUILayout.LayerField("", obj.layer, GUILayout.Width(window.position.width / 2.75f));
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    string scenePath = obj.scene.path;
                    GUILayout.Label("Scene: " + GetSceneName(obj.scene.name));
                    if (!string.IsNullOrEmpty(scenePath)) { GUILayout.Label("(" + obj.scene.path + ")"); }
                    if (obj.scene != EditorSceneManager.GetActiveScene()) { EditorGUILayout.HelpBox("This object is in another scene", MessageType.Info); }

                    EditorGUILayout.Space();
                    GUILayout.Label("Components:");

                    //list all components on the object, BOLD the ones that match search
                    foreach (Behaviour component in obj.GetComponents<Behaviour>())
                    {
                        string componentName = component.GetType().Name;
                        Texture2D componentIcon = (Texture2D)EditorGUIUtility.ObjectContent(null, component.GetType()).image;
                        if (componentIcon == null) { componentIcon = GetDefaultScriptIcon(); }
                        style = EditorStyles.toggle;
                        style.fontStyle = componentName.ToLower().Contains(objectLookup.ToLower()) ? FontStyle.Bold : FontStyle.Normal;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(componentIcon, GUILayout.Width(20f), GUILayout.Height(20f));
                        component.enabled = GUILayout.Toggle(component.enabled, componentName, style);
                        GUILayout.EndHorizontal();

                        GUILayout.Label("Component index: " + componentIndex++);
                        List<string> hierarchy = new List<string>();
                        GetObjectHierarchy(obj.transform, ref hierarchy);
                        GUILayout.Label("Hierarchy Order:");
                        for (int e = 0; e < hierarchy.Count; e++) { GUILayout.Label(AddSpace(e * 2) + "↳ " + hierarchy[e]); }
                        GUILayout.Label("Child Index: " + obj.transform.GetSiblingIndex());
                        EditorGUILayout.Space();
                    }

                    if (GUILayout.Button("Find Object")) { EditorGUIUtility.PingObject(obj); }
                    if (obj.scene != EditorSceneManager.GetActiveScene()) { if (GUILayout.Button("Load Scene (" + obj.scene.name + ")")) { fold[i] = false; SwitchScenes(obj); } }

                    //if (obj.scene != EditorSceneManager.GetActiveScene()) { if (GUILayout.Button("Load Scene (" + obj.scene.name + ")")) { sceneObjName = obj.name; EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChanged; fold[i] = false; window.autoRepaintOnSceneChange = true; UnloadAllScenes(null); SceneSwitcher.SwitchScenes(obj.scene.path); } }
                }
            }
            EditorGUILayout.Space();
        }

        GUILayout.EndScrollView();

        // Add to search history
        if (!string.IsNullOrEmpty(objectLookup) && GUI.GetNameOfFocusedControl() != "SearchField" && lastSearchInstance.ToLower() != objectLookup.ToLower())
        {
            SceneSearchHistory.AddToHistory(this, new SceneSearchHistory.SearchHistory(objectLookup, filter, searchAllScenes, searchResults.Count));
            lastSearchInstance = objectLookup;
            GUI.SetNextControlName("SearchField");
            GUI.FocusControl(null);
        }
    }

    private void EditorSceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        EditorGUIUtility.PingObject(GameObject.Find(sceneObjName));
    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    void Unfocus()
    {
        // Add to search history
        SceneSearchHistory.AddToHistory(this, new SceneSearchHistory.SearchHistory(objectLookup, filter, searchAllScenes, searchResults.Count));

        // Remove focus if cleared
        objectLookup = "";
        GUI.FocusControl(null);
    }

    public string AddSpace(int count) { string spaces = " "; for (int i = 0; i < count; i++) { spaces += "  "; } return spaces; }

    void OnSearchFilterChanged(object filterMode)
    {
        if (filterMode.GetType() == typeof(SceneSearchFilter)) { filter = (SceneSearchFilter)filterMode; }
        else if (filterMode.GetType() == typeof(bool)) { searchAllScenes = (bool)filterMode; PreOpenScenes(); }
    }

    void OnSearchHistoryOpened(object obj)
    {
        SceneSearchHistory.Init(this);
    }

    void UnloadAllScenes(object obj)
    {
        Unfocus();
        scenesPaths = null;
        searchAllScenes = false;
        searchResults.Clear();
        searchResults = new List<GameObject>();
    }

    static void PreOpenScenes()
    {
        if (searchAllScenes)
        {
            var scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
            scenesPaths = scenesGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            for (int i = 0; i < scenesPaths.Count(); i++)
            {
                Scene scene = EditorSceneManager.OpenScene(scenesPaths[i], OpenSceneMode.Additive);
            }
        }
    }

    void GetObjectHierarchy(Transform obj, ref List<string> hierarchy)
    {
        if (obj.parent != null) { hierarchy.Insert(0, obj.name); GetObjectHierarchy(obj.parent, ref hierarchy); }
        else { hierarchy.Insert(0, obj.name); }
    }

    void SearchChildObjects(Transform parentObj)
    {
        for (int i = 0; i < parentObj.childCount; i++)
        {
            //BY OBJECT OR COMPONENT
            if (filter == SceneSearchFilter.ObjectsAndComponents)
            {
                if (parentObj.GetChild(i).name.ToLower().Contains(objectLookup.ToLower()) && !searchResults.Contains(parentObj.GetChild(i).gameObject)) { searchResults.Add(parentObj.GetChild(i).gameObject); }

                Component[] objComponents = parentObj.transform.GetChild(i).GetComponents<Component>();
                foreach (Component component in objComponents)
                {
                    if (component.GetType().Name.ToLower().Contains(objectLookup.ToLower()) && !searchResults.Contains(parentObj.GetChild(i).gameObject)) { searchResults.Add(parentObj.GetChild(i).gameObject); break; }
                }

                //RECCURSIVE SEARCH (CHILD OBJECTS)
                SearchChildObjects(parentObj.GetChild(i));
            }

            //BY OBJECT NAME
            else if (filter == SceneSearchFilter.ObjectsOnly)
            {
                if (parentObj.GetChild(i).name.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(parentObj.GetChild(i).gameObject); }
            }

            //BY OBJECT COMPONENT
            else if (filter == SceneSearchFilter.ComponentsOnly)
            {
                Component[] objComponents = parentObj.transform.GetChild(i).GetComponents<Component>();
                foreach (Component component in objComponents)
                {
                    if (component.GetType().Name.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(parentObj.GetChild(i).gameObject); break; }
                }
            }

            //BY OBJECT TAG
            else if (filter == SceneSearchFilter.ByTag)
            {
                if (parentObj.GetChild(i).tag.ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(parentObj.GetChild(i).gameObject); }
            }

            //BY OBJECT LAYER
            else if (filter == SceneSearchFilter.ByLayer)
            {
                if (LayerMask.LayerToName(parentObj.GetChild(i).gameObject.layer).ToLower().Contains(objectLookup.ToLower())) { searchResults.Add(parentObj.GetChild(i).gameObject); }
            }

            //RECCURSIVE SEARCH (CHILD OBJECTS)
            SearchChildObjects(parentObj.GetChild(i));
        }
    }

    Texture2D GetDefaultScriptIcon()
    {
        foreach (Texture2D obj in Resources.FindObjectsOfTypeAll(typeof(Texture2D)))
        {
            if (obj.name.ToLower() == "cs script icon") { return obj; }
        }

        return null;
    }

    string GetSceneName(string name)
    {
        if (!string.IsNullOrEmpty(name)) { return name; }
        else { return "[Unsaved Scene]"; }
    }

    void SwitchScenes(GameObject obj)
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.isDirty) { AskToSave(); }
        else { Load(obj); return; }

        if (saveScene) { EditorSceneManager.SaveOpenScenes(); Load(obj); }
        else if (!cancelSave) { Load(obj); }
    }

    void AskToSave()
    {
        int result = EditorUtility.DisplayDialogComplex("Unsaved Scene Changes", "Do you want to save changes made to the scene:\n" + GetSceneName(EditorSceneManager.GetActiveScene().name) + "\n\nAny unsaved changes will be discarded.", "Save", "Don't Save", "Cancel");
        saveScene = result == 0;
        cancelSave = result == 2;
    }

    void Load(GameObject obj)
    {
        sceneObjName = obj.name;
        EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChanged;
        window.autoRepaintOnSceneChange = true;

        UnloadAllScenes(null);
        SceneSwitcher.SwitchScenes(obj.scene.path);
    }
}