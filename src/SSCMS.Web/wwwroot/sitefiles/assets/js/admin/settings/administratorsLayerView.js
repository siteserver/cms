var $url = '/settings/administratorsLayerView';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = utils.init({
  pageType: utils.getQueryString('pageType'),
  userId: utils.getQueryInt('userId'),
  userName: utils.getQueryString('userName'),
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
    this.getConfig();
  }
});