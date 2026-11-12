using UnityEditor;
using UnityEngine;

public static class LoopTimeSetter
{
    [MenuItem("Tools/Set Loop Time By Name")]
    static void Apply()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;

            var clips = importer.clipAnimations;
            if (clips == null || clips.Length == 0)
                clips = importer.defaultClipAnimations;

            foreach (var clip in clips)
                clip.loopTime = clip.name.Contains("Loop");

            importer.clipAnimations = clips;
            importer.SaveAndReimport();
        }
    }
}