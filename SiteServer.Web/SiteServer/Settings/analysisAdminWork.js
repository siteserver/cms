var $url = '/pages/settings/analysisAdminWork';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'chart',
  siteOptions: null,
  newX: null,
  newY: null,
  updateY: null,
  items: null,
  formInline: {
    siteIds: [],
    dateFrom: '',
    dateTo: ''
  },
  chartOption:null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.post($url, {
      siteId: 0,
      dateFrom: '',
      dateTo: ''
    }).then(function (response) {
      var res = response.data;

      $this.siteOptions = res.siteOptions;
      $this.formInline.siteIds = [res.siteId];
      $this.newX = res.newX;
      $this.newY = res.newY;
      $this.updateY = res.updateY;
      $this.items = res.items;
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
    $api.post($url, {
      siteId: this.formInline.siteIds && this.formInline.siteIds.length > 0 ? this.formInline.siteIds[0] : 0,
      dateFrom: this.formInline.dateFrom,
      dateTo: this.formInline.dateTo,
    }).then(function (response) {
      var res = response.data;

      $this.newX = res.newX;
      $this.newY = res.newY;
      $this.updateY = res.updateY;
      $this.items = res.items;
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
      legend: {
        data: ["新增信息数目", "更新信息数目"]
      },
      toolbox: {
        show: true,
        feature: {
          dataView: {
            show: true,
            readOnly: false
          },
          restore: {
            show: true
          },
          saveAsImage: {
            show: true
          }
        }
      },
      xAxis: [{
        type: 'category',
        data: this.newX
      }],
      yAxis: [{
        type: 'value'
      }],
      series: [{
          "name": "新增信息数目",
          "type": "bar",
          "data": this.newY
        },
        {
          "name": "更新信息数目",
          "type": "bar",
          "data": this.updateY
        }
      ],
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