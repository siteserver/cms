<template>
  <div class="card-box">
    <div class="row app-countdown" style="margin-top: 0">
      <div class="col-12">
        <div>
          <div>
            <h3>剩余页面：</h3>
          </div>
          <div>
            <span :style="{color: channelsCount? '#fa0' : '#00b19d'}">
              {{ channelsCount }}
            </span>
            <span>
              <b>栏目页</b>
            </span>
          </div>
          <div>
            <span :style="{color: contentsCount? '#fa0' : '#00b19d'}">
              {{ contentsCount }}
            </span>
            <span>
              <b>内容页</b>
            </span>
          </div>
          <div>
            <span :style=" {color: filesCount? '#fa0' : '#00b19d' }">
              {{ filesCount }}
            </span>
            <span>
              <b>文件页</b>
            </span>
          </div>
          <div>
            <span :style=" {color: specialsCount? '#fa0' : '#00b19d' }">
              {{ specialsCount }}
            </span>
            <span>
              <b>专题页</b>
            </span>
          </div>
          <div v-if="channelsCount > 0 || contentsCount > 0 || filesCount > 0 || specialsCount > 0">
            <button type="button" class="btn btn-sm btn-danger m-l-5" @click="btnCancelClick">
              取消生成任务
            </button>
          </div>
        </div>
      </div>
    </div>
    <hr>
    <div v-for="task in tasks" :key="task.id">
      <div v-if="task.isPending" class="form-row">
        <label class="col-2 col-form-label-sm">
          {{ task.type }}
        </label>
        <div class="col-10">
          <div class="progress progress-md m-t-5" style="height: 20px !important">
            <div class="progress-bar bg-warning progress-bar-striped progress-bar-animated" style="width: 100%;">
              {{ task.name }}
            </div>
          </div>
        </div>
      </div>
      <div v-else-if="task.isSuccess" class="form-row">
        <label class="col-2 col-form-label-sm">
          {{ task.type }}
        </label>
        <div class="col-10">
          <div class="progress progress-md m-t-5" style="height: 20px !important">
            <div class="progress-bar bg-primary" style="width: 100%;">
              <a :href="utils.getRedirectUrl(task)" target="_blank" style="color:#fff;">
                {{ task.name }}（用时：{{
                  task.timeSpan }}）
              </a>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="form-row">
        <label class="col-2 col-form-label-sm">
          {{ task.type }}
        </label>
        <div class="col-10">
          <div class="progress progress-md m-t-5" style="height: 20px !important">
            <div class="progress-bar bg-danger" style="width: 100%;">
              {{ task.name }}（错误：{{ task.errorMessage }}）
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      siteId: parseInt(this.$route.query.siteId),
      tasks: null,
      channelsCount: null,
      contentsCount: null,
      filesCount: null,
      specialsCount: null,
      timeoutId: null
    }
  },
  mounted() {
    this.load()
  },
  methods: {
    async load() {
      const data = await this.$api.createStatus.get({
        siteId: this.siteId
      })

      this.tasks = data.value.tasks
      this.channelsCount = data.value.channelsCount
      this.contentsCount = data.value.contentsCount
      this.filesCount = data.value.filesCount
      this.specialsCount = data.value.specialsCount

      this.pageLoad = true
      this.timeoutId = setTimeout(() => {
        this.load()
      }, 3000)
    },

    async btnCancelClick() {
      clearTimeout(this.timeoutId)

      this.$store.commit('loading', true)
      await this.$api.createStatus.postAt('actions/cancel', {
        siteId: this.siteId
      })

      this.load()
    }
  }
}
</script>
