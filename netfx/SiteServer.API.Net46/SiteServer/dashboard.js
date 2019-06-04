var $api = new apiUtils.Api(apiUrl + "/pages/dashboard");
var $routeUnCheckedList = "unCheckedList";

new Vue({
  el: "#main",
  data: {
    pageLoad: false,
    pageAlert: null,
    version: null,
    lastActivityDate: null,
    updateDate: null,
    unCheckedList: null,
    unCheckedListTotalCount: 0
  },
  created: function () {
    var $this = this;

    $api.get(null, function (err, res) {
      this.pageLoad = true;
      if (err || !res || !res.value) return;

      $this.version = res.value.version;
      $this.lastActivityDate = res.value.lastActivityDate;
      $this.updateDate = res.value.updateDate;

      $this.getUnCheckedList();
    });
  },
  methods: {
    getUnCheckedList: function () {
      var $this = this;
      this.pageLoad = true;
      $api.get(
        null,
        function (err, res) {
          if (err || !res || !res.value) return;

          $this.unCheckedList = res.value;
          for (i = 0; i < $this.unCheckedList.length; i++) {
            $this.unCheckedListTotalCount += $this.unCheckedList[i].count;
          }
        },
        $routeUnCheckedList
      );
    }
  }
});