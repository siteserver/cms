var $url = "/cms/contents/contentsRecycle";
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
  columns: null,
  
  multipleSelection: [],

  checkedColumns: [],
  searchColumns: [],

  channelIds: [],
  searchForm: {
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

  isRestoreForm: false,
  restoreTitle: null,
  restoreForm: {},
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
      $this.columns = res.columns;
      var titleColumn = _.find($this.columns, function(o) { return o.attributeName == 'Title'; });
      $this.searchColumns.push({
        attributeName: titleColumn.attributeName,
        displayName: titleColumn.displayName,
        value: ''
      });
      $this.searchForm.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });

      $this.apiList($this.siteId, 1);
    }).catch(function(error) {
      utils.loading($this, false);
      utils.error(error);
    })
  },

  apiList: function(useless, page, message) {
    var $this = this;

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
    
    var channelId = this.channelIds && this.channelIds.length > 0 ? this.channelIds[this.channelIds.length - 1] : null;

    utils.loading(this, true);
    var request = _.assign({
      siteId: this.siteId,
      isCheckedLevels: this.isCheckedLevels,
      channelId: channelId,
      page: page,
      items: items
    }, this.searchForm);
    $api.post($url + '/actions/list', request).then(function(response) {
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

  apiDelete: function(request, message) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: request
    }).then(function(response) {
      var res = response.data;

      $this.apiList($this.siteId, 1, message);
    }).catch(function(error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  btnDeleteAllClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除全部内容',
      text: '确实要永久性地删除全部内容吗？',
      callback: function () {
        $this.apiDelete({
          siteId: $this.siteId,
          action: 'DeleteAll',
          channelContentIds : '',
          restoreChannelId: 0
        }, '成功删除全部内容！');
      }
    });
  },

  btnDeleteSelectClick: function () {
    var $this = this;

    utils.alertDelete({
      title: '删除选中内容',
      text: '确实要永久性地删除选中内容吗？',
      callback: function () {
        $this.apiDelete({
          siteId: $this.siteId,
          action: 'Delete',
          channelContentIds : $this.channelContentIdsString,
          restoreChannelId: 0
        }, '成功删除选中内容！');
      }
    });
  },

  btnDeleteClick: function (content) {
    var $this = this;

    utils.alertDelete({
      title: '删除内容',
      text: '确实要永久性地删除此内容吗？',
      callback: function () {
        $this.apiDelete({
          siteId: $this.siteId,
          action: 'Delete',
          channelContentIds : content.channelId + '_' + content.id,
          restoreChannelId: 0
        }, '成功删除内容！');
      }
    });
  },

  btnRestoreAllClick: function () {
    this.restoreTitle = '还原全部内容';
    this.restoreForm = {
      action: 'RestoreAll',
      channelContentIds : '',
      restoreChannelIds: null
    }
    this.isRestoreForm = true;
  },

  btnRestoreSelectClick: function () {
    this.restoreTitle = '还原选中内容';
    this.restoreForm = {
      action: 'Restore',
      channelContentIds : this.channelContentIdsString,
      restoreChannelIds: null
    }
    this.isRestoreForm = true;
  },

  btnRestoreClick: function (content) {
    this.restoreTitle = '还原内容';
    this.restoreForm = {
      action: 'Restore',
      channelContentIds : content.channelId + '_' + content.id,
      restoreChannelIds: null
    }
    this.isRestoreForm = true;
  },

  btnRestoreSubmitClick: function () {
    var $this = this;
    this.$refs.restoreForm.validate(function(valid) {
      if (valid) {
        $this.isRestoreForm = false;
        $this.apiDelete({
          siteId: $this.siteId,
          action: $this.restoreForm.action,
          channelContentIds : $this.restoreForm.channelContentIds,
          restoreChannelId: $this.restoreForm.restoreChannelIds[$this.restoreForm.restoreChannelIds.length - 1],
        }, '成功' + $this.restoreTitle + '！');
      }
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

  btnSearchClick: function () {
    var $this = this;

    this.$refs.searchForm.validate(function(valid) {
      if (valid) {
        $this.apiList($this.siteId, 1);
      }
    });
  },

  btnEditClick: function(content) {
    utils.openLayer({
      title: "编辑内容",
      url: this.getEditUrl(content),
      full: true,
      max: true
    });
  },

  btnAdminClick: function(adminId) {
    utils.openLayer({
      title: "管理员查看",
      url: utils.getCommonUrl('adminLayerView', {adminId: adminId}),
      full: true
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
