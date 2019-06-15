using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    internal class TemplateIe
    {
        private readonly int _siteId;
        private readonly string _filePath;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplateIe(int siteId, string filePath)
        {
            _siteId = siteId;
            _filePath = filePath;
        }

        public void ExportTemplates()
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoList = _templateRepository.GetTemplateInfoListBySiteId(_siteId);

            foreach (var templateInfo in templateInfoList)
            {
                var entry = ExportTemplateInfo(templateInfo);
                feed.Entries.Add(entry);
            }
            feed.Save(_filePath);
        }

        public void ExportTemplates(List<int> templateIdList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            var templateInfoList = _templateRepository.GetTemplateInfoListBySiteId(_siteId);

            foreach (var templateInfo in templateInfoList)
            {
                if (templateIdList.Contains(templateInfo.Id))
                {
                    var entry = ExportTemplateInfo(templateInfo);
                    feed.Entries.Add(entry);
                }
            }
            feed.Save(_filePath);
        }

        private AtomEntry ExportTemplateInfo(TemplateInfo templateInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            var siteInfo = _siteRepository.GetSiteInfo(_siteId);

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.Id), "TemplateID" }, templateInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(TemplateInfo.SiteId), "PublishmentSystemID" }, templateInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.TemplateName), templateInfo.TemplateName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Type), templateInfo.Type.Value);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName), templateInfo.RelatedFileName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName), templateInfo.CreatedFileFullName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName), templateInfo.CreatedFileExtName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(TemplateInfo.Default), templateInfo.Default.ToString());

            var templateContent = _templateRepository.GetTemplateContent(siteInfo, templateInfo);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Content", AtomUtility.Encrypt(templateContent));

            return entry;
        }

        public void ImportTemplates(bool overwrite, string administratorName)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var siteInfo = _siteRepository.GetSiteInfo(_siteId);
            foreach (AtomEntry entry in feed.Entries)
            {
                var templateName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.TemplateName));
                if (string.IsNullOrEmpty(templateName)) continue;

                var templateInfo = new TemplateInfo
                {
                    SiteId = _siteId,
                    TemplateName = templateName,
                    Type =
                        TemplateType.Parse(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.Type))),
                    RelatedFileName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.RelatedFileName)),
                    CreatedFileFullName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileFullName)),
                    CreatedFileExtName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(TemplateInfo.CreatedFileExtName)),
                    Default = false
                };

                var templateContent = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Content"));

                var srcTemplateInfo = _templateRepository.GetTemplateInfoByTemplateName(_siteId, templateInfo.Type, templateInfo.TemplateName);

                int templateId;

                if (srcTemplateInfo != null)
                {
                    if (overwrite)
                    {
                        srcTemplateInfo.RelatedFileName = templateInfo.RelatedFileName;
                        srcTemplateInfo.Type = templateInfo.Type;
                        srcTemplateInfo.CreatedFileFullName = templateInfo.CreatedFileFullName;
                        srcTemplateInfo.CreatedFileExtName = templateInfo.CreatedFileExtName;
                        _templateRepository.Update(siteInfo, srcTemplateInfo, templateContent, administratorName);
                        templateId = srcTemplateInfo.Id;
                    }
                    else
                    {
                        templateInfo.TemplateName = _templateRepository.GetImportTemplateName(_siteId, templateInfo.TemplateName);
                        templateId = _templateRepository.Insert(templateInfo, templateContent, administratorName);
                    }
                }
                else
                {
                    templateId = _templateRepository.Insert(templateInfo, templateContent, administratorName);
                }

                if (templateInfo.Type == TemplateType.FileTemplate)
                {
                    _createManager.CreateFile(_siteId, templateId);
                }
            }
        }

    }
}
