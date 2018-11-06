var $api = new utils.Api('/home/contents');

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
  page: 1,
  pageContents: [],
  count: null,
  pages: null,
  sites: [],
  channels: [],
  site: {},
  channel: {},
  permissions: {},
  columns: null,
  pageOptions: null,
  isAllChecked: false
};

var methods = {
  load: function (pageConfig, sites, channels, site, channel) {
    this.pageConfig = pageConfig;
    this.sites = sites;
    this.channels = channels;
    this.loadChannel(site, channel);
  },

  loadChannel: function (site, channel) {
    this.site = site;
    this.channel = channel;
    if (this.site && this.channel) {
      Cookies.set('SS-USER-SITE-ID', this.site.id, {
        expires: 7
      });
      Cookies.set('SS-USER-CHANNEL-ID', this.channel.id, {
        expires: 7
      });

      this.loadContents(1);
    } else {
      this.pageType = 'Unauthorized'
    }
  },

  btnAddClick: function () {
    parent.location.hash = 'pages/contentAdd.html?siteId=' + this.site.id + '&channelId=' + this.channel.id + '&returnUrl=' + encodeURIComponent(parent.location.hash);
  },

  btnEditClick: function (contentId) {
    parent.location.hash = 'pages/contentAdd.html?siteId=' + this.site.id + '&channelId=' + this.channel.id + '&contentId=' + contentId + '&returnUrl=' + encodeURIComponent(parent.location.hash);
  },

  btnLayerClick: function (options) {
    event.stopPropagation();

    this.pageAlert = null;
    var url = "pages/contentsLayer" +
      options.name +
      ".html?siteId=" +
      this.site.id +
      "&channelId=" +
      this.channel.id;
    if (options.withContents) {
      if (this.selectedContentIds.length === 0) return;
      url += "&contentIds=" + this.selectedContentIds.join(",")
    } else if (options.contentId) {
      url += "&contentId=" + options.contentId
    }
    url += '&returnUrl=' + encodeURIComponent(parent.location.hash);

    parent.utils.openLayer({
      title: options.title,
      url: url,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
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

  onSiteSelect(site) {
    if (site.id === this.site.id) return;
    var $this = this;
    this.pageLoad = false;
    utils.getConfig({
      pageName: 'contents',
      siteId: site.id
    }, function (res) {
      $this.pageLoad = true;
      $this.load(res.value, res.sites, res.channels, res.site, res.channel);
    });
  },

  onChannelSelect(channel) {
    if (channel.id === this.channel.id) return;
    this.loadChannel(this.site, channel);
    this.loadContents(1);
  },

  onPageSelect(option) {
    this.loadContents(option);
  },

  loadContents: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      parent.utils.loading(true);
    }

    $api.get({
        siteId: $this.site.id,
        channelId: $this.channel.id,
        page: page
      },
      function (err, res) {
        if ($this.pageLoad) {
          parent.utils.loading(false);
          parent.utils.scrollToTop();
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
        $this.columns = res.columns;

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
    var siteId = parseInt(utils.getQueryString('siteId') || Cookies.get('SS-USER-SITE-ID') || 0);
    var channelId = parseInt(utils.getQueryString('channelId') || Cookies.get('SS-USER-CHANNEL-ID') || 0);

    utils.getConfig({
      pageName: 'contents',
      siteId: siteId,
      channelId: channelId
    }, function (res) {
      if (res.value) {
        $this.load(res.config, res.sites, res.channels, res.site, res.channel);
      } else {
        utils.redirectLogin();
      }
    });
  }
});