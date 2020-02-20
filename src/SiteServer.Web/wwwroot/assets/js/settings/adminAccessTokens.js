var $url = '/pages/settings/adminAccessTokens';

var data = utils.initData({
  pageType: null,
  items: null,
  adminNames: null,
  scopes: null,
  adminName: null,
  item: null
});

var methods = {
  getList: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.adminNames = res.adminNames;
      $this.scopes = res.scopes;
      $this.adminName = res.adminName;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getItemScopes: function (item) {
    if (!item.scopes) return '';
    var itemScopes = item.scopes.split(',');
    var retval = [];
    for (var i = 0; i < this.scopes.length; i++) {
      if (itemScopes.indexOf(this.scopes[i]) !== -1) {
        retval.push(this.scopes[i]);
      }
    }

    return retval.join(',');
  },

  delete: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        id: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  submit: function (item) {
    var $this = this;

    this.item.scopes = this.item.scopeList ? this.item.scopeList.join(',') : '';

    utils.loading(this, true);
    $api.post($url, item).then(function (response) {
      var res = response.data;

      $this.item = null;
      $this.items = res.value;
      $this.pageType = 'list';
      $this.$message.success(item.id ? 'API密钥修改成功！' : 'API密钥添加成功！');
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnAddClick: function (item) {
    this.pageType = 'add';
    this.item = item;
    this.item.adminName = this.item.adminName ? this.item.adminName : this.adminName;
    this.item.scopeList = this.item.scopes ? this.item.scopes.split(',') : [];
  },

  btnSubmitClick: function () {
    this.submit(this.item);
  },

  btnCancelClick: function () {
    this.pageType = 'list';
  },

  btnViewClick: function (item) {
    utils.openLayer({
      title: '获取密钥',
      url: 'adminAccessTokensViewLayer.cshtml?id=' + item.id,
      height: 410
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除API密钥',
      text: '此操作将删除API密钥 ' + item.title + '，确定吗？',
      callback: function () {
        $this.delete(item);
        this.pageType = 'list';
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});