var $url = "/cms/editor";
var $urlInsert = $url + "/actions/insert";
var $urlUpdate = $url + "/actions/update";
var $urlPreview = $url + "/actions/preview";
var $urlTags = $url + "/actions/tags";
var $urlCensor = $url + "/actions/censor";
var $urlCensorAddWords = $url + "/actions/censorAddWords";
var $urlCloudCensor = "cms/censor";
var $urlCloudCensorAddWords = "cms/censor/actions/addWords";
var $urlSpell = $url + "/actions/spell";
var $urlSpellAddWords = $url + "/actions/spellAddWords";
var $urlCloudSpell = "cms/spell";
var $urlCloudSpellAddWords = "cms/spell/actions/addWords";

var $regCensor = /<a[^>]*_censor_original=["']([^"']*)["'][^>]*>[^>]*<\/a>/g;
var $regSpell = /<a[^>]*_spell_original=["']([^"']*)["'][^>]*>[^>]*<\/a>/g;

Date.prototype.Format = function (fmt) {
  var o = {
    "M+": this.getMonth() + 1, // 月份
    "d+": this.getDate(), // 日
    "h+": this.getHours(), // 时
    "m+": this.getMinutes(), // 分
    "s+": this.getSeconds(), // 秒
    "q+": Math.floor((this.getMonth() + 3) / 3), // 季度
    S: this.getMilliseconds(), // 毫秒
  };

  if (/(y+)/.test(fmt))
    fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

  for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt))
      fmt = fmt.replace(
        RegExp.$1,
        RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length)
      );

  return fmt;
};

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  channelId: utils.getQueryInt("channelId"),
  contentId: utils.getQueryInt("contentId"),
  page: utils.getQueryInt("page"),
  tabName: utils.getQueryString("tabName"),
  reloadChannelId: utils.getQueryInt("reloadChannelId"),
  mainHeight: "",
  isSettings: true,
  sideType: "settings",
  collapseSettings: ["checkedLevel", "addDate"],
  collapseMore: ["templateId", "translates"],

  csrfToken: null,
  site: null,
  siteUrl: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: null,
  linkTypes: null,
  linkTo: {
    channelIds: [],
    contentId: 0,
    contentTitle: '',
  },
  root: null,
  styles: null,
  relatedFields: null,
  templates: null,
  form: null,
  breadcrumbItems: [],
  translates: [],
  isPreviewSaving: false,
  isScheduledDialog: false,
  scheduledForm: {
    isScheduled: false,
    scheduledDate: new Date(),
  },
  settings: null,
  censorSettings: {
    isCloudCensor: false,
    isCensorText: false,
    isCensorTextAuto: false,
    isCensorTextIgnore: false,
    isCensorTextWhiteList: false,
    isCensorPassed: false,
    ignoreWords: [],
    whiteListWords: [],
    isCensorChecking: false,
    activeNames: [],
  },
  censorResults: null,
  spellSettings: {
    isCloudSpell: false,
    isSpellingCheck: false,
    isSpellingCheckAuto: false,
    isSpellingCheckIgnore: false,
    isSpellingCheckWhiteList: false,
    isSpellPassed: false,
    ignoreWords: [],
    whiteListWords: [],
    isSpellChecking: false,
  },
  spellResults: null,
});

