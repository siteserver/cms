<template>
  <div class="wrapper-page">
    <div class="card-box">
      <h4 class="m-l-5">
        系统错误
      </h4>

      <form class="form-horizontal m-t-20">
        <template v-if="pageAlert">
          <div class="alert" :class="{ 'alert-warning': pageAlert.type === 'warning', 'alert-success': pageAlert.type === 'success', 'alert-danger': pageAlert.type === 'danger' }">
            {{ pageAlert.html }}
          </div>
        </template>

        <hr>

        <div class="text-muted text-center">
          <a href="https://www.siteserver.cn" target="_blank">
            <small>Copyright &copy; SiteServer CMS</small>
          </a>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  layout: 'root',
  data() {
    return {
      pageSubmit: false,
      pageAlert: {
        type: 'danger',
        html: this.$route.query.message
      }
    }
  }
}
</script>
