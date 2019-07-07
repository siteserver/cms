/** When your routing table is too long, you can split it into small modules**/

const routers = [
  {
    path: 'install',
    component: () => import('@/views/charts/keyboard'),
    name: 'pluginsInstall',
    meta: { icon: 'add' }
  },
  {
    path: 'manage',
    component: () => import('@/views/charts/line'),
    name: 'pluginsManage',
    meta: { icon: 'plugin' }
  }
]

export default routers
