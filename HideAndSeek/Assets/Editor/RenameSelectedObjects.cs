using UnityEditor;
using UnityEngine;
public class RenameSelectedObjects
{
    [MenuItem("Tools/Rename Selected Objects")]
    static void Rename()
    {
        GameObject[] objects = Selection.gameObjects;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].name = objects[i].name + "_" + (i + 1);
        }
    }
}