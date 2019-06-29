// translate router.meta.title, be used in breadcrumb sidebar tagsview
export function generateTitle(route) {
  const hasKey = this.$te('route.' + route.name)

  if (hasKey) {
    // $t :this method from vue-i18n, inject in @/lang/index.js
    const translatedTitle = this.$t('route.' + route.name)

    return translatedTitle
  }
  return route.meta.title
}
