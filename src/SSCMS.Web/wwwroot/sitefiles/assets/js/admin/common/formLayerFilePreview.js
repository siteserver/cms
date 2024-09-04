var $url = '/common/form/layerFilePreview';

var data = utils.init({
  fileUrl: utils.getQueryString('fileUrl'),
  isImage: null,
  isVideo: null,
  isAudio: null,
  isOffice: null,
  isPdf: null,
  url: null,
  file: null,
  winHeight: 0,
  isMobile: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    var localhost = location.hostname == 'localhost' || location.hostname == '127.0.0.1';
    $api.get($url, {
      params: {
        localhost: localhost,
        fileUrl: this.fileUrl,
      }
    }).then(function (response) {
      var res = response.data;

       $this.isImage = res.isImage;
       $this.isVideo = res.isVideo;
       $this.isAudio = res.isAudio;
       $this.isOffice = res.isOffice;
       $this.isPdf = res.isPdf;
       $this.url = res.url;
       $this.file = res.file;

       if ($this.isOffice) {
        $this.url = 'https://view.officeapps.live.com/op/view.aspx?src=' + location.protocol + '//' + location.host + res.url;
       }
       if ($this.isPdf) {
        PDFObject.embed(res.url, document.body, {});
       }
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  winResize: function () {
    this.winHeight = $(window).height() - 5;
    this.isMobile = $(window).width() < 600;
  },

  btnDownloadClick: function () {
    window.open(this.url);
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
    window.onresize = this.winResize;
    window.onresize();
  }
});
