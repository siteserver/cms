var $url = '/admin/cms/contents/contentsLayerExport';

var data = utils.initData({
  columns: null,
  checkedLevels: null,
  checkedLevel: null,

  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    channelContentIds: utils.getQueryString('channelContentIds'),
    exportType: 'zip',
    isAllCheckedLevel: true,
    checkedLevelKeys: [],
    isAllDate: true,
    startDate: new Date(new Date().setDate(new Date().getDate() - 30)),
    endDate: new Date(),
    isAllColumns: true,
    columnNames: []
  }
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.form.siteId,
        channelId: this.form.channelId
      }
    }).then(function (response) {
      var res = response.data;

      $this.columns = res.value;
      $this.form.columnNames = [];
      for (var i = 0; i < $this.columns.length; i++) {
        var attribute = $this.columns[i];
        if (attribute.isList) {
          $this.form.columnNames.push(attribute.attributeName);
        }
      }
      $this.checkedLevels = res.checkedLevels;
      for (var i = 0; i < $this.checkedLevels.length; i++) {
        var checkedLevel = $this.checkedLevels[i];
        $this.form.checkedLevelKeys.push(checkedLevel.key);
      }
      $this.checkedLevel = res.checkedLevel;
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      if (res.isSuccess) {
        window.open(res.value);
        utils.closeLayer();
      } else {
        return $this.$message.error('没有符合条件的内容，请重新选择导出条件');
      }
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnIsAllCheckedLevelClick: function () {
    this.isAllCheckedLevel = !this.form.isAllCheckedLevel;
    this.form.checkedLevelKeys = [];
    if (this.form.isAllCheckedLevel) {
      for (var i = 0; i < this.checkedLevels.length; i++) {
        var checkedLevel = this.checkedLevels[i];
        this.form.checkedLevelKeys.push(checkedLevel.key);
      }
    }
  },

  btnIsAllColumnsClick: function () {
    this.form.isAllColumns = !this.form.isAllColumns;
    this.form.columnNames = ['Title'];
    if (this.form.isAllColumns) {
      for (var i = 0; i < this.columns.length; i++) {
        var column = this.columns[i];
        this.form.columnNames.push(column.attributeName);
      }
    }
  },

  btnSubmitClick: function () {
    if (this.form.checkedLevelKeys.length === 0) {
      return this.$message.error('必须至少选择一项内容状态');
    }

    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});