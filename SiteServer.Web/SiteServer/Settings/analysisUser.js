var $url = '/pages/settings/analysisUser';

var data = {
  pageLoad: false,
  pageAlert: null,
  dateX: null,
  dateY: null,
  formInline: {
    dateFrom: '',
    dateTo: '',
    xType: 'Day'
  },
  chartOption:null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.post($url, this.formInline).then(function (response) {
      var res = response.data;

      $this.dateX = res.dateX;
      $this.dateY = res.dateY;
      $this.loadCharts();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSearchClick() {
    var $this = this;

    utils.loading($this, true);
    $api.post($url, this.formInline).then(function (response) {
      var res = response.data;

      $this.dateX = res.dateX;
      $this.dateY = res.dateY;
      $this.loadCharts();
    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  loadCharts: function () {
    this.chartOption = {
      tooltip: {
        show: true
      },
      title: {
        text: '用户注册量'
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
  }
};

Vue.component('v-chart', VueECharts);

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});