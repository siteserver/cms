var $url = "/clouds/vod";
var $urlCloud = "cms/vod";

var data = utils.init({
  activeName: "settings",
  count: 0,
  videos: [],
  formInline: {
    keyword: "",
    currentPage: 1,
    offset: 0,
    limit: 30,
  },
  isEdit: false,
  editForm: {
    id: 0,
    title: "",
  },
  isUpload: false,
  uploadToken: null,
  uploadUrl: null,
  uploadErrorMessage: null,
  uploadProgressPercent: null,
  uploadProgressInterval: null,
  isCloudVod: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url)
      .then(function (response) {
        var res = response.data;

        $this.isCloudVod = res.isCloudVod;
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .post($url, {
        isCloudVod: this.isCloudVod,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("视频点播设置保存成功！");
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  btnTabsClick: function () {
    if (this.activeName == "videos") {
      this.apiCloudGet();
    }
  },

  apiCloudGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud, {
      params: this.formInline,
    })
    .then(function (response) {
      var res = response.data;

      $this.count = res.count;
      $this.videos = res.videos;
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  apiCloudDelete: function (id) {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloud + "/actions/delete", {
      id: id,
    })
    .then(function (response) {
      var res = response.data;

      utils.success("云视频删除成功！");
      $this.apiCloudGet();
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  apiCloudEdit: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloud + "/actions/edit", this.editForm)
    .then(function (response) {
      var res = response.data;

      utils.success("视频编辑成功！");
      $this.isEdit = false;
      $this.apiCloudGet();
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function () {
    this.apiCloudGet();
  },

  handleCurrentChange: function (val) {
    this.formInline.currentPage = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  },

  btnViewClick: function (video) {
    utils.openLayer({
      title: "预览视频",
      url: utils.getCommonUrl("editorLayerPreviewVideo", {
        videoUrl: video.playUrl
      }),
      width: 600,
      height: 500,
    });
  },

  btnDeleteClick: function (vod) {
    var $this = this;

    utils.alertDelete({
      title: "删除云视频",
      text: "此操作将删除云视频 “" + vod.title + "”，确定吗？",
      callback: function () {
        $this.apiCloudDelete(vod.id);
      },
    });
  },

  btnEditClick: function (vod) {
    this.isEdit = true;
    this.editForm.id = vod.id;
    this.editForm.title = vod.title;
  },

  btnEditSubmitClick: function () {
    var $this = this;
    this.$refs.editForm.validate(function (valid) {
      if (valid) {
        $this.apiCloudEdit();
      }
    });
  },

  uploadBefore: function(file) {
    this.uploadErrorMessage = '';
    var re = /(\.3gp|\.asf|\.avi|\.dat|\.dv|\.flv|\.f4v|\.gif|\.m2t|\.m4v|\.mj2|\.mjpeg|\.mkv|\.mov|\.mp4|\.mpe|\.mpg|\.mpeg|\.mts|\.ogg|\.qt|\.rm|\.rmvb|\.swf|\.ts|\.vob|\.wmv|\.webm)$/i;
    if(!re.exec(file.name))
    {
      this.uploadErrorMessage = '请选择有效的文件上传!';
      return false;
    }
    return true;
  },

  uploadRequest: function(data) {
    var $this = this;
    var formData = new FormData()
    formData.append('file', data.file)
    var config = {
      onUploadProgress: function(progressEvent) {
        $this.uploadProgressPercent = Number((progressEvent.loaded / progressEvent.total * 30).toFixed(2));
        if (progressEvent.loaded === progressEvent.total) {
          $this.uploadProgressInterval = setInterval(function() {
            $this.uploadProgressPercent += 1;
            if ($this.uploadProgressPercent === 99) {
              clearInterval($this.uploadProgressInterval);
            }
          }, 1000);
        }
      }
    };
    cloud.post(this.uploadUrl, formData, config)
    .then(function (response) {
      var res = response.data;

      utils.success('云视频上传成功!');
      $this.isUpload = false;
      $this.apiCloudGet();
    })
    .catch(function (error) {
      $this.uploadProgressPercent = null;
      $this.uploadErrorMessage = utils.getErrorMessage(error);
    })
    .then(function () {
      clearInterval($this.uploadProgressInterval);
    });
  },

  btnUploadClick: function () {
    this.uploadProgressPercent = null;
    this.uploadProgressInterval = null;
    this.isUpload = true;
    this.uploadToken = $cloudToken;
    this.uploadUrl = cloud.defaults.baseURL + '/' + $urlCloud;
  },

  getSize: function(vod) {
    return (vod.size / 1024 / 1024).toFixed(2);
  },

  getDuration: function(vod) {
    return new Date(parseInt(vod.duration) * 1000).toISOString().slice(14, 19);
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    var $this = this;
    cloud.checkAuth(function () {
      $this.apiGet();
    });
  },
});
