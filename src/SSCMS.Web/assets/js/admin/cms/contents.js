var $url = "/cms/contents/contents";
var $defaultWidth = 160;

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId") || utils.getQueryInt("siteId"),
  root: null,
  siteUrl: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: [],
  expendedChannelIds: [],

  filterText: '',
  
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  permissions: null,
  columns: null,
  
  asideHeight: 0,
  tableMaxHeight: 0,
  multipleSelection: [],

  checkedColumns: [],

  searchForm: {
    searchType: 'Title',
    searchText: '',
    isAllContents: false
  },

  isAdvancedForm: false,
  advancedForm: {
    checkedLevels: [],
    isTop: false,
    isRecommend: false,
    isHot: false,
    isColor: false,
    groupNames: [],
    tagNames: []
  }
});

var methods = {
  apiTree: function(reload) {
    var $this = this;

    $api.post($url + '/actions/tree', {
      siteId: this.siteId,
      reload: reload
    }).then(function(response) {
      var res = response.data;

      $this.root = res.root;
      if (!reload) {
        $this.siteUrl = res.siteUrl;
        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.checkedLevels = res.checkedLevels;
        $this.advancedForm.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });
        $this.expendedChannelIds = [$this.siteId];
      }else{
        $this.expendedChannelIds = [$this.siteId, $this.channelId];
      }
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      $this.asideHeight = $(window).height() - 4;
      $this.tableMaxHeight = $(window).height() - 128;
      utils.loading($this, false);
    });
  },

  apiList: function(channelId, page, message, reload) {
    var $this = this;

    utils.loading(this, true);
    var request = _.assign({
      siteId: this.siteId,
      channelId: channelId,
      page: page,
      searchType: this.searchForm.searchType,
      searchText: this.searchForm.searchText,
      isAdvanced: this.isAdvanced
    }, this.advancedForm);
    $api.post($url + '/actions/list', request).then(function(response) {
      var res = response.data;
      
      $this.pageContents = res.pageContents;
      $this.columns = res.columns;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;
      $this.permissions = res.permissions;
      $this.expendedChannelIds = [$this.siteId, channelId];
      $this.searchForm.isAllContents = res.isAllContents;

      if (message) {
        utils.success(message);
      }
      if (reload) {
        $this.apiTree(true);
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
      channelId: this.channelId,
      attributeNames: attributeNames
    }).then(function(response) {
      var res = response.data;

    }).catch(function(error) {
      utils.error(error);
    });
  },

  handleAllChange: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/all', {
      siteId: this.siteId,
      channelId: this.channelId,
      isAllContents: this.searchForm.isAllContents
    }).then(function(response) {
      var res = response.data;

      $this.apiList($this.channelId, 1);
    }).catch(function(error) {
      utils.error(error);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
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

  btnSearchClick: function() {
    this.isAdvancedForm = false;
    this.apiList(this.channelId, 1);
  },

  handleTagNamesChange: function(visible) {
    if (!visible) {
      this.isAdvancedForm = false;
      this.apiList(this.channelId, 1);
    }
  },

  btnAddClick: function (command) {
    if (command === 'Word') {
      this.btnLayerClick({title: '批量导入Word', name: 'Word', full: true});
    } else if (command === 'Import') {
      this.btnLayerClick({title: '批量导入', name: 'Import', full: true});
    } else {
      utils.addTab('添加内容', this.getAddUrl());
    }
  },

  btnMoreClick: function(command) {
    if (command === 'ExportAll') {
      this.btnLayerClick({title: '导出全部', name: 'Export', full: true});
    } else if (command === 'ExportSelected') {
      this.btnLayerClick({title: '导出选中', name: 'Export', full: true, withContents: true});
    } else if (command === 'Arrange') {
      this.btnLayerClick({title: '整理排序', name: 'Arrange', width: 550, height: 350});
    } else if (command === 'Hits') {
      this.btnLayerClick({title: '设置点击量', name: 'Hits', width: 450, height: 320, withContents: true});
    }
  },

  btnEditClick: function(content) {
    // location.href = 'pageContentAdd.aspx?siteId=' + this.siteId + '&channelId=' + this.channelId + '&contentId=' + this.contentId;
    // utils.openLayer({
    //   title: "编辑内容",
    //   url: this.getEditUrl(content),
    //   full: true,
    //   max: true
    // });
    utils.addTab('编辑内容', this.getEditUrl(content));
  },

  btnAdminClick: function(adminId) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getSharedUrl(adminLayerView, {adminId: adminId}),
      width: 550,
      height: 450
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
      page: this.page,
      tabName: utils.getTabName()
    });
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
      query.channelId = this.channelId;
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
        channelId: this.channelId,
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
        channelId: this.channelId,
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

  handleChannelClick: function(data) {
    this.channelId = data.value;
    this.apiList(data.value, 1);
  },

  filterNode: function(value, data) {
    if (!value) return true;
    return data.label.indexOf(value) !== -1;
  },

  handleSelectionChange: function(val) {
    this.multipleSelection = val;
  },

  toggleSelection: function(row) {
    this.$refs.multipleTable.toggleRowSelection(row);
  },

  handleCurrentChange: function(val) {
    this.apiList(this.channelId, val);
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
    isAdvanced: function() {
      if (this.checkedLevels.length !== this.advancedForm.checkedLevels.length) return true;
      if (this.advancedForm.isTop || this.advancedForm.isRecommend || this.advancedForm.isHot || this.advancedForm.isColor) return true;
      if (this.advancedForm.groupNames.length > 0 || this.advancedForm.tagNames.length > 0) return true;
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
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter(val);
    }
  },
  created: function() {
    this.apiTree(false);
  }
});
