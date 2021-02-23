var $url = '/cms/channels/channels';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  root: null,
  expandedChannelIds: [],
  indexNames: [],
  groupNames: [],
  channelTemplates: [],
  contentTemplates: [],
  defaultChannelTemplate: null,
  defaultContentTemplate: null,
  columns: null,
  commandsWidth: 160,

  channelIds: [],

  filterText: '',
  filterIndexName: '',
  filterGroupName: '',

  appendPanel: false,
  appendForm: null,

  editPanel: false,
  form: null,
  editLinkTypes: [],
  editTaxisTypes: [],
  styles: [],
  siteUrl: null,
  isTemplateEditable: false,

  deletePanel: false,
  deleteForm: null,

  importPanel: false,
  importForm: null,
  importUploadList: []
});

var methods = {
  runFormLayerImageUploadText: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerImageUploadEditor: function(attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runMaterialLayerImageSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerFileUpload: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerFileSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerVideoUpload: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerVideoSelect: function(attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runEditorLayerImage: function(attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  insertText: function(attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)];
    if (count && count < no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  insertEditor: function(attributeName, html) {
    if (!attributeName) attributeName = 'Body';
    if (!html) return;
    utils.getEditor(attributeName).execCommand('insertHTML', html);
  },
  
  setRuleText: function(rule, isChannel) {
    if (isChannel) {
      this.form.channelFilePathRule = rule;
    } else {
      this.form.contentFilePathRule = rule;
    }
  },

  updateGroups: function(res, message) {
    this.groupNames = res.groupNames;
    utils.success(message);
  },

  apiList: function(message, expandedChannelIds) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.root = [res.channel];
      $this.indexNames = res.indexNames;
      $this.groupNames = res.groupNames;
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;
      $this.defaultChannelTemplate = $this.channelTemplates.find(function(x) {
        return x.defaultTemplate;
      });
      $this.defaultContentTemplate = $this.contentTemplates.find(function(x) {
        return x.defaultTemplate;
      });
      $this.columns = res.columns;
      $this.commandsWidth = res.commandsWidth;
      $this.isTemplateEditable = res.isTemplateEditable;
      $this.expandedChannelIds = expandedChannelIds ? expandedChannelIds : [$this.siteId];
      $this.editLinkTypes = res.linkTypes;
      $this.editTaxisTypes = res.taxisTypes;
      $this.siteUrl = res.siteUrl;

      if (message) {
        utils.success(message);
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGet: function(channelId) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + this.siteId + '/' + channelId).then(function (response) {
      var res = response.data;

      $this.form = _.assign({}, res.entity);
      if (!$this.form.groupNames) {
        $this.form.groupNames = [];
      }
      $this.styles = res.styles;
      $this.form.filePath = res.filePath;
      $this.form.channelFilePathRule = res.channelFilePathRule;
      $this.form.contentFilePathRule = res.contentFilePathRule;

      $this.editPanel = true;
      utils.loadEditors($this.styles, $this.form);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiAppend: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/append', {
      siteId: this.siteId,
      parentId: this.appendForm.parentIds[this.appendForm.parentIds.length - 1],
      channelTemplateId: this.appendForm.channelTemplateId,
      contentTemplateId: this.appendForm.contentTemplateId,
      isParentTemplates: this.appendForm.isParentTemplates,
      isIndexName: this.appendForm.isIndexName,
      channels: this.appendForm.channels,
    }).then(function (response) {
      var res = response.data;

      $this.appendPanel = false;
      $this.apiList('栏目添加成功!', res);
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiEdit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.put($url, this.form).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList('栏目编辑成功!', res);
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiDelete: function () {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: this.deleteForm
    }).then(function (response) {
      var res = response.data;

      $this.deletePanel = false;
      $this.apiList('栏目删除成功!', res);
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiImport: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/import', {
      siteId: this.importForm.siteId,
      channelId: this.importForm.channelIds[this.importForm.channelIds.length - 1],
      fileName: this.importForm.fileName,
      isOverride: this.importForm.isOverride,
    }).then(function (response) {
      var res = response.data;

      $this.importPanel = false;
      $this.apiList('栏目导入成功!', res);
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
    })
  },

  apiDrop: function (sourceId, targetId, dropType) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/drop', {
      siteId: this.siteId,
      sourceId: sourceId,
      targetId: targetId,
      dropType: dropType
    }).then(function (response) {
      var res = response.data;
      
      // $this.apiList('栏目排序成功!', [$this.siteId, sourceId]);
      utils.success('栏目排序成功!');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiColumns: function(attributeNames) {
    var $this = this;

    $api.post($url + '/actions/columns', {
      siteId: this.siteId,
      attributeNames: attributeNames
    }).then(function(response) {
      var res = response.data;

    }).catch(function(error) {
      utils.error(error);
    });
  },

  handleColumnsChange: function() {
    var listColumns = _.filter(this.columns, function(o) { return o.isList; });
    var attributeNames = _.map(listColumns, function(column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  },

  btnSetClick: function(channelId, isChannel, rule) {
    var url = utils.getCmsUrl('settingsCreateRuleLayerSet', {
      siteId: this.siteId,
      isChannel: isChannel,
      channelId: channelId,
      rule: rule
    });

    utils.openLayer({
      title: '构造',
      url: url,
      width: 800,
      height: 500
    });
  },
  
  getColumnWidth: function(attributeName) {
    if (attributeName === 'Id') return 80;
    if (attributeName === 'ChannelTemplateId' || attributeName === 'ContentTemplateId') return 120;
    if (attributeName === 'IndexName') return 120;
    return 180;
  },

  getTemplate: function(isChannel, templateId) {
    var template = null;
    if (isChannel) {
      template = this.channelTemplates.find(function(x) {
        return x.id === templateId;
      });
    } else {
      template = this.contentTemplates.find(function(x) {
        return x.id === templateId;
      });
    }
    if (!template) {
      template = isChannel ? this.defaultChannelTemplate : this.defaultContentTemplate;
    }
    return template;
  },

  getTemplateEditorUrl: function(isChannel, templateId) {
    return utils.getCmsUrl('templatesEditor', {
      siteId: this.siteId,
      templateId: templateId,
      templateType: isChannel ? 'ChannelTemplate' : 'ContentTemplate',
    });
  },

  btnEditAddGroupClick: function() {
    utils.openLayer({
      title: '新增栏目组',
      url: utils.getCommonUrl('groupChannelLayerAdd', {siteId: this.siteId}),
      width: 500,
      height: 300
    });
  },

  handleDrop: function(draggingNode, dropNode, dropType, ev) {
    this.apiDrop(draggingNode.data.value, dropNode.data.value, dropType);
  },

  allowDrop: function(draggingNode, dropNode, type) {
    if (dropNode.data.value === this.siteId) {
      return false;
    } else {
      return true;
    }
  },

  allowDrag: function(draggingNode) {
    return draggingNode.data.value !== this.siteId;
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {siteId: this.siteId, channelId: data.value});
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnCheckClick: function(row) {
    if (this.channelIds.indexOf(row.value) !== -1) {
      this.channelIds.splice(this.channelIds.indexOf(row.value), 1);
    } else {
      this.channelIds.push(row.value);
    }
    this.$refs.tree.setCheckedKeys(this.channelIds);
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.channelName && value.indexName && value.groupName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.channel.indexName === value.indexName && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName && value.indexName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.channel.indexName === value.indexName;
    } else if (value.channelName && value.groupName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName && value.groupName) {
      return data.channel.indexName === value.indexName && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName);
    } else if (value.groupName) {
      return data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName) {
      return data.channel.indexName === value.indexName;
    }
    return true;
  },

  btnCancelClick: function() {
    this.appendPanel = false;
    this.deletePanel = false;
    this.importPanel = false;
    this.editPanel = false;
  },

  btnAppendClick: function() {
    this.appendForm = {
      parentIds: [this.siteId],
      channelTemplateId: 0,
      contentTemplateId: 0,
      isParentTemplates: true,
      isIndexName: false,
      channels: ''
    };
    this.appendPanel = true;
  },

  btnAppendSubmitClick: function() {
    var $this = this;
    this.$refs.appendForm.validate(function(valid) {
      if (valid) {
        $this.apiAppend();
      }
    });
  },

  btnEditClick: function(row) {
    this.apiGet(row.value);
  },

  btnEditSubmitClick: function() {
    var $this = this;
    this.$refs.editForm.validate(function(valid) {
      if (valid) {
        $this.apiEdit();
      }
    });
  },

  btnDeleteClick: function(data) {
    this.deleteForm = {
      siteId: this.siteId,
      channelId: data.value,
      label: data.label,
      channelName: null,
      deleteFiles: false
    };
    this.deletePanel = true;
  },

  btnDeleteSubmitClick: function() {
    var $this = this;
    this.$refs.deleteForm.validate(function(valid) {
      if (valid) {
        if ($this.deleteForm.channelName == $this.deleteForm.label) {
          $this.apiDelete();
        } else {
          utils.error('请检查您输入的栏目名称是否正确');
        }
      }
    });
  },

  btnImportClick: function() {
    this.importForm = {
      siteId: this.siteId,
      channelIds: [this.siteId],
      fileName: null,
      isOverride: true
    };
    this.importPanel = true;
  },

  btnTranslateClick: function() {
    location.href = utils.getCmsUrl('channelsTranslate', {
      siteId: this.siteId,
      channelIds: this.channelIds.join(','),
      returnUrl: location.href
    });
  },

  btnImportSubmitClick: function() {
    var $this = this;
    this.$refs.importForm.validate(function(valid) {
      if (valid) {
        $this.apiImport();
      }
    });
  },

  btnExportClick: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/export', {
      siteId: this.siteId,
      channelIds: this.channelIds
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSetGroupClick: function() {
    utils.openLayer({
      title: '设置栏目组',
      url: utils.getCmsUrl('channelsLayerGroup', {
        siteId: this.siteId,
        channelIds: this.channelIds.join(',')
      }),
      width: 700,
      height: 400
    });
  },

  btnSetTaxisClick: function() {
    utils.openLayer({
      title: '栏目排序',
      url: utils.getCmsUrl('channelsLayerTaxis', {
        siteId: this.siteId,
        channelIds: this.channelIds.join(',')
      }),
      width: 500,
      height: 260
    });
  },

  btnCreateClick: function() {
    utils.openLayer({
      title: '生成页面',
      url: utils.getCmsUrl('channelsLayerCreate', {
        siteId: this.siteId,
        channelIds: this.channelIds.join(',')
      }),
      width: 500,
      height: 260
    });
  },

  btnMenuClick: function(menu, channel) {
    var url = utils.addQuery(menu.link, {
      siteId: this.siteId,
      channelId: channel.value
    });

    if (menu.target == '_layer') {
      utils.openLayer({
        title: menu.text,
        url: url,
        full: true
      });
    } else if (menu.target == '_self') {
      location.href = url;
    } else if (menu.target == '_parent') {
      parent.location.href = url;
    }  else if (menu.target == '_top') {
      top.location.href = url;
    } else if (menu.target == '_blank') {
      window.open(url);
    } else {
      utils.addTab(menu.text, url);
    }
  },

  uploadBefore(file) {
    var isZip = file.name.indexOf('.zip', file.name.length - '.zip'.length) !== -1;
    if (!isZip) {
      utils.error('导入文件只能是 Zip 格式!');
    }
    return isZip;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    this.loading && this.loading.close();
    this.importForm.fileName = res.value;
  },

  uploadError: function(err) {
    this.loading && this.loading.close();
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId
    };

    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.no) {
      query.no = options.no;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query)
    };
    if (!options.full) {
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }
    utils.openLayer(args);
  },

  btnExtendAddClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
    this.form = _.assign({}, this.form);
  },

  btnExtendRemoveClick: function(style) {
    var no = this.form[utils.getCountName(style.attributeName)];
    this.form[utils.getCountName(style.attributeName)] = no - 1;
    this.form[utils.getExtendName(style.attributeName, no)] = '';
    this.form = _.assign({}, this.form);
  },

  btnExtendPreviewClick: function(attributeName, no) {
    var count = this.form[utils.getCountName(attributeName)];
    var data = [];
    for (var i = 0; i <= count; i++) {
      var imageUrl = this.form[utils.getExtendName(attributeName, i)];
      imageUrl = utils.getUrl(this.siteUrl, imageUrl);
      data.push({
        "src": imageUrl
      });
    }
    layer.photos({
      photos: {
        "start": no,
        "data": data
      }
      ,anim: 5
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter({
        channelName: val,
        indexName: this.filterIndexName,
        groupName: this.filterGroupName
      });
    },
    filterIndexName: function(val) {
      this.$refs.tree.filter({
        channelName: this.filterText,
        indexName: val,
        groupName: this.filterGroupName
      });
    },
    filterGroupName: function(val) {
      this.$refs.tree.filter({
        channelName: this.filterText,
        indexName: this.filterIndexName,
        groupName: val
      });
    }
  },
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.siteId;
    this.apiList();
  }
});