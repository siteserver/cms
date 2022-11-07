var $url = "/cms/editor";
var $urlInsert = $url + "/actions/insert";
var $urlUpdate = $url + "/actions/update";
var $urlPreview = $url + "/actions/preview";
var $urlCensor = $url + "/actions/censor";
var $urlSpell = $url + "/actions/spell";
var $urlTags = $url + "/actions/tags";

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
  sideType: "first",
  collapseSettings: ["checkedLevel", "addDate"],
  collapseMore: ["templateId", "translates"],

  csrfToken: null,
  site: null,
  siteUrl: null,
  channel: null,
  groupNames: null,
  tagNames: null,
  checkedLevels: null,
  censorSettings: null,
  spellSettings: null,
  isCensorPassed: false,
  isSpellPassed: false,
  linkTypes: null,
  root: null,
  styles: null,
  relatedFields: null,
  templates: null,
  form: null,

  translates: [],
  isPreviewSaving: false,
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

        $this.csrfToken = res.csrfToken;

        $this.site = res.site;
        $this.siteUrl = res.siteUrl;
        $this.channel = res.channel;
        $this.groupNames = res.groupNames;
        $this.tagNames = res.tagNames;
        $this.checkedLevels = res.checkedLevels;
        $this.linkTypes = res.linkTypes;
        $this.root = [res.root];
        $this.censorSettings = res.censorSettings;
        $this.spellSettings = res.spellSettings;

        $this.styles = res.styles;
        $this.relatedFields = res.relatedFields;
        $this.templates = res.templates;
        $this.form = _.assign({}, res.content);

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

  apiCensor: function (callback) {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlCensor, {
        siteId: this.siteId,
        channelId: this.channelId,
        content: this.form,
      })
      .then(function (response) {
        var res = response.data;
        callback(res);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSpell: function (callback) {
    var $this = this;

    utils.loading(this, true);
    $api
      .csrfPost(this.csrfToken, $urlSpell, {
        siteId: this.siteId,
        channelId: this.channelId,
        content: this.form,
      })
      .then(function (response) {
        var res = response.data;
        callback(res);
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
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
    if (!this.isCensorPassed && this.censorSettings.isCensorText && this.censorSettings.isCensorTextAuto) {
      this.btnCensorTextClick(true);
    } else if (!this.isSpellPassed && this.spellSettings.isSpellingCheck && this.spellSettings.isSpellingCheckAuto) {
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
        $this.form[utils.toCamelCase(style.attributeName)] = editor.getContent();
      });
    }
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

  btnLayerClick: function (options) {
    var query = {
      siteId: this.siteId,
      channelId: this.channelId,
      editorAttributeName: "Body",
    };

    if (options.contentId) {
      query.contentId = options.contentId;
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
      args.width = options.width ? options.width : 700;
      args.height = options.height ? options.height : 500;
    }

    utils.openLayer(args);
  },

  handleTranslationClose: function (summary) {
    this.translates = _.remove(this.translates, function (n) {
      return summary !== n.summary;
    });
  },

  btnSaveClick: function () {
    var $this = this;
    this.syncEditors();
    this.$refs.form.validate(function (valid) {
      if (valid) {
        $this.isCensorPassed = this.isSpellPassed = false;
        $this.apiSave();
      } else {
        utils.error("保存失败，请检查表单值是否正确");
      }
    });
  },

  btnCensorTextClick: function (isSave) {
    this.syncEditors();
    utils.openLayer({
      title: "内容违规检测",
      width: 550,
      height: 600,
      url: utils.getCmsUrl('editorLayerCensor', {
        siteId: this.siteId,
        channelId: this.channelId,
        isCensorTextIgnore: this.censorSettings.isCensorTextIgnore,
        isCensorTextWhiteList: this.censorSettings.isCensorTextWhiteList,
        isSave: isSave
      })
    });
  },

  btnSpellingCheckClick: function (isSave) {
    this.syncEditors();
    utils.openLayer({
      title: "错别字检查",
      width: 550,
      height: 600,
      url: utils.getCmsUrl('editorLayerSpell', {
        siteId: this.siteId,
        channelId: this.channelId,
        isSpellingCheckIgnore: this.spellSettings.isSpellingCheckIgnore,
        isSpellingCheckWhiteList: this.spellSettings.isSpellingCheckWhiteList,
        isSave: isSave
      })
    });
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
          videoUrl: videoUrl
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
