var $url = "/pages/cms/contents/contents";
var $defaultWidth = 160;

var data = utils.initData({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId") || utils.getQueryInt("siteId"),
  root: null,
  siteUrl: null,
  checkedLevels: null,
  groupNames: null,
  tagNames: null,
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
    tagNames: [],
    startDate: null,
    endDate: null,
    keywords: '',
    description: ''
  }
});

var methods = {
  deleteRow(index, rows) {
    rows.splice(index, 1);
  },
  
  apiTree: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/tree', {
      siteId: this.siteId,
      channelId: this.channelId
    }).then(function(response) {
      var res = response.data;

      $this.root = res.root;
      $this.siteUrl = res.siteUrl;
      $this.checkedLevels = res.checkedLevels;
      $this.groupNames = res.groupNames;
      $this.tagNames = res.tagNames;
      $this.expendedChannelIds = [$this.siteId];
      $this.advancedForm.checkedLevels = _.map(res.checkedLevels, function(x) { return x.label; });
    }).catch(function(error) {
      utils.error($this, error);
    }).then(function() {
      $this.asideHeight = $(window).height() - 4;
      $this.tableMaxHeight = $(window).height() - 128;
      utils.loading($this, false);
    });
  },

  apiList: function(channelId, page, message) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        channelId: channelId,
        searchType: this.searchForm.searchType,
        searchText: this.searchForm.searchText,
        isAllContents: this.searchForm.isAllContents,
        page: page
      }
    }).then(function(response) {
      var res = response.data;
      
      $this.pageContents = res.pageContents;
      $this.permissions = res.permissions;
      $this.columns = res.columns;
      $this.total = res.total;
      $this.pageSize = res.pageSize;
      $this.page = page;
      $this.expendedChannelIds = [$this.siteId, channelId];
      $this.searchForm.isAllContents = res.isAllContents;

      if (message) {
        $this.$message.success(message);
      }
    }).catch(function(error) {
      utils.error($this, error);
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
      utils.error($this, error);
    });
  },

  getChannelUrl: function(data) {
    return '../redirect.cshtml?siteId=' + this.siteId + '&channelId=' + data.value;
  },

  getContentUrl: function (content) {
    if (content.checked) {
      return '../redirect.cshtml?siteId=' + content.siteId + '&channelId=' + content.channelId + '&contentId=' + content.id;
    }
    return apiUrl + '/preview/' + content.siteId + '/' + content.channelId + '/' + content.id;
  },

  btnSearchClick: function() {
    this.apiList(this.channelId, 1);
  },

  btnAdvancedSubmitClick: function() {
    var $this = this;
    this.$refs.advancedForm.validate(function(valid) {
      if (valid) {
        console.log($this.advancedForm);
      }
    });
  },

  btnAdvancedCancelClick: function() {
    this.isAdvancedForm = false;
  },

  btnAddClick: function (command) {
    if (command === 'Word') {
      this.btnLayerClick({title: '批量导入Word', name: 'Word', full: true});
    } else if (command === 'Import') {
      this.btnLayerClick({title: '批量导入', name: 'Import', full: true});
    } else {
      top.utils.openLayer({
        title: "添加内容",
        url: this.getAddUrl(),
        full: true,
        max: true
      });
    }
  },

  btnMoreClick: function(command) {
    if (command === 'ExportAll') {
      this.btnLayerClick({title: '导出全部', name: 'Export', full: true, withOptionalContents: true});
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
    utils.openLayer({
      title: "编辑内容",
      url: this.getEditUrl(content),
      full: true,
      max: true
    });
  },

  getAddUrl: function() {
    return "cms/editor.cshtml?siteId=" +
    this.siteId +
    "&channelId=" +
    this.channelId +
    "&returnUrl=" +
    encodeURIComponent(location.href);
  },

  getEditUrl: function(content) {
    return (
      "editor.cshtml?siteId=" +
      this.siteId +
      "&channelId=" +
      content.channelId +
      "&contentId=" +
      content.id +
      "&returnUrl=" +
      encodeURIComponent(location.href)
    );
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

      parent.$vue.openPageCreateStatus();
    }).catch(function(error) {
      utils.error($this, error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  btnLayerClick: function(options) {
    var url = "contentsLayer" + options.name + ".cshtml?siteId=" + this.siteId;

    if (options.channelId) {
      url += "&channelId=" + options.channelId;
    } else {
      url += "&channelId=" + this.channelId;
    }
    url += '&page=' + this.page;
    if (options.contentId) {
      url += "&contentId=" + options.contentId;
    }

    if (options.withContents) {
      if (!this.isContentChecked) return;
      url += "&channelContentIds=" + this.channelContentIdsString;
    }

    if (options.withOptionalContents) {
      if (this.isContentChecked) {
        url += "&channelContentIds=" + this.channelContentIdsString;
      }
    }

    options.url = url;

    utils.openLayer(options);
  },

  btnContentViewClick: function(contentId) {
    utils.openLayer({
      title: "查看内容",
      url:
        "contentsLayerView.cshtml?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&contentId=" +
        contentId,
      full: true
    });
  },

  btnContentStateClick: function(contentId) {
    utils.openLayer({
      title: "查看审核状态",
      url:
        "contentsLayerState.cshtml?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&contentId=" +
        contentId,
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

  getPluginMenuUrl: function(pluginMenu) {
    return pluginMenu.href + "&returnUrl=" + encodeURIComponent(location.href);
  },

  btnPluginMenuClick: function(pluginMenu) {
    if (pluginMenu.target === "_layer") {
      utils.openLayer({
        title: pluginMenu.text,
        url: this.getPluginMenuUrl(pluginMenu),
        full: true
      });
    }
  },

  treeIconClick: function(data) {
    return false;
  },

  handleNodeClick: function(data) {
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
    if (column.attributeName === 'Sequence' || column.attributeName === 'Id' || column.attributeName === 'Hits') {
      return 70;
    }
    if (column.attributeName === 'Guid') {
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
  },

  getUrl: function(virtualUrl) {
    if (!virtualUrl) return '';
    return _.replace(virtualUrl, '@/', this.siteUrl + '/');
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
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
    this.apiTree();
  }
});
