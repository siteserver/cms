var $url = '/pages/cms/libraryLayerImage';
var $urlUpload = apiUrl + '/pages/cms/libraryLayerImage/actions/upload?siteId=' + utils.getQueryInt('siteId');

var data = {
  pageLoad: false,
  pageAlert: null,
  uploadList: [],
  dialogImageUrl: '',
  dialogVisible: false,
  fileUrls: []
};

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (this.fileUrls.length === 0) {
      this.$message.error('请选择需要插入的图片文件！');
      return false;
    }

    for (var i = 0; i < this.fileUrls.length; i++) {
      var fileUrl = this.fileUrls[i];
      parent.insertHtml('<img src="' + fileUrl + '" border="0" /><br/>');
    }

    parent.layer.closeAll();
  },

  btnCancelClick: function () {
    parent.layer.closeAll();
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
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

  uploadSuccess: function(res) {
    this.fileUrls.push(res.url);
    utils.loading(false);
  },

  uploadError: function(err) {
    utils.loading(false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  },

  uploadRemove(file) {
    if (file.response) {
      this.fileUrls.splice(this.fileUrls.indexOf(file.response.url), 1);
    }
  },

  uploadPictureCardPreview(file) {
    this.dialogImageUrl = file.url;
    this.dialogVisible = true;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.pageLoad = true;
  }
});