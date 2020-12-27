var $url = "/cms/contents/contentsSearch";
var $defaultWidth = 160;

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  root: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: [],
  
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  titleColumn: null,
  columns: null,
  permissions: null,
  menus: null,
  
  tableMaxHeight: 999999999999,
  multipleSelection: [],

  checkedColumns: [],
  searchColumns: [],

  searchForm: {
    searchType: 'All',
    channelIds: [utils.getQueryInt("siteId")],
    isAllContents: true,
    startDate: null,
    endDate: null,
    checkedLevels: [],
    isTop: false,
    isRecommend: false,
    isHot: false,
    isColor: false,
    groupNames: [],
    tagNames: []
  },
});

var methods = {
  apiTree: function() {
    var $this = this;

    $api.post($url + '/actions/tree', {
      siteId: this.siteId
    }).then(function(response) {
      var res = response.data;

      $this.root = res.root;
      $this.siteUrl = res.siteUrl;
      $this.groupNames = res.groupNames;
      $this.tagNames = res.tagNames;
      $this.checkedLevels = res.checkedLevels;
      $this.titleColumn = res.titleColumn;
      $this.columns = res.columns;
      $this.permissions = res.permissions;
      $this.permissions.isAdd = false;
      var keyword = utils.getQueryString("keyword") || '';
      $this.searchColumns.push({
        attributeName: $this.titleColumn.attributeName,
        displayName: $this.titleColumn.displayName,
        value: keyword
      });

      $this.searchForm.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });
      if (keyword) {
        $this.apiList($this.siteId, 1);
      } else {
        utils.loading($this, false);
      }
    }).catch(function(error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiList: function(useless, page, message) {
    var $this = this;

    var channelIds = [];
    for (var i = 0; i < this.searchForm.channelIds.length; i++) {
      var obj = this.searchForm.channelIds[i];
      if (Array.isArray(obj)) {
        channelIds.push(obj[obj.length - 1]);
      } else {
        channelIds.push(obj);
      }
    }

    var items = [];
    for (var i = 0; i < this.searchColumns.length; i++) {
      var column = this.searchColumns[i];
      if (column.attributeName && column.value) {
        items.push({
          key: column.attributeName,
          value: column.value
        });
      }
    }

    utils.loading(this, true);
    $api.post($url + '/actions/list', {
      siteId: this.siteId,
      searchType: this.searchForm.searchType,
      channelIds: channelIds,
      isAllContents: this.searchForm.isAllContents,
      startDate: this.searchForm.startDate,
      endDate: this.searchForm.endDate,
      items: items,
      page: page,
      isAdvanced: this.isAdvanced,
      isCheckedLevels: this.isCheckedLevels,
      checkedLevels: this.searchForm.checkedLevels,
      isTop: this.searchForm.isTop,
      isRecommend: this.searchForm.isRecommend,
      isHot: this.searchForm.isHot,
      isColor: this.searchForm.isColor,
      groupNames: this.searchForm.groupNames,
      tagNames: this.searchForm.tagNames
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

  btnSelectColumnClick: function(column) {
    var searchColumn = _.find(this.searchColumns, function(o) { return o.attributeName == column.attributeName; });
    if (searchColumn) {
      this.searchColumns.splice(this.searchColumns.indexOf(searchColumn), 1);
    } else {
      this.searchColumns.push({
        attributeName: column.attributeName,
        displayName: column.displayName,
        value: ''
      });
    }
  },

  getColumnEffect: function(column) {
    var searchColumn = _.find(this.searchColumns, function(o) { return o.attributeName == column.attributeName; });
    return searchColumn ? 'dark' : 'plain';
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

  btnTabClick: function () {
    if (this.pageContents) {
      this.btnSearchClick();
    }
  },

  btnSearchClick: function () {
    var $this = this;

    this.$refs.searchForm.validate(function(valid) {
      if (valid) {
        $this.apiList($this.siteId, 1);
      }
    });
  },

  btnEditClick: function(content) {
    utils.addTab('编辑内容', this.getEditUrl(content));
  },

  btnAdminClick: function(adminId) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getCommonUrl('adminLayerView', {adminId: adminId}),
      full: true
    });
  },

  getAddUrl: function() {
    return utils.getCmsUrl('editor', {
      siteId: this.siteId,
      channelId: this.channelId,
      page: this.page,
      tabName: utils.getTabName()
    });
  },

  getEditUrl: function(content) {
    return utils.getCmsUrl('editor', {
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

  btnContentStateClick: function(content) {
    utils.openLayer({
      title: "查看审核状态",
      url: utils.getCmsUrl('contentsLayerState', {
        siteId: content.siteId,
        channelId: content.channelId,
        contentId: content.id
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

  handleHeaderDragend: function(newWidth, oldWidth, column) {
    if (column.columnKey) {
      this.apiWidth(column.columnKey, newWidth);
    }
  },

  handleColumnsChange: function() {
    var listColumns = _.filter(this.columns, function(o) { return o.isList; });
    var attributeNames = _.map(listColumns, function(column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    isAdvanced: function() {
      if (this.searchForm.searchType !== 'All') return true;
      if (this.searchForm.channelIds.length > 1) return true;
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      if (this.searchForm.isTop || this.searchForm.isRecommend || this.searchForm.isHot || this.searchForm.isColor) return true;
      if (this.searchForm.groupNames.length > 0 || this.searchForm.tagNames.length > 0) return true;
      if (this.searchForm.startDate || this.searchForm.endDate) return true;
      for (var i = 0; i < this.searchColumns.length; i++) {
        if (this.searchColumns[i].value) return true;
      }
      return false;
    },

    isCheckedLevels: function() {
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      return false;
    },

    isFiltered: function() {
      if (this.checkedLevels.length !== this.searchForm.checkedLevels.length) return true;
      if (this.searchForm.isTop || this.searchForm.isRecommend || this.searchForm.isHot || this.searchForm.isColor) return true;
      if (this.searchForm.groupNames.length > 0 || this.searchForm.tagNames.length > 0) return true;
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
