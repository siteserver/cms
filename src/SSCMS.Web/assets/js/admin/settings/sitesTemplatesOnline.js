var $url = '/settings/sitesTemplatesOnline';

var data = utils.init({
  siteAddPermission: false,
  page: utils.getQueryInt('page', 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),
  templateInfoList: null,
  count: null,
  pages: null,
  allTagNames: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.siteAddPermission = res.siteAddPermission;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      $this.load();
    });
  },

  load: function () {
    var $this = this;

    utils.loading(this, true);
    $apiCloud.get('templates', {
        params: {
          page: this.page,
          word: this.word,
          tag: this.tag,
          price: this.price,
          order: this.order
        }
      })
      .then(function (response) {
        var res = response.data;

        $this.templateInfoList = res.value;
        $this.count = res.count;
        $this.pages = res.pages;
        $this.allTagNames = res.allTagNames;
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },
  
  getDisplayUrl: function (templateId) {
    return 'https://www.siteserver.cn/templates/template.html?id=' + templateId;
  },

  getTemplateUrl: function (relatedUrl) {
    return 'https://www.siteserver.cn/templates/' + relatedUrl;
  },

  btnImageClick: function(templateId) {
    window.open(this.getDisplayUrl(templateId));
  },

  btnPreviewClick: function (templateId) {
    window.open('https://demo.siteserver.cn/' + templateId);
  },

  btnCreateClick: function(templateId) {
    location.href = utils.getSettingsUrl('sitesAdd', {
      type: 'submit',
      createType: 'cloud',
      createTemplateId: templateId
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
    this.load();
  },

  orderChanged: function () {
    this.load();
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