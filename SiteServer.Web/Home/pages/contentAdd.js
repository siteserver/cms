var $api = new apiUtils.Api(apiUrl + "/home/contentAdd");
var $createApi = new apiUtils.Api(apiUrl + "/home/contents/actions/create");

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
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  contentDict: null,
  count: null,
  pages: null,
  sites: [],
  channels: [],
  site: {},
  channel: {},
  styles: [],
  isTop: false,
  isRecommend: false,
  isHot: false,
  isColor: false,
  groupIds: [],
  tags: []
};

var methods = {
  load: function (res) {
    this.pageConfig = res.value;
    this.sites = res.sites;
    this.channels = res.channels;
    this.site = res.site;
    this.channel = res.channel;
    this.styles = res.styles;
  },
  btnAddClick: function () {
    location.href = 'pageContentAdd.aspx?siteId=' + this.site.id + '&channelId=' + this.channel.Id;
  },
  btnSearchClick: function () {
    parent.location.href = 'pageContentSearch.aspx?siteId=' + this.site.id + '&channelId=' + this.channel.Id;
  },
  btnCreateClick: function () {
    var $this = this;
    $this.pageAlert = null;
    if ($this.selectedContentIds.length === 0) return;

    pageUtils.loading(true);
    $createApi.post({
      siteId: $this.site.id,
      channelId: $this.channel.Id,
      contentIds: $this.selectedContentIds.join(',')
    }, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: "success",
        html: "内容已添加至生成列队！<a href='createStatus.cshtml?siteId=" + $this.site.id + "'>生成进度查看</a>"
      };
    });
  },
  btnFuncClick: function (options) {
    this.pageAlert = null;

    if (options.withoutContents) {
      pageUtils.openLayer({
        title: "批量" + options.title,
        url: "contentsLayer" + options.name + ".cshtml?siteId=" +
          this.site.id +
          "&channelId=" +
          this.channel.Id,
        full: options.full,
        width: options.width ? options.width : 700,
        height: options.height ? options.height : 500
      });
      return;
    }

    if (this.selectedContentIds.length === 0) return;

    if (options.redirect) {
      location.href =
        "pageContent" + options.name + ".aspx?siteId=" +
        this.site.id +
        "&channelId=" +
        this.channel.Id +
        "&contentIdCollection=" +
        this.selectedContentIds.join(",");
    } else {
      pageUtils.openLayer({
        title: "批量" + options.title,
        url: "contentsLayer" +
          options.name +
          ".cshtml?siteId=" +
          this.site.id +
          "&channelId=" +
          this.channel.Id +
          "&contentIds=" +
          this.selectedContentIds.join(","),
        full: options.full,
        width: options.width ? options.width : 700,
        height: options.height ? options.height : 500
      });
    }
  },
  btnContentViewClick: function (contentId) {
    pageUtils.openLayer({
      title: "查看内容",
      url: "contentsLayerView.cshtml?siteId=" +
        this.site.id +
        "&channelId=" +
        this.channel.Id +
        "&contentId=" +
        contentId,
      full: true
    });
  },
  btnContentStateClick: function (contentId) {
    pageUtils.openLayer({
      title: "查看审核状态",
      url: "contentsLayerState.cshtml?siteId=" +
        this.site.id +
        "&channelId=" +
        this.channel.Id +
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
  onSiteSelect: function (site) {
    if (site.id === this.site.id) return;
    var $this = this;
    this.pageLoad = false;
    pageUtils.getConfig({
      pageName: 'contents',
      siteId: site.id
    }, function (res) {
      $this.pageLoad = true;
      $this.load(res.value, res.sites, res.channels, res.site, res.channel);
    });
  },
  onChannelSelect: function (channel) {
    if (channel.id === this.channel.id) return;
    this.channel = channel;
    this.loadContents(1);
  },
  onPageSelect: function (option) {
    this.loadContents(option);
  },
  btnSubmitClick: function () {

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
        siteId: $this.site.id,
        channelId: $this.channel.id,
        page: page
      },
      function (err, res) {
        if ($this.pageLoad) {
          pageUtils.loading(false);
          $this.scrollToTop();
        } else {
          $this.pageLoad = true;
        }

        if (err) {
          $this.pageAlert = {
            type: 'danger',
            html: err.message
          };
          return;
        }

        $this.permissions = res.permissions;
        $this.attributes = res.attributes;

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
        $this.page = page;
        $this.pageOptions = [];
        for (var i = 1; i <= $this.pages; i++) {
          $this.pageOptions.push(i);
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
    var $this = this;
    if (authUtils.isAuthenticated()) {
      pageUtils.getConfig('contentAdd', function (res) {
        if (res.isUserLoggin) {
          $this.load(res);
        } else {
          authUtils.redirectLogin();
        }
      });
    } else {
      authUtils.redirectLogin();
    }
  }
});