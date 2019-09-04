/** When your routing table is too long, you can split it into small modules**/

/* Layout */
import Layout from '@/layout'

const routers = [
  {
    path: 'sites',
    component: () => import('@/settings/sites/index'),
    name: 'settingsSites',
    meta: { icon: 'site' },
    children: [
      {
        path: 'create',
        component: () => import('@/settings/sites/create'),
        name: 'settingsSitesCreate'
      },
      {
        path: 'all',
        component: () => import('@/settings/sites/all'),
        name: 'settingsSitesAll'
      },
      {
        path: 'url',
        component: () => import('@/settings/sites/url'),
        name: 'settingsSitesUrl'
      },
      {
        path: 'assets',
        component: () => import('@/settings/sites/assets'),
        name: 'settingsSitesAssets'
      },
      {
        path: 'api',
        component: () => import('@/settings/sites/api'),
        name: 'settingsSitesApi'
      },
      {
        path: 'tables',
        component: () => import('@/settings/sites/tables'),
        name: 'settingsSitesTables'
      }
    ]
  },
  {
    path: 'themes',
    component: Layout,
    name: 'settingsThemes',
    meta: { icon: 'site' },
    children: [
      {
        path: 'themes',
        component: () => import('@/settings/themes/local'),
        name: 'settingsThemesLocal'
      },
      {
        path: 'online',
        component: () => import('@/settings/themes/online'),
        name: 'settingsThemesOnline'
      }
    ]
  },
  {
    path: 'users',
    component: () => import('@/settings/users/index'),
    name: 'settingsUsers',
    meta: { icon: 'user' },
    children: [
      {
        path: 'all',
        component: () => import('@/settings/users/all'),
        name: 'settingsUsersAll'
      },
      {
        path: 'roles',
        component: () => import('@/settings/users/keyboard'),
        name: 'settingsUsersRoles'
      },
      {
        path: 'config',
        component: () => import('@/views/charts/line'),
        name: 'settingsUsersConfig'
      },
      {
        path: 'departments',
        component: () => import('@/views/charts/line'),
        name: 'settingsUsersDepartments'
      },
      {
        path: 'areas',
        component: () => import('@/views/charts/line'),
        name: 'settingsUsersAreas'
      },
      {
        path: 'tokens',
        component: () => import('@/settings/users/tokens'),
        name: 'settingsUsersTokens'
      }
    ]
  },
  {
    path: 'tools',
    component: () => import('@/settings/tools/index'),
    name: 'settingsTools',
    meta: { icon: 'tools' },
    children: [
      {
        path: 'info',
        component: () => import('@/settings/tools/info'),
        name: 'settingsToolsInfo'
      }
    ]
  }
]

export default routers
