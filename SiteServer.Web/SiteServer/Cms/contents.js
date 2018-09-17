var $apiUrl = $apiConfig.apiUrl;
var $api = new apiUtils.Api($apiUrl + "/pages/cms/contents");
var $createApi = new apiUtils.Api($apiUrl + "/pages/cms/contents/actions/create");

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
  pageOptions: null,
  isAllChecked: false
};

var methods = {
  btnAddClick: function () {
    location.href = 'pageContentAdd.aspx?siteId=' + this.siteId + '&channelId=' + this.channelId;
  },
  btnCreateClick: function () {
    this.pageAlert = null;

    var $this = this;

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
  btnFuncClick: function (options) {
    this.pageAlert = null;
    if (options.withoutContents) {
      pageUtils.openLayer({
        title: "批量" + options.title,
        url: "contentsLayer" + options.name + ".cshtml?siteId=" +
          this.siteId +
          "&channelId=" +
          this.channelId,
        full: options.full,
        width: options.width ? options.width : 700,
        height: options.height ? options.height : 500
      });
      return;
    }

    if (options.redirect) {
      location.href =
        "pageContent" + options.name + ".aspx?siteId=" +
        this.siteId +
        "&channelId=" +
        this.channelId +
        "&contentIdCollection=" +
        this.selectedContentIds.join(",");
    } else {
      pageUtils.openLayer({
        title: "批量" + options.title,
        url: "contentsLayer" +
          options.name +
          ".cshtml?siteId=" +
          this.siteId +
          "&channelId=" +
          this.channelId +
          "&contentIds=" +
          this.selectedContentIds.join(","),
        full: options.full,
        width: options.width ? options.width : 700,
        height: options.height ? options.height : 500
      });
    }
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
  onPageSelect(option) {
    this.loadContents(option);
  },
  scrollToTop() {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
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
        if (page === 1) {
          $this.permissions = res.permissions;
        }
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