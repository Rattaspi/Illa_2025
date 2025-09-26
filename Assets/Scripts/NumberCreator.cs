using UnityEditor;
using UnityEngine;

public class NumberCreator : MonoBehaviour {
    [SerializeField] UnityEngine.Object numberPrefab;
    [SerializeField] int maxNumbers = 400;

    [ContextMenu("Create numbers")]
    public void GenerateNumbers() {
        for (int i = 0; i < maxNumbers; i++) {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(numberPrefab, this.transform);
            go.name = (i+1).ToString("000");

            Number number = go.GetComponent<Number>();
            number.SetNumber(i+1);

            Undo.RegisterCreatedObjectUndo(go, "Number creation");
        }
    }
}
