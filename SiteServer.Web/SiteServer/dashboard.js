var $url = "/pages/dashboard";
var $urlUnCheckedList = "/pages/dashboard/unCheckedList";

var $data = {
  pageLoad: false,
  pageAlert: null,
  version: null,
  lastActivityDate: null,
  updateDate: null,
  unCheckedList: null,
  unCheckedListTotalCount: 0
};

var $methods = {
  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.version = res.value.version;
      $this.lastActivityDate = res.value.lastActivityDate;
      $this.updateDate = res.value.updateDate;

      $this.getUnCheckedList();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  getUnCheckedList: function () {
    var $this = this;

    $api.get($urlUnCheckedList).then(function (response) {
      var res = response.data;

      $this.unCheckedList = res.value;
      for (i = 0; i < $this.unCheckedList.length; i++) {
        $this.unCheckedListTotalCount += $this.unCheckedList[i].count;
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

new Vue({
  el: "#main",
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});