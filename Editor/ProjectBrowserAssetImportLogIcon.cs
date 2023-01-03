using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class ProjectBrowserAssetImportLogIcon
    {
        static ProjectBrowserAssetImportLogIcon()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        private static void OnProjectWindowItemOnGUI
        (
            string guid,
            Rect   selectionRect
        )
        {
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            var importLog = AssetImporter.GetImportLog( assetPath );

            if ( importLog == null ) return;

            var logEntries = importLog.logEntries;

            if ( logEntries == null ) return;

            var flags = logEntries.Any( x => x.flags == ImportLogFlags.Error )
                    ? ImportLogFlags.Error
                    : logEntries.Any( x => x.flags == ImportLogFlags.Warning )
                        ? ImportLogFlags.Warning
                        : ImportLogFlags.None
                ;

            if ( flags == ImportLogFlags.None ) return;

            var name = flags switch
            {
                ImportLogFlags.Warning => "warning",
                ImportLogFlags.Error   => "error",
                _                      => throw new ArgumentOutOfRangeException()
            };

            var iconContent = EditorGUIUtility.IconContent( name );

            const float size = 18;

            var position = new Rect( selectionRect )
            {
                x      = selectionRect.xMax - size,
                y      = selectionRect.y,
                width  = size,
                height = size,
            };

            if ( !GUI.Button( position, iconContent, GUIStyle.none ) ) return;

            var type = typeof( ImportLog );

            var methodInfo = type.GetMethod
            (
                "PrintToConsole",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            methodInfo!.Invoke( importLog, Array.Empty<object>() );
        }
    }
}