var $url = '/pages/cloud/dashboard';

var $data = {
  pageLoad: false,
  pageAlert: null,
  userName: null,
  accessToken: null,
};

var $methods = {
  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.userName = res.value;
      $this.accessToken = res.accessToken;
      $this.pageLoad = true;
    }).catch(function (error) {
      location.href = 'login.cshtml';
    });


    // $ssApi.get($url).then(function (response) {
    //   var res = response.data;

    //   $this.userInfo = res.value;
    // }).catch(function (error) {
    //   $this.pageAlert = utils.getPageAlert(error);

    //   if (error.response && error.response.status === 401) {
    //     ssUtils.redirectToLogin();
    //   }
    // }).then(function () {
    //   $this.pageLoad = true;
    // });

  }
}

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});