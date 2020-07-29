using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentScheduler;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    ////群发媒体文件时传入mediaId,群发文本消息时传入content,群发卡券时传入cardId
    public partial class OpenManager
    {
        public async Task SendPreviewAsync(string token, MaterialType materialType, string value, string wxName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(wxName)) return;
            
            await GroupMessageApi.SendGroupMessagePreviewAsync(token, GetGroupMessageType(materialType), value,
                null, StringUtils.Trim(wxName));
        }

        public async Task SendAsync(string token, MaterialType materialType, string value, bool isToAll, string tagId, bool isTiming, DateTime runOnceAt)
        {
            if (isTiming)
            {
                JobManager.AddJob(async () =>
                {
                    await GroupMessageApi.SendGroupMessageByTagIdAsync(token, tagId, value,
                        GetGroupMessageType(materialType), isToAll);
                }, s => s.ToRunOnceAt(runOnceAt));
            }
            else
            {
                await GroupMessageApi.SendGroupMessageByTagIdAsync(token, tagId, value,
                    GetGroupMessageType(materialType), isToAll);
            }
        }
    }
}
