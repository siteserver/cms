var $url = "/cms/material/editor";
var $urlPreview = "/cms/material/editor/actions/preview";

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  page: utils.getQueryInt("page"),
  tabName: utils.getQueryString("tabName"),
  messageId: utils.getQueryInt("messageId"),
  groupId: utils.getQueryInt("groupId"),
  mainHeight: '',
  editor: null,
  previewDialog: false,
  previewForm: {
    wxNames: null
  },

  items: null,
  commentTypes: null,
  siteType: null,
  form: null,
  action: null
});

var methods = {
  runMaterialLayerArticlesSubmit: function(items, itemId) {
    var $this = this;

    if (itemId) {
      var index = this.items.findIndex(function(x) {
        return x.id === itemId;
      });
      this.items.splice(index, 1);
    }

    if (items && items.length > 0) {
      items.forEach(function(item) {
        var exists = $this.items.find(function(x) {
          return x.id === item.id;
        });
        if (!exists) {
          $this.items.push(item);
        }
      });

      this.form = items[0];
      this.editor.setContent(this.form.content);
    }
  },

  runMaterialLayerImageUploadText: function(thumbUrl) {
    this.form.thumbUrl = thumbUrl;
  },

  runMaterialLayerImageUploadEditor: function(html)
  {
    if (!html) return;
    this.form.content = this.editor.getContent() + '<br />' + html;
    this.editor.setContent(this.form.content);
  },

  runEditorLayerImage: function(attributeName, html) {
    this.insertHtml(html);
  },

  insertHtml: function(html) {
    if (!html) return;
    this.editor.execCommand('insertHTML', html);
  },

  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        messageId: this.messageId,
        siteId: this.siteId
      }
    }).then(function(response) {
      var res = response.data;
      
      $this.items = res.items;
      $this.commentTypes = res.commentTypes;
      $this.siteType = res.siteType;
      $this.form = $this.items[0];

      setTimeout(function () {
        $this.editor = utils.getEditor('editor');
        $this.editor.ready(function () {
          this.addListener("contentChange", function () {
            $this.form.content = this.getContent();
          });
        });

        window.onresize = function () {
          $this.mainHeight = ($(window).height() - 70) + 'px';
        };
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

  apiCreate: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      groupId: this.groupId,
      items: this.items
    })
    .then(function(response) {
      var res = response.data;
      $this.messageId = res.messageId;

      if ($this.action === 'preview') {
        $this.apiPreview();
      } else if ($this.action === 'send') {
        var vue = utils.getTabVue($this.tabName);
        if (vue) {
          vue.apiList($this.page);
        }
        utils.success('保存成功！');
        utils.removeTab();
        utils.addTab('新建群发', utils.getCmsUrl('openSend', {
          siteId: $this.siteId,
          messageId: $this.messageId
        }));
      } else {
        var vue = utils.getTabVue($this.tabName);
        if (vue) {
          vue.apiList($this.page);
          utils.openTab($this.tabName);
        }
        utils.success('创建图文消息成功！');
        utils.removeTab();
      }
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiUpdate: function() {
    var $this = this;

    utils.loading(this, true);
    $api.put($url, {
      messageId: this.messageId,
      groupId: this.groupId,
      items: this.items
    })
    .then(function(response) {
      var res = response.data;

      if ($this.action === 'preview') {
        $this.apiPreview();
      } else if ($this.action === 'send') {
        var vue = utils.getTabVue($this.tabName);
        if (vue) {
          vue.apiList($this.page);
        }
        utils.success('保存成功！');
        utils.removeTab();
        utils.addTab('新建群发', utils.getCmsUrl('openSend', {
          siteId: $this.siteId,
          messageId: $this.messageId
        }));
      } else {
        var vue = utils.getTabVue($this.tabName);
        if (vue) {
          vue.apiList($this.page);
          utils.openTab($this.tabName);
        }
        utils.success('修改图文消息成功！');
        utils.removeTab();
      }
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiPreview: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlPreview, {
      siteId: this.siteId,
      messageId: this.messageId,
      wxNames: this.previewForm.wxNames
    })
    .then(function(response) {
      var res = response.data;

      $this.previewDialog = false;
      utils.success('预览图文消息已发送至微信，请进入微信查看！');
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnItemClick: function (item) {
    this.form = item;
    this.editor.setContent(item.content);
  },

  btnAddClick: function (command) {
    if (command === 'new') {
      this.form = {};
      this.editor.setContent('');
      this.items.push(this.form);
    } else if (command === 'select') {
      var articleIds = this.items.filter(function(x) {
        return x.materialType === 'Article';
      }).map(function(x) {
        return x.materialId;
      }).join(',');

      utils.openLayer({
        title: '选择图文',
        url: utils.getCommonUrl('materialLayerArticleSelect', {
          siteId: this.siteId,
          articleIds: articleIds
        })
      });
    }
  },

  btnMenuSelect: function(key) {
    var command = key.split(':')[0];
    var index = key.split(':')[1];
    var item = this.items[index];
    if (command === 'select') {
      var articleIds = this.items.filter(function(x) {
        return x.materialType === 'Article';
      }).map(function(x) {
        return x.materialId;
      }).join(',');

      utils.openLayer({
        title: '选择图文',
        url: utils.getCommonUrl('materialLayerArticleSelect', {
          siteId: this.siteId,
          itemId: item.id,
          articleIds: articleIds
        })
      });
    } else if (command === 'up') {
      this.items.splice(index, 1);
      this.items.splice(index - 1, 0, item);
      this.btnItemClick(item);
    } else if (command === 'down') {
      this.items.splice(index, 1);
      this.items.splice(index + 1, 0, item);
      this.btnItemClick(item);
    } else if (command === 'delete') {
      this.btnItemClick(this.items[0]);
      this.items.splice(index, 1);
    }
    return false;
  },

  btnLayerClick: function(options) {
    utils.openLayer({
      title: options.title,
      url: utils.getCommonUrl('editorLayer' + options.name, {
        siteId: this.siteId,
        attributeName: options.attributeName
      }),
      full: options.full,
      width: options.width ? options.width : 700,
      height: options.height ? options.height : 500
    });
  },

  btnUploadClick: function() {
    utils.openLayer({
      title: '上传封面',
      url: utils.getCmsUrl('materialLayerImageUpload', {
        siteId: this.siteId,
        editorAttributeName: 'Body'
      }),
      width: 700,
      height: 500
    });
  },

  btnSubmitClick: function () {
    var $this = this;

    this.$refs.form.validate(function(valid) {
      if (valid) {
        for (var i = 0; i < $this.items.length; i++) {
          var item = $this.items[i];
          if (!item.title || !item.content) {
            $this.btnItemClick(item);
            return utils.error('标题及内容为必填项，请填写后提交保存！');
          }
        }
        
        if ($this.messageId === 0) {
          $this.apiCreate();
        } else {
          $this.apiUpdate();
        }
      } else {
        return utils.error('标题及内容为必填项，请填写后提交保存！');
      }
    });
  },

  btnPreviewClick: function() {
    this.previewDialog = true;
  },

  btnSendClick: function() {
    this.action = 'send';
    this.btnSubmitClick();
  },

  btnCloseClick: function() {
    utils.removeTab();
  },

  btnPreviewSubmitClick: function() {
    var $this = this;

    this.$refs.previewForm.validate(function(valid) {
      if (valid) {
        $this.action = 'preview';
        $this.btnSubmitClick();
      }
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  computed: {
    wxNamesCount: function (val, oldVal) {
      if (!this.previewForm.wxNames) return 1;
      return this.previewForm.wxNames.split('\n').length;
    }
  },
  methods: methods,
  created: function() {
    this.apiGet();
  }
});
