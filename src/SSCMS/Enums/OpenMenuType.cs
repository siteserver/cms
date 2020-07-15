using System;
using System.Linq;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;
using SSCMS.Utils;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(OpenMenuTypeConverter))]
    public enum OpenMenuType
    {
        [DataEnum(DisplayName = "点击事件（传回关键词）", Value = "click")]
        Click,

        [DataEnum(DisplayName = "访问网页（直接跳转）", Value = "view")]
        View,
        [DataEnum(DisplayName = "小程序（直接跳转）", Value = "miniprogram")]
        MiniProgram,

        [DataEnum(DisplayName = "弹出地理位置选择器", Value = "location_select")]
        Location_Select,

        [DataEnum(DisplayName = "弹出拍照或者相册发图", Value = "pic_photo_or_album")]
        Pic_Photo_Or_Album,

        [DataEnum(DisplayName = "弹出系统拍照发图", Value = "pic_sysphoto")]
        Pic_SysPhoto,

        [DataEnum(DisplayName = "弹出微信相册发图器", Value = "pic_weixin")]
        Pic_WeiXin,

        [DataEnum(DisplayName = "扫码推事件", Value = "scancode_push")]
        ScanCode_Push,

        [DataEnum(DisplayName = "扫码推事件且弹出“消息接收中”提示框", Value = "scancode_waitmsg")]
        ScanCode_WaitMsg,

        [DataEnum(DisplayName = "下发消息（除文本消息）", Value = "media_id")]
        Media_Id,

        [DataEnum(DisplayName = "跳转图文消息URL", Value = "view_limited")]
        View_Limited
    }

    class OpenMenuTypeConverter : JsonConverter<OpenMenuType>
    {
        public override OpenMenuType ReadJson(JsonReader reader, Type objectType, OpenMenuType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = reader.Value as string ?? reader.Value.ToString();
            var list = ListUtils.GetEnums<OpenMenuType>();
            return list.FirstOrDefault(x => x.GetValue() == token);
        }

        public override void WriteJson(JsonWriter writer, OpenMenuType value, JsonSerializer serializer)
        {
            writer.WriteValue(value.GetValue());
        }
    }
}
