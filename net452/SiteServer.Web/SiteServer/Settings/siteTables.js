var $url = '/pages/settings/siteTables';
var $urlGetColumns = '/pages/settings/siteTables/actions/getColumns';
var $urlRemoveCache = '/pages/settings/siteTables/actions/removeCache';

var $data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  tableNames: null,
  nameDict: null,
  tableName: null,
  columns: null,
  count: null
};

var $methods = {
  getTables: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.tableNames = res.value;
      $this.nameDict = res.nameDict;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnColumnsClick: function (tableName) {
    var $this = this;

    utils.loading(true);
    $api.post($urlGetColumns, {
      tableName: tableName
    }).then(function (response) {
      var res = response.data;

      $this.pageType = 'columns';
      $this.tableName = tableName;
      $this.columns = res.value;
      $this.count = res.count;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnCancelClick: function () {
    this.pageType = 'tables';
    this.tableName = null;
    this.pageAlert = null;
  },

  btnRemoveCacheClick: function () {
    var $this = this;

    utils.loading(true);
    $api.post($urlRemoveCache, {
      tableName: $this.tableName
    }).then(function (response) {
      var res = response.data;

      $this.pageType = 'columns';
      $this.columns = res.value;
      $this.count = res.count;
      utils.loading(false);
      $this.pageAlert = {
        type: 'success',
        html: '内容表缓存清除成功！'
      };
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.getTables();
  }
});