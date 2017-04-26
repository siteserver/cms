export class User {
  id: number
  userName: string
  groupId: number
  createDate: Date
  lastResetPasswordDate: Date
  lastActivityDate: Date
  isChecked: boolean
  isLockedOut: boolean
  loginCount: number
  writingCount: number
  displayName: string
  email: string
  mobile: string
  avatarUrl: string
  organization: string
  department: string
  position: string
  gender: string
  birthday: string
  education: string
  graduation: string
  address: string
  weiXin: string
  qq: string
  weiBo: string
  interests: string
  signature: string
  isAnonymous: boolean
  additional: {
    lastWritingPublishmentSystemId: number
    lastWritingNodeId: number
  }
}
