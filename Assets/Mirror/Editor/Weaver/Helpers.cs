using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.CecilX;

namespace Mirror.Weaver
{
    internal class Helpers
    {
        // This code is taken from SerializationWeaver

        private class AddSearchDirectoryHelper
        {
            private delegate void AddSearchDirectoryDelegate(string directory);

            private readonly AddSearchDirectoryDelegate _addSearchDirectory;

            public AddSearchDirectoryHelper(IAssemblyResolver assemblyResolver)
            {
                // reflection is used because IAssemblyResolver doesn't implement AddSearchDirectory but both DefaultAssemblyResolver and NuGetAssemblyResolver do
                var addSearchDirectory = assemblyResolver.GetType().GetMethod("AddSearchDirectory",
                    BindingFlags.Instance | BindingFlags.Public, null, new Type[] {typeof(string)}, null);
                if (addSearchDirectory == null)
                    throw new Exception("Assembly resolver doesn't implement AddSearchDirectory method.");
                _addSearchDirectory =
                    (AddSearchDirectoryDelegate) Delegate.CreateDelegate(typeof(AddSearchDirectoryDelegate),
                        assemblyResolver, addSearchDirectory);
            }

            public void AddSearchDirectory(string directory)
            {
                _addSearchDirectory(directory);
            }
        }

        public static string UnityEngineDLLDirectoryName()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            return directoryName?.Replace(@"file:\", "");
        }

        public static string DestinationFileFor(string outputDir, string assemblyPath)
        {
            var fileName = Path.GetFileName(assemblyPath);
            Debug.Assert(fileName != null, "fileName != null");

            return Path.Combine(outputDir, fileName);
        }
    }
}