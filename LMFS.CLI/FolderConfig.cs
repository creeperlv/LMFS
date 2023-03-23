namespace LMFS.CLI {
    [Serializable]
    public class FolderConfig {
        public string remote_server;
        public string remote_path;
        public SyncMethod SyncMethod= SyncMethod.Full;
    }
    public enum SyncMethod{ Full, TimeDiff, HashDiff}
}