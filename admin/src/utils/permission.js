import store from '@/store'

/**
 * @param {Array} value
 * @returns {Boolean}
 * @example see @/views/permission/directive.vue
 */
export function checkPermission(value) {
  if (value && value instanceof Array && value.length > 0) {
    const roles = store.getters && store.getters.roles
    const permissionRoles = value

    const hasPermission = roles.some(role => {
      return permissionRoles.includes(role)
    })

    if (!hasPermission) {
      return false
    }
    return true
  } else {
    console.error(`need roles! Like v-permission="['admin','editor']"`)
    return false
  }
}

// {
//   path: '/permission',
//   component: Layout,
//   redirect: '/permission/index',
//   hidden: true,
//   alwaysShow: true,
//   meta: { roles: ['admin','editor'] }, // you can set roles in root nav
//   children: [{
//     path: 'index',
//     component: _import('permission/index'),
//     name: 'permission',
//     meta: {
//       title: 'permission',
//       icon: 'lock',
//       roles: ['admin','editor'], // or you can only set roles in sub nav
//       noCache: true
//     }
//   }]
// }

// {
//   "id": "site",
//   "text": "站点管理",
//   "iconClass": null,
//   "link": null,
//   "target": null,
//   "pluginId": null,
//   "selected": false,
//   "permissions": null,
//   "menus": [

export function parseRoutes(menus) {
  if (!menus || menus.length === 0) return []
  const routes = []
  for (const menu of menus) {
    routes.push(parseRoute(menu))
  }
  return routes
}

export function parseRoute(menu) {
  if (!menu) return null

  if (menu.menus && menu.menus.length > 0) {
    const children = []
    for (const child of menu.menus) {
      var route = parseRoute(child)
      if (route) {
        children.push(route)
      }
    }
    return {
      path: `/${menu.id}`,
      component: 'layout/Layout',
      name: menu.id,
      meta: { title: menu.text, icon: 'documentation', affix: true },
      children: children
    }
  } else {
    return {
      path: `/${menu.id}`,
      component: 'layout/Layout',
      children: [
        {
          path: 'index',
          component: `views/${menu.id}/index`,
          name: menu.id,
          meta: { title: menu.text, icon: 'documentation', affix: true }
        }
      ]
    }
  }
}
