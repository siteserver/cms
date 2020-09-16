var $url = '/cms/settings/settingsUploadAudio';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  pageType: null,
  form: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.form = {
        siteId: $this.siteId,
        audioUploadDirectoryName: res.value.audioUploadDirectoryName,
        audioUploadDateFormatString: res.value.audioUploadDateFormatString,
        isAudioUploadChangeFileName: res.value.isAudioUploadChangeFileName,
        audioUploadExtensions: res.value.audioUploadExtensions,
        audioUploadTypeMaxSize: res.value.audioUploadTypeMaxSize,
      };
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('音频上传设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
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