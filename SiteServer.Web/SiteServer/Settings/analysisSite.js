var $url = '/pages/settings/analysisSite';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: 'new',
  newX: null,
  newY: null,
  updateX: null,
  updateY: null,
  items: null,
  formInline: {
    dateFrom: '',
    dateTo: ''
  },
  chartOption:null
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.post($url, {
      dateFrom: '',
      dateTo: ''
    }).then(function (response) {
      var res = response.data;

      $this.newX = res.newX;
      $this.newY = res.newY;
      $this.updateX = res.updateX;
      $this.updateY = res.updateY;
      $this.items = res.items;
      $this.loadCharts();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSearchClick() {
    var $this = this;

    utils.loading(true);
    $api.post($url, {
      dateFrom: this.formInline.dateFrom,
      dateTo: this.formInline.dateTo,
    }).then(function (response) {
      var res = response.data;

      $this.newX = res.newX;
      $this.newY = res.newY;
      $this.updateX = res.updateX;
      $this.updateY = res.updateY;
      $this.items = res.items;
      $this.loadCharts();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  loadCharts: function () {
    if (this.pageType === 'new') {
      this.chartOption = {
        tooltip: {
          show: true
        },
        title: {
          text: '新增内容量'
        },
        xAxis: [{
          type: 'category',
          data: this.newX
        }],
        yAxis: [{
          type: 'value'
        }],
        series: [{
          "name": "增加量",
          "type": "bar",
          "data": this.newY
        }],
        animationDuration: 2000
      };
    } else if (this.pageType === 'update') {
      this.chartOption = {
        tooltip: {
          show: true
        },
        title: {
          text: '修改内容量'
        },
        xAxis: [{
          type: 'category',
          data: this.updateX
        }],
        yAxis: [{
          type: 'value'
        }],
        series: [{
          "name": "增加量",
          "type": "bar",
          "data": this.updateY
        }],
        animationDuration: 2000
      };
    }
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