using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;

namespace SiteServer.API.Controllers.extents
{
    [RoutePrefix("api/ContentExt")]
    public class ContentExtController : ApiController
    {
        [HttpGet, Route("getListByNodeId")]
        public IHttpActionResult getListByNodeId(int NodeId, int totalnum = 0, string Orderby = "", bool includechild = false)
        {
            try
            {
                if (NodeId == 0) return BadRequest();
                using (IDbConnection conn = SqlUtils.GetIDbConnection())
                {
                    var sql = "select a.ID,b.NodeId,b.NodeName,a.AddUserName,a.AddDate,a.Tags,a.Title,a.SubTitle,a.ImageUrl,a.videoUrl,a.FileUrl,a.LinkUrl,a.Summary,a.Author,a.Source from model_content a inner join siteserver_node b on a.NodeID=b.NodeId where b.ContentModelId='Content' and b.nodeid=" + NodeId;
                    if (includechild) sql += " or b.ParentId=" + NodeId;
                    if (!string.IsNullOrEmpty(Orderby)) sql += " order by " + Orderby + " desc";
                    if (totalnum != 0) sql += " limit " + totalnum;
                    var data = conn.Query<ContentInfo>(sql);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet, Route("getListChannelGroup")]
        public IHttpActionResult getListChannelGroup(string groupname, int totalnum = 0, string Orderby = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupname)) return BadRequest();
                using (IDbConnection conn = SqlUtils.GetIDbConnection())
                {
                    var sql = "select a.ID,b.NodeId,b.NodeName,a.AddUserName,a.AddDate,a.Tags,a.Title,a.SubTitle,a.ImageUrl,a.videoUrl,a.FileUrl,a.LinkUrl,a.Summary,a.Author,a.Source from model_content a inner join siteserver_node b on a.NodeID=b.NodeId where b.ContentModelId='Content' and b.NodeGroupNameCollection like '%" + groupname + "%'";
                    if (!string.IsNullOrEmpty(Orderby)) sql += " order by " + Orderby + " desc";
                    if (totalnum != 0) sql += " limit " + totalnum;
                    var data = conn.Query<ContentInfo>(sql);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



    }
    public class ContentInfo
    {
        public int ID { get; set; }

        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public string AddUserName { get; set; }
        public DateTime AddDate { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ImageUrl { get; set; }
        public string videoUrl { get; set; }
        public string FileUrl { get; set; }
        public string LinkUrl { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public string Source { get; set; }

    }
}
