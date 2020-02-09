var $url = '/pages/shared/adminLayerView';

var data = utils.initData({
  adminId: utils.getQueryInt('adminId'),
  administrator: null,
  level: null,
  isSuperAdmin: null,
  siteNames: null,
  isOrdinaryAdmin: null,
  roleNames: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        adminId: this.adminId
      }
    }).then(function (response) {
      var res = response.data;

      $this.administrator = res.administrator;
      $this.level = res.level;
      $this.isSuperAdmin = res.isSuperAdmin;
      $this.siteNames = res.siteNames;
      $this.isOrdinaryAdmin = res.isOrdinaryAdmin;
      $this.roleNames = res.roleNames;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});