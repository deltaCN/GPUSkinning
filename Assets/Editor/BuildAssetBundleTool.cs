using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
public static class BuildAssetBundleTool 
{
	[MenuItem("Assets/BuildAssetbundle")]
	private static void BuildAssetBundleBySelect()
	{
		string exportDir = "ExportAssetBundle";
		if (!Directory.Exists (exportDir))
			Directory.CreateDirectory (exportDir);
		var assetBundleBuilds = GetSelectObect().Select (item => new AssetBundleBuild () {
			assetBundleName = Path.GetFileNameWithoutExtension (item),
			assetNames = new string[]{item}
		});
		BuildPipeline.BuildAssetBundles (exportDir, assetBundleBuilds.ToArray (), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
	}
	private static IEnumerable<string> GetSelectObect()
	{
		return GetSelectAssets<UnityEngine.Object>("Object");
	}
	private static IEnumerable<string>  GetSelectAssets<T> (string assetType) where T : UnityEngine.Object
	{
		IEnumerable<string> selectPaths = Selection.assetGUIDs.Select (item => AssetDatabase.GUIDToAssetPath (item));
		var findAsset = selectPaths.Where(item => File.Exists(item) && AssetDatabase.LoadAssetAtPath<T>(item) != null);
		IEnumerable<string> directorPaths = selectPaths.Where (item => !File.Exists (item));
		if (directorPaths.Count () != 0) 
			findAsset = findAsset.Concat(AssetDatabase.FindAssets (string.Format ("t:{0}", assetType), directorPaths.ToArray ())
				.Select(item => AssetDatabase.GUIDToAssetPath(item)));
		return findAsset;
	}
}
