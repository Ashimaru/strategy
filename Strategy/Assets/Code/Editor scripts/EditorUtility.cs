using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LamTools
{
    public static class EditorUtility
    {
        public static string GetCurrentEditorDirectory()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }
    }
}