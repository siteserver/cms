using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EContentModelType
	{
        Content,
        Photo,
        GovPublic,
        GovInteract,
        Vote,
        Job,
        UserDefined
	}

    public class EContentModelTypeUtils
	{
		public static string GetValue(EContentModelType type)
		{
			if (type == EContentModelType.Content)
			{
                return "Content";
			}
            else if (type == EContentModelType.Photo)
            {
                return "Photo";
            }
            else if (type == EContentModelType.GovPublic)
            {
                return "GovPublic";
            }
            else if (type == EContentModelType.GovInteract)
            {
                return "GovInteract";
            }
            else if (type == EContentModelType.Vote)
            {
                return "Vote";
            }
            else if (type == EContentModelType.Job)
            {
                return "Job";
            }
            else if (type == EContentModelType.UserDefined)
            {
                return "UserDefined";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EContentModelType type)
		{
            if (type == EContentModelType.Content)
			{
                return "内容";
			}
            else if (type == EContentModelType.Photo)
            {
                return "图片";
            }
            else if (type == EContentModelType.GovPublic)
            {
                return "信息公开";
            }
            else if (type == EContentModelType.GovInteract)
            {
                return "互动交流";
            }
            else if (type == EContentModelType.Vote)
            {
                return "投票";
            }
            else if (type == EContentModelType.Job)
            {
                return "招聘";
            }
            else if (type == EContentModelType.UserDefined)
            {
                return "自定义";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EContentModelType GetEnumType(string typeStr)
		{
			var retval = EContentModelType.Content;

            if (Equals(EContentModelType.Content, typeStr))
			{
                retval = EContentModelType.Content;
			}
            else if (Equals(EContentModelType.Photo, typeStr))
            {
                retval = EContentModelType.Photo;
            }
            else if (Equals(EContentModelType.GovPublic, typeStr))
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
            else if (Equals(EContentModelType.UserDefined, typeStr))
            {
                retval = EContentModelType.UserDefined;
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
                listControl.Items.Add(GetListItem(EContentModelType.Photo, false));
                listControl.Items.Add(GetListItem(EContentModelType.GovPublic, false));
                listControl.Items.Add(GetListItem(EContentModelType.GovInteract, false));
                listControl.Items.Add(GetListItem(EContentModelType.Vote, false));
                listControl.Items.Add(GetListItem(EContentModelType.Job, false));
            }
        }

        public static ContentModelInfo GetContentModelInfo(EPublishmentSystemType publishmentSystemType, int siteID, string tableName, EContentModelType modelType)
        {
            var modelInfo = new ContentModelInfo();
            modelInfo.ModelId = GetValue(modelType);
            modelInfo.ModelName = GetText(modelType);
            modelInfo.SiteId = siteID;
            modelInfo.IsSystem = true;
            modelInfo.TableName = tableName;
            if (modelType == EContentModelType.Content)
            {
                modelInfo.TableType = EAuxiliaryTableType.BackgroundContent;
            }
            else if (modelType == EContentModelType.Photo)
            {
                modelInfo.TableType = EAuxiliaryTableType.BackgroundContent;
                modelInfo.IconUrl = "photo.gif";
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
            }
            return modelInfo;
        }

        public static EContentModelType GetEnumTypeByPublishmentSystemType(EPublishmentSystemType publishmentSystemType)
        {
            return EContentModelType.Content;
        }

        public static bool IsPhoto(string contentModelID)
        {
            var modelType = GetEnumType(contentModelID);

            if (modelType == EContentModelType.Photo)
            {
                return true;
            }
            return false;
        }
	}
}
