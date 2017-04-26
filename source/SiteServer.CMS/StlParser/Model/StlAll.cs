using System.Collections;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlElement.Inner;
using SiteServer.CMS.StlParser.StlElement.WCM;
using SiteServer.CMS.StlParser.StlEntity;

namespace SiteServer.CMS.StlParser.Model
{
    public class StlAll
    {
        public class StlElements
        {
            private StlElements()
            {
            }

            public static IDictionary GetElementsNameDictionary()
            {
                return new SortedList
                {
                    {StlA.ElementName, "获取链接"},
                    {StlAction.ElementName, "执行动作"},
                    {StlAd.ElementName, "固定广告"},
                    {StlAnalysis.ElementName, "显示浏览量"},
                    {StlAudio.ElementName, "播放音频"},
                    {StlChannel.ElementName, "栏目值"},
                    {StlChannels.ElementName, "栏目列表"},
                    {StlComment.ElementName, "评论值"},
                    {StlCommentInput.ElementName, "提交评论"},
                    {StlComments.ElementName, "评论列表"},
                    {StlContainer.ElementName, "容器"},
                    {StlContent.ElementName, "内容值"},
                    {StlContents.ElementName, "内容列表"},
                    {StlCount.ElementName, "显示数值"},
                    {StlDigg.ElementName, "掘客"},
                    {StlDynamic.ElementName, "动态加载"},
                    {StlEach.ElementName, "项循环"},
                    {StlFile.ElementName, "文件下载链接"},
                    {StlFlash.ElementName, "显示Flash"},
                    {StlFocusViewer.ElementName, "滚动焦点图"},
                    {StlIf.ElementName, "条件判断"},
                    {StlImage.ElementName, "显示图片"},
                    {StlInclude.ElementName, "包含文件"},
                    {StlInput.ElementName, "提交表单"},
                    {StlInputContent.ElementName, "提交表单值"},
                    {StlInputContents.ElementName, "提交表单列表"},
                    {StlItem.ElementName, "列表项"},
                    {StlLayout.ElementName, "布局"},
                    {StlLocation.ElementName, "当前位置"},
                    {StlMarquee.ElementName, "无间隔滚动"},
                    {StlMenu.ElementName, "下拉菜单"},
                    {StlNavigation.ElementName, "显示导航"},
                    {StlPageChannels.ElementName, "翻页栏目列表"},
                    {StlPageComments.ElementName, "翻页评论列表"},
                    {StlPageContents.ElementName, "翻页内容列表"},
                    {StlPageInputContents.ElementName, "翻页提交表单列表"},
                    {StlPageItem.ElementName, "翻页项"},
                    {StlPageItems.ElementName, "翻页项容器"},
                    {StlPageSqlContents.ElementName, "翻页数据库数据列表"},
                    {StlPhoto.ElementName, "内容图片"},
                    {StlPlayer.ElementName, "播放视频"},
                    {StlPrinter.ElementName, "打印"},
                    {StlQueryString.ElementName, "SQL查询语句"},
                    {StlResume.ElementName, "提交简历"},
                    {StlRss.ElementName, "Rss订阅"},
                    {StlSearchOutput.ElementName, "搜索结果"},
                    {StlSelect.ElementName, "下拉列表"},
                    {StlSite.ElementName, "站点值"},
                    {StlSites.ElementName, "站点列表"},
                    {StlSlide.ElementName, "图片幻灯片"},
                    {StlSqlContent.ElementName, "数据库数据值"},
                    {StlSqlContents.ElementName, "数据库数据列表"},
                    {StlStar.ElementName, "评分"},
                    {StlTabs.ElementName, "页签切换"},
                    {StlTags.ElementName, "标签"},
                    {StlTree.ElementName, "树状导航"},
                    {StlValue.ElementName, "获取值"},
                    {StlVideo.ElementName, "播放视频"},
                    {StlVote.ElementName, "投票"},
                    {StlZoom.ElementName, "文字缩放"},
                    {StlGovInteractApply.ElementName, "互动交流提交"},
                    {StlGovInteractQuery.ElementName, "互动交流查询"},
                    {StlGovPublicApply.ElementName, "依申请公开提交"},
                    {StlGovPublicQuery.ElementName, "依申请公开查询"}
                };
            }

