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

			if (args.Length > 1)
			{
				output = args[1];
			}

			using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
			using (var ad = AssemblyDefinition.ReadAssembly(path))
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
