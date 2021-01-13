var $url = "/dashboard";
var $urlUnCheckedList = "/dashboard/actions/unCheckedList";

var data = utils.init({
  version: null,
  lastActivityDate: null,
  updateDate: null,
  unCheckedList: null,
  unCheckedListTotalCount: 0,
  adminWelcomeHtml: null,
  frameworkDescription: null,
  osArchitecture: null,
  osDescription: null,
  containerized: null,
  cpuCores: null,
  userName: null,
  level: null,
  unCheckedList: [],
  unCheckedListTotalCount: 0,
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.version;
      $this.lastActivityDate = res.lastActivityDate;
      $this.updateDate = res.updateDate;
      $this.adminWelcomeHtml = res.adminWelcomeHtml || '欢迎使用 SSCMS 管理后台';

      $this.frameworkDescription = res.frameworkDescription;
      $this.osArchitecture = res.osArchitecture;
      $this.osDescription = res.osDescription;
      $this.containerized = res.containerized;
      $this.cpuCores = res.cpuCores;
      $this.userName = res.userName;
      $this.level = res.level;

      $this.apiGetUnCheckedList();
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiGetUnCheckedList: function() {
    var $this = this;

    $api.get($urlUnCheckedList).then(function (response) {
      var res = response.data;

      $this.unCheckedList = res.unCheckedList;
      $this.unCheckedListTotalCount = res.totalCount;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCheckClick: function(siteId) {
    utils.addTab('内容审核', utils.getCmsUrl('contentsCheck', {
      siteId: siteId
    }));
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  created: function () {
    this.apiGet();
  },
  methods: methods
});