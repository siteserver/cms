var $url = '/open';

var data = utils.init({
  
});

var methods = {
  apiGet: function () {
    var $this = this;

    // utils.loading(this, true);
    // $api.get($url).then(function (response) {
    //   var res = response.data;

    //   $this.form.adminTitle = res.adminTitle;
    //   $this.form.adminLogoUrl = res.adminLogoUrl;
    //   $this.form.adminWelcomeHtml = res.adminWelcomeHtml || '欢迎使用 SS CMS 管理后台';

    //   if ($this.form.adminLogoUrl) {
    //     $this.uploadFileList.push({name: 'avatar', url: $this.form.adminLogoUrl});
    //   }
    // }).catch(function (error) {
    //   utils.error(error);
    // }).then(function () {
      utils.loading($this, false);
    // });
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