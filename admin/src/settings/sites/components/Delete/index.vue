<template>
  <div>
    <el-form ref="appendForm" :model="appendForm" :rules="rules" label-width="80px">
      <el-form-item label="站点名称">{{ site.siteName }}</el-form-item>
      <el-form-item label="文件夹">{{ site.siteDir }}</el-form-item>
      <el-form-item label="文件夹" prop="siteDir">
        <el-input v-model="appendForm.siteDir" placeholder="请输入需要删除的站点文件夹名称" />
      </el-form-item>
      <el-form-item>
        <el-button type="danger" :loading="loading" @click="onSubmit">删除站点</el-button>
        <el-button @click="onCancel">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { deleteSite } from '@/api/sites'

export default {
  props: {
    site: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      appendForm: {
        siteDir: null
      },
      rules: {
        siteDir: [{ required: true }]
      }
    }
  },
  methods: {
    async apiSubmit() {
      await deleteSite(this.site.id)
      this.$emit('submit')
    },

    onSubmit() {
      this.$refs['appendForm'].validate(valid => {
        if (valid) {
          if (this.loading) return
          this.loading = true
          this.apiSubmit()
        }
      })
    },
    onCancel() {
      this.$emit('close')
    }
  }
}
</script>
