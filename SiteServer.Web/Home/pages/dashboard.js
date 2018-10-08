new Vue({
  el: '#main',
  data: {
    pageConfig: null,
    pageUser: null
  },
  methods: {
    load: function (pageConfig) {
      this.pageConfig = pageConfig;
      this.pageUser = authUtils.getUser();
    }
  },
  created: function () {
    var $this = this;
    if (authUtils.isAuthenticated()) {
      pageUtils.getConfig('dashboard', function (res) {
        if (res.isUserLoggin) {
          $this.load(res.value);
        } else {
          authUtils.redirectLogin();
        }
      });
    } else {
      authUtils.redirectLogin();
    }
  }
});