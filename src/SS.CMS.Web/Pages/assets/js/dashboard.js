var $url = "/admin/dashboard";

var data = utils.initData({
  version: null,
  lastActivityDate: null,
  updateDate: null,
  unCheckedList: null,
  unCheckedListTotalCount: 0,
  adminWelcomeHtml: null
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
      $this.adminWelcomeHtml = res.adminWelcomeHtml || '欢迎使用 SiteServer CMS 管理后台';

      $this.getUnCheckedList();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getUnCheckedList: function () {
    var $this = this;

    $api.get($url + '/actions/unCheckedList').then(function (response) {
      var res = response.data;

      $this.unCheckedList = res.value;
      for (i = 0; i < $this.unCheckedList.length; i++) {
        $this.unCheckedListTotalCount += $this.unCheckedList[i].count;
      }
    }).catch(function (error) {
      utils.error($this, error);
    });
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