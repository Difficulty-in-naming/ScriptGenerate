# Github-ScriptGenerate
一个生成脚本的工具

#使用说明

![index](https://github.com/pk27602017/ScriptGenerate/raw/master/Image/1.png)
你可以定义一个[@xxxx]
这样声明一个区域,在此区域内你可以使用{@xxxx}来声明一个小变量.

在代码中你需要
new Generate(path, Action<Generate.Info, Serializer>).StartWrite();
开始创建脚本
如果需要把脚本的内容格式化
你可以调用 Generate.FormatScript(string str);这样就会重新布局脚本内的内容了
