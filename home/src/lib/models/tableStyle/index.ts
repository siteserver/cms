export class TableStyle {
  tableStyleId: string
  relatedIdentity: string
  tableName: string
  attributeName: string
  taxis: string
  displayName: string
  helpText: string
  isVisible: boolean
  isVisibleInList: boolean
  isSingleLine: boolean
  inputType: string
  defaultValue: string
  isHorizontal: boolean

  additional: {
    height: number
    width: string
    columns: number
    isFormatString: boolean
    editorTypeString: string
    relatedFieldId: number
    relatedFieldStyle: string
    isValidate: boolean
    isRequired: boolean
    minNum: number
    maxNum: number
    validateType: string
    regExp: string
    errorMessage: string
    isUseStatistics: boolean
  }

  constructor() {}
}
