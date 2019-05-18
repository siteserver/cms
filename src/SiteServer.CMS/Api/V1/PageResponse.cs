using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.V1
{
    public class PageResponse
    {
        private int? _count;
        private int? _top;
        private int? _skip;
        private string _rowUrl;

        public PageResponse(object value, int top, int skip, string rowUrl)
        {
            Value = value;
            _top = top;
            _skip = skip;
            _rowUrl = rowUrl;
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
                if (_count != null && _top != null && _skip != null && _rowUrl != null)
                {
                    var url = PageUtils.RemoveQueryString(_rowUrl, new List<string> {"top", "skip"});
                    var pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((int)_count / _top)));
                    var pageIndex = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(_skip / _top)));

                    if (_skip > 0)
                    {
                        First = PageUtils.AddQueryString(url,
                            new NameValueCollection
                            {
                                {"top", _top.ToString()},
                                {"skip", "0"}
                            });

                        Prev = PageUtils.AddQueryString(url,
                            new NameValueCollection
                            {
                                {"top", _top.ToString()},
                                {"skip", ((pageIndex - 1) * _top).ToString()}
                            });
                    }

                    if (_top + _skip < _count)
                    {
                        Next =
                            PageUtils.AddQueryString(url,
                                new NameValueCollection
                                {
                                    {"top", _top.ToString()},
                                    {"skip", ((pageIndex + 1) * _top).ToString()}
                                });

                        Last =
                            PageUtils.AddQueryString(url,
                                new NameValueCollection
                                {
                                    {"top", _top.ToString()},
                                    {"skip", ((pages - 1) * _top).ToString()}
                                });
                    }
                }
            }
        }
    }
}