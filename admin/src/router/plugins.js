/** When your routing table is too long, you can split it into small modules**/

const routers = [
  {
    path: 'install',
    component: () => import('@/views/charts/keyboard'),
    name: 'plugins-install',
    meta: { icon: 'add' }
  },
  {
    path: 'manage',
    component: () => import('@/views/charts/line'),
    name: 'plugins-manage',
    meta: { icon: 'plugin' }
  }
]

export default routers
