# Github-ScriptGenerate
一个生成脚本的工具

#后续修改
重新修改API变得更加易用.

#使用说明

首先创建一个代码的模板文件.或者在代码中写入模板字符串
```
/********************************
  该脚本是自动生成的请勿手动修改
*********************************/
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Config.ConfigCore;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
namespace RestaurantPreview.Config
{
	[%CustomClass]
	{
		public partial class [@Class]
		{
			[%Field]{private [@Type] m[@Name];}
	    	[%Property]
	    	{
	    	    public [@Type] [@Name]
	    	    {
	    	    	get{ return m[@Name]; }
	    	    	set{ m[@Name] = value; }
	    	    }
	    	}
		}
	}
	[%CoreClass]
	{
		public partial class [@Class]Property : ConfigAssetManager<[@Class]Property>
		{
			[%Enum]
			{
				public enum [@Name]
				{
					[%Nested]{[@Key][@Value], }
				}
			}

			[%Field]{private [@Type] m[@Name];}
			[%Property]
			{
			/// <summary>
			/// [@Comment]
			/// </summary>
			public [@Type] [@Name]
			{
				get => m[@Name];
				set => m[@Name] = value;
			}
			}

			public static [@Class]Property Read([@KeyType] id, bool throwException = true)
			{
				return ConfigAssetManager<[@Class]Property>.Read(id, throwException);
			}

			[%HasKey]
			{
				public static [@Class]Property Read([@KeyType] id, bool throwException = true)
				{
					return ConfigAssetManager<[@Class]Property>.Read(id, throwException);
				}
			}

			public static Dictionary<[@KeyType],[@Class]Property> ReadDict()
			{
				return ConfigAssetManager<[@Class]Property>.Read[@KeyType]Dict();
			}

			public static List<[@Class]Property> ReadList()
			{
				return ConfigAssetManager<[@Class]Property>.ReadList();
			}
		}
	}
}
```

一些特殊的语法.标记为[@XXXX]格式的关键字可以通过API替换成对应的字符串.
标记为[%XXXX]{}格式的关键字.花括号中的内容可以通过API复制多份.

#示例

使用以上模板作为范例
调用以下代码
```c#
//创建代码生成器
Generate generate = new Generate(Config.ExePath + Path.DirectorySeparatorChar + "GenerateTemplate.txt");
//创建脚本
ScriptTemplate template = new ScriptTemplate();
//这里我们创建一个类定义
template.CoreClass = new ClassDefine();
//增加一个变量
template.CoreClass.Variables.Add(new VariableDefine("int","i","this is comment"));
//修改CoreClass中[@Class]的名字
template.CoreClass.Name = "HelloWorld";
//修改CoreClass中[@KeyType]的内容
template.CoreClass.KeyType = "int"

//接下来我们开始写入
//这里我们获取模板中的CoreClass节点内容
var coreClass = generate.Tree.GetChild("CoreClass");
coreClass.GetChild("Class").Replace(template.CoreClass.Name);
coreClass.GetChild("KeyType").Replace(template.CoreClass.KeyType);
for (int i = 0; i < template.CoreClass.Variables.Count; i++)
{
    var variable = template.CoreClass.Variables[i];
    var field = coreClass.GetChild("Field").Clone();
    field.GetChild("Type").Replace(variable.Type);
    field.GetChild("Name").Replace(variable.Name);
    coreClass.Add("Field", field);
    var property = coreClass.GetChild("Property").Clone();
    property.GetChild("Comment").Replace(variable.Comment);
    property.GetChild("Type").Replace(variable.Type);
    property.GetChild("Name").Replace(variable.Name);
    coreClass.Add("Property", property);
}

//最后不要忘记了把生成出来的类插入到脚本当中替换原脚本的模板.
//当然你也可以重复Add “CoreClass” 
//比如 generate.Tree.Add("CoreClass", coreClass);
//     generate.Tree.Add("CoreClass", OtherClass);
//这样会在模板对应的位置插入一个CoreClass 一个 OtherClass
generate.Tree.Add("CoreClass", coreClass);

//最后将模板转换为脚本
var scriptResult = generate.ToString();
```
