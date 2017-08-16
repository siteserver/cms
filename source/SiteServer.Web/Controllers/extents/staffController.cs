using BaiRong.Core;
using SiteServer.CMS.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SiteServer.API.Controllers.extents
{
    [RoutePrefix("api/staff")]
    public class staffController : ApiController
    {
        [HttpGet, Route("search")]
        public IHttpActionResult searchstaff()
        {
            int conditionNumber = 0;

            try
            {
                var body = new RequestBody();

                var name = body.GetQueryString("name");
                var cardno = body.GetQueryString("cardno");
                var staffid = body.GetQueryString("staffid");
                var sql = "select * from staffinfo where 1=1 ";



                if (!string.IsNullOrEmpty(name))
                {
                    conditionNumber++;
                    sql += " and name='" + name + "'";
                }
                if (!string.IsNullOrEmpty(cardno))
                {
                    conditionNumber++;
                    sql += " and cardno='" + cardno + "'";
                }
                if (!string.IsNullOrEmpty(staffid))
                {
                    conditionNumber++;
                    sql += " and staffid='" + staffid + "'";
                }
                if (conditionNumber < 2)
                {
                    return BadRequest();
                }

                var reader = WebConfigUtils.Helper.ExecuteReader(WebConfigUtils.ConnectionString, CommandType.Text, sql);

                if (reader.Read())
                {
                    var result = new
                    {
                        staffid = reader["staffid"].ToString(),
                        name = reader["name"].ToString(),
                        photo = reader["photo"].ToString(),
                        department = reader["department"].ToString(),
                        post = reader["post"].ToString(),
                        Tel = reader["Tel"].ToString(),
                        phone = reader["phone"].ToString(),
                        email = reader["email"].ToString()
                    };
                    reader.Close();
                    return Ok(result);
                }
                else
                {
                    reader.Close();
                    return Ok();
                }


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
