using System;
using Taobao.Top.Link.Endpoints;

namespace Top.Tmc
{
    /// <summary>消息服务客户端标识</summary>
    public class TmcClientIdentity : Identity
    {
        /// <summary>获取appKey
        /// </summary>
        public string AppKey { get; private set; }
        /// <summary>获取groupName
        /// </summary>
        public string GroupName { get; private set; }

        public TmcClientIdentity(string appKey, string groupName)
        {
            this.AppKey = appKey;
            this.GroupName = groupName;
        }

        public bool Equals(Identity id)
        {
            var tmcId = id as TmcClientIdentity;
            return tmcId != null
                && this.AppKey == tmcId.AppKey
                && this.GroupName == tmcId.GroupName;
        }

        public Identity Parse(object data)
        {
            throw new NotImplementedException();
        }

        public void Render(object to)
        {

        }

        public override string ToString()
        {
            return this.AppKey + "-" + this.GroupName;
        }

        public override int GetHashCode()
        {
            return (this.AppKey + this.GroupName).GetHashCode();
        }
    }

    /// <summary>TMC服务端标识
    /// </summary>
    public class TmcServerIdentity : Identity
    {
        public bool Equals(Identity id)
        {
            return id is TmcServerIdentity;
        }

        public Identity Parse(object data)
        {
            throw new NotImplementedException();
        }

        public void Render(object to)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "tmc-server";
        }
    }
}