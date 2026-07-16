using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Phase1.Editor.Model
{
    public class EditorSceneNavigatorData : ScriptableObject
    {
        [CreateProperty] public List<EditorSceneNavigatorSceneData> scenePaths;

        [CreateProperty] public DisplayStyle showButtons = DisplayStyle.None;

        public void AddSceneAssets(SceneAsset[] sceneAssets)
        {
            this.scenePaths ??= new List<EditorSceneNavigatorSceneData>();
            var currentSceneAssets = this.scenePaths.Select(sceneData => sceneData.sceneAsset);
            var newSceneAssets = sceneAssets.Where(sceneAsset => !currentSceneAssets.Contains(sceneAsset));
            this.scenePaths.AddRange(newSceneAssets.Select(sceneAsset => new EditorSceneNavigatorSceneData(this) { sceneAsset = sceneAsset }));
            this.SetAssetDirty();
        }

        public void SetSceneAssets(SceneAsset[] sceneAssets)
        {
            this.scenePaths ??= new List<EditorSceneNavigatorSceneData>();
            this.scenePaths.Clear();
            this.scenePaths.AddRange(sceneAssets.Select(sceneAsset => new EditorSceneNavigatorSceneData(this) { sceneAsset = sceneAsset }));
            this.SetAssetDirty();
        }

        public void SetShowButtons(bool showButtonsInput)
        {
            this.showButtons = showButtonsInput ? DisplayStyle.Flex : DisplayStyle.None;
            this.SetAssetDirty();
        }

        public void SetAssetDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}