var methods = {
  apiGet: function () {
    var $this = this;

    window.onresize = function () {
      $this.mainHeight = $(window).height() - 70 + "px";
    };
    window.onresize();

    $api
      .get($url, {
        params: {
          siteId: $this.siteId,
          channelId: $this.channelId,
          contentId: $this.contentId,
        },
      })
      .then(function (response) {
        var res = response.data;
        if (res.channel.isChangeBanned) {
          return utils.alertWarning({
            title: '禁止修改内容',
            text: '栏目已开启禁止维护内容(添加/修改/删除)功能，修改内容请先在栏目中关闭此功能！',
            callback: function() {
              utils.removeTab();
            }
          });
        }

        $this.csrfToken = res.csrfToken;

        $this.site = res.site;
        $this.siteUrl = res.siteUrl;
        $this.channel = res.channel;

        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.checkedLevels = res.checkedLevels;
        $this.linkTypes = res.linkTypes;
        $this.linkTo = res.linkTo;
        $this.root = [res.root];
        $this.settings = res.settings;
        $this.censorSettings = _.assign({}, $this.censorSettings, res.settings.censorSettings, {
          isCloudCensor: res.settings.isCloudCensor,
        });
        $this.spellSettings = _.assign({}, $this.spellSettings, res.settings.spellSettings, {
          isCloudSpell: res.settings.isCloudSpell,
        });

        $this.styles = res.styles;
        $this.relatedFields = res.relatedFields;
        $this.templates = res.templates;
        $this.form = _.assign({}, res.content);
        $this.breadcrumbItems = res.breadcrumbItems;

        $this.scheduledForm.isScheduled = res.isScheduled;
        $this.scheduledForm.scheduledDate = res.scheduledDate;

        if (!$this.form.addDate) {
          $this.form.addDate = new Date().Format("yyyy-MM-dd hh:mm:ss");
        } else {
          $this.form.addDate = new Date($this.form.addDate).Format("yyyy-MM-dd hh:mm:ss");
        }

        if ($this.form.checked) {
          $this.form.checkedLevel = $this.site.checkContentLevel;
        }
        var targetCheckedLevel = $this.checkedLevels.find(
          (x) => x.value === $this.form.checkedLevel
        );
        if (!!!targetCheckedLevel) {
          $this.form.checkedLevel = res.checkedLevel;
        }
        if ($this.form.top || $this.form.recommend || $this.form.hot || $this.form.color) {
          $this.collapseSettings.push("attributes");
        }
        if ($this.form.groupNames && $this.form.groupNames.length > 0) {
          $this.collapseSettings.push("groupNames");
        } else {
          $this.form.groupNames = [];
        }
        if ($this.form.tagNames && $this.form.tagNames.length > 0) {
          $this.collapseSettings.push("tagNames");
        } else {
          $this.form.tagNames = [];
        }
        if (($this.form.linkType && $this.form.linkType != "None") || $this.form.linkUrl) {
          $this.collapseSettings.push("link");
        }

        for (var i = 0; i < $this.styles.length; i++) {
          var style = $this.styles[i];
          if (style.inputType === "CheckBox" || style.inputType === "SelectMultiple") {
            var value = $this.form[utils.toCamelCase(style.attributeName)];
            if (!Array.isArray(value)) {
              if (!value) {
                $this.form[utils.toCamelCase(style.attributeName)] = [];
              } else {
                $this.form[utils.toCamelCase(style.attributeName)] = utils.toArray(value);
              }
            }
          } else if (
            style.inputType === "Image" ||
            style.inputType === "File" ||
            style.inputType === "Video"
          ) {
            $this.form[utils.getCountName(style.attributeName)] = utils.toInt(
              $this.form[utils.getCountName(style.attributeName)]
            );
          } else if (
            style.inputType === "Text" ||
            style.inputType === "TextArea" ||
            style.inputType === "TextEditor"
          ) {
            if ($this.contentId === 0) {
              $this.form[utils.toCamelCase(style.attributeName)] = style.defaultValue;
            }
          }
        }

        setTimeout(function () {
          for (var i = 0; i < $this.styles.length; i++) {
            var style = $this.styles[i];
            if (style.inputType === "TextEditor") {
              var editor = utils.getEditor(style.attributeName, style.height);
              editor.styleIndex = i;
              editor.ready(function () {
                this.addListener("contentChange", function () {
                  var style = $this.styles[this.styleIndex];
                  $this.form[utils.toCamelCase(style.attributeName)] = this.getContent();
                });
              });
            }
          }
        }, 100);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiInsert: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlInsert, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
        translates: this.translates,
        linkTo: this.linkTo,
        isScheduled: this.scheduledForm.isScheduled,
        scheduledDate: this.scheduledForm.scheduledDate,
      })
      .then(function (response) {
        var res = response.data;

        $this.closeAndRedirect(false);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  getText: function (isCensor, isSpell) {
    var text = "";
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.inputType === "TextEditor") {
        var editor = utils.getEditor(style.attributeName);
        var editorText = this.regexReplace($regCensor, editor.getContent());
        editorText = this.regexReplace($regSpell, editorText);
        text += editorText;
      }
    }
    var replaceWords = [];
    if (isCensor) {
      for (var word of this.censorSettings.ignoreWords) {
        replaceWords.push(word);
      }
      for (var word of this.censorSettings.whiteListWords) {
        replaceWords.push(word);
      }
    }
    if (isSpell) {
      for (var word of this.spellSettings.ignoreWords) {
        replaceWords.push(word);
      }
      for (var word of this.spellSettings.whiteListWords) {
        replaceWords.push(word);
      }
    }
    for (var word of replaceWords) {
      text = text.replace(new RegExp(word, 'g'), '');
    }
    return text;
  },

  parseText: function () {
    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.inputType === "TextEditor") {
        var editor = utils.getEditor(style.attributeName);

        var html = this.regexReplace($regCensor, editor.getContent());
        html = this.regexReplace($regSpell, html);

        if (this.censorResults && this.censorResults.badWords && this.censorResults.badWords.length > 0) {
          for (var badWord of this.censorResults.badWords) {
            for (var word of badWord.words) {
              html = html.replace(new RegExp(word, 'g'), '<a href="javascript:;" _censor="true" _censor_original="' + word + '" style="color: red;  margin-left: 30px; margin-right: 30px; text-decoration: underline;">疑似违规：' + word + '</a>');
            }
          }
        }
        if (this.spellResults && this.spellResults.errorWords && this.spellResults.errorWords.length > 0) {
          for (var errorWord of this.spellResults.errorWords) {
            html = html.replace(new RegExp(errorWord.original, 'g'), '<a href="javascript:;" _spell="true" _spell_original="' + errorWord.original + '" _spell_correct="' + errorWord.correct + '" style="color: red; margin-left: 30px; margin-right: 30px; text-decoration: underline;">疑似错误：' + errorWord.original + '，系统建议：' + (errorWord.correct ? errorWord.correct : '<span style="text-decoration: line-through;">删除<span>') + '</a>');
          }
        }
        editor.setContent(html);
      }
    }
  },

  parse: function(command, original, correct) {
    if (command === 'censor_delete' || command === 'censor_ignore' || command === 'censor_whitelist') {
      this.censorResults.isBadWords = false;
      for (var badWord of this.censorResults.badWords) {
        var index = badWord.words.indexOf(original);
        if (index !== -1) {
          badWord.words.splice(index, 1);
        }
        if (badWord.words.length > 0) {
          this.censorResults.isBadWords = true;
        }
      }
    } else if (command === 'spell_replace' || command === 'spell_ignore' || command === 'spell_whitelist') {
      this.spellResults.isErrorWords = false;
      for (var errorWord of this.spellResults.errorWords) {
        if (errorWord.original == original) {
          var index = this.spellResults.errorWords.indexOf(errorWord);
          this.spellResults.errorWords.splice(index, 1);
        }
        if (this.spellResults.errorWords.length > 0) {
          this.spellResults.isErrorWords = true;
        }
      }
    }

    if (command === 'censor_ignore') {
      this.censorSettings.ignoreWords.push(original);
    } else if (command === 'censor_whitelist') {
      this.censorSettings.whiteListWords.push(original);
    } else if (command === 'spell_ignore') {
      this.spellSettings.ignoreWords.push(original);
    } else if (command === 'spell_whitelist') {
      this.spellSettings.whiteListWords.push(original);
    }

    for (var i = 0; i < this.styles.length; i++) {
      var style = this.styles[i];
      if (style.inputType === "TextEditor") {
        var editor = utils.getEditor(style.attributeName);

        var html = this.regexReplace($regCensor, editor.getContent());
        html = this.regexReplace($regSpell, html);

        if (command === 'censor_delete') {
          html = html.replace(new RegExp(original, 'g'), '');
        } else if (command === 'spell_replace') {
          html = html.replace(new RegExp(original, 'g'), correct);
        }
        editor.setContent(html);
      }
    }
    this.parseText();
    utils.success('操作成功！');
  },

  isCensorButton: function () {
    return (
      this.censorSettings.isCensorText && !this.censorSettings.isCensorTextAuto
    );
  },

  isSpellButton: function () {
    return (
      this.spellSettings.isSpellingCheck &&
      !this.spellSettings.isSpellingCheckAuto
    );
  },

  apiCloudCensorAddWords: function (word) {
    var $this = this;

    utils.loading(this, true);
    if (this.censorSettings.isCloudCensor) {
      cloud.post($urlCloudCensorAddWords, {
        isWhiteList: true,
        words: word
      })
      .then(function (response) {
        var res = response.data;

        $this.parse('censor_whitelist', word);
      })
      .catch(function (error) {
        utils.error(error, {
          ignoreAuth: true,
        });
      })
      .then(function () {
        utils.loading($this, false);
      });
    } else {
      $api
        .post($urlCensorAddWords, {
          siteId: this.siteId,
          channelId: this.channelId,
          word: word
        })
        .then(function (response) {
          var res = response.data;

          this.parse('censor_whitelist', word);
        })
        .catch(function (error) {
          utils.error(error);
        })
        .then(function () {
          utils.loading($this, false);
        });
    }
  },

  apiCloudCensor: function (isSave) {
    var $this = this;
    this.censorSettings.isCensorChecking = true;
    if (this.censorSettings.isCloudCensor) {
      cloud.post($urlCloudCensor, {
        text: this.getText(true, false),
      })
      .then(function (response) {
        var res = response.data;
        $this.censorResults = _.assign({}, res);
        $this.censorSettings.activeNames = [];
        for (var badWord of $this.censorResults.badWords) {
          if (badWord.words.length > 0) {
            $this.censorSettings.activeNames.push(badWord.type);
          }
        }
        $this.sideType = "censor";
        $this.parseText(res);

        if (isSave && !$this.censorResults.isBadWords) {
          $this.censorSettings.isCensorPassed = true;
          $this.apiSave();
        }
      })
      .catch(function (error) {
        utils.error(error, {
          ignoreAuth: true,
        });
        layer.closeAll();
      })
      .then(function () {
        $this.censorSettings.isCensorChecking = false;
      });
    } else {
      $api
        .csrfPost(this.csrfToken, $urlCensor, {
          siteId: this.siteId,
          channelId: this.channelId,
          text: this.getText(true, false),
        })
        .then(function (response) {
          var res = response.data;
          $this.censorResults = _.assign({}, res);
          $this.censorSettings.activeNames = [];
          for (var badWord of $this.censorResults.badWords) {
            if (badWord.words.length > 0) {
              $this.censorSettings.activeNames.push(badWord.type);
            }
          }
          $this.sideType = "censor";
          $this.parseText(res);

          if (isSave && !$this.censorResults.isBadWords) {
            $this.censorSettings.isCensorPassed = true;
            $this.apiSave();
          }
        })
        .catch(function (error) {
          utils.error(error);
          layer.closeAll();
        })
        .then(function () {
          $this.censorSettings.isCensorChecking = false;
        });
    }
  },

  apiCloudSpellAddWords: function (word) {
    var $this = this;

    utils.loading(this, true);
    if (this.spellSettings.isCloudSpell) {
      cloud.post($urlCloudSpellAddWords, {
        words: word
      })
      .then(function (response) {
        var res = response.data;

        $this.parse('spell_whitelist', word);
      })
      .catch(function (error) {
        utils.error(error, {
          ignoreAuth: true,
        });
      })
      .then(function () {
        utils.loading($this, false);
      });
    } else {
      $api
        .post($urlSpellAddWords, {
          siteId: this.siteId,
          channelId: this.channelId,
          word: word
        })
        .then(function (response) {
          var res = response.data;

          this.parse('spell_whitelist', word);
        })
        .catch(function (error) {
          utils.error(error);
        })
        .then(function () {
          utils.loading($this, false);
        });
    }
  },

  apiCloudSpell: function (isSave) {
    var $this = this;
    this.spellSettings.isSpellChecking = true;
    if (this.spellSettings.isCloudSpell) {
      cloud.post($urlCloudSpell, {
        text: this.getText(false, true),
      })
      .then(function (response) {
        var res = response.data;
        $this.spellResults = _.assign({}, res);
        $this.sideType = "spell";
        $this.parseText(res);

        if (isSave && !$this.spellResults.isErrorWords) {
          $this.spellSettings.isSpellPassed = true;
          $this.apiSave();
        }
      })
      .catch(function (error) {
        utils.error(error, {
          ignoreAuth: true,
        });
        layer.closeAll();
      })
      .then(function () {
        $this.spellSettings.isSpellChecking = false;
      });
    } else {
      $api
        .csrfPost(this.csrfToken, $urlSpell, {
          siteId: this.siteId,
          channelId: this.channelId,
          text: this.getText(false, true),
        })
        .then(function (response) {
          var res = response.data;
          $this.spellResults = _.assign({}, res);
          $this.sideType = "spell";
          $this.parseText(res);

          if (isSave && !$this.spellResults.isErrorWords) {
            $this.spellSettings.isSpellPassed = true;
            $this.apiSave();
          }
        })
        .catch(function (error) {
          utils.error(error);
          layer.closeAll();
        })
        .then(function () {
          $this.spellSettings.isSpellChecking = false;
        });
    }
  },

  apiTags: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlTags, {
        siteId: this.siteId,
        channelId: this.channelId,
        content: this.form.body,
      })
      .then(function (response) {
        var res = response.data;

        if (res.tags && res.tags.length > 0) {
          $this.form.tagNames = _.union($this.form.tagNames, res.tags);
          utils.success("成功提取标签！");
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiPreview: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlPreview, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
      })
      .then(function (response) {
        var res = response.data;

        $this.isPreviewSaving = false;
        window.open(res.url);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiUpdate: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlUpdate, {
        siteId: this.siteId,
        channelId: this.channelId,
        contentId: this.contentId,
        content: this.form,
        translates: this.translates,
        linkTo: this.linkTo,
        isScheduled: this.scheduledForm.isScheduled,
        scheduledDate: this.scheduledForm.scheduledDate,
      })
      .then(function (response) {
        var res = response.data;

        $this.closeAndRedirect(true);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSave: function () {
    if (
      !this.censorSettings.isCensorPassed &&
      this.censorSettings.isCensorText &&
      this.censorSettings.isCensorTextAuto
    ) {
      this.btnCensorTextClick(true);
    } else if (
      !this.spellSettings.isSpellPassed &&
      this.spellSettings.isSpellingCheck &&
      this.spellSettings.isSpellingCheckAuto
    ) {
      this.btnSpellingCheckClick(true);
    } else {
      if (this.contentId === 0) {
        this.apiInsert();
      } else {
        this.apiUpdate();
      }
    }
  },

  runFormLayerImageUploadText: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerImageUploadEditor: function (attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runMaterialLayerImageSelect: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerFileUpload: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runMaterialLayerFileSelect: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runFormLayerVideoUpload: function (attributeName, no, text, coverUrl) {
    this.insertText(attributeName, no, text);
    if (coverUrl) {
      this.runFormLayerImageUploadText("ImageUrl", no, coverUrl);
    }
  },

  runMaterialLayerVideoSelect: function (attributeName, no, text) {
    this.insertText(attributeName, no, text);
  },

  runEditorLayerImage: function (attributeName, html) {
    this.insertEditor(attributeName, html);
  },

  runLayerContentSelect: function (content) {
    this.linkTo.contentId = content.id;
    this.linkTo.contentTitle = content.title;
  },

  getContentUrl: function() {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId:  this.linkTo.channelIds[this.linkTo.channelIds.length - 1],
      contentId: this.linkTo.contentId
    });
  },

  insertText: function (attributeName, no, text) {
    var count = this.form[utils.getCountName(attributeName)] || 0;
    if (count <= no) {
      this.form[utils.getCountName(attributeName)] = no;
    }
    this.form[utils.getExtendName(attributeName, no)] = text;
    this.form = _.assign({}, this.form);
  },

  insertEditor: function (attributeName, html) {
    if (!attributeName) attributeName = "Body";
    if (!html) return;
    utils.getEditor(attributeName).execCommand("insertHTML", html);
  },

  addTranslation: function (targetSiteId, targetChannelId, translateType, summary) {
    this.translates.push({
      siteId: this.siteId,
      channelId: this.channelId,
      targetSiteId: targetSiteId,
      targetChannelId: targetChannelId,
      translateType: translateType,
      summary: summary,
    });
  },

  updateGroups: function (res, message) {
    this.groupNames = res.groupNames;
    utils.success(message);
  },

  syncEditors: function () {
    var $this = this;
    if (UE) {
      $.each(UE.instants, function (index, editor) {
        editor.sync();
        var style = $this.styles[editor.styleIndex];
        var text = $this.regexReplace($regCensor, editor.getContent());
        text = $this.regexReplace($regSpell, text);
        $this.form[utils.toCamelCase(style.attributeName)] = text;
      });
    }
  },

  regexReplace: function(regex, text) {
    var retVal = text;
    while ((match = regex.exec(text)) !== null) {
      retVal = retVal.replace(match[0], match[1]);
    }
    return retVal;
  },

  closeAndRedirect: function (isEdit) {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      if (isEdit) {
        tabVue.apiList(
          this.reloadChannelId > 0 ? this.reloadChannelId : this.channelId,
          this.page,
          "内容编辑成功！"
        );
      } else {
        tabVue.apiList(this.channelId, this.page, "内容新增成功！", true);
      }
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  btnImageSelectClick: function (args) {
    var inputType = args.inputType;
    var attributeName = args.attributeName;
    var no = args.no;
    var type = args.type;

    if (type === "uploadedImages") {
      this.btnLayerClick({
        title: "选择已上传图片",
        name: "formLayerImageSelect",
        inputType: inputType,
        attributeName: attributeName,
        no: no,
        full: true,
      });
    } else if (type === "materialImages") {
      this.btnLayerClick({
        title: "选择素材库图片",
        name: "materialLayerImageSelect",
        inputType: inputType,
        attributeName: attributeName,
        no: no,
        full: true,
      });
    } else if (type === "cloudImages") {
      utils.openLayer({
        title: "选择免版权图库",
        url: utils.getCloudsUrl("layerImagesSelect", {
          inputType: inputType,
          attributeName: args.attributeName,
          no: args.no,
        }),
      });
    }
  },

  btnLayerClick: function (options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId,
      editorAttributeName: "Body",
    };

    if (options.contentId) {
      query.contentId = options.contentId;
    }
    if (options.inputType) {
      query.inputType = options.inputType;
    }
    if (options.attributeName) {
      query.attributeName = options.attributeName;
    }
    if (options.no) {
      query.no = options.no;
    }

    var args = {
      title: options.title,
      url: utils.getCommonUrl(options.name, query),
    };
    if (!options.full) {
      args.width = options.width ? options.width : 750;
      args.height = options.height ? options.height : 550;
    }

    utils.openLayer(args);
  },

  handleTranslationClose: function (summary) {
    this.translates = _.remove(this.translates, function (n) {
      return summary !== n.summary;
    });
  },

  btnSaveCommandClick: function (command) {
    this.isScheduledDialog = command === 'scheduled';
  },

  btnSubmitClick: function () {
    var $this = this;
    this.syncEditors();
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.$refs.linkToForm.validate(function(valid) {
          if (valid) {
            $this.censorSettings.isCensorPassed = $this.spellSettings.isSpellPassed = false;
            $this.apiSave();
          }
        });
      }
    });
  },

  btnScheduledSaveClick: function () {
    var $this = this;
    this.$refs.scheduledForm.validate(function (valid) {
      if (valid) {
        var minutesDate = new Date();
        minutesDate.setMinutes(minutesDate.getMinutes() + 5);
        if (!$this.scheduledForm.scheduledDate || (new Date($this.scheduledForm.scheduledDate)).getTime() < minutesDate.getTime()) {
          utils.error("定时发布失败，定时发布时间只能是5分钟之后的某一时刻");
          return;
        }

        $this.isScheduledDialog = false;
        $this.btnSubmitClick();
      }
    });
  },

  btnSaveClick: function () {
    this.scheduledForm.isScheduled = false;
    this.btnSubmitClick();
  },

  btnCensorTextClick: function (isSave) {
    this.syncEditors();
    this.apiCloudCensor(isSave);
  },

  btnSpellingCheckClick: function (isSave) {
    this.syncEditors();
    this.apiCloudSpell(isSave);
  },

  btnTagsClick: function () {
    this.syncEditors();
    if (!this.form.body) return;
    this.apiTags();
  },

  btnPreviewClick: function () {
    var $this = this;
    if (this.isPreviewSaving) return;
    this.syncEditors();
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.apiPreview();
      } else {
        utils.error("预览失败，请检查表单值是否正确");
      }
    });
  },

  btnCloseClick: function () {
    utils.removeTab();
  },

  btnGroupAddClick: function () {
    utils.openLayer({
      title: "新增内容组",
      url: utils.getCommonUrl("groupContentLayerAdd", { siteId: this.siteId }),
      width: 500,
      height: 300,
    });
  },

  btnTranslateAddClick: function () {
    utils.openLayer({
      title: "选择转移栏目",
      url: utils.getCmsUrl("editorLayerTranslate", {
        siteId: this.siteId,
        channelId: this.channelId,
      }),
      width: 620,
      height: 400,
    });
  },

  btnLinkToContentClick: function () {
    var channelId = this.linkTo.channelIds[this.linkTo.channelIds.length - 1];
    utils.openLayer({
      title: "选择指定内容",
      url: utils.getCmsUrl("layerContentSelect", {
        siteId: this.siteId,
        channelId: channelId,
        contentId: this.contentId,
      }),
    });
  },

  btnExtendAddClick: function (style) {
    var no = this.form[utils.getCountName(style.attributeName)] + 1;
    this.form[utils.getCountName(style.attributeName)] = no;
    this.form[utils.getExtendName(style.attributeName, no)] = "";
    this.form = _.assign({}, this.form);
  },

  btnExtendRemoveClick: function (style) {
    var no = this.form[utils.getCountName(style.attributeName)];
    this.form[utils.getCountName(style.attributeName)] = no - 1;
    this.form[utils.getExtendName(style.attributeName, no)] = "";
    this.form = _.assign({}, this.form);
  },

  btnExtendPreviewClick: function (attributeName, no) {
    var count = this.form[utils.getCountName(attributeName)];
    var data = [];
    for (var i = 0; i <= count; i++) {
      var imageUrl = this.form[utils.getExtendName(attributeName, i)];
      imageUrl = utils.getUrl(this.siteUrl, imageUrl);
      data.push({
        src: imageUrl,
      });
    }
    layer.photos({
      photos: {
        start: no,
        data: data,
      },
      anim: 5,
    });
  },

  btnExtendPreviewVideoClick: function (videoUrl) {
    if (videoUrl) {
      utils.openLayer({
        title: "预览视频",
        url: utils.getCommonUrl("editorLayerPreviewVideo", {
          siteId: this.siteId,
          videoUrl: videoUrl,
        }),
        width: 600,
        height: 500,
      });
    }
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSaveClick, this.btnCloseClick);
    this.apiGet();
  },
});
