var $url = '/pages/settings/adminAccessTokens';

var $data = {
  pageLoad: false,
  pageAlert: {
    type: 'warning',
    html: 'API密钥可以用于访问 SiteServer REST API <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>'
  },
  pageType: null,
  items: null,
  adminNames: null,
  scopes: null,
  adminName: null,
  item: null
};

var $methods = {
  getList: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.adminNames = res.adminNames;
      $this.scopes = res.scopes;
      $this.adminName = res.adminName;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
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

    utils.loading(true);
    $api.delete($url, {
      params: {
        id: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  submit: function (item) {
    var $this = this;

    $this.item.scopes = $this.item.scopeList ? $this.item.scopeList.join(',') : '';

    utils.loading(true);
    $api.post($url, $this.item).then(function (response) {
      var res = response.data;

      $this.pageAlert = {
        type: 'success',
        html: item.id ? 'API密钥修改成功！' : 'API密钥添加成功！'
      };
      $this.item = null;
      $this.items = res.value;
      $this.pageType = 'list';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
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

    swal2({
        title: '删除API密钥',
        text: '此操作将删除API密钥 ' + item.title + '，确定吗？',
        type: 'question',
        confirmButtonText: '删 除',
        confirmButtonClass: 'btn btn-danger',
        showCancelButton: true,
        cancelButtonText: '取 消'
      })
      .then(function (result) {
        if (result.value) {
          $this.delete(item);
          $this.pageType = 'list';
        }
      });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.getList();
  }
});