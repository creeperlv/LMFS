using System;
using System.Collections;
using System.Collections.Specialized;

namespace LMFS.Extensible {
    public static class LMFSExtensibleEnv {
        public static string CurrentDirectory = Environment.CurrentDirectory;
        public static StringDictionary Environments;
        static LMFSExtensibleEnv() {
            Environments = new StringDictionary();
            foreach (DictionaryEntry v in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process)) {
                Environments.Add(v.Key.ToString(), v.Value.ToString());
            }
        }
    }

}
