var $url = '/settings/analysisSiteContent';

var now = new Date();
var data = utils.init({
  form: {
    siteId: utils.getQueryInt('siteId'),
    dateFrom: utils.getQueryString('dateFrom') || utils.formatDate(new Date(now.setMonth(now.getMonth()-1))),
    dateTo: utils.getQueryString('dateTo') || utils.formatDate(new Date()),
  },
  sites: null,
  chartData: null,
  siteIds: [utils.getQueryInt('siteId')]
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      $this.sites = res.sites;

      $this.chartData = {
        labels: res.days,
        datasets: [
          {
            label: "新增内容数",
            backgroundColor: "#409EFF",
            data: res.addCount
          },
          {
            label: "修改内容数",
            backgroundColor: "#67C23A",
            data: res.editCount
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
    var siteId = this.siteIds && this.siteIds.length > 0 ? this.siteIds[this.siteIds.length - 1] : 0;
    location.href = '?dateFrom=' + this.form.dateFrom + '&dateTo=' + this.form.dateTo + '&siteId=' + siteId;
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