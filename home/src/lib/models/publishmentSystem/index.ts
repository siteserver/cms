export class PublishmentSystem {
  publishmentSystemId: number
  publishmentSystemName: string
  publishmentSystemType: string
  auxiliaryTableForContent: string
  auxiliaryTableForGoods: string
  auxiliaryTableForBrand: string
  auxiliaryTableForGovPublic: string
  auxiliaryTableForGovInteract: string
  auxiliaryTableForVote: string
  auxiliaryTableForJob: string
  isCheckContentUseLevel: boolean
  checkContentLevel: number
  publishmentSystemDir: string
  publishmentSystemUrl: string
  isHeadquarters: boolean
  parentPublishmentSystemId: number
  taxis: number

  constructor() {}
}
