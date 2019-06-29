/** When your routing table is too long, you can split it into small modules**/

/* Layout */
import Layout from '@/layout'

const routers = [
  {
    path: 'create',
    component: () => import('@/views/charts/line'),
    name: 'settings-create',
    meta: { icon: 'add' }
  },
  {
    path: 'sites',
    component: Layout,
    name: 'settings-sites',
    meta: { icon: 'site' },
    children: [
      {
        path: 'all',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-all'
      },
      {
        path: 'url',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-url'
      },
      {
        path: 'assets',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-assets'
      },
      {
        path: 'api',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-api'
      },
      {
        path: 'tables',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-tables'
      },
      {
        path: 'sitetemplates',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-sitetemplates'
      },
      {
        path: 'templatesonline',
        component: () => import('@/views/charts/line'),
        name: 'settings-sites-templatesonline'
      }
    ]
  },
  {
    path: 'users',
    component: () => import('@/settings/users/index'),
    name: 'settings-users',
    meta: { icon: 'user' },
    children: [
      {
        path: 'all',
        component: () => import('@/views/charts/line'),
        name: 'settings-users-all'
      },
      {
        path: 'roles',
        component: () => import('@/settings/users/keyboard'),
        name: 'settings-users-roles'
      },
      {
        path: 'config',
        component: () => import('@/views/charts/line'),
        name: 'settings-users-config'
      },
      {
        path: 'departments',
        component: () => import('@/views/charts/line'),
        name: 'settings-users-departments'
      },
      {
        path: 'areas',
        component: () => import('@/views/charts/line'),
        name: 'settings-users-areas'
      },
      {
        path: 'tokens',
        component: () => import('@/settings/users/tokens'),
        name: 'settings-users-tokens'
      }
    ]
  },
  {
    path: 'tools',
    component: () => import('@/settings/tools/index'),
    name: 'settings-tools',
    meta: { icon: 'tools' },
    children: [
      {
        path: 'info',
        component: () => import('@/settings/tools/info'),
        name: 'settings-tools-info'
      },
      {
        path: 'cache',
        component: () => import('@/settings/tools/cache'),
        name: 'settings-tools-cache'
      }
    ]
  }
]

export default routers
