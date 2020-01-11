var $url = "/pages/cms/channels"
var $urlUpload = apiUrl + '/pages/cms/channels/actions/upload?siteId=' + utils.getQueryInt("siteId");

var data = {
  siteId: utils.getQueryInt("siteId"),
  pageLoad: false,
  tableData: [],
  channels: [],
  indexNames: [],
  groupNames: [],
  channelTemplates: [],
  contentTemplates: [],
  contentPlugins: [],
  relatedPlugins: [],

  expandedChannelIds: [],
  filterText: '',
  filterIndexName: '',
  filterGroupName: '',

  appendPanel: false,
  appendParent: null,
  appendForm: null,

  editPanel: false,
  editChannel: null,
  editLinkTypes: [],
  editTaxisTypes: [],
  editIsEditor: false,
  editEditor: null,

  deletePanel: false,
  deleteForm: null,

  importPanel: false,
  importForm: null,
  importUploadList: [],

  loading: null,

  tableData: [{
    id: 1,
    date: '2016-05-02',
    name: '王小虎',
    address: '上海市普陀区金沙江路 1518 弄'
  }],
  value: true
};

var methods = {
  apiList: function(message) {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.channels = [res.channel];
      $this.indexNames = res.indexNames;
      $this.groupNames = res.groupNames;
      $this.channelTemplates = res.channelTemplates;
      $this.contentTemplates = res.contentTemplates;
      $this.contentPlugins = res.contentPlugins;
      $this.relatedPlugins = res.relatedPlugins;
      $this.expandedChannelIds = [$this.siteId];

      if (message) {
        $this.$message({
          type: 'success',
          message: message
        });
      }
    }).catch(function (error) {
      utils.notifyError($this, error);
    }).then(function () {
      $this.pageLoad = true;
      $this.loading && $this.loading.close();
    });
  },

  apiGet: function(channelId) {
    var $this = this;

    this.loading = this.$loading();
    $api.get($url + '/' + this.siteId + '/' + channelId).then(function (response) {
      var res = response.data;

      $this.editChannel = res.channel;
      $this.editLinkTypes = res.linkTypes;
      $this.editTaxisTypes = res.taxisTypes;
      $this.editPanel = true;
    }).catch(function (error) {
      utils.notifyError($this, error);
    }).then(function () {
      $this.loading.close();
    });
  },

  apiAppend: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.post($url + '/actions/append', this.appendForm).then(function (response) {
      var res = response.data;

      $this.appendPanel = false;
      $this.apiList('栏目添加成功!');
    }).catch(function (error) {
      utils.notifyError($this, error);
    });
  },

  apiEdit: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.put($url, this.editChannel).then(function (response) {
      var res = response.data;

      $this.editPanel = false;
      $this.apiList('栏目编辑成功!');
    }).catch(function (error) {
      utils.notifyError($this, error);
    });
  },

  apiDelete: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.delete($url, {
      data: this.deleteForm
    }).then(function (response) {
      var res = response.data;

      $this.deletePanel = false;
      $this.apiList('栏目删除成功!');
    }).catch(function (error) {
      utils.notifyError($this, error);
    });
  },

  apiImport: function () {
    var $this = this;

    this.loading = this.$loading();
    $api.post($url + '/actions/import', {
      siteId: this.importForm.siteId,
      channelId: this.importForm.channelIds[this.importForm.channelIds.length - 1],
      fileName: this.importForm.fileName,
      isOverride: this.importForm.isOverride,
    }).then(function (response) {
      var res = response.data;

      $this.importPanel = false;
      $this.apiList('栏目导入成功!');
    }).catch(function (error) {
      utils.notifyError($this, error);
    });
  },

  apiExport: function (channelIds) {
    var $this = this;

    this.loading = this.$loading();
    $api.post($url + '/actions/export', {
      siteId: this.siteId,
      channelIds: channelIds
    }).then(function (response) {
      var res = response.data;

      window.open(res.value);
    }).catch(function (error) {
      utils.notifyError($this, error);
    });
  },

  appendChannels: function(channels) {
    
  },

  btnAppendClick: function(data) {
    this.appendPanel = true
    this.appendParent = data
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
    return '../redirect.cshtml?siteId=' + this.siteId + '&channelId=' + data.value;
  },

  getCheckedNodes: function() {
    console.log(this.$refs.tree.getCheckedNodes());
  },
  
  getCheckedKeys: function() {
    return this.$refs.tree.getCheckedKeys();
  },

  filterNode: function(value, data) {
    if (!value) return true;
    if (value.channelName && value.indexName && value.groupName) {
      return data.label.indexOf(value.channelName) !== -1 && data.dict.indexName === value.indexName && data.dict.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName && value.indexName) {
      return data.label.indexOf(value.channelName) !== -1 && data.indexName === value.indexName;
    } else if (value.channelName && value.groupName) {
      return data.label.indexOf(value.channelName) !== -1 && data.dict.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName && value.groupName) {
      return data.indexName === value.indexName && data.dict.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.channelName) {
      return data.label.indexOf(value.channelName) !== -1;
    } else if (value.groupName) {
      return data.dict.groupNames.indexOf(value.groupName) !== -1;
    } else if (value.indexName) {
      return data.dict.indexName === value.indexName;
    }
    return true;
  },

  btnCancelClick: function() {
    this.appendPanel = false;
    this.deletePanel = false;
    this.importPanel = false;
    this.editPanel = false;
  },

  btnAppendClick: function(row) {
    this.appendParent = row;
    this.appendForm = {
      siteId: this.siteId,
      parentId: this.appendParent.value,
      channelTemplateId: 0,
      contentTemplateId: 0,
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
    this.editIsEditor = false;
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

  btnEditContentClick: function() {
    this.editIsEditor = true;
    if (!this.editEditor) {
      var $this = this;
      var E = window.wangEditor;
      this.editEditor = new E('#editChannel_Content1', '#editChannel_Content2');
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
        $this.editChannel.content = html;
      };
      this.editEditor.create();
    }
    this.editEditor.txt.html(this.editChannel.content);
  },

  btnDeleteClick: function(data) {
    //   var $this = this;
    //   this.$confirm('此操作将永久删除栏目 <span style="color:#F56C6C">' + data.label + '</span> 及其子栏目, 是否继续?', '警告', {
    //     confirmButtonText: '永久删除',
    //     confirmButtonClass: 'el-button--danger',
    //     cancelButtonText: '取消',
    //     type: 'warning',
    //     dangerouslyUseHTMLString: true
    //   }).then(function() {
    //     $this.apiDelete(node, data);
    //   })
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
        $this.apiDelete();
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

  btnImportSubmitClick: function() {
    var $this = this;
    this.$refs.importForm.validate(function(valid) {
      if (valid) {
        $this.apiImport();
      }
    });
  },

  btnExportClick: function() {
    var channelIds = this.getCheckedKeys();
    if (!channelIds || channelIds.length === 0){
      this.$message({
        type: 'error',
        message: '请选择需要导出的栏目!'
      });
    } else {
      this.apiExport(channelIds);
    }
  },

  uploadBefore(file) {
    var isZip = file.name.indexOf('.zip', file.name.length - '.zip'.length) !== -1;
    if (!isZip) {
      this.$message.error('导入文件只能是 Zip 格式!');
    }
    return isZip;
  },

  uploadProgress: function() {
    this.loading = this.$loading();
  },

  uploadSuccess: function(res, file) {
    this.loading && this.loading.close();
    this.importForm.fileName = res.value;
  },

  uploadError: function(err) {
    this.loading && this.loading.close();
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  },

  btnLayerClick: function(options) {
    var url = "editorLayer" + options.name + ".cshtml?siteId=" + this.siteId;

    if (options.attributeName) {
      url += "&attributeName=" + options.attributeName;
    }
    if (options.contentId) {
      url += "&contentId=" + options.contentId;
    }

    utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },
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
    this.apiList();
  }
});