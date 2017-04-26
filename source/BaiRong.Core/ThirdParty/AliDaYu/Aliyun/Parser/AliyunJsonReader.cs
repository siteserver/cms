using System;
using System.Collections;
using System.Collections.Generic;
using Top.Api.Parser;

namespace Aliyun.Api.Parser
{
    /// <summary>
    /// TOP JSON响应通用读取器。
    /// </summary>
    public class AliyunJsonReader : ITopReader
    {
        private IDictionary json;

        public AliyunJsonReader(IDictionary json)
        {
            this.json = json;
        }

        public bool HasReturnField(object name)
        {
            return json.Contains(name);
        }

        public object GetPrimitiveObject(object name)
        {
            return json[name];
        }

        public object GetReferenceObject(object name, Type type, DTopConvert convert)
        {
            IDictionary dict = json[name] as IDictionary;
            if (dict != null && dict.Count > 0)
            {
                return convert(new TopJsonReader(dict), type);
            }
            else
            {
                return null;
            }
        }

        public IList GetListObjects(string listName, string itemName, Type type, DTopConvert convert)
        {
            IList listObjs = null;

            IDictionary jsonMap = json[listName] as IDictionary;
            if (jsonMap != null && jsonMap.Count > 0)
            {
                IList jsonList = jsonMap[itemName] as IList;
                if (jsonList != null && jsonList.Count > 0)
                {
                    Type listType = typeof(List<>).MakeGenericType(new Type[] { type });
                    listObjs = Activator.CreateInstance(listType) as IList;
                    foreach (object item in jsonList)
                    {
                        if (typeof(IDictionary).IsAssignableFrom(item.GetType())) // object
                        {
                            IDictionary subMap = item as IDictionary;
                            object subObj = convert(new TopJsonReader(subMap), type);
                            if (subObj != null)
                            {
                                listObjs.Add(subObj);
                            }
                        }
                        else if (typeof(IList).IsAssignableFrom(item.GetType())) // list or array
                        {
                            // TODO not support yet
                        }
                        else // string, bool, long, double, null, other
                        {
                            listObjs.Add(item);
                        }
                    }
                }
            }

            return listObjs;
        }
    }
}
