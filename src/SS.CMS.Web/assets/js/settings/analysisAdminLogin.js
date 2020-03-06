var $url = '/admin/settings/analysisAdminLogin';

var data = utils.initData({
  pageType: 'date',
  dateX: null,
  dateY: null,
  nameX: null,
  nameY: null,
  formInline: {
    dateFrom: '',
    dateTo: '',
    xType: 'Day'
  },
  chartOption:null
});

var methods = {
  getConfig: function () {
    var $this = this;

    $api.post($url, this.formInline).then(function (response) {
      var res = response.data;

      $this.dateX = res.dateX;
      $this.dateY = res.dateY;
      $this.nameX = res.nameX;
      $this.nameY = res.nameY;
      $this.loadCharts();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.formInline).then(function (response) {
      var res = response.data;

      $this.dateX = res.dateX;
      $this.dateY = res.dateY;
      $this.nameX = res.nameX;
      $this.nameY = res.nameY;
      $this.loadCharts();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  loadCharts: function () {
    if (this.pageType === 'date') {
      this.chartOption = {
        tooltip: {
          show: true
        },
        title: {
          text: '按日期统计'
        },
        xAxis: [{
          type: 'category',
          data: this.dateX
        }],
        yAxis: [{
          type: 'value'
        }],
        series: [{
          "name": "登录次数",
          "type": "line",
          "data": this.dateY
        }],
        animationDuration: 2000
      };
    } else {
      this.chartOption = {
        tooltip: {
          show: true
        },
        title: {
          text: '按管理员统计'
        },
        xAxis: [{
          type: 'category',
          data: this.nameX
        }],
        yAxis: [{
          type: 'value'
        }],
        series: [{
          "name": "登录次数",
          "type": "bar",
          "data": this.nameY
        }],
        animationDuration: 2000
      };
    }
  }
};

Vue.component('v-chart', VueECharts);

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});