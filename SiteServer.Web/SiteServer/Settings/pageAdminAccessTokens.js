var api = new apiUtils.Api($apiUrl + '/settings/admin/accessTokens');

var $vue = new Vue({
  el: '#main',
  data: {
    pageLoad: false,
    pageAlert: {
      type: 'warning',
      html: 'API密钥可以用于访问 SiteServer REST API <a href="https://docs.siteserver.cn/api/" target="_blank">阅读更多</a>'
    },
    pageType: null,
    items: null,
    adminName: null,
    item: null,
    adminNames: null
  },
  methods: {
    getItems: function () {
      var $this = this;

      api.get(null, function (err, res) {
        if (err || !res || !res.value) return;

        $this.items = res.value;
        $this.adminName = res.adminName;
        $this.pageLoad = true;
      });
    },
    getAdminNames: function () {
      if (this.adminNames) return;
      var $this = this;

      new apiUtils.Api($apiUrl + '/settings/admin/accessTokens/action/getAdminNames').get(null, function (err, res) {
        if (err || !res || !res.value) return;

        $this.adminNames = res.value;
      });
    },
    getAccessToken: function (item) {
      var $this = this;

      new apiUtils.Api($apiUrl + '/settings/admin/accessTokens/action/getAccessToken/' + item.id).get(null, function (err, res) {
        if (err || !res || !res.value) return;

        item.accessToken = res.value;
        $this.item = item;
        $this.pageType = 'view';
        pageUtils.openLayer({
          title: '获取密钥',
          domId: 'modal',
          height: 320
        });
      });
    },
    delete: function (item) {
      var $this = this;

      api.delete({ id: item.id }, function (err, res) {
        if (err || !res || !res.value) return;

        $this.items = res.value;
      });
    },
    submit: function (item) {
      var $this = this;

      this.item.scopes = this.item.scopes ? this.item.scopes.join(',') : '';

      api.post(item, function (err, res) {
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
    regenerate: function (item) {
      var $this = this;

      new apiUtils.Api($apiUrl + '/settings/admin/accessTokens/action/regenerate/' + item.id).post(null, function (err, res) {
        if (err || !res || !res.value) return;

        $this.pageType = 'view';
        $this.item.accessToken = res.value;
      });
    },
    btnAddClick: function (item) {
      this.pageType = 'add';
      this.item = item;
      this.item.adminName = this.item.adminName ? this.item.adminName : this.adminName;
      this.item.scopes = this.item.scopes ? this.item.scopes.split(',') : [];
      this.getAdminNames();
    },
    btnSubmitClick: function () {
      this.submit(this.item);
    },
    btnCancelClick: function () {
      pageUtils.closeLayer();
      this.pageType = 'list';
      this.item = null;
    },
    btnViewClick: function (item) {
      this.getAccessToken(item);
    },
    btnRegenerateClick: function (item) {
      this.pageType = 'list';
      this.regenerate(item);
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
  }
});

$vue.getItems();