var $url = '/loading';
var $redirectUrl = utils.getQueryString('redirectUrl');

var data = {
  pageLoad: false,
  pageAlert: null,
};

var methods = {
  apiGet: function () {
    var $this = this;

    $api.post($url, {
      redirectUrl: $redirectUrl
    }).then(function (response) {
      var res = response.data;
      if (res && res.value) {
        setTimeout(function () {
          location.href = res.value;
        }, 200);
      }
    }).catch(function (error) {
      utils.error(error);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
