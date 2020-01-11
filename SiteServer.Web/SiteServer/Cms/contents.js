var $url = "/pages/cms/contents";

Object.defineProperty(Object.prototype, "getProp", {
  value: function(prop) {
    var key,
      self = this;
    for (key in self) {
      if (key.toLowerCase() == prop.toLowerCase()) {
        return self[key];
      }
    }
  }
});

var data = {
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId") || utils.getQueryInt("siteId"),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  loading: false,

  searchType: 'title',
  searchText: '',

  filterText: '',
  treeData: null,
  pageContents: null,
  total: null,
  pageSize: null,
  page: 1,
  permissions: null,
  columns: null,
  isAllContents: false,
  
  asideHeight: 0,
  tableMaxHeight: 0,
  multipleSelection: [],

  advancedForm: {
    startDate: null,
    endDate: null,
    keywords: '',
    description: ''
  },
  tableData: [{
    id: 1,
    date: '2016-05-02',
    name: '王小虎',
    address: '上海市普陀区金沙江路 1518 弄'
  }],
};

var methods = {
  deleteRow(index, rows) {
    rows.splice(index, 1);
  },
  
  apiConfig: function() {
    var $this = this;

    $api
      .get($url + '/channels', {
        params: {
          siteId: $this.siteId
        }
      })
      .then(function(response) {
        var res = response.data;

        $this.treeData = [res];
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.asideHeight = $(window).height();
        $this.tableMaxHeight = $(window).height() - 85;
        $this.pageLoad = true;
      });
  },

  apiList: function(channelId, page) {
    var $this = this;

    $this.loading = true;
    $api
      .get($url, {
        params: {
          siteId: $this.siteId,
          channelId: channelId,
          page: page
        }
      })
      .then(function(response) {
        var res = response.data;

        if (!$this.treeData) {
          $this.treeData = [res.rootChannel];
        }
        
        // var pageContents = [];
        // for (var i = 0; i < res.pageContents.length; i++) {
        //   var content = _.assign({}, res.pageContents[i], {
        //     isSelected: false
        //   });
        //   pageContents.push(content);
        // }
        // $this.pageContents = pageContents;
        $this.pageContents = res.pageContents;
        $this.permissions = res.permissions;
        $this.columns = res.columns;
        $this.isAllContents = res.isAllContents;
        $this.total = res.total;
        $this.pageSize = res.pageSize;
        $this.page = page;
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.loading = false;
        $this.scrollToTop();
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

  btnAdvancedSubmitClick: function() {

  },

  btnAdvancedCancelClick: function() {

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

  btnExportClick: function(command) {
    this.btnLayerClick({title: '批量导出', name: 'Export', full: true, withOptionalContents: true});
  },

  btnMoreClick: function(command) {
    if (command === 'Options') {
      this.btnLayerClick({title: '设置选项', name: 'Options', full: true});
    } else if (command === 'Arrange') {
      this.btnLayerClick({title: '批量整理', name: 'Arrange', width: 550, height: 350});
    } else if (command === 'Attributes') {
      this.btnLayerClick({title: '批量设置属性', name: 'Attributes', width: 450, height: 320, withContents: true});
    } else if (command === 'Group') {
      this.btnLayerClick({title: '批量设置组别', name: 'Group', withContents: true});
    } else if (command === 'Taxis') {
      this.btnLayerClick({title: '批量排序', name: 'Taxis', width: 450, height: 280, withContents: true});
    } else if (command === 'Cut') {
      this.btnLayerClick({title: '批量转移', name: 'Cut', full: true, withContents: true});
    } else if (command === 'Copy') {
      this.btnLayerClick({title: '批量复制', name: '', full: true, withContents: true});
    }
  },

  btnEditClick: function(content) {
    top.utils.openLayer({
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
      "cms/editor.cshtml?siteId=" +
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
    this.pageAlert = null;
    if (!this.isContentChecked) return;

    $this.loading = true;
    $api
      .post($url + "/actions/create", {
        siteId: $this.siteId,
        channelContentIds: this.channelContentIds
      })
      .then(function(response) {
        var res = response.data;

        parent.openPageCreateStatus();
      })
      .catch(function(error) {
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function() {
        $this.loading = false;
      });
  },

  btnLayerClick: function(options) {
    this.pageAlert = null;
    var url = "contentsLayer" + options.name + ".cshtml?siteId=" + this.siteId;

    if (options.channelId) {
      url += "&channelId=" + options.channelId;
    } else {
      url += "&channelId=" + this.channelId;
    }
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
    url += "&returnUrl=" + encodeURIComponent(location.href);

    utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
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

  handleCurrentChange: function(val) {
    this.apiList(this.channelId, val);
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
        retVal.push({
          channelId: content.channelId,
          id: content.id
        });
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
    this.apiConfig();
  }
});
