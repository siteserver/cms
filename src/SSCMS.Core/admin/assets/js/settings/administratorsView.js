var $url = '/admin/settings/administratorsView';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = utils.initData({
  pageType: utils.getQueryString('pageType'),
  userId: utils.getQueryInt('userId'),
  userName: utils.getQueryString('userName'),
  returnUrl: utils.getQueryString('returnUrl'),
  administrator: null,
  level: null,
  isSuperAdmin: null,
  siteNames: null,
  isOrdinaryAdmin: null,
  roleNames: null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url, {
      params: {
        userId: this.userId,
        userName: this.userName
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

  btnReturnClick: function () {
    location.href = this.returnUrl || utils.getSettingsUrl('administrators');
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});