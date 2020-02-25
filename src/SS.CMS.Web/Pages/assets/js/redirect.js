var $url = '/admin/redirect';
var $redirectUrl = utils.getQueryString('redirectUrl');

var data = {
  pageLoad: true,
  pageAlert: null,
};

var methods = {
  load: function () {
    var $this = this;

    var params = {};
    if (location.search) {
      var pairs = location.search.slice(1).split('&');
      for (var i = 0; i < pairs.length; i++) {
        var pair = pairs[i];
        pair = pair.split('=');
        params[pair[0]] = decodeURIComponent(pair[1] || '');
      }
    }

    $api.post($url, params).then(function (response) {
      var res = response.data;
      if (res && res.value) {
        setTimeout(function () {
          location.href = res.value;
        }, 200);
      }
    }).catch(function (error) {
      utils.error($this, error);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});
