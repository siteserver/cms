var $url = '/admin/settings/sitesTables';

var data = utils.initData({
  pageType: null,
  tableNames: null,
  nameDict: null,
  tableName: null,
  columns: null,
  count: null
});

var methods = {
  getTables: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.tableNames = res.value;
      $this.nameDict = res.nameDict;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnColumnsClick: function (tableName) {
    var $this = this;

    utils.loading(this, true);
    $api.get($url + '/' + tableName).then(function (response) {
      var res = response.data;

      $this.pageType = 'columns';
      $this.tableName = tableName;
      $this.columns = res.columns;
      $this.count = res.count;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    this.pageType = 'tables';
    this.tableName = null;
  },

  btnRemoveCacheClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/' + this.tableName + '/actions/removeCache').then(function (response) {
      var res = response.data;

      $this.pageType = 'columns';
      $this.columns = res.columns;
      $this.count = res.count;
      $this.$message.success('内容表缓存清除成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getTables();
  }
});