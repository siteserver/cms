var $url = "/clouds/spell"
var $urlCloud = "cms/spell";

var data = utils.init({
  activeName: "settings",
  count: 0,
  cloudWords: [],
  isAdd: false,
  formInline: {
    keyword: "",
    currentPage: 1,
    offset: 0,
    limit: 30,
  },
  form: {
    words: '',
  },
  isCloudSpellingCheck: false,
  isCloudSpellingCheckAuto: false,
  isCloudSpellingCheckIgnore: false,
  isCloudSpellingCheckWhiteList: false
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isCloudSpellingCheck = res.isCloudSpellingCheck;
      $this.isCloudSpellingCheckAuto = res.isCloudSpellingCheckAuto;
      $this.isCloudSpellingCheckIgnore = res.isCloudSpellingCheckIgnore;
      $this.isCloudSpellingCheckWhiteList = res.isCloudSpellingCheckWhiteList;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isCloudSpellingCheck: this.isCloudSpellingCheck,
      isCloudSpellingCheckAuto: this.isCloudSpellingCheckAuto,
      isCloudSpellingCheckIgnore: this.isCloudSpellingCheckIgnore,
      isCloudSpellingCheckWhiteList: this.isCloudSpellingCheckWhiteList,
    }).then(function (response) {
      var res = response.data;

      utils.success('文本纠错设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  btnTabsClick: function () {
    if (this.activeName == "whitelist") {
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
    cloud.post($urlCloud + "/actions/delete", {
      id: id,
    })
    .then(function (response) {
      var res = response.data;

      utils.success("错别字白名单删除成功！");
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

  apiCloudSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.post($urlCloud, {
      words: this.form.words,
    })
    .then(function (response) {
      var res = response.data;

      utils.success("错别字白名单添加成功！");
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

  btnDeleteClick: function(cloudWord) {
    var $this = this;

    utils.alertDelete({
      title: '删除错别字白名单',
      text: '此操作将删除错别字白名单 “' + cloudWord.word + '”，确定吗？',
      callback: function () {
        $this.apiCloudDelete(cloudWord.id);
      }
    });
  },

  btnAddSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiCloudSubmit();
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
    cloud.checkAuth(function() {
      $this.apiGet();
    });
  }
});
