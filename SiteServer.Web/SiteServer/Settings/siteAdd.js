var $url = '/pages/settings/siteAdd';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: utils.getQueryString('type') || 'selectType',

  siteTemplates: null,
  isRootExists: null,
  siteList: null,
  tableNameList: null,

  page: parseInt(utils.getQueryString('page') || 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),
  templateInfoList: null,
  count: null,
  pages: null,
  allTagNames: [],

  createType: utils.getQueryString('createType'),
  createTemplateId: utils.getQueryString('createTemplateId'),
  siteName: '',
  isRoot: false,
  parentId: 0,
  siteDir: '',
  tableRule: 'Choose',
  tableChoose: '',
  tableHandWrite: '',
  isImportContents: true,
  isImportTableStyles: true
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.siteTemplates = res.value;
      $this.isRootExists = res.isRootExists;
      $this.siteList = res.siteList;
      $this.tableNameList = res.tableNameList;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      if ($this.pageType == 'selectCloud') {
        $this.load();
      }
    });
  },

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
  },

  load: function () {
    var $this = this;

    if (this.pageLoad) {
      utils.loading(true);
    }
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
        utils.loading(false);
        this.pageLoad = true;
      });
  },

  getCreateType: function () {
    if (this.createType == 'cloud') {
      return '使用在线站点模板创建站点，站点模板：<a href="' + this.getDisplayUrl(this.createTemplateId) + '" target="_blank">' + this.createTemplateId + '</a>';
    } else if (this.createType == 'local') {
      return '使用本地站点模板创建站点，站点模板：' + this.createTemplateId;
    } else {
      return '创建空站点（不使用站点模板）';
    }
  },

  btnLocalClick: function () {
    this.pageType = 'selectLocal';
    this.createType = 'local';
  },

  btnCloudClick: function () {
    this.pageType = 'selectCloud';
    this.createType = 'cloud';
    this.load();
  },

  btnCreateClick: function (createType, templateId) {
    this.createType = createType;
    this.createTemplateId = templateId;
    this.pageType = 'create';
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);
        $api.post($url, {
          createType: $this.createType,
          createTemplateId: $this.createTemplateId,
          siteName: $this.siteName,
          isRoot: $this.isRoot,
          parentId: $this.parentId,
          siteDir: $this.siteDir,
          tableRule: $this.tableRule,
          tableChoose: $this.tableChoose,
          tableHandWrite: $this.tableHandWrite,
          isImportContents: $this.isImportContents,
          isImportTableStyles: $this.isImportTableStyles
        }).then(function (response) {
          var res = response.data;
          location.href = res.value;
        }).catch(function (error) {
          utils.loading(false);
          $this.pageAlert = utils.getPageAlert(error);
        });
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});