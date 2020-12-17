var $url = '/common/adminLayerView';

var data = utils.init({
  adminId: utils.getQueryInt('adminId'),
  userName: utils.getQueryString('userName'),
  administrator: null,
  level: null,
  siteNames: null,
  roleNames: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        adminId: this.adminId,
        userName: this.userName
      }
    }).then(function (response) {
      var res = response.data;

      $this.administrator = res.administrator;
      $this.level = res.level;
      $this.siteNames = res.siteNames;
      $this.roleNames = res.roleNames;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
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