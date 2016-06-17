using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Common
{
   public class DictionaryHelper
    {
       /// <summary>
       /// 遍历字典
       /// </summary>
       public void ForDictionary()
       {
           Dictionary<string, string> dic = null; ;
           foreach (var item in dic)
           {
               Console.Write(item.Key + item.Value);
           }
           //KeyValuePair<T,K>
           foreach (KeyValuePair<string, string> kv in dic)
           {
               Console.Write(kv.Key + kv.Value);
           }
           //通过键的集合取
           foreach (string key in dic.Keys)
           {
               Console.Write(key + dic[key]);
           }
           //直接取值
           foreach (string val in dic.Values)
           {
               Console.Write(val);
           }
           //非要采用for的方法也可
           List<string> test = new List<string>(dic.Keys);
           for (int i = 0; i < dic.Count; i++)
           {
               Console.Write(test[i] + dic[test[i]]);
           }
       }
    }
}
