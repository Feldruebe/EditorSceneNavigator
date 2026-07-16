using System;
using Unity.Properties;
using UnityEditor;
using UnityEngine.UIElements;

namespace Phase1.Editor.Model
{
    [Serializable]
    public class EditorSceneNavigatorSceneData
    {
        
        public EditorSceneNavigatorSceneData(EditorSceneNavigatorData parent)
        {
            this.parent = parent;
        }
    
        [CreateProperty]
        public string sceneName => this.sceneAsset != null ? this.sceneAsset.name : string.Empty;
    
        [CreateProperty]
        public SceneAsset sceneAsset;

        [CreateProperty]
        public DisplayStyle showButtons => this.parent.showButtons;

        public EditorSceneNavigatorData parent;
    }
}