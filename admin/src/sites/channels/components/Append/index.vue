<template>
  <el-form ref="appendForm" :model="appendForm" :rules="rules" label-width="80px">
    <el-form-item label="父栏目">
      <el-input v-model="parent.channelName" :disabled="true" />
    </el-form-item>
    <el-form-item label="栏目索引">
      <el-switch v-model="appendForm.isIndexName" active-text="将栏目名称作为栏目索引" />
    </el-form-item>
    <el-form-item label="栏目" prop="channels">
      <el-input v-model="appendForm.channels" type="textarea" rows="5" :autosize="{ minRows: 5}" placeholder="栏目之间用换行分割，下级栏目在栏目前添加“-”字符，索引可以放到括号中" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" :loading="loading" @click="onSubmit">立即创建</el-button>
      <el-button @click="onCancel">取消</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import { insertChannel } from '@/api/channels'

export default {
  props: {
    parent: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      appendForm: {
        isIndexName: false,
        channels: ''
      },
      rules: {
        channels: [
          { required: true }
        ]
      }
    }
  },
  methods: {
    async apiSubmit() {
      const channelName = this.appendForm.channels
      const indexName = this.appendForm.isIndexName ? channelName : ''
      const array = []
      var channel = await insertChannel(this.$route.params.siteId, {
        parentId: this.parent.id,
        channelName,
        indexName
      })
      array.push(channel)
      this.$emit('submit', array)
    },

    onSubmit() {
      this.$refs['appendForm'].validate((valid) => {
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
