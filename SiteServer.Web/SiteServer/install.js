var $url = "/pages/install";
var $routeUnCheckedList = "unCheckedList";

var data = {
  pageLoad: false,
  pageAlert: null,

  productVersion: null,
  netVersion: null,
  contentRootPath: null,
  rootWritable: null,
  siteFilesWritable: null,
  databaseTypes: null,
  adminUrl: null,
  oraclePrivileges: null,

  databaseNames: null,

  pageIndex: 0,

  agreement: false,

  connectErrorMessage: null,

  form: {
    databaseType: null,
    server: null,
    isDefaultPort: true,
    port: null,
    userName: null,
    password: null,
    oraclePrivilege: null,
    oracleIsSid: null,
    oracleDatabase: null,
  }
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.productVersion = res.productVersion;
      $this.netVersion = res.netVersion;
      $this.contentRootPath = res.contentRootPath;
      $this.rootWritable = res.rootWritable;
      $this.siteFilesWritable = res.siteFilesWritable;
      $this.databaseTypes = res.databaseTypes;
      $this.adminUrl = res.adminUrl;
      $this.oraclePrivileges = res.oraclePrivileges;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnNextClick: function () {
    this.pageIndex++;
  },

  btnPreviousClick: function () {
    this.pageIndex--;
  },

  btnConnectClick: function () {
    var $this = this;

    utils.loading($this, true);
    $api.post($url + '/actions/connect', this.form).then(function (response) {
      var res = response.data;

      $this.productVersion = res.productVersion;
      $this.netVersion = res.netVersion;
      $this.contentRootPath = res.contentRootPath;
      $this.rootWritable = res.rootWritable;
      $this.siteFilesWritable = res.siteFilesWritable;
      $this.databaseTypes = res.databaseTypes;
      $this.adminUrl = res.adminUrl;
      $this.oraclePrivileges = res.oraclePrivileges;
    }).catch(function (error) {
      $this.connectErrorMessage = error.message;
    }).then(function () {
      utils.loading($this, false);
    });
  }
}

new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});