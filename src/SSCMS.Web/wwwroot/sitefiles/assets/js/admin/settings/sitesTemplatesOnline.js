var $url = '/settings/sitesTemplatesOnline';

var data = utils.init({
  siteAddPermission: false,
  page: utils.getQueryInt('page', 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),
  buy: utils.getQueryString('buy'),
  themes: null,
  count: null,
  pages: null,
  tags: [],
  orderedGuids: [],
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.siteAddPermission = res.siteAddPermission;
      $this.cloudApiGet();
    }).catch(function (error) {
      utils.error(error);
      utils.loading($this, false);
    });
  },

  cloudApiGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.getThemes(this.page, this.word, this.tag, this.price, this.order)
    .then(function (response) {
      var res = response.data;

      $this.themes = res.themes;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.tags = res.tags;
      $this.orderedGuids = res.orderedGuids;

      if ($this.buy) {
        var userName = $this.buy.split('.')[0];
        var name = $this.buy.split('.')[1];
        $this.btnBuyClick(userName, name);
      }
    }).catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getTemplatesUrl: function() {
    return cloud.host + '/templates/';
  },

  getDisplayUrl: function (theme) {
    return cloud.getThemesUrl('template.html?userName=' + theme.userName + '&name=' + theme.name);
  },

  getCoverUrl: function (theme) {
    return cloud.hostStorage + '/themes/' + theme.userName + '/' + theme.name + '/' + _.trim(theme.coverUrl, '/');
  },

  isCreatable: function (theme) {
    return theme.price === 0 || this.orderedGuids.indexOf(theme.guid) !== -1;
  },

  btnImageClick: function(theme) {
    window.open(this.getDisplayUrl(theme));
  },

  btnPreviewClick: function (theme) {
    window.open(cloud.hostDemo + '/' + theme.userName + '/' + theme.name + '/');
  },

  btnCreateClick: function(theme) {
    location.href = utils.getSettingsUrl('sitesAdd', {
      type: 'submit',
      createType: 'cloud',
      cloudThemeUserName: theme.userName,
      cloudThemeName: theme.name,
      isCloudThemeFree: theme.price === 0
    });
  },

  getPageUrl: function (page) {
    if (page < 1 || page > this.pages || page == this.page) return 'javascript:;';
    return this.getUrl(page, this.word, this.tag, this.price, this.order);
  },

  getTagUrl: function (tag) {
    return this.getUrl(this.page, this.word, tag, this.price, this.order);
  },

  getPriceUrl: function (price) {
    return this.getUrl(this.page, this.word, this.tag, price, this.order);
  },

  getOrderUrl: function (order) {
    return this.getUrl(this.page, this.word, this.tag, this.price, order);
  },

  btnRedirectClick: function(url) {
    location.href = url;
  },

  handlePageChange: function(page) {
    location.href = this.getPageUrl(page);
  },

  getUrl: function (page, word, tag, price, order) {
    var url = '?type=selectCloud&page=' + page;
    if (word) {
      url += '&word=' + word;
    }
    if (tag) {
      url += '&tag=' + tag;
    }
    if (price) {
      url += '&price=' + (price);
    }
    if (order) {
      url += '&order=' + (order);
    }
    return url;
  },

  priceChanged: function () {
    this.cloudApiGet();
  },

  orderChanged: function () {
    this.cloudApiGet();
  },

  getThemeUrl: function(theme) {
    if (theme) {
      return cloud.host + '/templates/template.html?userName=' + encodeURIComponent(theme.userName) + '&name=' + encodeURIComponent(theme.name);
    }
    return 'javascript:;';
  },

  btnBuyClick: function(userName, name) {
    var $this = this;
    var url = utils.addQuery(location.href, {
      buy: userName + '.' + name
    });
    cloud.checkAuth(function() {
      utils.openLayer({
        title: '购买',
        width: 600,
        height: 500,
        url: cloud.host + '/layer/pay.html?resourceType=Theme&userName=' + userName + '&name=' + name
      });

      window.addEventListener(
        'message',
        function(e) {
          if (e.origin !== cloud.host) return;
          var userName = e.data.userName;
          var name = e.data.name;
          if (userName && name) {
            $this.btnCreateClick(e.data);
          }
        },
        false,
      );
    }, url);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCloseClick);
    this.apiGet();
  }
});
