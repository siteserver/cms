new Vue({
  el: '#main',
  data: {
    pageUser: null,
    pageConfig: null
  },
  methods: {
    load: function (pageUser, pageConfig) {
      this.pageUser = pageUser;
      this.pageConfig = pageConfig;
    }
  },
  created: function () {
    var $this = this;
    utils.getConfig('dashboard', function (res) {
      if (res.value) {
        $this.load(res.value, res.config);
      } else {
        utils.redirectLogin();
      }
    });
  }
});