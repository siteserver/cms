var $url = "/cms/library/editor";
var $urlUpload = $apiUrl + "/cms/library/editor/actions/upload?siteId=" + utils.getQueryInt("siteId");

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  mainHeight: '',
  isSettings: true,
  activeNames: ['0', '1'],

  form: {
    libraryId: utils.getQueryInt("libraryId"),
    title: null,
    body: null,
    imageUrl: null,
    summary: null
  },
  editor: null,
});

var methods = {
  insertHtml: function(html) {
    if (!html) return;
    UE.getEditor('body', {allowDivTransToP: false, maximumWords:99999999}).execCommand('insertHTML', html);
  },

  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        libraryId: this.form.libraryId,
        siteId: this.siteId
      }
    }).then(function(response) {
      var res = response.data;

      $this.form.title = res.library.title;
      $this.form.body = res.library.body;
      $this.form.imageUrl = res.library.imageUrl;
      $this.form.summary = res.library.summary;

      setTimeout(function () {
        var editor = UE.getEditor('body', {
          allowDivTransToP: false,
          maximumWords: 99999999
        });
        editor.ready(function () {
          editor.addListener("contentChange", function () {
            $this.form.body = this.getContent();
          });
        });

        window.onresize = $this.winResize;
        window.onresize();
      }, 100);
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  winResize: function () {
    this.mainHeight = ($(window).height() - 52) + 'px';
  },

  btnLayerClick: function(options) {
    this.pageAlert = null;
    utils.openLayer({
      title: options.title,
      url: utils.getCmsUrl('libraryLayer' + options.name, {siteId: this.siteId}),
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnSaveClick: function() {
    this.form.body = this.editor.html.get(true);
    var $this = this;

    utils.loading(this, true);
    if (this.form.libraryId === 0) {
      $api.post($url, this.form)
      .then(function(response) {
        var res = response.data;

        utils.closeLayer(true);
      })
      .catch(function(error) {
        utils.error(error);
      })
      .then(function() {
        utils.loading($this, false);
      });
    } else {
      $api.put($url, {
        libraryId: this.form.libraryId,
        title: this.title,
        body: this.body,
        imageUrl: this.imageUrl,
        summary: this.summary
      })
      .then(function(response) {
        var res = response.data;

        utils.closeLayer(true);
      })
      .catch(function(error) {
        utils.error(error);
      })
      .then(function() {
        utils.loading($this, false);
      });
    }
  },

  btnPreviewClick: function() {

  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.svg|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess(res, file) {
    this.imageUrl = res.value;
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    if (this.form.libraryId === 0) {
      utils.loading(this, false);
    } else {
      this.apiGet();
    }
  }
});
