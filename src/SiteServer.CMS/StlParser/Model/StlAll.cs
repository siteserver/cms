using System;
using SiteServer.CMS.StlParser.StlElement;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.Model
{
    public static class StlAll
    {
        public static SortedList<string, Type> Elements => new SortedList<string, Type>
        {
            {
                StlA.ElementName,
                typeof(StlA)
            },
            {
                StlAction.ElementName,
                typeof(StlAction)
            },
            {
                StlAudio.ElementName,
                typeof(StlAudio)
            },
            {
                StlChannel.ElementName,
                typeof(StlChannel)
            },
            {
                StlChannels.ElementName,
                typeof(StlChannels)
            },
            {
                StlContainer.ElementName,
                typeof(StlContainer)
            },
            {
                StlContent.ElementName,
                typeof(StlContent)
            },
            {
                StlContents.ElementName,
                typeof(StlContents)
            },
            {
                StlCount.ElementName,
                typeof(StlCount)
            },
            {
                StlDynamic.ElementName,
                typeof(StlDynamic)
            },
            {
                StlEach.ElementName,
                typeof(StlEach)
            },
            {
                StlFile.ElementName,
                typeof(StlFile)
            },
            {
                StlFlash.ElementName,
                typeof(StlFlash)
            },
            {
                StlFocusViewer.ElementName,
                typeof(StlFocusViewer)
            },
            {
                StlIf.ElementName,
                typeof(StlIf)
            },
            {
                StlImage.ElementName,
                typeof(StlImage)
            },
            {
                StlInclude.ElementName,
                typeof(StlInclude)
            },
            {
                StlItemTemplate.ElementName,
                typeof(StlItemTemplate)
            },
            {
                StlLoading.ElementName,
                typeof(StlLoading)
            },
            {
                StlLocation.ElementName,
                typeof(StlLocation)
            },
            {
                StlMarquee.ElementName,
                typeof(StlMarquee)
            },
            {
                StlNavigation.ElementName,
                typeof(StlNavigation)
            },
            {
                StlNo.ElementName,
                typeof(StlNo)
            },
            {
                StlPageChannels.ElementName,
                typeof(StlPageChannels)
            },
            {
                StlPageContents.ElementName,
                typeof(StlPageContents)
            },
            {
                StlPageItem.ElementName,
                typeof(StlPageItem)
            },
            {
                StlPageItems.ElementName,
                typeof(StlPageItems)
            },
            {
                StlPageSqlContents.ElementName,
                typeof(StlPageSqlContents)
            },
            {
                StlPlayer.ElementName,
                typeof(StlPlayer)
            },
            {
                StlPrinter.ElementName,
                typeof(StlPrinter)
            },
            {
                StlQueryString.ElementName,
                typeof(StlQueryString)
            },
            {
                StlRss.ElementName,
                typeof(StlRss)
            },
            {
                StlSearch.ElementName,
                typeof(StlSearch)
            },
            {
                StlSelect.ElementName,
                typeof(StlSelect)
            },
            {
                StlSite.ElementName,
                typeof(StlSite)
            },
            {
                StlSites.ElementName,
                typeof(StlSites)
            },
            {
                StlSqlContent.ElementName,
                typeof(StlSqlContent)
            },
            {
                StlSqlContents.ElementName,
                typeof(StlSqlContents)
            },
            {
                StlTabs.ElementName,
                typeof(StlTabs)
            },
            {
                StlTags.ElementName,
                typeof(StlTags)
            },
            {
                StlTree.ElementName,
                typeof(StlTree)
            },
            {
                StlValue.ElementName,
                typeof(StlValue)
            },
            {
                StlVideo.ElementName,
                typeof(StlVideo)
            },
            {
                StlYes.ElementName,
                typeof(StlYes)
            },
            {
                StlZoom.ElementName,
                typeof(StlZoom)
            }
        };

        //public class StlElements
        //{
        //    private StlElements()
        //    {
        //    }

        //    public static SortedList<string, StlAttribute> GetElementsNameDictionary()
        //    {
        //        var stlAttribute = typeof(StlAttribute);
        //        return new SortedList<string, StlAttribute>
        //        {
        //            {
        //                StlA.ElementName,
        //                typeof(StlA),
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlA), stlAttribute)
        //            },
        //            {
        //                StlAction.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlAction), stlAttribute)
        //            },
        //            {
        //                StlAudio.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlAudio), stlAttribute)
        //            },
        //            {
        //                StlChannel.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlChannel), stlAttribute)
        //            },
        //            {
        //                StlChannels.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlChannels), stlAttribute)
        //            },
        //            {
        //                StlCode.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlCode), stlAttribute)
        //            },
        //            {
        //                StlContainer.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlContainer), stlAttribute)
        //            },
        //            {
        //                StlContent.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlContent), stlAttribute)
        //            },
        //            {
        //                StlContents.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlContents), stlAttribute)
        //            },
        //            {
        //                StlCount.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlCount), stlAttribute)
        //            },
        //            {
        //                StlDynamic.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlDynamic), stlAttribute)
        //            },
        //            {
        //                StlEach.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlEach), stlAttribute)
        //            },
        //            {
        //                StlFile.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlFile), stlAttribute)
        //            },
        //            {
        //                StlFlash.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlFlash), stlAttribute)
        //            },
        //            {
        //                StlIf.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlIf), stlAttribute)
        //            },
        //            {
        //                StlImage.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlImage), stlAttribute)
        //            },
        //            {
        //                StlInclude.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlInclude), stlAttribute)
        //            },
        //            {
        //                StlItemTemplate.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlItemTemplate), stlAttribute)
        //            },
        //            {
        //                StlLoading.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlLoading), stlAttribute)
        //            },
        //            {
        //                StlLocation.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlLocation), stlAttribute)
        //            },
        //            {
        //                StlMarquee.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlMarquee), stlAttribute)
        //            },
        //            {
        //                StlNavigation.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlNavigation), stlAttribute)
        //            },
        //            {
        //                StlNo.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlNo), stlAttribute)
        //            },
        //            {
        //                StlPageChannels.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPageChannels), stlAttribute)
        //            },
        //            {
        //                StlPageContents.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPageContents), stlAttribute)
        //            },
        //            {
        //                StlPageItem.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPageItem), stlAttribute)
        //            },
        //            {
        //                StlPageItems.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPageItems), stlAttribute)
        //            },
        //            {
        //                StlPageSqlContents.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPageSqlContents), stlAttribute)
        //            },
        //            {
        //                StlPlayer.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPlayer), stlAttribute)
        //            },
        //            {
        //                StlPrinter.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlPrinter), stlAttribute)
        //            },
        //            {
        //                StlQueryString.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlQueryString), stlAttribute)
        //            },
        //            {
        //                StlRss.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlRss), stlAttribute)
        //            },
        //            {
        //                StlSearch.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSearch), stlAttribute)
        //            },
        //            {
        //                StlSelect.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSelect), stlAttribute)
        //            },
        //            {
        //                StlSite.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSite), stlAttribute)
        //            },
        //            {
        //                StlSites.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSites), stlAttribute)
        //            },
        //            {
        //                StlSqlContent.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSqlContent), stlAttribute)
        //            },
        //            {
        //                StlSqlContents.ElementName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSqlContents), stlAttribute)
        //            },
        //            {
        //                StlTabs.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlTabs), stlAttribute)
        //            },
        //            {
        //                StlTags.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlTags), stlAttribute)
        //            },
        //            {
        //                StlTree.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlTree), stlAttribute)
        //            },
        //            {
        //                StlValue.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlValue), stlAttribute)
        //            },
        //            {
        //                StlVideo.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlVideo), stlAttribute)
        //            },
        //            {
        //                StlYes.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlYes), stlAttribute)
        //            },
        //            {
        //                StlZoom.ElementName, 
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlZoom), stlAttribute)
        //            }
        //        };
        //    }

        //    public static SortedList<string, List<Attr>> AttributesDictionary => new SortedList<string, List<Attr>>
        //    {
        //        {StlA.ElementName, StlA.Attributes}
        //    };

        //    public static SortedList<string, SortedList<string, string>> ElementsAttributesDictionary => new SortedList<string, SortedList<string, string>>
        //    {
        //        {StlAction.ElementName, StlAction.AttributeList},
        //        {StlAudio.ElementName, StlAudio.AttributeList},
        //        {StlChannel.ElementName, StlChannel.AttributeList},
        //        {StlChannels.ElementName, StlChannels.AttributeList},
        //        {StlCode.ElementName, StlCode.AttributeList},
        //        {StlContainer.ElementName, StlContainer.AttributeList},
        //        {StlContent.ElementName, StlContent.AttributeList},
        //        {StlContents.ElementName, StlContents.AttributeList},
        //        {StlCount.ElementName, StlCount.AttributeList},
        //        {StlDynamic.ElementName, StlDynamic.AttributeList},
        //        {StlEach.ElementName, StlEach.AttributeList},
        //        {StlFile.ElementName, StlFile.AttributeList},
        //        {StlFlash.ElementName, StlFlash.AttributeList},
        //        {StlFocusViewer.ElementName, StlFocusViewer.AttributeList},
        //        {StlIf.ElementName, StlIf.AttributeList},
        //        {StlImage.ElementName, StlImage.AttributeList},
        //        {StlInclude.ElementName, StlInclude.AttributeList},
        //        {StlItemTemplate.ElementName, StlItemTemplate.AttributeList},
        //        {StlLoading.ElementName, StlLoading.AttributeList},
        //        {StlLocation.ElementName, StlLocation.AttributeList},
        //        {StlMarquee.ElementName, StlMarquee.AttributeList},
        //        {StlNavigation.ElementName, StlNavigation.AttributeList},
        //        {StlNo.ElementName, StlNo.AttributeList},
        //        {StlPageChannels.ElementName, StlPageChannels.AttributeList},
        //        {StlPageContents.ElementName, StlPageContents.AttributeList},
        //        {StlPageItem.ElementName, StlPageItem.AttributeList},
        //        {StlPageItems.ElementName, StlPageItems.AttributeList},
        //        {StlPageSqlContents.ElementName, StlPageSqlContents.AttributeList},
        //        {StlPlayer.ElementName, StlPlayer.AttributeList},
        //        {StlPrinter.ElementName, StlPrinter.AttributeList},
        //        {StlQueryString.ElementName, StlQueryString.AttributeList},
        //        {StlRss.ElementName, StlRss.AttributeList},
        //        {StlSearch.ElementName, StlSearch.AttributeList},
        //        {StlSelect.ElementName, StlSelect.AttributeList},
        //        {StlSite.ElementName, StlSite.AttributeList},
        //        {StlSites.ElementName, StlSites.AttributeList},
        //        {StlSqlContent.ElementName, StlSqlContent.AttributeList},
        //        {StlSqlContents.ElementName, StlSqlContents.AttributeList},
        //        {StlTabs.ElementName, StlTabs.AttributeList},
        //        {StlTags.ElementName, StlTags.AttributeList},
        //        {StlTree.ElementName, StlTree.AttributeList},
        //        {StlValue.ElementName, StlValue.AttributeList},
        //        {StlVideo.ElementName, StlVideo.AttributeList},
        //        {StlYes.ElementName, StlYes.AttributeList},
        //        {StlZoom.ElementName, StlZoom.AttributeList}
        //    };
        //}

        //public class StlEntities
        //{
        //    private StlEntities()
        //    {
        //    }

        //    public static SortedList<string, StlAttribute> GetEntitiesNameDictionary()
        //    {
        //        var stlAttribute = typeof(StlAttribute);
        //        return new SortedList<string, StlAttribute>
        //        {
        //            {
        //                StlChannelEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlChannelEntities), stlAttribute)
        //            },
        //            {
        //                StlContentEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlContentEntities), stlAttribute)
        //            },
        //            {
        //                StlElementEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlElementEntities), stlAttribute)
        //            },
        //            {
        //                StlNavigationEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlNavigationEntities), stlAttribute)
        //            },
        //            {
        //                StlRequestEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlRequestEntities), stlAttribute)
        //            },
        //            {
        //                StlSqlEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlSqlEntities), stlAttribute)
        //            },
        //            {
        //                StlStlEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlStlEntities), stlAttribute)
        //            },
        //            {
        //                StlUserEntities.EntityName,
        //                (StlAttribute) Attribute.GetCustomAttribute(typeof(StlUserEntities), stlAttribute)
        //            }
        //        };
        //    }

        //    public static SortedList<string, SortedList<string, string>> EntitiesAttributesDictionary
        //        => new SortedList<string, SortedList<string, string>>
        //        {
        //            {StlChannelEntities.EntityName, StlChannelEntities.AttributeList},
        //            {StlContentEntities.EntityName, StlContentEntities.AttributeList},
        //            {StlElementEntities.EntityName, StlElementEntities.AttributeList},
        //            {StlNavigationEntities.EntityName, StlNavigationEntities.AttributeList},
        //            {StlRequestEntities.EntityName, StlRequestEntities.AttributeList},
        //            {StlSqlEntities.EntityName, StlSqlEntities.AttributeList},
        //            {StlStlEntities.EntityName, StlStlEntities.AttributeList},
        //            {StlUserEntities.EntityName, StlUserEntities.AttributeList}
        //        };
        //}
    }
}
