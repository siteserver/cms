var $url = '/pages/loading';

var $data = {
  pageLoad: false,
  pageAlert: null,
  redirectUrl: utils.getQueryString('redirectUrl'),
  encryptedUrl: utils.getQueryString('encryptedUrl'),
  siteId: utils.getQueryString('siteId'),
  channelId: utils.getQueryString('channelId'),
  contentId: utils.getQueryString('contentId'),
  fileTemplateId: utils.getQueryString('fileTemplateId'),
  specialId: utils.getQueryString('specialId')
};

var $methods = {
  load: function () {
    var $this = this;

    $api.post($url, {
      redirectUrl: $this.redirectUrl,
      encryptedUrl: $this.encryptedUrl,
      siteId: $this.siteId,
      channelId: $this.channelId,
      contentId: $this.contentId,
      fileTemplateId: $this.fileTemplateId,
      specialId: $this.specialId
    }).then(function (response) {
      var res = response.data;

      location.href = res.value;
    }).catch(function (error) {
      $this.pageLoad = true;
      $this.pageAlert = utils.getPageAlert(error);
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});