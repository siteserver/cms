var $api = new apiUtils.Api(apiUrl + '/pages/settings/siteTables');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  tableNames: null,
  nameDict: null,
  tableName: null,
  columns: null,
  count: null
};

var methods = {
  getTables: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.tableNames = res.value;
      $this.nameDict = res.nameDict;
      $this.pageLoad = true;
    });
  },
  btnColumnsClick: function (tableName) {
    var $this = this;
    pageUtils.loading(true);

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.pageType = 'columns';
      $this.tableName = tableName;
      $this.columns = res.value;
      $this.count = res.count;
      pageUtils.loading(false);
    }, tableName);
  },
  btnCancelClick: function () {
    this.pageType = 'tables';
    this.tableName = null;
  },
  btnRemoveCacheClick: function () {
    var $this = this;
    pageUtils.loading(true);

    $api.post(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.pageType = 'columns';
      $this.columns = res.value;
      $this.count = res.count;
      pageUtils.loading(false);
      $this.pageAlert = {
        type: 'success',
        html: '内容表缓存清除成功！'
      };
    }, $this.tableName + '/actions/removeCache');
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getTables();
  }
});