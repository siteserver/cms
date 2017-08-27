using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IParseApi
    {
        void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes,
            out string no);

        string Parse(string innerXml, int publishmentSystemId, int channelId, int contentId);

        void Parse(StringBuilder builder, int publishmentSystemId, int channelId, int contentId);
    }
}
