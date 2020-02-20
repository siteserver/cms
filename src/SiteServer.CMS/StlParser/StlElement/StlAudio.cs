using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "播放音频", Description = "通过 stl:audio 标签在模板中显示并播放音频文件")]
    public class StlAudio
	{
        private StlAudio() { }
		public const string ElementName = "stl:audio";
        public const string EditorPlaceHolder = @"src=""../assets/editor-images/audio.png""";

        [StlAttribute(Title = "指定存储音频地址的内容字段，默认为VideoUrl")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "音频地址，优先级高于type属性")]
        public const string PlayUrl = nameof(PlayUrl);

        [StlAttribute(Title = "是否自动播放")]
        public const string IsAutoPlay = nameof(IsAutoPlay);

        [StlAttribute(Title = "是否预载入")]
        private const string IsPreload = nameof(IsPreload);

        [StlAttribute(Title = "是否循环播放")]
        private const string IsLoop = nameof(IsLoop);

	    public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
		{
            var type = ContentAttribute.VideoUrl;
            var playUrl = string.Empty;
            var isAutoPlay = false;
            var isPreLoad = true;
            var isLoop = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PlayUrl) || StringUtils.EqualsIgnoreCase(name, "src"))
                {
                    playUrl = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoPlay) || StringUtils.EqualsIgnoreCase(name, "play"))
                {
                    isAutoPlay = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsPreload))
                {
                    isPreLoad = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLoop) || StringUtils.EqualsIgnoreCase(name, "loop"))
                {
                    isLoop = TranslateUtils.ToBool(value, false);
                }
            }

            return await ParseImplAsync(pageInfo, contextInfo, type, playUrl, isAutoPlay, isPreLoad, isLoop);
		}

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, string type, string playUrl, bool isAutoPlay, bool isPreLoad, bool isLoop)
        {
            var contentId = contextInfo.ContentId;

            if (string.IsNullOrEmpty(playUrl))
            {
                if (contentId != 0)//获取内容视频
                {
                    var contentInfo = await contextInfo.GetContentAsync();
                    if (contentInfo != null)
                    {
                        playUrl = contentInfo.Get<string>(type);
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contentInfo.Get<string>(ContentAttribute.VideoUrl);
                        }
                        if (string.IsNullOrEmpty(playUrl))
                        {
                            playUrl = contentInfo.Get<string>(ContentAttribute.FileUrl);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(playUrl)) return string.Empty;

            playUrl = await PageUtility.ParseNavigationUrlAsync(pageInfo.Site, playUrl, pageInfo.IsLocal);

            // 如果是实体标签，则只返回数字
            if (contextInfo.IsStlEntity)
            {
                return playUrl;
            }
            else
            {
                await pageInfo.AddPageBodyCodeIfNotExistsAsync(PageInfo.Const.Jquery);
                await pageInfo.AddPageBodyCodeIfNotExistsAsync(PageInfo.Const.JsAcMediaElement);

                return $@"
<audio class=""mejs__player"" src=""{playUrl}"" {(isAutoPlay ? "autoplay" : string.Empty)} {(isPreLoad ? string.Empty : @"preload=""none""")} {(isLoop ? "loop" : string.Empty)}>
    <object width=""460"" height=""40"" type=""application/x-shockwave-flash"" data=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.MediaElement.Swf)}"">
        <param name=""movie"" value=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.MediaElement.Swf)}"" />
        <param name=""flashvars"" value=""controls=true&file={playUrl}"" />
    </object>
</audio>
";
            }
        }
    }
}
