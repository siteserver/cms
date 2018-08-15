using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.V1
{
    public class OResponse
    {
        private ORequest _request;
        private int? _count;

        public OResponse(object value)
        {
            Value = value;
        }

        public OResponse(ORequest request, object value)
        {
            _request = request;
            Value = value;
        }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "first", NullValueHandling = NullValueHandling.Ignore)]
        public string First { get; private set; }

        [JsonProperty(PropertyName = "prev", NullValueHandling = NullValueHandling.Ignore)]
        public string Prev { get; private set; }

        [JsonProperty(PropertyName = "next", NullValueHandling = NullValueHandling.Ignore)]
        public string Next { get; private set; }

        [JsonProperty(PropertyName = "last", NullValueHandling = NullValueHandling.Ignore)]
        public string Last { get; private set; }

        

        [JsonProperty(PropertyName = "count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count
        {
            get { return _count; }
            set
            {
                _count = value;
                if (_request != null && _count != null)
                {
                    var url = PageUtils.RemoveQueryString(_request.RawUrl, new List<string> {"top", "skip"});
                    var pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((int)_count / _request.Top)));
                    var pageIndex = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_request.Skip / _request.Top)));

                    if (_request.Skip > 0)
                    {
                        First = PageUtils.AddQueryString(url,
                            new NameValueCollection
                            {
                                {"top", _request.Top.ToString()},
                                {"skip", "0"}
                            });

                        Prev = PageUtils.AddQueryString(url,
                            new NameValueCollection
                            {
                                {"top", _request.Top.ToString()},
                                {"skip", ((pageIndex - 1) * _request.Top).ToString()}
                            });
                    }

                    if (_request.Top + _request.Skip < _count)
                    {
                        Next =
                            PageUtils.AddQueryString(url,
                                new NameValueCollection
                                {
                                    {"top", _request.Top.ToString()},
                                    {"skip", ((pageIndex + 1) * _request.Top).ToString()}
                                });

                        Last =
                            PageUtils.AddQueryString(url,
                                new NameValueCollection
                                {
                                    {"top", _request.Top.ToString()},
                                    {"skip", ((pages - 1) * _request.Top).ToString()}
                                });
                    }
                }
            }
        }
    }
}