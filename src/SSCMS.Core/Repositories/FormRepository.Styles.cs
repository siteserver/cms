using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class FormRepository
    {
        public List<int> GetRelatedIdentities(int formId)
        {
            return new List<int> { formId };
        }

        public async Task<List<TableStyle>> GetTableStylesAsync(int formId)
        {
            return await _tableStyleRepository.GetTableStylesAsync(FormDataRepository.TABLE_NAME, GetRelatedIdentities(formId), MetadataAttributes.Value);
        }

        public async Task DeleteTableStyleAsync(int formId, string attributeName)
        {
            await _tableStyleRepository.DeleteAsync(FormDataRepository.TABLE_NAME, formId, attributeName);
        }

        private static readonly Lazy<List<string>> MetadataAttributes = new Lazy<List<string>>(() => new List<string>
        {
            nameof(FormData.SiteId),
            nameof(FormData.ChannelId),
            nameof(FormData.ContentId),
            nameof(FormData.FormId),
            nameof(FormData.IsReplied),
            nameof(FormData.ReplyDate),
            nameof(FormData.ReplyContent)
        });

        public async Task CreateDefaultStylesAsync(Form form)
        {
            var relatedIdentities = GetRelatedIdentities(form.Id);

            await _tableStyleRepository.InsertAsync(relatedIdentities, new TableStyle
            {
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentity = relatedIdentities[0],
                AttributeName = "Name",
                DisplayName = "姓名",
                HelpText = "请输入您的姓名",
                InputType = InputType.Text,
                Rules = new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Required,
                        Message = ValidateType.Required.GetDisplayName()
                    }
                }
            });

            await _tableStyleRepository.InsertAsync(relatedIdentities, new TableStyle
            {
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentity = relatedIdentities[0],
                AttributeName = "Mobile",
                DisplayName = "手机",
                HelpText = "请输入您的手机号码",
                InputType = InputType.Text,
                Rules = new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Mobile,
                        Message = ValidateType.Mobile.GetDisplayName()
                    }
                }
            });

            await _tableStyleRepository.InsertAsync(relatedIdentities, new TableStyle
            {
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentity = relatedIdentities[0],
                AttributeName = "Email",
                DisplayName = "邮箱",
                HelpText = "请输入您的电子邮箱",
                InputType = InputType.Text,
                Rules = new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Email,
                        Message = ValidateType.Email.GetDisplayName()
                    }
                }
            });

            await _tableStyleRepository.InsertAsync(relatedIdentities, new TableStyle
            {
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentity = relatedIdentities[0],
                AttributeName = "Content",
                DisplayName = "留言",
                HelpText = "请输入您的留言",
                InputType = InputType.TextArea,
                Rules = new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Required,
                        Message = ValidateType.Required.GetDisplayName()
                    }
                }
            });
        }
    }
}
