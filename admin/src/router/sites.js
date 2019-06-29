/** When your routing table is too long, you can split it into small modules**/

const routers = [
  {
    path: 'keyboard',
    component: () => import('@/views/charts/keyboard'),
    name: 'SitesKeyboardChart',
    meta: { title: 'keyboardChart' }
  },
  {
    path: 'line',
    component: () => import('@/views/charts/line'),
    name: 'SitesLineChart',
    meta: { title: 'lineChart' }
  },
  {
    path: 'mix-chart',
    component: () => import('@/views/charts/mix-chart'),
    name: 'SitesMixChart',
    meta: { title: 'mixChart' }
  }
]

export default routers
