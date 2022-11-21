var $url = "/cms/editor/editorLayerSpell";
var $urlAddWords = $url + "/actions/addWords";
var $urlCloudAddWords = "cms/spell/actions/addWords";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  isCloudSpell: utils.getQueryBoolean("isCloudSpell"),
  isSpellingCheckIgnore: utils.getQueryBoolean("isSpellingCheckIgnore"),
  isSpellingCheckWhiteList: utils.getQueryBoolean("isSpellingCheckWhiteList"),
  isSave: utils.getQueryBoolean("isSave"),
  checkCount: 0,
  res: null,
  count: null,
  errorWords: null,
  errorMessage: null,
  checking: true,
  isErrorWords: null,
  results: null,
  ignoreWords: [],
  whiteListWords: [],
});

var methods = {
  apiAddWords: function (word) {
    var $this = this;

    this.checking = true;
    if (this.isCloudSpell) {
      cloud
      .post($urlCloudAddWords, {
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

  calcItems: function () {
    this.isErrorWords = false;
    var errorWords = [];
    for (var word of this.errorWords) {
      if (this.ignoreWords.indexOf(word.original) === -1 && this.whiteListWords.indexOf(word.original) === -1) {
        this.isErrorWords = true;
        errorWords.push(word);
      }
    }
    this.errorWords = errorWords;
    this.count = errorWords.length;
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
        results: res,
      })
      .then(function (response) {
        var res = response.data;

        $this.checkCount++;
        $this.isErrorWords = res.isErrorWords;
        $this.count = res.count;
        $this.errorWords = res.errorWords;
        $this.checking = false;

        if ($this.checkCount === 1 && $this.isSave && !$this.isErrorWords) {
          $this.btnSaveClick();
        }
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  btnIgnoreClick: function (errorWord) {
    var $this = this;
    utils.alertDelete({
      title: "忽略错别字“" + errorWord.original + "”",
      text: "此操作将忽略此错别字，是否确认？",
      button: "忽 略",
      callback: function () {
        $this.ignoreWords.push(errorWord.original);
        $this.calcItems();
      },
    });
  },

  btnAddWhiteListClick: function (errorWord) {
    var $this = this;
    utils.alertDelete({
      title: "将错别字“" + errorWord.original + "”加入白名单",
      text: "添加白名单后系统将自动忽略白名单内的错别字，是否添加？",
      button: "加入白名单",
      callback: function () {
        $this.apiAddWords(errorWord.original);
      },
    });
  },

  btnSaveClick: function () {
    utils.closeLayer();
    window.parent.$vue.isSpellPassed = true;
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
    window.parent.$vue.apiSpell(this.apiSubmit);

    // var res = {
    //   isErrorWords: true,
    //   count: 5,
    //   errorWords: [
    //     { original: "实践", correct: "事件" },
    //   ],
    // };
    // this.apiSubmit(res);
  },
});
