var $api = new apiUtils.Api(apiUrl + "/pages/cms/contents");
var $createApi = new apiUtils.Api(apiUrl + "/pages/cms/contents/actions/create");

Object.defineProperty(Object.prototype, "getProp", {
  value: function (prop) {
    var key, self = this;
    for (key in self) {
      if (key.toLowerCase() == prop.toLowerCase()) {
        return self[key];
      }
    }
  }
});

var data = {
  siteId: parseInt(pageUtils.getQueryStringByName("siteId")),
  channelId: parseInt(pageUtils.getQueryStringByName("channelId")),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  page: 1,
  pageContents: null,
  count: null,
  pages: null,
  permissions: null,
  columns: null,
  pageOptions: null,
  isAllChecked: false
};

var methods = {
  btnAddClick: function (e) {
    e.stopPropagation();
    location.href = 'pageContentAdd.aspx?siteId=' + this.siteId + '&channelId=' + this.channelId;
  },

  btnCreateClick: function (e) {
    e.stopPropagation();

    var $this = this;
    $this.pageAlert = null;
    if ($this.selectedContentIds.length === 0) return;

    pageUtils.loading(true);
    $createApi.post({
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentIds: $this.selectedContentIds.join(',')
    }, function (err, res) {
      if (err || !res || !res.value) return;
      pageUtils.loading(false);
      $this.pageAlert = {
        type: "success",
        html: "内容已添加至生成列队！<a href='createStatus.cshtml?siteId=" + $this.siteId + "'>生成进度查看</a>"
      };
    });
  },

  btnLayerClick: function (options, e) {
    e.stopPropagation();

    this.pageAlert = null;
    var url = "contentsLayer" +
      options.name +
      ".cshtml?siteId=" +
      this.siteId +
      "&channelId=" +
      this.channelId;
    if (options.withContents) {
      if (this.selectedContentIds.length === 0) return;
      url += "&contentIds=" + this.selectedContentIds.join(",")
    } else if (options.contentId) {
      url += "&contentId=" + options.contentId
    }
    url += '&returnUrl=' + encodeURIComponent(location.href);

    pageUtils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnContentViewClick: function (contentId, e) {
    e.stopPropagation();

    pageUtils.openLayer({
      title: "查看内容",
      url: "contentsLayerView.cshtml?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&contentId=" +
        contentId,
      full: true
    });
  },

  btnContentStateClick: function (contentId, e) {
    e.stopPropagation();

    pageUtils.openLayer({
      title: "查看审核状态",
      url: "contentsLayerState.cshtml?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&contentId=" +
        contentId,
      full: true
    });
  },

  toggleChecked: function (content) {
    content.isSelected = !content.isSelected;
    if (!content.isSelected) {
      this.isAllChecked = false;
    }
  },

  selectAll: function () {
    this.isAllChecked = !this.isAllChecked;
    for (var i = 0; i < this.pageContents.length; i++) {
      this.pageContents[i].isSelected = this.isAllChecked;
    }
  },

  loadFirstPage: function () {
    if (this.page === 1) return;
    this.loadContents(1);
  },

  loadPrevPage: function () {
    if (this.page - 1 <= 0) return;
    this.loadContents(this.page - 1);
  },

  loadNextPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.page + 1);
  },

  loadLastPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadContents(this.pages);
  },

  onPageSelect: function (option) {
    this.loadContents(option);
  },

  scrollToTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  getPluginMenuUrl: function (pluginMenu) {
    return pluginMenu.href + '&returnUrl=' + encodeURIComponent(location.href);
  },

  btnPluginMenuClick: function (pluginMenu, e) {
    e.stopPropagation();

    if (pluginMenu.target === '_layer') {
      pageUtils.openLayer({
        title: pluginMenu.text,
        url: this.getPluginMenuUrl(pluginMenu),
        full: true
      });
    }
  },

  loadContents: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      pageUtils.loading(true);
    }

    $api.get({
        siteId: $this.siteId,
        channelId: $this.channelId,
        page: page
      },
      function (err, res) {
        if (err || !res || !res.value) return;

        var pageContents = [];
        for (var i = 0; i < res.value.length; i++) {

          var content = _.assign({}, res.value[i], {
            isSelected: false
          });
          pageContents.push(content);
        }
        $this.pageContents = pageContents;
        $this.count = res.count;
        $this.pages = res.pages;
        $this.permissions = res.permissions;
        $this.columns = res.columns;
        $this.page = page;
        $this.pageOptions = [];
        for (var i = 1; i <= $this.pages; i++) {
          $this.pageOptions.push(i);
        }

        if ($this.pageLoad) {
          pageUtils.loading(false);
          $this.scrollToTop();
        } else {
          $this.pageLoad = true;
        }
      }
    );
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  computed: {
    selectedContentIds: function () {
      var retval = [];
      if (this.pageContents) {
        for (var i = 0; i < this.pageContents.length; i++) {
          if (this.pageContents[i].isSelected) {
            retval.push(this.pageContents[i].id);
          }
        }
      }
      return retval;
    }
  },
  created: function () {
    this.loadContents(1);
  }
});