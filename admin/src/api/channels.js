import request from '@/utils/request'

export function getChannels(siteId) {
  return request({
    url: `/sites/${siteId}/channels`,
    method: 'get'
  })
}

export function getChannel(siteId, channelId) {
  return request({
    url: `/sites/${siteId}/channels/${channelId}`,
    method: 'get'
  })
}

export function insertChannel(siteId, data) {
  return request({
    url: `/sites/${siteId}/channels`,
    method: 'post',
    data
  })
}

export function deleteChannel(siteId, channelId) {
  return request({
    url: `/sites/${siteId}/channels/${channelId}`,
    method: 'delete'
  })
}
