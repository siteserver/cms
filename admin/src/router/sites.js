/** When your routing table is too long, you can split it into small modules**/

const routers = [
  {
    path: '',
    component: () => import('@/sites/index'),
    name: 'sitesSite',
    meta: { icon: 'site' },
    children: [
      {
        path: 'contents',
        component: () => import('@/sites/contents/index'),
        name: 'sitesSiteContents'
      },
      {
        path: 'channels',
        component: () => import('@/sites/channels/index'),
        name: 'sitesSiteChannels'
      }
    ]
  },
  {
    path: 'themes',
    component: () => import('@/sites/themes/index'),
    name: 'sitesThemes',
    meta: { icon: 'site' },
    children: [
      {
        path: 'local',
        component: () => import('@/sites/themes/local'),
        name: 'sitesThemesLocal'
      },
      {
        path: 'online',
        component: () => import('@/sites/themes/online'),
        name: 'sitesThemesOnline'
      }
    ]
  },
  {
    path: 'users',
    component: () => import('@/sites/users/index'),
    name: 'sitesUsers',
    meta: { icon: 'user' },
    children: [
      {
        path: 'all',
        component: () => import('@/views/charts/line'),
        name: 'sitesUsersAll'
      },
      {
        path: 'roles',
        component: () => import('@/sites/users/keyboard'),
        name: 'sitesUsersRoles'
      },
      {
        path: 'config',
        component: () => import('@/views/charts/line'),
        name: 'sitesUsersConfig'
      },
      {
        path: 'departments',
        component: () => import('@/views/charts/line'),
        name: 'sitesUsersDepartments'
      },
      {
        path: 'areas',
        component: () => import('@/views/charts/line'),
        name: 'sitesUsersAreas'
      },
      {
        path: 'tokens',
        component: () => import('@/sites/users/tokens'),
        name: 'sitesUsersTokens'
      }
    ]
  },
  {
    path: 'tools',
    component: () => import('@/sites/tools/index'),
    name: 'sitesTools',
    meta: { icon: 'tools' },
    children: [
      {
        path: 'info',
        component: () => import('@/sites/tools/info'),
        name: 'sitesToolsInfo'
      }
    ]
  }
]

export default routers
