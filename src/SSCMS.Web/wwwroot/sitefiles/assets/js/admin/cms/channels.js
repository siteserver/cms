var $url = '/cms/channels/channels';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  root: null,
  expandedChannelIds: [],
  indexNames: [],
  groupNames: [],
  channelTemplates: [],
  contentTemplates: [],

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
  editEditor: null,
  styles: [],
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
    UE.getEditor(attributeName, {allowDivTransToP: false, maximumWords:99999999}).execCommand('insertHTML', html);
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
      $this.expandedChannelIds = expandedChannelIds ? expandedChannelIds : [$this.siteId];

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
      $this.editLinkTypes = res.linkTypes;
      $this.editTaxisTypes = res.taxisTypes;
      $this.styles = res.styles;
      $this.isTemplateEditable = res.isTemplateEditable;
      $this.editPanel = true;
      setTimeout(function () {
        $this.loadEditor();
      }, 100);
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

  loadEditor: function () {
    var $this = this;

    document.getElementById('form_Content1').innerHTML = '';
    document.getElementById('form_Content2').innerHTML = '';

    var E = window.wangEditor;
    this.editEditor = new E('#form_Content1', '#form_Content2');
    this.editEditor.customConfig.menus = [
      'head',  // 标题
      'bold',  // 粗体
      'fontSize',  // 字号
      'fontName',  // 字体
      'italic',  // 斜体
      'underline',  // 下划线
      'strikeThrough',  // 删除线
      'foreColor',  // 文字颜色
      'backColor',  // 背景颜色
      'link',  // 插入链接
      'list',  // 列表
      'justify',  // 对齐方式
      'quote',  // 引用
      'table',  // 表格
      'undo',  // 撤销
      'redo'  // 重复
    ];
    this.editEditor.customConfig.onchange = function (html) {
      $this.form.content = html;
    };
    this.editEditor.create();
    this.editEditor.txt.html(this.form.content);
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

  btnTemplateEditClick: function(isChannel, templateId) {
    var templateName = '';
    if (isChannel) {
      templateName = this.channelTemplates.find(function(x) {
        return x.id === templateId;
      }).templateName;
    } else {
      templateName = this.contentTemplates.find(function(x) {
        return x.id === templateId;
      }).templateName;
    }
    utils.addTab('编辑:' + templateName, utils.getCmsUrl('templatesEditor', {
      siteId: this.siteId,
      templateId: templateId,
      templateType: isChannel ? 'ChannelTemplate' : 'ContentTemplate',
    }));
  },

  btnEditAddGroupClick: function() {
    utils.openLayer({
      title: '新增栏目组',
      url: utils.getCommonUrl('groupChannelLayerAdd', {siteId: this.siteId}),
      width: 500,
      height: 300
    });
  },

  handleDragStart: function(node, ev) {
    console.log('drag start', node);
  },

  handleDragEnter: function(draggingNode, dropNode, ev) {
    console.log('tree drag enter: ', dropNode.channelName);
  },

  handleDragLeave: function(draggingNode, dropNode, ev) {
    console.log('tree drag leave: ', dropNode.channelName);
  },

  handleDragOver: function(draggingNode, dropNode, ev) {
    console.log('tree drag over: ', dropNode.channelName);
  },
  
  handleDragEnd: function(draggingNode, dropNode, dropType, ev) {
    console.log('tree drag end: ', dropNode && dropNode.channelName, dropType);
  },

  handleDrop: function(draggingNode, dropNode, dropType, ev) {
    console.log('tree drop: ', dropNode.channelName, dropType);
  },

  allowDrop: function(draggingNode, dropNode, type) {
    if (dropNode.data.channelName === '二级 3-1') {
      return type !== 'inner';
    } else {
      return true;
    }
  },

  allowDrag: function(draggingNode) {
    return draggingNode.data.channelName.indexOf('三级 3-2-2') === -1;
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {siteId: this.siteId, channelId: data.value});
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.channelName && value.indexName && value.groupName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.indexName === value.indexName && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName && value.indexName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.indexName === value.indexName;
    } else if (value.channelName && value.groupName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName) && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName && value.groupName) {
      return data.indexName === value.indexName && data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName) {
      return (data.label.indexOf(value.channelName) !== -1 || data.value + '' === value.channelName);
    } else if (value.groupName) {
      return data.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName) {
      return data.indexName === value.indexName;
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

  btnOrderClick: function(row, isUp) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/order', {
      siteId: this.siteId,
      channelId: row.value,
      parentId: row.parentId,
      taxis: row.taxis,
      isUp: isUp
    }).then(function (response) {
      var res = response.data;
      
      $this.apiList('栏目排序成功!', res);
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error(error);
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