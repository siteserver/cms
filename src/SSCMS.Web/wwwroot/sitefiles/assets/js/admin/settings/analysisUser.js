var $url = '/settings/analysisUser';

var now = new Date();
var data = utils.init({
  form: {
    dateFrom: utils.getQueryString('dateFrom') || utils.formatDate(new Date(now.setMonth(now.getMonth()-1))),
    dateTo: utils.getQueryString('dateTo') || utils.formatDate(new Date()),
  },
  chartData: null
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      $this.chartData = {
        labels: res.days,
        datasets: [
          {
            label: "用户注册数",
            backgroundColor: "#409EFF",
            data: res.registerCount
          },
          {
            label: "用户登录数",
            backgroundColor: "#67C23A",
            data: res.loginCount
          }
          
        ]
      };
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnSearchClick() {
    location.href = '?dateFrom=' + this.form.dateFrom + '&dateTo=' + this.form.dateTo;
  }
};

Vue.component("line-chart", {
  extends: VueChartJs.Line,
  mounted: function() {
    this.renderChart(
      this.$root.chartData,
      {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true
              }
            }
          ]
        }
      }
    );
  }
});

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});