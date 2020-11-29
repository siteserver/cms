var $url = '/settings/sitesTemplatesOnline';

var data = utils.init({
  siteAddPermission: false,
  page: utils.getQueryInt('page', 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),
  themes: null,
  count: null,
  pages: null,
  tags: []
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
    cloud.getThemes(this.page, this.word, this.tag, this.price, this.order).then(function (response) {
      var res = response.data;

      $this.themes = res.themes;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.tags = res.tags;
    }).catch(function (error) {
      utils.error(error);
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
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});