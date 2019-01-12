var $url = '/pages/cms/contentsLayerExport';

var $data = {
  siteId: parseInt(utils.getQueryString('siteId')),
  channelId: parseInt(utils.getQueryString('channelId')),
  pageLoad: false,
  pageAlert: null,
  columns: null,
  checkedLevels: null,
  checkedLevel: null,
  exportType: 'zip',
  isAllCheckedLevel: true,
  checkedLevelKeys: [],
  isAllDate: true,
  startDate: new Date(new Date().setDate(new Date().getDate() - 30)),
  endDate: new Date(),
  isAllColumns: false,
  columnNames: []
};

var $methods = {
  loadConfig: function () {
    var $this = this;
    $api.get($url, {
      params: {
        siteId: $this.siteId,
        channelId: $this.channelId
      }
    }).then(function (response) {
      var res = response.data;

      $this.columns = res.value;
      $this.columnNames = [];
      for (var i = 0; i < $this.columns.length; i++) {
        var attribute = $this.columns[i];
        if (attribute.isList) {
          $this.columnNames.push(attribute.attributeName);
        }
      }
      $this.checkedLevels = res.checkedLevels;
      for (var i = 0; i < $this.checkedLevels.length; i++) {
        var checkedLevel = $this.checkedLevels[i];
        $this.checkedLevelKeys.push(checkedLevel.key);
      }
      $this.checkedLevel = res.checkedLevel;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnIsAllCheckedLevelClick: function () {
    this.isAllCheckedLevel = !this.isAllCheckedLevel;
    this.checkedLevelKeys = [];
    if (this.isAllCheckedLevel) {
      for (var i = 0; i < this.checkedLevels.length; i++) {
        var checkedLevel = this.checkedLevels[i];
        this.checkedLevelKeys.push(checkedLevel.key);
      }
    }
  },

  btnIsAllColumnsClick: function () {
    this.isAllColumns = !this.isAllColumns;
    this.columnNames = ['Title'];
    if (this.isAllColumns) {
      for (var i = 0; i < this.columns.length; i++) {
        var column = this.columns[i];
        this.columnNames.push(column.attributeName);
      }
    }
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    if (this.checkedLevelKeys.length === 0) {
      return this.pageAlert = {
        type: 'danger',
        html: '必须至少选择一项内容状态'
      };
    }

    utils.loading(true);
    $api.post($url, {
      siteId: $this.siteId,
      channelId: $this.channelId,
      exportType: $this.exportType,
      isAllCheckedLevel: $this.isAllCheckedLevel,
      checkedLevelKeys: $this.checkedLevelKeys,
      isAllDate: $this.isAllDate,
      startDate: $this.startDate,
      endDate: $this.endDate,
      columnNames: $this.columnNames
    }).then(function (response) {
      var res = response.data;

      if (res.isSuccess) {
        window.open(res.value);
        parent.layer.closeAll();
      } else {
        return $this.pageAlert = {
          type: 'danger',
          html: '没有符合条件的内容，请重新选择导出条件'
        };
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

Vue.component("date-picker", window.DatePicker.default);

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.loadConfig();
  }
});