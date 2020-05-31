var $url = "/pages/cms/libraryEditor";
var $urlUpload = apiUrl + "/pages/cms/libraryEditor/actions/upload?siteId=" + utils.getQueryInt("siteId");

function insertHtml(html)
{
  if (html)
  {
    var editor = UE.getEditor('content', {
      allowDivTransToP: false,
      maximumWords: 99999999
    });

    editor.execCommand("insertHTML", html);
  }
}

var data = {
  siteId: utils.getQueryInt("siteId"),
  textId: utils.getQueryInt("textId"),
  mainHeight: '',
  pageLoad: false,
  pageAlert: null,
  isSettings: true,
  activeNames: ['0', '1'],

  title: null,
  content: null,
  imageUrl: null,
  summary: null,
  // editor: null,
};

var methods = {
  getConfig: function() {
    var $this = this;

    if ($this.textId === 0) {
      $this.pageLoad = true;
      $this.loadEditor();
      return;
    }

    window.onresize = $this.winResize;
    window.onresize();

    $api
      .get($url + '/' + $this.textId, {
        params: {
          siteId: $this.siteId
        }
      })
      .then(function(response) {
        var res = response.data;

        $this.loadEditor(res);
      })
      .catch(function(error) {
        utils.notifyError($this, error);
      })
      .then(function() {
        $this.pageLoad = true;
      });
  },

  loadEditor: function(res) {
    if (res) {
      this.title = res.title;
      this.content = res.content;
      this.imageUrl = res.imageUrl;
      this.summary = res.summary;
    }

    var $this = this;

    setTimeout(function () {
      var editor = UE.getEditor('content', {
        allowDivTransToP: false,
        maximumWords: 99999999
      });
      editor.ready(function () {
        editor.addListener("contentChange", function () {
          $this.content = this.getContent();
        });
      });
    }, 100);
  },

  winResize: function () {
    this.mainHeight = ($(window).height() - 85) + 'px';
  },

  btnLayerClick: function(options) {
    this.pageAlert = null;
    utils.openLayer({
      title: options.title,
      url: "libraryLayer" + options.name + ".cshtml?siteId=" + this.siteId,
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnCancelClick: function() {
    utils.closeLayer();
  },

  btnSaveClick: function() {
    // var editor = UE.getEditor('content');
    // this.content = editor.getContent();

    // this.content = this.editor.html.get(true);
    var $this = this;

    if (!this.title) {
      this.$message.error('标题不能为空!');
      return;
    }
    if (!this.content) {
      this.$message.error('正文不能为空!');
      return;
    }

    utils.loading(true);
    if (this.textId === 0) {
      $api
      .post($url, {
        title: this.title,
        content: this.content,
        imageUrl: this.imageUrl,
        summary: this.summary
      })
      .then(function(response) {
        var res = response.data;

        var frmMain = parent.document.getElementById('frmMain');
        var oDoc = frmMain.contentWindow || frmMain.contentDocument;
        if (oDoc.document) {
            oDoc = oDoc.document;
        }
        oDoc.location.reload();
        utils.closeLayer();
      })
      .catch(function(error) {
        utils.notifyError($this, error);
      })
      .then(function() {
        utils.loading(false);
      });
    } else {
      $api
      .put($url + '/' + this.textId, {
        title: this.title,
        content: this.content,
        imageUrl: this.imageUrl,
        summary: this.summary
      })
      .then(function(response) {
        var res = response.data;

        var frmMain = parent.document.getElementById('frmMain');
        var oDoc = frmMain.contentWindow || frmMain.contentDocument;
        if (oDoc.document) {
          oDoc = oDoc.document;
        }
        oDoc.location.reload();
        utils.closeLayer();
      })
      .catch(function(error) {
        utils.notifyError($this, error);
      })
      .then(function() {
        utils.loading(false);
      });
    }
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.svg|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      this.$message.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(true)
  },

  uploadSuccess(res, file) {
    this.imageUrl = res.value;
    utils.loading(false);
  },

  uploadError: function(err) {
    utils.loading(false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  }
};

new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function() {
    this.getConfig();
  }
});
