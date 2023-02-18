var $url = "/clouds/censor";
var $urlCloud = "cms/censor";
var $urlCloudAddWords = $urlCloud + "/actions/addWords";
var $urlCloudDelete = $urlCloud + "/actions/delete"

var data = utils.init({
  activeName: "settings",
  count: 0,
  cloudWords: [],
  isAdd: false,
  formInline: {
    isWhiteList: false,
    keyword: "",
    currentPage: 1,
    offset: 0,
    limit: 30,
  },
  form: {
    words: '',
  },
  isCloudCensorText: false,
  isCloudCensorTextAuto: false,
  isCloudCensorTextIgnore: false,
  isCloudCensorTextWhiteList: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url)
      .then(function (response) {
        var res = response.data;

        $this.isCloudCensorText = res.isCloudCensorText;
        $this.isCloudCensorTextAuto = res.isCloudCensorTextAuto;
        $this.isCloudCensorTextIgnore = res.isCloudCensorTextIgnore;
        $this.isCloudCensorTextWhiteList = res.isCloudCensorTextWhiteList;
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
        isCloudCensorText: this.isCloudCensorText,
        isCloudCensorTextAuto: this.isCloudCensorTextAuto,
        isCloudCensorTextIgnore: this.isCloudCensorTextIgnore,
        isCloudCensorTextWhiteList: this.isCloudCensorTextWhiteList,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("违规检测设置保存成功！");
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
    this.formInline.isWhiteList = this.activeName === "whitelist";
    if (this.activeName == "whitelist" || this.activeName == "blacklist") {
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
      $this.cloudWords = res.cloudWords;
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
    cloud.post($urlCloudDelete, {
      isWhiteList: this.formInline.isWhiteList,
      id: id,
    })
    .then(function (response) {
      var res = response.data;

      utils.success($this.getWordType() + "违规词删除成功！");
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

  apiCloudAddWords: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloudAddWords, {
      isWhiteList: this.formInline.isWhiteList,
      words: this.form.words,
    })
    .then(function (response) {
      var res = response.data;

      utils.success($this.getWordType() + "违规词添加成功！");
      $this.isAdd = false;
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

  btnAddClick: function () {
    this.isAdd = true;
    this.form.words = '';
  },

  btnSearchClick: function () {
    this.apiCloudGet();
  },

  handleCurrentChange: function (val) {
    this.formInline.currentPage = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  },

  getWordType: function () {
    return this.formInline.isWhiteList ? "白名单" : "黑名单";
  },

  getDialogTitle: function () {
    return "添加" + this.getWordType() + "违规词";
  },

  btnDeleteClick: function(cloudWord) {
    var $this = this;
    var wordType = this.getWordType();

    utils.alertDelete({
      title: '删除' + wordType + '违规词',
      text: '此操作将删除' + wordType + '违规词 “' + cloudWord.word + '”，确定吗？',
      callback: function () {
        $this.apiCloudDelete(cloudWord.id);
      }
    });
  },

  btnAddSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiCloudAddWords();
      }
    });
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
