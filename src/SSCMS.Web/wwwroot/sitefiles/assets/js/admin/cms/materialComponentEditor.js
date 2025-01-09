var $url = '/cms/material/componentEditor';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  groupId: utils.getQueryInt("groupId"),
  componentId: utils.getQueryInt("componentId"),
  tabName: utils.getQueryString("tabName"),
  component: null,
  form: null,
  contentEditor: null
});

var methods = {
  apiGet: function () {
    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId,
        groupId: this.groupId,
        componentId: this.componentId,
      }
    }).then(response => {
      var res = response.data;

      this.form = Object.assign({}, res.component);
      this.form.parameters = [];
      for (const item of (res.component.parameters || '').split(',')) {
        this.form.parameters.push({
          key: item
        });
      }
      this.setEditorContent(res.component.content);
    }).catch(function (error) {
      utils.error(error);
    }).then(() => {
      utils.loading(this, false);
    });
  },

  apiSubmit: function (isClose) {
    var parameters = [];
    for (const item of this.form.parameters) {
      if (item.key) {
        parameters.push(item.key);
      }
    }
    var data = {
      siteId: this.siteId,
      groupId: this.groupId,
      componentId: this.componentId,
      title: this.form.title,
      description: this.form.description,
      parameters: parameters.join(','),
      content: this.getEditorContent(),
    };

    utils.loading(this, true);
    $api.post($url, data).then(response => {
      var res = response.data;

      this.componentId = res.value;
      utils.success('组件保存成功!');
      if (isClose) {
        this.closeAndReload();
      }
    }).catch(function (error) {
      utils.error(error);
    }).then(() => {
      utils.loading(this, false);
    });
  },

  closeAndReload: function() {
    var tabVue = utils.getTabVue(this.tabName);
    if (tabVue) {
      tabVue.apiGet();
    }
    utils.removeTab();
    utils.openTab(this.tabName);
  },

  getEditorContent: function() {
    return this.contentEditor.getModel().getValue();
  },

  setEditorContent: function(val) {
    if (this.contentEditor) {
      this.contentEditor.getModel().setValue(val);
      this.contentEditor.focus();
    } else {
      setTimeout(() => {
        $('#content').height(500);
        require.config({ paths: { 'vs': utils.getAssetsUrl('lib/monaco-editor/min/vs') }});
        require(['vs/editor/editor.main'], () => {
            this.contentEditor = monaco.editor.create(document.getElementById('content'), {
                value: val,
                language: 'html'
            });
            this.contentEditor.focus();
        });
      }, 100);
    }
  },

  btnRemoveClick: function (index) {
    this.form.parameters.splice(index, 1);
  },

  btnAddClick: function () {
    this.form.parameters.push({
      key: ''
    })
  },

  btnFormatClick: function() {
    this.contentEditor.getAction('editor.action.formatDocument').run().then(function() {
      utils.success('组件代码格式化成功!');
    });
  },

  btnSubmitClick: function(isClose) {
    this.$refs.form1.validate((valid) => {
      if (valid) {
        this.$refs.form2.validate((valid) => {
          if (valid) {
            this.apiSubmit(isClose);
          }
        });
      }
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    this.apiGet();
  }
});
