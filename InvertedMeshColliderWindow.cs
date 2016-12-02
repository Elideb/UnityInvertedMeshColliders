using UnityEditor;
using UnityEngine;

namespace Elideb {
    public class InvertedMeshColliderWindow : EditorWindow {

        private string[] allAssetsGUIDs = null;
        private int currentAssetIdx = 0;
        private int prefabsFound = 0;

        [MenuItem("Custom/Elideb/Log prefabs with inverted mesh colliders")]
        public static void FindInvertedMeshColliderPrefabs() {
            var win = EditorWindow.GetWindow<InvertedMeshColliderWindow>("Inverted meshes", true);
            if (win)
                win.Show();
        }

        void Awake() {
            this.allAssetsGUIDs = AssetDatabase.FindAssets("t:Object");
            this.currentAssetIdx = 0;
            this.conflictivePrefabs = 0;
            this.conflictiveMeshColliders = 0;

            this.csvLog = new System.Text.StringBuilder("Prefab\tGameObject\tLocal scale\tLossy scale\tMesh\tPresent\tRead/Write\tCheck");
            this.csvLog.AppendLine();
        }

        private int conflictiveMeshColliders = 0;
        private int conflictivePrefabs = 0;

        private System.Text.StringBuilder csvLog = null;

        void Update() {
            int i = 0;
            while (i < 5 && this.currentAssetIdx < this.allAssetsGUIDs.Length) {
                var assetGUID = allAssetsGUIDs[currentAssetIdx];
                string path = AssetDatabase.GUIDToAssetPath(assetGUID);
                var loadedAsset = AssetDatabase.LoadMainAssetAtPath(path);
                if (loadedAsset) {
                    try {
                        PrefabType t = PrefabUtility.GetPrefabType(loadedAsset);
                        if (t == PrefabType.Prefab) {
                            ++this.prefabsFound;
                            var instance = Instantiate(loadedAsset);
                            GameObject go = instance as GameObject;
                            if (go != null) {
                                go.name = loadedAsset.name;
                                Transform trans = go.transform;
                                if (this.CheckHierarchyForMeshColliders(path, trans, go.name))
                                    ++this.conflictivePrefabs;

                                DestroyImmediate(go);
                            }
                        }
                    } catch (System.Exception e) {
                        Debug.LogError(e.Message + System.Environment.NewLine + e.StackTrace);
                    }
                }

                ++i;
                ++this.currentAssetIdx;

                if (this.currentAssetIdx < this.allAssetsGUIDs.Length
                 && EditorUtility.DisplayCancelableProgressBar("Looking for inverted meshes", this.currentAssetIdx + "/" + this.allAssetsGUIDs.Length, (float)this.currentAssetIdx / (float)this.allAssetsGUIDs.Length)) {
                    this.EndProcess();
                    break;
                }
            }

            if (this.currentAssetIdx >= this.allAssetsGUIDs.Length)
                this.EndProcess();
        }

        private bool CheckHierarchyForMeshColliders(string path, Transform trans, string hierarchy) {
            bool result = this.CheckForConflictiveMeshCollider(path, trans, hierarchy);
            foreach (Transform child in trans) {
                result = this.CheckHierarchyForMeshColliders(path, child, hierarchy + "/" + child.gameObject.name) || result;
            }

            return result;
        }

        private bool CheckForConflictiveMeshCollider(string path, Transform trans, string hierarchy) {
            if (this.HasConflictiveMeshCollider(trans)) {
                ++this.conflictiveMeshColliders;
                this.Log(path, trans, hierarchy);

                return true;
            }

            return false;
        }

        private void Log(string path, Transform trans, string hierarchy) {
            ////Debug.Log("Prefab:" + path + System.Environment.NewLine
            ////        + "GameObject " + hierarchy + System.Environment.NewLine
            ////        + "MeshCollider and scale " + trans.localScale + " (lossy " + trans.lossyScale + ")");

            // "Prefab\tGameObject\tLocal scale\tLossy scale\tMesh\tMesh Asset\tRead/write\tCheck"
            MeshCollider mc = trans.GetComponent<MeshCollider>();
            Mesh mesh = mc.sharedMesh;
            string assetPath = mesh ? AssetDatabase.GetAssetPath(mesh) : "-";

            ModelImporter meshImporter = mesh ? AssetImporter.GetAtPath(assetPath) as ModelImporter : null;

            this.csvLog.AppendLine(path + "\t"
                                 + hierarchy + "\t"
                                 + "(" + trans.localScale.x + "," + trans.localScale.y + "," + trans.localScale.z + ")\t"
                                 + "(" + trans.lossyScale.x + "," + trans.lossyScale.y + "," + trans.lossyScale.z + ")\t"
                                 + (mesh ? mesh.name : "[null]") + "\t"
                                 + (mesh ? AssetDatabase.GetAssetPath(mesh) : "-") + "\t"
                                 + (meshImporter ? meshImporter.isReadable.ToString() : "-") + "\t");
        }

        private bool HasConflictiveMeshCollider(Transform trans) {
            MeshCollider mc = trans.gameObject.GetComponent<MeshCollider>();
            if (mc != null) {
                Vector3 scaleDiff = (Vector3.one - trans.localScale);
                float x = trans.localScale.x;
                float y = trans.localScale.y;
                float z = trans.localScale.z;
                return x < 0 || y < 0 || z < 0
                    || Mathf.Abs(x - y) > .01f || Mathf.Abs(x - z) > .01f || Mathf.Abs(y - z) > .01f;
            }

            return false;
        }

        private void EndProcess() {
            Debug.Log("Found " + this.prefabsFound + " prefabs.");
            Debug.Log("Conflictive prefabs: " + this.conflictivePrefabs);
            Debug.Log("Conflictive MeshCollider: " + this.conflictiveMeshColliders);

            try {
                using (System.IO.TextWriter writer = System.IO.File.CreateText(Application.dataPath + "/../ConflictiveMeshColliders.csv")) {
                    writer.Write(this.csvLog.ToString());
                }
            } finally {
                EditorUtility.ClearProgressBar();
                this.Close();
            }
        }

        private string GetTransformHierarchy(Transform trans) {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder(trans.gameObject.name);
            Transform current = trans;
            while (current.parent) {
                current = current.parent;
                strBuilder.Insert(0, current.gameObject.name + "/");
            }

            return strBuilder.ToString();
        }

    }
}
