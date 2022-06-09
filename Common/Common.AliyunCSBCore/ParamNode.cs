using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.AliyunCSBCore
{
    public class ParamNode
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ParamNode() { }

        public ParamNode(string name, string value)
        {
            Name = name;
            Value= value;
        }

        public override string ToString()
        {
            return Name + "=" + Value;
        }

        public int CompareTo(ParamNode other)
        {
            var res=Name.CompareTo(other.Name);
            if (res == 0)
            {
                res=Value.CompareTo(other.Value);
            }
            return res;
        }
    }

    public static class ParamNodeExtensions
    {
        public static List<ParamNode> DictionaryToParamNodeList(this IDictionary<string, string> dict)
        {
            var list = new List<ParamNode>();
            foreach (var item in dict)
            {
                list.Add(new ParamNode(item.Key, item.Value));
            }
            return list;
        }

        //public static string JoinToString(this List<ParamNode> paramNodes)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (paramNodes.Any())
        //    {
        //        sb.Append(paramNodes.First().ToString());
        //    }
        //    for (int i = 1; i < paramNodes.Count(); i++)
        //    {
        //        sb.Append("&" + paramNodes[i].ToString());
        //    }
        //    return sb.ToString();
        //}
    }
}
