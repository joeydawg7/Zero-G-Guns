using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneFinder;

/// <summary>
/// Search History - manages logging searches from SceneFinder.cs and SceneSwitcher.cs
/// Developer: Dibbie
/// Email: mailto:strongstrenth@hotmail.com [for questions/help and inquires]
/// Website: http://www.simpleminded.x10host.com
/// Discord: https://discord.gg/33PFeMv
/// </summary>
public class SceneSearchHistory : EditorWindow
{
    const int MIN_CACHE_SIZE = 10;
    const int MAX_CACHE_SIZE = 300;

    static EditorWindow window;
    static EditorWindow historyForWindow;

    [SerializeField]
    static List<SearchHistory> sceneFinderHistory = new List<SearchHistory>();
    [SerializeField]
    static List<SearchHistory> sceneSwitcherHistory = new List<SearchHistory>();

    [SerializeField]
    public static int historyCacheSize = 100;
    [SerializeField]
    static bool disableHistoryLogging;

    static Vector2 scrollPos;

    public class SearchHistory
    {
        public string search;
        public SceneSearchFilter searchFilter;
        public bool allScenes;
        public int resultsFound;
        
        public bool searchByPath;

        public SearchHistory(string lookup, SceneSearchFilter filter, bool searchAllScenes, int totalResults)
        {
            search = lookup;
            filter = searchFilter;
            allScenes = searchAllScenes;
            resultsFound = totalResults;
        }

        public SearchHistory(string lookup, bool includePathSearching, int totalResults)
        {
            search = lookup;
            searchByPath = includePathSearching;
            resultsFound = totalResults;
        }
    }
    
    public static void Init(EditorWindow editorType)
    {
        historyForWindow = editorType;
        window = GetWindow(typeof(SceneSearchHistory), true, "Search History");
        window.Show();
    }

    public static void AddToHistory(EditorWindow window, SearchHistory result)
    {
        historyForWindow = window;
        if (!disableHistoryLogging && !string.IsNullOrEmpty(result.search.Trim()) && !string.IsNullOrWhiteSpace(result.search.Trim()))
        {
            if (historyForWindow.GetType() == typeof(SceneFinder))
            {
                if (sceneFinderHistory.Count >= historyCacheSize) { sceneFinderHistory.RemoveAt(0); }
                sceneFinderHistory.Add(result);
            }
            else if (historyForWindow.GetType() == typeof(SceneSwitcher))
            {
                if (sceneSwitcherHistory.Count >= historyCacheSize) { sceneSwitcherHistory.RemoveAt(0); }
                sceneSwitcherHistory.Add(result);
            }
        }
    }

    void OnGUI()
    {
        if (window == null) { window = GetWindow(typeof(SceneSearchHistory)); }

        disableHistoryLogging = GUILayout.Toggle(disableHistoryLogging, new GUIContent("Disable History Logging", "If enabled, both the Scene Search and Scene Switcher will stop logging search results."));
        historyCacheSize = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("History Cache Size", "Cache Size represents the max number of logged search results, for both Scene Search and Scene Switcher. Between " + MIN_CACHE_SIZE + " and " + MAX_CACHE_SIZE), historyCacheSize), MIN_CACHE_SIZE, MAX_CACHE_SIZE);
        if(GUILayout.Button(new GUIContent("Clear Cache", "Clears the cache for both Scene Search and Scene Switcher.")))
        {
            sceneFinderHistory.Clear();
            sceneFinderHistory = new List<SearchHistory>();
        }

        EditorGUILayout.Space();

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        if (historyForWindow.GetType() == typeof(SceneFinder))
        {
            for (int i = 0; i < sceneFinderHistory.Count; i++)
            {
                GUILayout.Label("\"" + sceneFinderHistory[i].search + "\"", EditorStyles.boldLabel);
                GUILayout.Label("Search Filter: " + sceneFinderHistory[i].searchFilter.ToString());
                GUILayout.Toggle(sceneFinderHistory[i].allScenes, "Search All Scenes");
                GUILayout.Label("Search Results Found: " + sceneFinderHistory[i].resultsFound);

                if (GUILayout.Button("Search Again"))
                {
                    SceneFinder.SearchFromHistory(sceneFinderHistory[i]);
                    window.Close();
                }

                EditorGUILayout.Space();
            }
        }
        else if(historyForWindow.GetType() == typeof(SceneSwitcher))
        {
            for (int i = 0; i < sceneSwitcherHistory.Count; i++)
            {
                GUILayout.Label("\"" + sceneSwitcherHistory[i].search + "\"", EditorStyles.boldLabel);
                GUILayout.Toggle(sceneSwitcherHistory[i].searchByPath, "Search By Path");
                GUILayout.Label("Search Results Found: " + sceneSwitcherHistory[i].resultsFound);

                if (GUILayout.Button("Search Again"))
                {
                    SceneSwitcher.SearchFromHistory(sceneSwitcherHistory[i]);
                    window.Close();
                }

                EditorGUILayout.Space();
            }
        }
        GUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