            public static IDictionary GetElementsAttributesDictionary()
            {
                return new SortedList
                {
                    {StlA.ElementName, StlA.AttributeList},
                    {StlAction.ElementName, StlAction.AttributeList},
                    {StlAd.ElementName, StlAd.AttributeList},
                    {StlAnalysis.ElementName, StlAnalysis.AttributeList},
                    {StlAudio.ElementName, StlAudio.AttributeList},
                    {StlChannel.ElementName, StlChannel.AttributeList},
                    {StlChannels.ElementName, StlChannels.AttributeList},
                    {StlComment.ElementName, StlComment.AttributeList},
                    {StlCommentInput.ElementName, StlCommentInput.AttributeList},
                    {StlComments.ElementName, StlComments.AttributeList},
                    {StlContainer.ElementName, StlContainer.AttributeList},
                    {StlContent.ElementName, StlContent.AttributeList},
                    {StlContents.ElementName, StlContents.AttributeList},
                    {StlCount.ElementName, StlCount.AttributeList},
                    {StlDigg.ElementName, StlDigg.AttributeList},
                    {StlDynamic.ElementName, StlDynamic.AttributeList},
                    {StlEach.ElementName, StlEach.AttributeList},
                    {StlFile.ElementName, StlFile.AttributeList},
                    {StlFlash.ElementName, StlFlash.AttributeList},
                    {StlFocusViewer.ElementName, StlFocusViewer.AttributeList},
                    {StlIf.ElementName, StlIf.AttributeList},
                    {StlImage.ElementName, StlImage.AttributeList},
                    {StlInclude.ElementName, StlInclude.AttributeList},
                    {StlInput.ElementName, StlInput.AttributeList},
                    {StlInputContent.ElementName, StlInputContent.AttributeList},
                    {StlInputContents.ElementName, StlInputContents.AttributeList},
                    {StlItem.ElementName, StlItem.AttributeList},
                    {StlLayout.ElementName, StlLayout.AttributeList},
                    {StlLocation.ElementName, StlLocation.AttributeList},
                    {StlMarquee.ElementName, StlMarquee.AttributeList},
                    {StlMenu.ElementName, StlMenu.AttributeList},
                    {StlNavigation.ElementName, StlNavigation.AttributeList},
                    {StlPageChannels.ElementName, StlPageChannels.AttributeList},
                    {StlPageComments.ElementName, StlPageComments.AttributeList},
                    {StlPageContents.ElementName, StlPageContents.AttributeList},
                    {StlPageItem.ElementName, StlPageItem.AttributeList},
                    {StlPageItems.ElementName, StlPageItems.AttributeList},
                    {StlPageSqlContents.ElementName, StlPageSqlContents.AttributeList},
                    {StlPhoto.ElementName, StlPhoto.AttributeList},
                    {StlPlayer.ElementName, StlPlayer.AttributeList},
                    {StlPrinter.ElementName, StlPrinter.AttributeList},
                    {StlResume.ElementName, StlResume.AttributeList},
                    {StlRss.ElementName, StlRss.AttributeList},
                    {StlSearchOutput.ElementName, StlSearchOutput.AttributeList},
                    {StlSelect.ElementName, StlSelect.AttributeList},
                    {StlSite.ElementName, StlSite.AttributeList},
                    {StlSites.ElementName, StlSites.AttributeList},
                    {StlSlide.ElementName, StlSlide.AttributeList},
                    {StlSqlContent.ElementName, StlSqlContent.AttributeList},
                    {StlSqlContents.ElementName, StlSqlContents.AttributeList},
                    {StlStar.ElementName, StlStar.AttributeList},
                    {StlTabs.ElementName, StlTabs.AttributeList},
                    {StlTags.ElementName, StlTags.AttributeList},
                    {StlTree.ElementName, StlTree.AttributeList},
                    {StlValue.ElementName, StlValue.AttributeList},
                    {StlVideo.ElementName, StlVideo.AttributeList},
                    {StlVote.ElementName, StlVote.AttributeList},
                    {StlZoom.ElementName, StlZoom.AttributeList},
                    {StlGovInteractApply.ElementName, StlGovInteractApply.AttributeList},
                    {StlGovInteractQuery.ElementName, StlGovInteractQuery.AttributeList},
                    {StlGovPublicApply.ElementName, StlGovPublicApply.AttributeList},
                    {StlGovPublicQuery.ElementName, StlGovPublicQuery.AttributeList}
                };
            }
        }

        public class StlEntities
        {
            private StlEntities()
            {
            }

            public static IDictionary GetEntitiesNameDictionary()
            {
                return new SortedList
                {
                    {StlEntity.StlEntities.EntityName, "通用实体"},
                    {StlChannelEntities.EntityName, "栏目实体"},
                    {StlContentEntities.EntityName, "内容实体"},
                    {StlCommentEntities.EntityName, "评论实体"},
                    {StlNavigationEntities.EntityName, "导航实体"},
                    {StlPhotoEntities.EntityName, "图片实体"},
                    {StlRequestEntities.EntityName, "请求实体"},
                    {StlSqlEntities.EntityName, "数据库实体"},
                    {StlUserEntities.EntityName, "用户实体"}
                };
            }

            public static IDictionary GetEntitiesAttributesDictionary()
            {
                return new SortedList
                {
                    {StlEntity.StlEntities.EntityName, StlEntity.StlEntities.AttributeList},
                    {StlChannelEntities.EntityName, StlChannelEntities.AttributeList},
                    {StlContentEntities.EntityName, StlContentEntities.AttributeList},
                    {StlCommentEntities.EntityName, StlCommentEntities.AttributeList},
                    {StlNavigationEntities.EntityName, StlNavigationEntities.AttributeList},
                    {StlPhotoEntities.EntityName, StlPhotoEntities.AttributeList},
                    {StlSqlEntities.EntityName, StlSqlEntities.AttributeList},
                    {StlUserEntities.EntityName, StlUserEntities.AttributeList}
                };
            }
        }
    }
}
