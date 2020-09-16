var $url = "/write/contents";
var $defaultWidth = 160;

var data = utils.init({
  pageType: null,
  sites: null,
  siteName: null,
  siteUrl: null,
  root: null,
  allCheckedLevels: [],
  checkedLevels: [],
  
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  columns: null,
  permissions: null,
  menus: null,
  
  tableMaxHeight: 999999999999,
  multipleSelection: [],

  checkedColumns: [],

  siteId: 0,
  channelId: null
});

var methods = {
  apiTree: function() {
    var $this = this;

    $api.post($url + '/actions/tree', {
      siteId: this.siteId,
      reload: false
    }).then(function(response) {
      var res = response.data;

      if (!res.siteId) {
        $this.pageType = 'Unauthorized';
        utils.loading($this, false);
        return;
      }

      $this.sites = res.sites;
      $this.siteId = res.siteId;
      $this.siteName = res.siteName;
      $this.siteUrl = res.siteUrl;
      $this.root = res.root;
      $this.columns = res.columns;
      $this.permissions = res.permissions;

      $this.allCheckedLevels = res.checkedLevels;
      $this.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });
      $this.apiList($this.siteId, 1);
    }).catch(function(error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiList: function(useless, page, message) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/list', {
      siteId: this.siteId,
      channelId: this.channelId,
      page: page,
      isCheckedLevels: this.isCheckedLevels,
      checkedLevels: this.checkedLevels
    }).then(function(response) {
      var res = response.data;
      
      $this.pageContents = res.pageContents;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;

      if (message) {
        utils.success(message);
      }
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
      $this.scrollToTop();
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

  getContentUrl: function (content) {
    return utils.getRootUrl('redirect', {
      siteId: content.siteId,
      channelId: content.channelId,
      contentId: content.id
    });
  },

  btnTitleClick: function(content) {
    if (content.checked && content.channelId > 0) return false;
    utils.openLayer({
      title: "查看内容",
      url: utils.getCmsUrl('contentsLayerView', {
        siteId: this.siteId,
        channelId: Math.abs(content.channelId),
        contentId: content.id
      }),
      full: true
    });
  },

  btnEditClick: function(content) {
    utils.addTab('编辑内容', this.getEditUrl(content));
  },

  btnAdminClick: function(adminId) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getCommonUrl('adminLayerView', {adminId: adminId}),
      width: 550,
      height: 450
    });
  },

  getAddUrl: function() {
    return utils.getRootUrl('write/editor', {
      siteId: this.siteId,
      channelId: this.channelId,
      page: this.page,
      tabName: utils.getTabName()
    });
  },

  getEditUrl: function(content) {
    return utils.getRootUrl('write/editor', {
      siteId: this.siteId,
      channelId: content.channelId,
      contentId: content.id,
      page: this.page
    });
  },

  btnAddClick: function () {
    utils.addTab('添加内容', this.getAddUrl());
  },

  btnImportClick: function (command) {
    if (command === 'Word') {
      this.btnLayerClick({title: '批量导入Word', name: 'Word', full: true});
    } else if (command === 'Import') {
      this.btnLayerClick({title: '批量导入', name: 'Import', full: true});
    }
  },

  btnMoreClick: function(command) {
    if (command === 'Group') {
      this.btnLayerClick({title: '批量设置分组', name: 'Group', width: 700, height: 400, withContents: true});
    } else if (command === 'Tag') {
      this.btnLayerClick({title: '批量设置标签', name: 'Tag', width: 700, height: 400, withContents: true});
    } else if (command === 'Copy') {
      this.btnLayerClick({title: '批量复制', name: 'Copy', withContents: true});
    } else if (command === 'ExportAll') {
      this.btnLayerClick({title: '导出全部', name: 'Export', full: true});
    } else if (command === 'ExportSelected') {
      this.btnLayerClick({title: '导出选中', name: 'Export', full: true, withContents: true});
    } else if (command === 'Arrange') {
      this.btnLayerClick({title: '整理排序', name: 'Arrange', width: 550, height: 350});
    } else if (command === 'Hits') {
      this.btnLayerClick({title: '设置点击量', name: 'Hits', width: 450, height: 320, withContents: true});
    }
  },

  btnCreateClick: function() {
    var $this = this;

    if (!this.isContentChecked) return;

    utils.loading(this, true);
    $api.post($url + "/actions/create", {
      siteId: $this.siteId,
      channelContentIds: this.channelContentIdsString
    }).then(function(response) {
      var res = response.data;

      utils.addTab('生成进度查看', utils.getCmsUrl('createStatus', {siteId: $this.siteId}));
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId, 
      page: this.page
    };

    if (options.channelId) {
      query.channelId = options.channelId;
    } else {
      query.channelId = this.siteId;
    }
    if (options.contentId) {
      query.contentId = options.contentId;
    }

    if (options.withContents) {
      if (!this.isContentChecked) return;
      query.channelContentIds = this.channelContentIdsString;
    }

    options.url = utils.getCmsUrl('contentsLayer' + options.name, query);
    utils.openLayer(options);
  },

  btnContentViewClick: function(contentId) {
    utils.openLayer({
      title: "查看内容",
      url: utils.getCmsUrl('contentsLayerView', {
        siteId: this.siteId,
        channelId: this.siteId,
        contentId: contentId
      }),
      full: true
    });
  },

  btnContentStateClick: function(contentId) {
    utils.openLayer({
      title: "查看审核状态",
      url: utils.getCmsUrl('contentsLayerState', {
        siteId: this.siteId,
        channelId: this.siteId,
        contentId: contentId
      }),
      full: true
    });
  },

  scrollToTop: function() {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  tableRowClassName: function(scope) {
    if (this.multipleSelection.indexOf(scope.row) !== -1) {
      return 'current-row';
    }
    return '';
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  toggleSelection: function(row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  handleCurrentChange: function(val) {
    this.apiList(this.siteId, val);
  },

  handleColumnsChange: function() {
    var listColumns = _.filter(this.columns, function(o) { return o.isList; });
    var attributeNames = _.map(listColumns, function(column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  },

  getColumnWidth: function(column) {
    if (column.attributeName === 'Sequence' || column.attributeName === 'Id' || column.attributeName === 'Hits' || column.attributeName === 'HitsByDay' || column.attributeName === 'HitsByWeek' || column.attributeName === 'HitsByMonth' || column.attributeName === 'Downloads') {
      return 70;
    }
    if (column.attributeName === 'ImageUrl') {
      return 100;
    }
    if (column.attributeName === 'Guid' || column.attributeName === 'SourceId') {
      return 310;
    }
    if (column.attributeName === 'Title') {
      return '';
    }
    return $defaultWidth;
  },

  getColumnMinWidth: function(column) {
    if (column.attributeName === 'Title') {
      return 400;
    }
    return '';
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    isCheckedLevels: function() {
      if (this.checkedLevels.length !== this.checkedLevels.length) return true;
      return false;
    },

    isContentChecked: function() {
      return this.multipleSelection.length > 0;
    },

    channelContentIds: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var content = this.multipleSelection[i];
        retVal.push({
          channelId: content.channelId,
          id: content.id
        });
      }
      return retVal;
    },

    channelContentIdsString: function() {
      var retVal = [];
      for (var i = 0; i < this.multipleSelection.length; i++) {
        var content = this.multipleSelection[i];
        retVal.push(content.channelId + '_' + content.id);
      }
      return retVal.join(",");
    }
  },
  created: function() {
    this.apiTree();
  }
});
