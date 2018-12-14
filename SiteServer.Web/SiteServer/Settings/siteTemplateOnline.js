var data = {
  pageLoad: false,
  pageAlert: null,
  page: parseInt(utils.getQueryString('page') || 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),

  templateInfoList: null,
  count: null,
  pages: null,
  allTagNames: []
};

var methods = {
  getDisplayUrl: function (templateId) {
    return 'https://www.siteserver.cn/templates/template.html?id=' + templateId;
  },

  getTemplateUrl: function (relatedUrl) {
    return 'https://templates.siteserver.cn/' + relatedUrl;
  },

  getPreviewUrl: function (templateId) {
    return 'https://demo.siteserver.cn/' + templateId;
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

  getUrl: function (page, word, tag, price, order) {
    var url = '?page=' + page;
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
  },

  load: function () {
    var $this = this;

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
        $this.pageAlert = utils.getPageAlert(error);
      })
      .then(function () {
        $this.pageLoad = true;
      });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});