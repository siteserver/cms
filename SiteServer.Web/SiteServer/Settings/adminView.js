var $url = '/pages/settings/adminView';
var $pageTypeAdmin = 'admin';
var $pageTypeUser = 'user';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: utils.getQueryString('pageType'),
  userId: parseInt(utils.getQueryString('userId') || '0'),
  adminInfo: null,
  departmentName: null,
  areaName: null,
  level: null,
  isSuperAdmin: null,
  siteNames: null,
  isOrdinaryAdmin: null,
  roleNames: null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '?userId=' + $this.userId).then(function (response) {
      var res = response.data;

      $this.adminInfo = res.value;
      $this.departmentName = res.departmentName;
      $this.areaName = res.areaName;
      $this.level = res.level;
      $this.isSuperAdmin = res.isSuperAdmin;
      $this.siteNames = res.siteNames;
      $this.isOrdinaryAdmin = res.isOrdinaryAdmin;
      $this.roleNames = res.roleNames;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnReturnClick: function () {
    location.href = 'pageAdministrator.aspx';
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});