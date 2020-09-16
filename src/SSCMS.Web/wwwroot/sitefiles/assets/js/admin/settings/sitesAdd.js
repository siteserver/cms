var $url = '/settings/sitesAdd';

var data = utils.init({
  pageType: utils.getQueryString('type') || 'selectType',

  siteTypes: null,
  siteTemplates: null,
  rootExists: null,
  sites: null,
  tableNameList: null,

  page: utils.getQueryInt('page', 1),
  word: utils.getQueryString('word'),
  tag: utils.getQueryString('tag'),
  price: utils.getQueryString('price'),
  order: utils.getQueryString('order'),
  templates: null,
  count: null,
  pages: null,
  allTagNames: [],
  
  parentIds: [0],
  form: {
    guid: null,
    siteType: null,
    createType: utils.getQueryString('createType'),
    createTemplateId: utils.getQueryString('createTemplateId'),
    siteName: '',
    root: false,
    parentId: 0,
    siteDir: '',
    tableRule: 'Create',
    tableChoose: '',
    tableHandWrite: '',
    isImportContents: true,
    isImportTableStyles: true
  },

  total: 1,
  current: 0,
  message: '',
  success: false
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.siteTypes = res.siteTypes;
      $this.siteTemplates = res.siteTemplates;
      $this.rootExists = res.rootExists;
      $this.sites = res.sites;
      $this.tableNameList = res.tableNameList;
      $this.form.guid = res.guid;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
      if ($this.pageType == 'selectCloud') {
        $this.load();
      }
    });
  },

  apiSubmit: function() {
    var $this = this;

    var interval = setTimeout(function () {
      $this.apiProcess();
    }, 3000);

    utils.loading(this, true);
    this.form.parentId = this.parentIds && this.parentIds.length > 0 ? this.parentIds[this.parentIds.length - 1] : 0;
    $api.post($url, this.form).then(function (response) {
      var res = response.data;
      
      $this.total = 1;
      $this.current = 1;
      $this.message = '站点创建成功！';
      $this.success = true;
      setTimeout(function () {
        parent.location.href = utils.getIndexUrl({siteId: res.value});
      }, 1000);
    }).catch(function (error) {
      clearTimeout(interval);
      $this.pageType = 'submit';
      utils.loading($this, false);
      utils.error(error);
    });
  },

  apiProcess: function() {
    var $this = this;
    $this.pageType = 'process';

    $api.post($url + '/actions/process', {
      guid: this.form.guid
    }).then(function (response) {
      var res = response.data;
      if ($this.success) return;
      
      $this.total = res.total || 1;
      $this.current = res.current;
      $this.message = res.message;

      if ($this.current > $this.total) {
        $this.current = $this.total;
      }

      if ($this.total > $this.current) {
        setTimeout(function () {
          $this.apiProcess();
        }, 3000);
      }
      utils.loading($this, false);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getDisplayUrl: function (templateId) {
    return cloud.getTemplatesUrl('template.html?id=' + templateId);
  },

  btnImageClick: function(templateId) {
    window.open(this.getDisplayUrl(templateId));
  },

  btnPreviewClick: function (templateId) {
    window.open(cloud.hostDemo + '/' + templateId.toLowerCase() + '/');
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
  },

  load: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.getTemplates(this.page, this.word, this.tag, this.price, this.order).then(function (response) {
        var res = response.data;

        $this.templates = res.value;
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

  btnLocalClick: function () {
    this.pageType = 'selectLocal';
    this.form.createType = 'local';
  },

  btnCloudClick: function () {
    this.pageType = 'selectCloud';
    this.form.createType = 'cloud';
    this.load();
  },

  btnCancelClick: function () {
    this.pageType = 'selectType';
  },

  btnCreateClick: function (createType, templateId) {
    this.form.createType = createType;
    this.form.createTemplateId = templateId;
    this.pageType = 'submit';
  },

  btnSubmitClick: function () {
    var $this = this;

    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
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