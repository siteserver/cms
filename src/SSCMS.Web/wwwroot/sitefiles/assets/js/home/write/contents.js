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
  
  isAdd: null,
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  titleColumn: null,
  columns: null,
  menus: null,
  
  tableMaxHeight: 999999999999,
  multipleSelection: [],

  checkedColumns: [],

  siteId: 0,
  channelIds: [],
  permissions: {
    isEdit: true
  }
});

var methods = {
  apiGet: function() {
    var $this = this;

    $api.get($url).then(function(response) {
      var res = response.data;

      if (res.unauthorized) {
        $this.pageType = 'Unauthorized';
        utils.loading($this, false);
        return;
      }

      $this.sites = res.sites;
      $this.siteId = res.siteId;
      $this.siteName = res.siteName;
      $this.siteUrl = res.siteUrl;
      $this.root = res.root;
      $this.titleColumn = res.titleColumn;
      $this.columns = res.columns;

      $this.allCheckedLevels = res.checkedLevels;
      $this.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });
      $this.apiList(1);
    }).catch(function(error) {
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiList: function(page) {
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
      
      $this.isAdd = res.isAdd;
      $this.pageContents = res.pageContents;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;
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

  btnSearchClick: function() {
    this.apiList(1);
  },

  btnTitleClick: function(content) {
    if (content.checked && content.channelId > 0) return false;
    utils.openLayer({
      title: "查看内容",
      url: utils.getRootUrl('write/contentsLayerView', {
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
      full: true
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
      page: this.page,
      tabName: utils.getTabName()
    });
  },

  btnAddClick: function () {
    utils.addTab('添加内容', this.getAddUrl());
  },

  btnLayerClick: function(options) {
    var query = {
      siteId: this.siteId, 
      channelId: this.channelId,
      page: this.page
    };

    if (options.withContents) {
      if (!this.isContentChecked) return;
      query.channelContentIds = this.channelContentIdsString;
    }

    options.url = utils.getRootUrl('write/contentsLayer' + options.name, query);
    utils.openLayer(options);
  },

  btnContentViewClick: function(contentId) {
    utils.openLayer({
      title: "查看内容",
      url: utils.getRootUrl('write/contentsLayerView', {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: contentId
      }),
      full: true
    });
  },

  btnContentStateClick: function(content) {
    utils.openLayer({
      title: "查看审核状态",
      url: utils.getRootUrl('write/contentsLayerState', {
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
    this.apiList(val);
  },

  handleColumnsChange: function() {
    var listColumns = _.filter(this.columns, function(o) { return o.isList; });
    var attributeNames = _.map(listColumns, function(column) {
      return column.attributeName;
    });
    this.apiColumns(attributeNames);
  },

  handleHeaderDragend: function(newWidth, oldWidth, column) {
    
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    channelId: function() {
      return this.channelIds.length === 0 ? 0 : this.channelIds[this.channelIds.length - 1];
    },
    
    isCheckedLevels: function() {
      if (this.checkedLevels.length !== this.allCheckedLevels.length) return true;
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
    this.apiGet();
  }
});
