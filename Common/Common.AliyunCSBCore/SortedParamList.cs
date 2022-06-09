using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.AliyunCSBCore
{
    public class SortedParamList : List<ParamNode>
    {
        private static readonly long serialVersionUID = 1L;

        public bool Add(ParamNode paramNode)
        {
            if (this.Count() == 0)
            {
                base.Add(paramNode);
                return true;
            }
            else
            {
                return binaryAdd(paramNode, 0, this.Count() - 1);
            }
        }

        public bool binaryAdd(ParamNode paramNode,int start ,int end)
        {
            if (start > end)
            {
                Insert(start, paramNode);
                return true;
            }
            else if (start == end)
            {
                if (paramNode.CompareTo(this[start]) <= 0)
                {
                    Insert(start, paramNode);
                }
                else
                {
                    Insert(start + 1, paramNode);
                }
                return true;
            }
            else
            {
                var mid = (start + end) / 2;
                var cmp=paramNode.CompareTo(this[mid]);
                if (cmp < 0)
                {
                    return binaryAdd(paramNode, start, mid - 1);
                }
                else if (cmp > 0)
                {
                    return binaryAdd(paramNode, mid + 1, end);
                }
                else
                {
                    Insert(mid, paramNode);
                    return true;
                }
            }
        }

        public bool AddAll(List<ParamNode> paramNodes)
        {
            foreach (var paramNode in paramNodes)
            {
                Add(paramNode);
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Any())
            {
                sb.Append(this.First().ToString());
            }
            for (int i = 1; i < this.Count(); i++)
            {
                sb.Append("&" + this[i].ToString());
            }
            return sb.ToString();
        }
    }
}
