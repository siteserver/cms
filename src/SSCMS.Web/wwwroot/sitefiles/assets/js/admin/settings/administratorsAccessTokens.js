var $url = '/settings/administratorsAccessTokens';

var data = utils.init({
  tokens: null,
  adminNames: null,
  scopes: null,
  adminName: null,

  panel: false,
  form: {},
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.tokens = res.tokens;
      $this.adminNames = res.adminNames;
      $this.scopes = res.scopes;
      $this.adminName = res.adminName;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function (item) {
    var $this = this;

    utils.loading(this, true);
    $api.delete($url, {
      data: {
        id: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.tokens = res.tokens;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      $this.tokens = res.tokens;
      utils.success($this.form.id > 0 ? 'API密钥修改成功！' : 'API密钥添加成功！');
      $this.panel = false;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getDocsUrl: function (url) {
    return cloud.getDocsUrl(url);
  },

  getItemScopes: function (item) {
    if (!item.scopes) return '';
    var itemScopes = item.scopes;
    var list = [];
    for (var i = 0; i < this.scopes.length; i++) {
      if (itemScopes.indexOf(this.scopes[i]) !== -1) {
        list.push(this.scopes[i]);
      }
    }

    return list.join(',');
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  },

  btnViewClick: function (item) {
    utils.openLayer({
      title: '获取密钥',
      url: utils.getSettingsUrl('administratorsAccessTokensLayerView', {id: item.id}),
      width: 550,
      height: 410
    });
  },

  btnDeleteClick: function (item) {
    var $this = this;

    utils.alertDelete({
      title: '删除API密钥',
      text: '此操作将删除API密钥 ' + item.title + '，确定吗？',
      callback: function () {
        $this.apiDelete(item);
      }
    });
  },

  btnEditClick: function(item) {
    this.form = {
      id: item.id,
      title: item.title,
      adminName: item.adminName ? item.adminName : this.adminName,
      scopes: item.scopes
    };
    this.panel = true;
  },

  btnCancelClick: function() {
    this.panel = false;
  },

  btnAddClick: function() {
    this.form = {
      id: 0,
      title: '',
      adminName: '',
      scopes: []
    };
    this.panel = true;
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});