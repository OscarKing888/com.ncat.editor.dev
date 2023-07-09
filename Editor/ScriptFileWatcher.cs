using System.IO;
using System.Threading;
using UnityEditor;

namespace NCatDev
{

    /// <summary>
    /// use fswatch to monitor Scripts folder，start auto compile in background before Unity gets focused
    /// for Mac users, use: 'brew install fswatch' to install fswatch
    /// </summary>
    [InitializeOnLoad]
    public class ScriptFileWatcher
    {
        public static string ScriptPath = "Assets";
        public static bool SetRefresh;

        static ScriptFileWatcher()
        {
            ThreadPool.QueueUserWorkItem(MonitorDirectory, ScriptPath);
            EditorApplication.update += OnUpdate;
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            SetRefresh = true;
        }

        private static void MonitorDirectory(object obj)
        {
            string path = (string)obj;

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Changed;
            fileSystemWatcher.Renamed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Changed;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void OnUpdate()
        {
            if (!SetRefresh) return;

            if (EditorApplication.isCompiling) return;
            if (EditorApplication.isUpdating) return;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport & ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(ScriptPath, ImportAssetOptions.ForceSynchronousImport & ImportAssetOptions.ForceUpdate);
            SetRefresh = false;
        }
    }
} // namespace NCatDev