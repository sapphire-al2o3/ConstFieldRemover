using System.Collections.Generic;
using Mono.Cecil;
using System.IO;

namespace ConstFieldRemover
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            string path = args[0];
            string output = null;
            var access = FileAccess.ReadWrite;

            ReaderParameters paramter = new ReaderParameters();
            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
            
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i] == "-o" && i + 1 < args.Length)
                    {
                        output = args[i + 1];
                        access = FileAccess.Read;
                        i++;
                    }
                    else if (args[i] == "-s" && i + 1 < args.Length)
                    {
                        assemblyResolver.AddSearchDirectory(args[i + 1]);
                        i++;
                    }
                }
            }

            paramter.AssemblyResolver = assemblyResolver;

            using (var fs = File.Open(path, FileMode.Open, access))
            using (var ad = AssemblyDefinition.ReadAssembly(fs, paramter))
            {
                List<FieldDefinition> fields = new List<FieldDefinition>();

                foreach (var mod in ad.Modules)
                {
                    foreach (var t in mod.Types)
                    {
                        fields.Clear();
                        foreach (var f in t.Fields)
                        {
                            if (f.HasConstant)
                            {
                                fields.Add(f);
                            }
                        }
                        foreach (var f in fields)
                        {
                            t.Fields.Remove(f);
                        }
                    }
                }

                if (output == null)
                {
                    ad.Write();
                }
                else
                {
                    ad.Write(output);
                }
            }
        }
    }
}
