using System.IO;
using System.Linq;
using Phase1.Editor.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Phase1.Editor
{
    public class EditorSceneNavigatorViewModel : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private EditorSceneNavigatorData data;
    
        private void Awake()
        {
            var fileName = "EditorSceneNavigatorData.asset";
            var path = $"Assets/Settings/EditorSceneNavigator/";
            var pathWithFileName = path + fileName;
            this.data = AssetDatabase.LoadAssetAtPath<EditorSceneNavigatorData>(pathWithFileName);
            if (this.data == null)
            {
                Directory.CreateDirectory(path);
                this.data = CreateInstance<EditorSceneNavigatorData>();
                AssetDatabase.CreateAsset(this.data, pathWithFileName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Window/Editor Scene Navigator")]
        public static void Show()
        {
            EditorSceneNavigatorViewModel wnd = GetWindow<EditorSceneNavigatorViewModel>();
            wnd.titleContent = new GUIContent("Scenes");
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = this.rootVisualElement;
            this.rootVisualElement.style.flexGrow = 1;
            VisualElement uxml = this.m_VisualTreeAsset.Instantiate();
            root.Add(uxml);

            var templateContainer = this.rootVisualElement.Q<TemplateContainer>();
            if (templateContainer != null)
            {
                templateContainer.style.flexGrow = 1;
            }

            var visualElement = uxml.Q<VisualElement>(name: "RootVisualElement");
            visualElement.dataSource = this.data;

            var scenesList = visualElement.Q<ListView>("ScenesList");
            scenesList.itemIndexChanged += (_, _) => this.data.SetAssetDirty();
            scenesList.bindItem = (element, index) =>
            {
                var sceneData = (EditorSceneNavigatorSceneData)scenesList.itemsSource[index];
                element.Q<Button>("LoadButton").clicked += () => this.OpenScene(sceneData);
                element.Q<Button>("SelectButton").clicked += () => EditorGUIUtility.PingObject(sceneData.sceneAsset);
                element.RegisterCallback<PointerDownEvent>(evt => this.ClickEvent(evt, sceneData));
            };
        
            root.Q<Button>("LoadBuildListButton").clicked += this.LoadScenesFromBuildList;
            root.Q<Toggle>("ShowButtonsToggle").RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                this.data.SetShowButtons(evt.newValue);
            } );

            root.RegisterCallback<DragUpdatedEvent>(this.OnDragUpdate);
            root.RegisterCallback<DragPerformEvent>(this.OnDragPerform);
        }

        private void LoadScenesFromBuildList()
        {
            var buildSettings = EditorBuildSettings.scenes;
            var sceneAssets = buildSettings
                .Where(scene => scene.enabled)
                .Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path))
                .ToArray();
            this.data.SetSceneAssets(sceneAssets);
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            if (DragAndDrop.paths.Length > 0 || DragAndDrop.objectReferences.Length > 0)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            var sceneAssets = DragAndDrop.objectReferences.OfType<SceneAsset>().ToArray();
            this.data.AddSceneAssets(sceneAssets);
        }

        private void OpenScene(EditorSceneNavigatorSceneData sceneData)
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string path = AssetDatabase.GetAssetPath(sceneData.sceneAsset);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);
            }
        }

        private void ClickEvent(PointerDownEvent evt, EditorSceneNavigatorSceneData sceneData)
        {
            switch (evt.button)
            {
                case 0:
                    if (evt.clickCount == 2)
                    {
                        this.OpenScene(sceneData);
                    }

                    break;
                case 1:
                    EditorGUIUtility.PingObject(sceneData.sceneAsset);
                    break;
            }
        } 
    }
}