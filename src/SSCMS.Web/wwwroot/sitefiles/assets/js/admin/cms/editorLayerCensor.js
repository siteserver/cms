var $url = "/cms/editor/editorLayerCensor";
var $urlAddWords = $url + "/actions/addWords";
var $urlCloudAddWords = "cms/censor/actions/addWords";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  isCloudCensor: utils.getQueryBoolean("isCloudCensor"),
  isCensorTextIgnore: utils.getQueryBoolean("isCensorTextIgnore"),
  isCensorTextWhiteList: utils.getQueryBoolean("isCensorTextWhiteList"),
  isSave: utils.getQueryBoolean("isSave"),
  checkCount: 0,
  res: null,
  items: null,
  errorMessage: null,
  checking: true,
  isBadWords: null,
  activeNames: null,
  results: null,
  ignoreWords: [],
  whiteListWords: [],
});

var methods = {
  apiAddWords: function (word) {
    var $this = this;

    this.checking = true;
    if (this.isCloudCensor) {
      cloud
      .post($urlCloudAddWords, {
        isWhiteList: true,
        words: word
      })
      .then(function (response) {
        var res = response.data;

        $this.whiteListWords.push(word);
        $this.calcItems();
        $this.checking = false;
      })
      .catch(function (error) {
        utils.error(error);
      });
    } else {
      $api
        .post($urlAddWords, {
          siteId: this.siteId,
          channelId: this.channelId,
          word: word
        })
        .then(function (response) {
          var res = response.data;

          $this.whiteListWords.push(word);
          $this.calcItems();
          $this.checking = false;
        })
        .catch(function (error) {
          utils.error(error);
        });
    }
  },

  apiSubmit: function (res) {
    var $this = this;

    if (!res.success) {
      this.errorMessage = res.errorMessage;
      return;
    }

    this.res = res;
    this.checking = true;
    $api
      .post($url, {
        siteId: this.siteId,
        channelId: this.channelId,
        results: res
      })
      .then(function (response) {
        var res = response.data;

        $this.checkCount++;
        $this.isBadWords = res.isBadWords;
        $this.activeNames = res.activeNames;
        $this.items = res.items;
        $this.checking = false;

        if ($this.checkCount === 1 && $this.isSave && !$this.isBadWords) {
          $this.btnSaveClick();
        }
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  calcItems: function () {
    this.isBadWords = false;
    for (var item of this.items) {
      var words = [];
      for (var word of item.words) {
        if (this.ignoreWords.indexOf(word) === -1 && this.whiteListWords.indexOf(word) === -1) {
          this.isBadWords = true;
          words.push(word);
        }
      }
      item.words = words;
      item.count = words.length;
    }
  },

  btnIgnoreClick: function (word) {
    var $this = this;
    utils.alertDelete({
      title: '忽略违规词“' + word + '”',
      text: '此操作将忽略此违规词，是否确认？',
      button: '忽 略',
      callback: function() {
        $this.ignoreWords.push(word);
        $this.calcItems();
      }
    });
  },

  btnAddWhiteListClick: function (word) {
    var $this = this;
    utils.alertDelete({
      title: '将违规词“' + word + '”加入白名单',
      text: '添加白名单后系统将自动忽略白名单内的违规词，是否添加？',
      button: '加入白名单',
      callback: function() {
        $this.apiAddWords(word);
      }
    });
  },

  btnSaveClick: function() {
    utils.closeLayer();
    window.parent.$vue.isCensorPassed = true;
    window.parent.$vue.apiSave();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    utils.loading(this, false);
    window.parent.$vue.apiCensor(this.apiSubmit);

    // var res = {
    //   success: true,
    //   errorMessage: null,
    //   isBadWords: true,
    //   badWords: [
    //     { type: "Illegal", message: "存在百度官方默认违禁词库不合规", words: ["人民法院"] },
    //   ],
    //   isWrongWords: false,
    //   wrongWords: null,
    // };

    // this.apiSubmit(res);
  },
});
