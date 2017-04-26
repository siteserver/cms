using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.GovPublic
{
	public class GovPublicManager
	{
        public static void Initialize(PublishmentSystemInfo publishmentSystemInfo)
        {
            if (publishmentSystemInfo.Additional.GovPublicNodeId > 0)
            {
                if (!DataProvider.NodeDao.IsExists(publishmentSystemInfo.Additional.GovPublicNodeId))
                {
                    publishmentSystemInfo.Additional.GovPublicNodeId = 0;
                }
            }
            if (publishmentSystemInfo.Additional.GovPublicNodeId == 0)
            {
                var govPublicNodeId = DataProvider.NodeDao.GetNodeIdByContentModelType(publishmentSystemInfo.PublishmentSystemId, EContentModelType.GovPublic);
                if (govPublicNodeId == 0)
                {
                    govPublicNodeId = DataProvider.NodeDao.InsertNodeInfo(publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.PublishmentSystemId, "信息公开", string.Empty, EContentModelTypeUtils.GetValue(EContentModelType.GovPublic));
                }
                publishmentSystemInfo.Additional.GovPublicNodeId = govPublicNodeId;
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }

            GovPublicCategoryManager.Initialize(publishmentSystemInfo);

            if (DataProvider.GovPublicIdentifierRuleDao.GetCount(publishmentSystemInfo.PublishmentSystemId) == 0)
            {
                var ruleInfoArrayList = new ArrayList
                {
                    new GovPublicIdentifierRuleInfo(0, "机构分类代码", publishmentSystemInfo.PublishmentSystemId,
                        EGovPublicIdentifierType.Department, 5, "-", string.Empty, string.Empty, 0, 0, string.Empty),
                    new GovPublicIdentifierRuleInfo(0, "主题分类代码", publishmentSystemInfo.PublishmentSystemId,
                        EGovPublicIdentifierType.Channel, 5, "-", string.Empty, string.Empty, 0, 0, string.Empty),
                    new GovPublicIdentifierRuleInfo(0, "生效日期", publishmentSystemInfo.PublishmentSystemId,
                        EGovPublicIdentifierType.Attribute, 0, "-", "yyyy", GovPublicContentAttribute.EffectDate, 0, 0,
                        string.Empty),
                    new GovPublicIdentifierRuleInfo(0, "顺序号", publishmentSystemInfo.PublishmentSystemId,
                        EGovPublicIdentifierType.Sequence, 5, string.Empty, string.Empty, string.Empty, 0, 0,
                        string.Empty)
                };

                foreach (GovPublicIdentifierRuleInfo ruleInfo in ruleInfoArrayList)
                {
                    DataProvider.GovPublicIdentifierRuleDao.Insert(ruleInfo);
                }
            }
        }

        public static string GetPreviewIdentifier(int publishmentSystemId)
        {
            var builder = new StringBuilder();

            var ruleInfoArrayList = DataProvider.GovPublicIdentifierRuleDao.GetRuleInfoArrayList(publishmentSystemId);
            foreach (GovPublicIdentifierRuleInfo ruleInfo in ruleInfoArrayList)
            {
                if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Department)
                {
                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append("D123".PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append("D123").Append(ruleInfo.Suffix);
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Channel)
                {
                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append("C123".PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append("C123").Append(ruleInfo.Suffix);
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Attribute)
                {
                    if (ruleInfo.AttributeName == GovPublicContentAttribute.AbolitionDate || ruleInfo.AttributeName == GovPublicContentAttribute.EffectDate || ruleInfo.AttributeName == GovPublicContentAttribute.PublishDate || ruleInfo.AttributeName == ContentAttribute.AddDate)
                    {
                        if (ruleInfo.MinLength > 0)
                        {
                            builder.Append(DateTime.Now.ToString(ruleInfo.FormatString).PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                        }
                        else
                        {
                            builder.Append(DateTime.Now.ToString(ruleInfo.FormatString)).Append(ruleInfo.Suffix);
                        }
                    }
                    else
                    {
                        if (ruleInfo.MinLength > 0)
                        {
                            builder.Append("A123".PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                        }
                        else
                        {
                            builder.Append("A123").Append(ruleInfo.Suffix);
                        }
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Sequence)
                {
                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append("1".PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append("1").Append(ruleInfo.Suffix);
                    }
                }
            }

            return builder.ToString();
        }

        public static bool IsIdentifierChanged(int channelId, int departmentId, DateTime effectDate, GovPublicContentInfo contentInfo)
        {
            var isIdentifierChanged = false;
            var ruleInfoArrayList = DataProvider.GovPublicIdentifierRuleDao.GetRuleInfoArrayList(contentInfo.PublishmentSystemId);
            foreach (GovPublicIdentifierRuleInfo ruleInfo in ruleInfoArrayList)
            {
                if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Department)
                {
                    if (contentInfo.DepartmentId != departmentId)
                    {
                        isIdentifierChanged = true;
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Channel)
                {
                    if (contentInfo.NodeId != channelId)
                    {
                        isIdentifierChanged = true;
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Attribute)
                {
                    if (StringUtils.EqualsIgnoreCase(ruleInfo.AttributeName, GovPublicContentAttribute.EffectDate) && TranslateUtils.ToDateTime(contentInfo.GetExtendedAttribute(ruleInfo.AttributeName)) != effectDate)
                    {
                        isIdentifierChanged = true;
                    }
                }
            }
            return isIdentifierChanged;
        }

        public static string GetIdentifier(PublishmentSystemInfo publishmentSystemInfo, int channelId, int departmentId, GovPublicContentInfo contentInfo)
        {
            var builder = new StringBuilder();
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);

            var ruleInfoArrayList = DataProvider.GovPublicIdentifierRuleDao.GetRuleInfoArrayList(publishmentSystemInfo.PublishmentSystemId);
            foreach (GovPublicIdentifierRuleInfo ruleInfo in ruleInfoArrayList)
            {
                if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Department)
                {
                    var departmentCode = DepartmentManager.GetDepartmentCode(departmentId);
                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append(departmentCode.PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append(departmentCode).Append(ruleInfo.Suffix);
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Channel)
                {
                    var channelCode = DataProvider.GovPublicChannelDao.GetCode(nodeInfo.NodeId);
                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append(channelCode.PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append(channelCode).Append(ruleInfo.Suffix);
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Attribute)
                {
                    if (ruleInfo.AttributeName == GovPublicContentAttribute.AbolitionDate || ruleInfo.AttributeName == GovPublicContentAttribute.EffectDate || ruleInfo.AttributeName == GovPublicContentAttribute.PublishDate || ruleInfo.AttributeName == ContentAttribute.AddDate)
                    {
                        var dateTime = TranslateUtils.ToDateTime(contentInfo.GetExtendedAttribute(ruleInfo.AttributeName));
                        if (ruleInfo.MinLength > 0)
                        {
                            builder.Append(dateTime.ToString(ruleInfo.FormatString).PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                        }
                        else
                        {
                            builder.Append(dateTime.ToString(ruleInfo.FormatString)).Append(ruleInfo.Suffix);
                        }
                    }
                    else
                    {
                        var attributeValue = contentInfo.GetExtendedAttribute(ruleInfo.AttributeName);
                        if (ruleInfo.MinLength > 0)
                        {
                            builder.Append(attributeValue.PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                        }
                        else
                        {
                            builder.Append(attributeValue).Append(ruleInfo.Suffix);
                        }
                    }
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Sequence)
                {
                    var targetPublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                    var targetNodeId = 0;
                    if (ruleInfo.Additional.IsSequenceChannelZero)
                    {
                        targetNodeId = nodeInfo.NodeId;
                    }
                    var targetDepartmentId = 0;
                    if (ruleInfo.Additional.IsSequenceDepartmentZero)
                    {
                        targetDepartmentId = departmentId;
                    }
                    var targetAddYear = 0;
                    if (ruleInfo.Additional.IsSequenceYearZero)
                    {
                        targetAddYear = contentInfo.AddDate.Year;
                    }

                    var sequence = DataProvider.GovPublicIdentifierSeqDao.GetSequence(targetPublishmentSystemId, targetNodeId, targetDepartmentId, targetAddYear, ruleInfo.Sequence);

                    if (ruleInfo.MinLength > 0)
                    {
                        builder.Append(sequence.ToString().PadLeft(ruleInfo.MinLength, '0')).Append(ruleInfo.Suffix);
                    }
                    else
                    {
                        builder.Append(sequence.ToString()).Append(ruleInfo.Suffix);
                    }
                }
            }

            return builder.ToString();
        }

        public static List<int> GetFirstDepartmentIdList(PublishmentSystemInfo publishmentSystemInfo)
        {
            return string.IsNullOrEmpty(publishmentSystemInfo.Additional.GovPublicDepartmentIdCollection) ? BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(0) : BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByDepartmentIdCollection(publishmentSystemInfo.Additional.GovPublicDepartmentIdCollection);
        }

        public static List<int> GetAllDepartmentIdList(PublishmentSystemInfo publishmentSystemInfo)
        {
            var firstIdList = GetFirstDepartmentIdList(publishmentSystemInfo);
            return BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByFirstDepartmentIdList(firstIdList);
        }
	}
}
