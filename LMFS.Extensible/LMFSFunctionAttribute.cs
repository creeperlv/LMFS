using System;

namespace LMFS.Extensible {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class LMFSFunctionAttribute : Attribute {
        readonly string name;

        // This is a positional argument
        public LMFSFunctionAttribute(string name) {
            this.name = name;
        }

        public string Name {
            get { return name; }
        }

    }

}
