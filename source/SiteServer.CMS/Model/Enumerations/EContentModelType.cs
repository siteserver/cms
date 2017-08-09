using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum EContentModelType
	{
        Content,
        GovPublic,
        GovInteract,
        Vote,
        Job
    }

    public class EContentModelTypeUtils
	{
		public static string GetValue(EContentModelType type)
		{
		    if (type == EContentModelType.GovPublic)
		    {
		        return "GovPublic";
		    }
		    if (type == EContentModelType.GovInteract)
		    {
		        return "GovInteract";
		    }
		    if (type == EContentModelType.Vote)
		    {
		        return "Vote";
		    }
		    if (type == EContentModelType.Job)
		    {
		        return "Job";
		    }
            return "Content";
        }

		public static string GetText(EContentModelType type)
		{
		    if (type == EContentModelType.GovPublic)
		    {
		        return "信息公开";
		    }
		    if (type == EContentModelType.GovInteract)
		    {
		        return "互动交流";
		    }
		    if (type == EContentModelType.Vote)
		    {
		        return "投票";
		    }
		    if (type == EContentModelType.Job)
		    {
		        return "招聘";
		    }
            return "内容";
        }

		public static EContentModelType GetEnumType(string typeStr)
		{
			var retval = EContentModelType.Content;

            if (Equals(EContentModelType.GovPublic, typeStr))
            {
                retval = EContentModelType.GovPublic;
            }
            else if (Equals(EContentModelType.GovInteract, typeStr))
            {
                retval = EContentModelType.GovInteract;
            }
            else if (Equals(EContentModelType.Vote, typeStr))
            {
                retval = EContentModelType.Vote;
            }
            else if (Equals(EContentModelType.Job, typeStr))
            {
                retval = EContentModelType.Job;
            }

			return retval;
		}

		public static bool Equals(EContentModelType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EContentModelType type)
		{
			return Equals(type, typeStr);
		}

		public static ListItem GetListItem(EContentModelType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static void AddListItemsForContentCheck(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EContentModelType.Content, false));
                listControl.Items.Add(GetListItem(EContentModelType.GovPublic, false));
                listControl.Items.Add(GetListItem(EContentModelType.GovInteract, false));
                listControl.Items.Add(GetListItem(EContentModelType.Vote, false));
                listControl.Items.Add(GetListItem(EContentModelType.Job, false));
            }
        }

        public static ContentModelInfo GetContentModelInfo(string tableName, EContentModelType modelType)
        {
            var modelInfo = new ContentModelInfo
            {
                ModelId = GetValue(modelType),
                ModelName = GetText(modelType),
                TableName = tableName
            };
            if (modelType == EContentModelType.Content)
            {
                modelInfo.TableType = EAuxiliaryTableType.BackgroundContent;
            }
            else if (modelType == EContentModelType.GovPublic)
            {
                modelInfo.TableType = EAuxiliaryTableType.GovPublicContent;
                modelInfo.IconUrl = "govpublic.gif";
            }
            else if (modelType == EContentModelType.GovInteract)
            {
                modelInfo.TableType = EAuxiliaryTableType.GovInteractContent;
                modelInfo.IconUrl = "govinteract.gif";
            }
            else if (modelType == EContentModelType.Vote)
            {
                modelInfo.TableType = EAuxiliaryTableType.VoteContent;
                modelInfo.IconUrl = "vote.gif";
            }
            else if (modelType == EContentModelType.Job)
            {
                modelInfo.TableType = EAuxiliaryTableType.JobContent;
                modelInfo.IconUrl = "job.gif";
                modelInfo.Links = new List<PluginContentLink>
                {
                    new PluginContentLink
                    {
                        Href = "../plugins/pageResumeContent.aspx",
                        Text = "简历"
                    }
                };
            }
            return modelInfo;
        }
	}
}
