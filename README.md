# ConstFieldRemover

ILからconstフィールドを取り除くツール

## 説明

以下のクラスのフィールドにconstを使ったコードでDLLを生成するする。

```csharp
using System;

namespace TestDLL
{
    public class Test
    {
        const string text = "hogehoge";
        public void Test1()
        {
            Console.WriteLine(text);
        }
        public void Test2()
        {
            Console.WriteLine(text);
        }
    }
}
```

生成されたDLLをildasmでILをダンプするとliteralフィールドが含まれる。

```
.class public auto ansi beforefieldinit TestDLL.Test
       extends [System.Runtime]System.Object
{
  .field private static literal string text = "hogehoge"
  .method public hidebysig instance void 
          Test1() cil managed
  {
    // コード サイズ       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "hogehoge"
    IL_0006:  call       void [System.Console]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Test::Test1

  .method public hidebysig instance void 
          Test2() cil managed
  {
    // コード サイズ       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "hogehoge"
    IL_0006:  call       void [System.Console]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Test::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // コード サイズ       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Test::.ctor

} // end of class TestDLL.Test
```

DLLからconstフィーイルドを取り除く

```
> ConstFieldRemover TestDLL.dll -o TestDLL_fix.DLL
```

変換後のDLLのILダンプ
```
.class public auto ansi beforefieldinit TestDLL.Test
       extends [System.Runtime]System.Object
{
  .method public hidebysig instance void 
          Test1() cil managed
  {
    // コード サイズ       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "hogehoge"
    IL_0006:  call       void [System.Console]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Test::Test1

  .method public hidebysig instance void 
          Test2() cil managed
  {
    // コード サイズ       13 (0xd)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldstr      "hogehoge"
    IL_0006:  call       void [System.Console]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ret
  } // end of method Test::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // コード サイズ       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Test::.ctor

} // end of class TestDLL.Test
```

## UnityのIL2CPPビルドで使用する

IL2CPPの前処理としてconstフィールドを取り除いてビルドをする。

```csharp
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Il2Cpp;
using UnityEditor;

public class PreIL2CPP : IIl2CppProcessor
{
    public int callbackOrder => 0;

    public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
    {
        string dllPath = data.inputDirectory + "/Assembly-CSharp.dll";
        if (System.IO.File.Exists(dllPath))
        {
            var proc = System.Diagnostics.Process.Start("ConstFieldRemover.exe", dllPath);
            proc.WaitForExit();
            Debug.Log(proc.ExitCode);
        }
    }
}
```
