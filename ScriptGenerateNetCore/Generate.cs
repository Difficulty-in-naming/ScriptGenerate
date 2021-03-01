using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DreamLib.Editor.Unity.Extensition
{
    public class Generate
    {
        public KeyPart Tree;
        public Generate(string templatePath)
        {
            var str = File.ReadAllText(templatePath);
            Tree = new KeyPart(new TextSpan(0, str.Length, str), str);
        }

        public override string ToString()
        {
            return Tree.ToString();
        }
    }

    public class TextSpan
    {
        public int Start { get; }
        public int End { get; }
        public string Text { get; }
        public TextSpan(int start, int end, string text)
        {
            Start = start;
            End = end;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
    
    public class KeyPart
    {
        private List<Tuple<TextSpan, KeyPart>> Child = new List<Tuple<TextSpan, KeyPart>>();
        private List<Tuple<TextSpan, KeyPart>> ReplaceList = new List<Tuple<TextSpan, KeyPart>>();
        private string ReplaceString;
        public TextSpan Span { get; private set; }
        public KeyPart(TextSpan span,string temp)
        {
            Span = span;
            Analyzis(temp);
        }
        
        public KeyPart GetChild(string child)
        {
            foreach (var node in Child)
            {
                var key = node.Item1.Text.TrimEnd(']').Substring(2);
                if (key == child)
                    return node.Item2;
            }
            return null;
        }

        public void Add(string insertPos,KeyPart part)
        {
            TextSpan span = null;
            foreach (var node in Child)
            {
                var key = node.Item1.Text.TrimEnd(']').Substring(2);
                if (key == insertPos)
                    span = node.Item1;
            }
            if (span == null)
                throw new Exception("没有找到对应的Key");
            ReplaceList.Add(new Tuple<TextSpan, KeyPart>(span, part));
        }

        public void Replace(string value)
        {
            ReplaceString = value;
        }
        
        private void Analyzis(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;
            bool startGroup = false;
            int pass = 0;
            int startBrackets = 0;
            int startGroupStartIndex = 0;
            int startGroupEndIndex = 0;
            StringBuilder key = new StringBuilder();
            bool needCheckBrackets = false;
            char[] ccc = str.ToCharArray();
            for (var index = 0; index < str.Length; index++)
            {
                var node = str[index];
                if (node == '[' && pass == 0)//检查是否开始填充
                {
                    needCheckBrackets = str[index + 1] == '%';
                    if (str[index + 1] == '@' || str[index + 1] == '%')
                    {
                        startGroupStartIndex = index;
                        index++;
                        startGroup = true;
                    }
                    continue;
                }
                if (node == ']' && startGroup)//填充结束
                {
                    startGroup = false;
                    startGroupEndIndex = index;
                    if (needCheckBrackets == false)
                    {
                        var keyText = str.Substring(startGroupStartIndex, startGroupEndIndex - startGroupStartIndex + 1);
                        var span = new TextSpan(startGroupStartIndex, startGroupEndIndex, keyText);
                        if(!Child.Exists(t1=>t1.Item1.Text == span.Text))
                            Child.Add(new Tuple<TextSpan, KeyPart>(span, new KeyPart(span,"")));
                        key.Clear();
                    }
                    continue;
                }
                if (startGroup)//如果已经开始填充了.我们把沿途的字符串存为key
                {
                    key.Append(node);
                    continue;
                }
                if (key.Length > 0 && needCheckBrackets) //填充已经结束了,但是我们还需要判断填充的对象是不是Group
                {
                    if (node != '\n' && node != '\r' && node != '\t' && !char.IsSeparator(node))
                    {
                        if (node == '{')
                        {
                            if (pass == 0) startBrackets = index + 1;
                            pass += 1;
                        }
                        else if (node == '}' && pass >= 1)
                        {
                            pass -= 1;
                            if (pass == 0)
                            {
                                var valueText = str.Substring(startBrackets, index - startBrackets);
                                var keyText = str.Substring(startGroupStartIndex, startGroupEndIndex - startGroupStartIndex + 1);
                                if(!Child.Exists(t1=>t1.Item1.Text == keyText))
                                    Child.Add(new Tuple<TextSpan, KeyPart>(new TextSpan(startGroupStartIndex, startGroupEndIndex, keyText),
                                        new KeyPart(new TextSpan(startBrackets, index, valueText), valueText)));
                                key.Clear();
                            }
                        }
                    }
                }
            }
        }

        public KeyPart Clone()
        {
            if (Child.Count > 0)
                return new KeyPart(Span, Span.Text);
            else
                return new KeyPart(Span, "");
        }

        public override string ToString()
        {
            //排序文本顺序
            StringBuilder builder = new StringBuilder();
            int position = 0;
            Dictionary<string, string> flag = new Dictionary<string, string>();
            foreach (var node in Child)
            {
                var debugStr = builder.ToString();
                var str = new StringBuilder(Span.Text.Substring(position, node.Item1.Start - position));
                builder.Append(str);
                if (!string.IsNullOrEmpty(node.Item2.ReplaceString))
                {
                    builder.Append(node.Item2.Span.Text);
                    flag.Add(node.Item2.Span.Text,node.Item2.ReplaceString);
                }
                else
                {
                    var list = ReplaceList.FindAll(t1 => t1.Item1 == node.Item1);
                    foreach (var target in list)
                    {
                        builder.AppendLine(target.Item2.ToString());
                    }
                }
                position = node.Item2.Span.End + 1;
            }

            builder.Append(Span.Text.Substring(position, Span.End - Span.Start - position));
            foreach (var f in flag)
                builder.Replace(f.Key, f.Value);
            return builder.ToString();
        }
    } 
}
