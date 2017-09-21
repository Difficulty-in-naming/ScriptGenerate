using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptGenerate
{
    public class GenerateConfigTemplate
    {
        public GenerateClassTemplate Class;
        public List<GeneratePropertiesTemplate> Properties = new List<GeneratePropertiesTemplate>();
        public List<GenerateMethodTemplate> Method = new List<GenerateMethodTemplate>();

        public void Add(GenerateMethodTemplate temp)
        {
            Method.Add(temp);
        }

        public void Add(GeneratePropertiesTemplate temp)
        {
            Properties.Add(temp);
        }
    }

    public class GenerateCommonTemplate
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type;
        /// <summary>
        /// 属性
        /// </summary>
        public string Attribute;
        /// <summary>
        /// 注释
        /// </summary>
        public string Remark;
        /// <summary>
        /// 自定义数据
        /// </summary>
        public object Data;
    }

    public class GenerateClassTemplate : GenerateCommonTemplate
    {
        /// <summary>
        /// 继承
        /// </summary>
        public string Inhert;
    }

    public class GeneratePropertiesTemplate : GenerateCommonTemplate
    {
    }

    public class GenerateMethodTemplate : GenerateCommonTemplate
    {
    }
}
