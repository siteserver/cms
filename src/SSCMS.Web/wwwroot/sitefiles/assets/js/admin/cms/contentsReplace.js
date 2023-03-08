var $url = '/cms/contents/contentsReplace';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  checkedChannelIds: [],
  channels: null,
  expandedChannelIds: [],
  filterText: '',

  attributes: null,

  channelIds: [],

  form: {
    attributeNames: [],
    replace: '',
    isCaseSensitive: true,
    isRegex: false,
    isDescendant: false,
    to: '',
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;
      $this.channels = [res.channels];
      $this.expandedChannelIds = _.union([$this.siteId], $this.checkedChannelIds);
      $this.attributes = res.attributes;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function (data) {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, data).then(function (response) {
      var res = response.data;

      utils.success('批量替换成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getChannelUrl: function(data) {
    return utils.getRootUrl('redirect', {
      siteId: this.siteId,
      channelId: data.value
    });
  },

  filterNode: function(value, data) {
    if (!value) return true;
    return data.label.indexOf(value) !== -1 || data.value + '' === value;
  },

  handleCheckChange() {
    this.channelIds = this.$refs.tree.getCheckedKeys();
  },

  btnReplaceClick: function() {
    this.form.attributeNames = [];
    for (var i = 0; i < this.attributes.length; i++) {
      var attribute = this.attributes[i];
      if (attribute.selected) {
        this.form.attributeNames.push(attribute.value);
      }
    }

    if (this.form.attributeNames.length === 0) {
      utils.error('请选择需要替换的字段！');
      return;
    }

    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        utils.alertDelete({
          title: '批量替换',
          text: '此操作将批量替换对应文字且无法恢复，确定吗？',
          button: '批量替换',
          callback: function () {
            $this.apiSubmit({
              siteId: $this.siteId,
              channelIds: $this.channelIds,
              attributeNames: $this.form.attributeNames,
              replace: $this.form.replace,
              isCaseSensitive: $this.form.isCaseSensitive,
              isRegex: $this.form.isRegex,
              isDescendant: $this.form.isDescendant,
              to: $this.form.to,
            });
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
  watch: {
    filterText: function(val) {
      this.$refs.tree.filter(val);
    }
  },
  created: function () {
    utils.keyPress(this.btnReplaceClick, this.btnCloseClick);
    this.apiGet();
  }
});
