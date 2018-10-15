var $api = new apiUtils.Api(apiUrl + '/pages/settings/adminAccessTokens');

var data = {
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

var methods = {
  getList: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.adminNames = res.adminNames;
      $this.scopes = res.scopes;
      $this.adminName = res.adminName;
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

    pageUtils.loading(true);
    $api.delete({
      id: item.id
    }, function (err, res) {
      pageUtils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
    });
  },
  submit: function (item) {
    var $this = this;

    this.item.scopes = this.item.scopeList ? this.item.scopeList.join(',') : '';

    pageUtils.loading(true);
    $api.post(item, function (err, res) {
      pageUtils.loading(false);
      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
      }

      $this.pageAlert = {
        type: 'success',
        html: item.id ? 'API密钥修改成功！' : 'API密钥添加成功！'
      };
      $this.item = null;
      $this.items = res.value;
      $this.pageType = 'list';
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
    pageUtils.openLayer({
      title: '获取密钥',
      url: 'adminAccessTokensViewLayer.cshtml?id=' + item.id,
      height: 410
    });
  },
  btnDeleteClick: function (item) {
    var $this = this;

    pageUtils.alertDelete({
      title: '删除API密钥',
      text: '此操作将删除API密钥 ' + item.title + '，确定吗？',
      callback: function () {
        $this.delete(item);
        this.pageType = 'list';
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});