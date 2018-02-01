/* 
  	* Enums.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
    * this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
    * notice, this list of conditions and the following disclaimer in the
    * documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright owner nor the names of its
    * contributors may be used to endorse or promote products derived from
    * this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
    LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
    INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    POSSIBILITY OF SUCH DAMAGE.
*/
using System;

namespace Atom.Core
{
	#region Language

	/// <summary>
	/// The allowed languages (locales) in the Atom feed.
	/// Visit <a href="http://www.ietf.org/rfc/rfc3066.txt">http://www.ietf.org/rfc/rfc3066.txt</a>,
	/// <a href="http://www.unicode.org/unicode/onlinedat/languages.html">http://www.unicode.org/unicode/onlinedat/languages.html</a> or
	/// <a href="http://www.loc.gov/standards/iso639-2/">http://www.loc.gov/standards/iso639-2/</a> for more infos on languages codes and
	/// <a href="http://www.iso.ch/iso/en/prods-services/iso3166ma/02iso-3166-code-lists/list-en1.html">http://www.iso.ch/iso/en/prods-services/iso3166ma/02iso-3166-code-lists/list-en1.html</a> for countries codes.
	/// <seealso cref="Atom.Core.AtomFeed"/>
	/// </summary>
	public enum Language
	{
		/// <summary>
		/// Arabic (Saudi Arabia)
		/// </summary>
		ar_SA,
		/// <summary>
		/// Arabic (Iraq)
		/// </summary>
		ar_IQ,
		/// <summary>
		/// Arabic (Egypt)
		/// </summary>
		ar_EG,
		/// <summary>
		/// Arabic (Lybian Arab Jamahiriya)
		/// </summary>
		ar_LY,
		/// <summary>
		/// Arabic (Algeria)
		/// </summary>
		ar_DZ,
		/// <summary>
		/// Arabic (Morocco)
		/// </summary>
		ar_MA,
		/// <summary>
		/// Arabic (Tunisia)
		/// </summary>
		ar_TN,
		/// <summary>
		/// Arabic (Oman)
		/// </summary>
		ar_OM,
		/// <summary>
		/// Arabic (Yemen)
		/// </summary>
		ar_YE,
		/// <summary>
		/// Arabic (Syrian Arab Republic)
		/// </summary>
		ar_SY,
		/// <summary>
		/// Arabic (Jordan)
		/// </summary>
		ar_JO,
		/// <summary>
		/// Arabic (Lebanon)
		/// </summary>
		ar_LB,
		/// <summary>
		/// Arabic (Kuwait)
		/// </summary>
		ar_KW,
		/// <summary>
		/// Arabic (United Arab Emirates)
		/// </summary>
		ar_AE,
		/// <summary>
		/// Arabic (Bahrain)
		/// </summary>
		ar_BH,
		/// <summary>
		/// Arabic (Qatar)
		/// </summary>
		ar_QA,
		/// <summary>
		/// Bulgarian (Bulgaria)
		/// </summary>
		bg_BG,
		/// <summary>
		/// Catalan (Spain)
		/// </summary>
		ca_ES,
		/// <summary>
		/// Chinese (Taiwan, province of China)
		/// </summary>
		zh_TW,
		/// <summary>
		/// Chinese (China)
		/// </summary>
		zh_CN,
		/// <summary>
		/// Chinese (Hong Kong)
		/// </summary>
		zh_HK,
		/// <summary>
		/// Chinese (Singapore)
		/// </summary>
		zh_SG,
		/// <summary>
		/// Chinese (Macao)
		/// </summary>
		zh_MO,
		/// <summary>
		/// Czech (Czech Republic)
		/// </summary>
		cs_CZ,
		/// <summary>
		/// Danish (Denmark)
		/// </summary>
		da_DK,
		/// <summary>
		/// German (Germany)
		/// </summary>
		de_DE,
		/// <summary>
		/// German (Switzerland)
		/// </summary>
		de_CH,
		/// <summary>
		/// German (Austria)
		/// </summary>
		de_AT,
		/// <summary>
		/// German (Luxembourg)
		/// </summary>
		de_LU,
		/// <summary>
		/// German (Liechtenstein)
		/// </summary>
		de_LI,
		/// <summary>
		/// Greek (Greece)
		/// </summary>
		el_GR,
		/// <summary>
		/// English (United States)
		/// </summary>
		en_US,
		/// <summary>
		/// English (United Kingdom)
		/// </summary>
		en_GB,
		/// <summary>
		/// English (Australia)
		/// </summary>
		en_AU,
		/// <summary>
		/// English (Canada)
		/// </summary>
		en_CA,
		/// <summary>
		/// English (New Zealand)
		/// </summary>
		en_NZ,
		/// <summary>
		/// English (Ireland)
		/// </summary>
		en_IE,
		/// <summary>
		/// English (South Africa)
		/// </summary>
		en_ZA,
		/// <summary>
		/// English (Jamaica)
		/// </summary>
		en_JM,
		/// <summary>
		/// English (Unknown nation)
		/// </summary>
		en_CB,
		/// <summary>
		/// English (Belize)
		/// </summary>
		en_BZ,
		/// <summary>
		/// English (Trinidad and Tobago)
		/// </summary>
		en_TT,
		/// <summary>
		/// English (Zimbabwe)
		/// </summary>
		en_ZW,
		/// <summary>
		/// English (Philippines)
		/// </summary>
		en_PH,
		/// <summary>
		/// Spanish (Mexico)
		/// </summary>
		es_MX,
		/// <summary>
		/// Spanish (Spain)
		/// </summary>
		es_ES,
		/// <summary>
		/// Spanish (Guatemala)
		/// </summary>
		es_GT,
		/// <summary>
		/// Spanish (Costa Rica)
		/// </summary>
		es_CR,
		/// <summary>
		/// Spanish (Panama)
		/// </summary>
		es_PA,
		/// <summary>
		/// Spanish (Dominican Republic)
		/// </summary>
		es_DO,
		/// <summary>
		/// Spanish (Venezuela)
		/// </summary>
		es_VE,
		/// <summary>
		/// Spanish (Colombia)
		/// </summary>
		es_CO,
		/// <summary>
		/// Spanish (Peru)
		/// </summary>
		es_PE,
		/// <summary>
		/// Spanish (Argentina)
		/// </summary>
		es_AR,
		/// <summary>
		/// Spanish (Ecuador)
		/// </summary>
		es_EC,
		/// <summary>
		/// Spanish (Chile)
		/// </summary>
		es_CL,
		/// <summary>
		/// Spanish (Urugay)
		/// </summary>
		es_UY,
		/// <summary>
		/// Spanish (Paraguay)
		/// </summary>
		es_PY,
		/// <summary>
		/// Spanish (Bolivia)
		/// </summary>
		es_BO,
		/// <summary>
		/// Spanish (El Salvador)
		/// </summary>
		es_SV,
		/// <summary>
		/// Spanish (Honduras)
		/// </summary>
		es_HN,
		/// <summary>
		/// Spanish (Nicaragua)
		/// </summary>
		es_NI,
		/// <summary>
		/// Spanish (Puerto Rico)
		/// </summary>
		es_PR,
		/// <summary>
		/// Finnish (Finland)
		/// </summary>
		fi_FI,
		/// <summary>
		/// French (France)
		/// </summary>
		fr_FR,
		/// <summary>
		/// French (Belgium)
		/// </summary>
		fr_BE,
		/// <summary>
		/// French (Canada)
		/// </summary>
		fr_CA,
		/// <summary>
		/// French (Switzerland)
		/// </summary>
		fr_CH,
		/// <summary>
		/// French (Luxembourg)
		/// </summary>
		fr_LU,
		/// <summary>
		/// French (Monaco)
		/// </summary>
		fr_MC,
		/// <summary>
		/// Hebrew (Israel)
		/// </summary>
		he_IL,
		/// <summary>
		/// Hungarian (Hungary)
		/// </summary>
		hu_HU,
		/// <summary>
		/// Icelandic (Iceland)
		/// </summary>
		is_IS,
		/// <summary>
		/// Italian (Italy)
		/// </summary>
		it_IT,
		/// <summary>
		/// Italian (Switzerland)
		/// </summary>
		it_CH,
		/// <summary>
		/// Japanese (Japan)
		/// </summary>
		ja_JP,
		/// <summary>
		/// Korean (Korea, Republic Of)
		/// </summary>
		ko_KR,
		/// <summary>
		/// Dutch (Netherlands)
		/// </summary>
		nl_NL,
		/// <summary>
		/// Dutch (Belgium)
		/// </summary>
		nl_BE,
		/// <summary>
		/// Norwegian Bokmål (Norway)
		/// </summary>
		nb_NO,
		/// <summary>
		/// Norwegian Nynorsk (Norway)
		/// </summary>
		nn_NO,
		/// <summary>
		/// Polish (Poland)
		/// </summary>
		pl_PL,
		/// <summary>
		/// Portuguese (Brazil)
		/// </summary>
		pt_BR,
		/// <summary>
		/// Portuguese (Portugal)
		/// </summary>
		pt_PT,
		/// <summary>
		/// Romanian (Romania)
		/// </summary>
		ro_RO,
		/// <summary>
		/// Russian (Russian Federation)
		/// </summary>
		ru_RU,
		/// <summary>
		/// Croatian (Croatia)
		/// </summary>
		hr_HR,
		/// <summary>
		/// Serbian Latin
		/// </summary>
		sr_SP_Latn,
		/// <summary>
		/// Serbian Cyrillic
		/// </summary>
		sr_SP_Cyrl,
		/// <summary>
		/// Slovak (Slovakia)
		/// </summary>
		sk_SK,
		/// <summary>
		/// Albanian (Albania)
		/// </summary>
		sq_AL,
		/// <summary>
		/// Swedish (Sweden)
		/// </summary>
		sv_SE,
		/// <summary>
		/// Swedish (Finland)
		/// </summary>
		sv_FI,
		/// <summary>
		/// Thai (Thailand)
		/// </summary>
		th_TH,
		/// <summary>
		/// Turkish (Turkey)
		/// </summary>
		tr_TR,
		/// <summary>
		/// Urdu (Pakistan)
		/// </summary>
		ur_PK,
		/// <summary>
		/// Indonesian (Indonesia)
		/// </summary>
		id_ID,
		/// <summary>
		/// Ukrainian (Ukraine)
		/// </summary>
		uk_UA,
		/// <summary>
		/// Byelorussian (Belarus)
		/// </summary>
		be_BY,
		/// <summary>
		/// Slovenian (Slovenia)
		/// </summary>
		sl_SI,
		/// <summary>
		/// Estonian (Estonia)
		/// </summary>
		et_EE,
		/// <summary>
		/// Latvian (Latvia)
		/// </summary>
		lv_LV,
		/// <summary>
		/// Lithuanian (Lithuania)
		/// </summary>
		lt_LT,
		/// <summary>
		/// Farsi (Iran, Islamic Republic Of)
		/// </summary>
		fa_IR,
		/// <summary>
		/// Vietnamese (Viet nam)
		/// </summary>
		vi_VN,
		/// <summary>
		/// Armenian (Armenia)
		/// </summary>
		hy_AM,
		/// <summary>
		/// Azerbaijani Latin (Azerbaijan)
		/// </summary>
		az_AZ_Latn,
		/// <summary>
		/// Azerbaijani Cyrillic (Azerbaijan)
		/// </summary>
		az_AZ_Cyrl,
		/// <summary>
		/// Basque (Spain)
		/// </summary>
		eu_ES,
		/// <summary>
		/// Macedonian (Macedonia, The Former Yugoslav Republic Of)
		/// </summary>
		mk_MK,
		/// <summary>
		/// Afrikaans (South Africa)
		/// </summary>
		af_ZA,
		/// <summary>
		/// Georgian (Georgia)
		/// </summary>
		ka_GE,
		/// <summary>
		/// Faeroese (Faroe Islands)
		/// </summary>
		fo_FO,
		/// <summary>
		/// Hindi (India)
		/// </summary>
		hi_IN,
		/// <summary>
		/// Malay (Malaysia)
		/// </summary>
		ms_MY,
		/// <summary>
		/// Malay (Brunei Darussalam)
		/// </summary>
		ms_BN,
		/// <summary>
		/// Kazakh (Kazakhstan)
		/// </summary>
		kk_KZ,
		/// <summary>
		/// Kirghiz (Kazakhstan)
		/// </summary>
		ky_KZ,
		/// <summary>
		/// Swahili (Kenya)
		/// </summary>
		sw_KE,
		/// <summary>
		/// Uzbek Latin (Uzbekistan)
		/// </summary>
		uz_UZ_Latn,
		/// <summary>
		/// Uzbek Cyrillic (Uzbekistan)
		/// </summary>
		uz_UZ_Cyrl,
		/// <summary>
		/// Tatar (Russian Federation)
		/// </summary>
		tt_RU,
		/// <summary>
		/// Punjabi (India)
		/// </summary>
		pa_IN,
		/// <summary>
		/// Gujarati (India)
		/// </summary>
		gu_IN,
		/// <summary>
		/// Tamil (Unknown nation)
		/// </summary>
		ta,
		/// <summary>
		/// Unknown language
		/// </summary>
		UnknownLanguage,
	}

	#endregion Language

	#region MediaType

	/// <summary>
	/// The media types used in Atom.
	/// Visit <a href="http://www.ietf.org/rfc/rfc2045.txt">http://www.ietf.org/rfc/rfc2045.txt</a>
	/// and <a href="http://www.iana.org/assignments/media-types/">http://www.iana.org/assignments/media-types/</a> for more infos on MIME types.
	/// <seealso cref="Atom.Core.AtomContentConstruct"/>
	/// </summary>
	public enum MediaType
	{
		ApplicationActiveMessage = 1,
		ApplicationAndrewInset,
		ApplicationAppleFile,
		ApplicationAtomicMail,
		ApplicationAtomXml,
		ApplicationBatchSMTP,
		ApplicationBeepXml,
		ApplicationCals1840,
		ApplicationCommonGround,
		ApplicationCyberCash,
		ApplicationDcaRft,
		ApplicationDecDx,
		ApplicationDicom,
		ApplicationDvcs,
		ApplicationEDIConsent,
		ApplicationEDIFACT,
		ApplicationEDIX12,
		ApplicationEshop,
		ApplicationFontTdprf,
		ApplicationHttp,
		ApplicationHyperStudio,
		ApplicationIges,
		ApplicationIndex,
		ApplicationIndexCmd,
		ApplicationIndexObj,
		ApplicationIndexResponse,
		ApplicationIndexVnd,
		ApplicationIotp,
		ApplicationIpp,
		ApplicationIsUp,
		ApplicationMacBinHex40,
		ApplicationMacWriteII,
		ApplicationMarc,
		ApplicationMathematica,
		ApplicationMpeg4Generic,
		ApplicationMsWord,
		ApplicationNewsMessageId,
		ApplicationNewsMessageTransmission,
		ApplicationOcspRequest,
		ApplicationOcspResponse,
		ApplicationOctetStream,
		ApplicationOda,
		ApplicationOgg,
		ApplicationParityFec,
		ApplicationPdf,
		ApplicationPgpEncrypted,
		ApplicationPgpKeys,
		ApplicationPgpSignature,
		ApplicationPkcs10,
		ApplicationPkcs7Mime,
		ApplicationPkcs7Signature,
		ApplicationPkixCert,
		ApplicationPkixCmp,
		ApplicationPkixCrl,
		ApplicationPkixPkipath,
		ApplicationPostscript,
		ApplicationPrsAlvestrandTitraxSheet,
		ApplicationPrsCww,
		ApplicationPrsNprend,
		ApplicationPrsPlucker,
		ApplicationQsig,
		ApplicationRemotePrinting,
		ApplicationRiscOS,
		ApplicationRtf,
		ApplicationSdp,
		ApplicationSetPayment,
		ApplicationSetPaymentInitiation,
		ApplicationSetRegistration,
		ApplicationSetRegistrationInitiation,
		ApplicationSgml,
		ApplicationSgmlOpenCatalog,
		ApplicationSieve,
		ApplicationSlate,
		ApplicationTimestampQuery,
		ApplicationTimestampReply,
		ApplicationTveTrigger,
		ApplicationVemmi,
		ApplicationVnd3GppPicBwLarge,
		ApplicationVnd3GppPicBwSmall,
		ApplicationVnd3GppPicBwVar,
		ApplicationVnd3GppSms,
        ApplicationVnd3MPostItNotes,
		ApplicationVndAccPacSimplyAso,
		ApplicationVndAccPacSimplyImp,
		ApplicationVndAcuCobol,
		ApplicationVndAcuCorp,
		ApplicationVndAdobeXfdf,
		ApplicationVndAetherImp,
		ApplicationVndAmigaAmi,
		ApplicationVndAnserWebCertificateIssueInitiation,
		ApplicationVndAnserWebFundsTransferInitiation,
		ApplicationVndAudiograph,
		ApplicationVndBlueIceMultipass,
		ApplicationVndBmi,
		ApplicationVndBusinessObjects,
		ApplicationVndCanonCpdl,
		ApplicationVndCanonLips,
		ApplicationVndCinderella,
		ApplicationVndClaymore,
		ApplicationVndCommerceBattelle,
		ApplicationVndCommonSpace,
		ApplicationVndCosmoCaller,
		ApplicationVndContactCmsg,
		ApplicationVndCriticalToolsWbsXml,
		ApplicationVndCtcPosml,
		ApplicationVndCupsPostscript,
		ApplicationVndCupsRaster,
		ApplicationVndCupsRaw,
		ApplicationVndCurl,
		ApplicationVndCybank,
		ApplicationVndDataVisionRdz,
		ApplicationVndDna,
		ApplicationVndDpGraph,
		ApplicationVndDreamFactory,
		ApplicationVndDxr,
		ApplicationVndEcdisUpdate,
		ApplicationVndEcowinChart,
		ApplicationVndEcowinFileRequest,
		ApplicationVndEcowinFileUpdate,
		ApplicationVndEcowinSeries,
		ApplicationVndEcowinSeriesRequest,
		ApplicationVndEcowinSeriesUpdate,
		ApplicationVndEnliven,
		ApplicationVndEpsonEsf,
		ApplicationVndEpsonMsf,
		ApplicationVndEpsonQuickAnime,
		ApplicationVndEpsonSalt,
		ApplicationVndEpsonSsf,
		ApplicationVndEriccsonQuickCall,
		ApplicationVndEudoraData,
		ApplicationVndFdf,
		ApplicationVndFfsns,
		ApplicationVndFints,
		ApplicationVndFloGraphIt,
		ApplicationVndFrameMaker,
		ApplicationVndFscWebLaunch,
		ApplicationVndFujitsuOasys,
		ApplicationVndFujitsuOasys2,
		ApplicationVndFujitsuOasys3,
		ApplicationVndFujitsuOasysGp,
		ApplicationVndFujitsuOasysPrs,
		ApplicationVndFujiXeroxDdd,
		ApplicationVndFujiXeroxDocuworks,
		ApplicationVndFujiXeroxDocuworksBinder,
		ApplicationVndFutMisnet,
		ApplicationVndGenomatixTuxedo,
		ApplicationVndGrafeq,
		ApplicationVndGrooveAccount,
		ApplicationVndGrooveHelp,
		ApplicationVndGrooveIdentityMessage,
		ApplicationVndGrooveInjector,
		ApplicationVndGrooveToolMessage,
		ApplicationVndGrooveToolTemplate,
		ApplicationVndGrooveVcard,
		ApplicationVndHbci,
		ApplicationVndHheLessonPlayer,
		ApplicationVndHPHpGpl,
		ApplicationVndHPGpId,
		ApplicationVndHPHps,
		ApplicationVndHPPcl,
		ApplicationVndHPPclXl,
		ApplicationVndHttphone,
		ApplicationVndHzn3dCrossWord,
		ApplicationVndIbmAfpLineData,
		ApplicationVndIbmElectronicMedia,
		ApplicationVndIbmMiniPlay,
		ApplicationVndIbmModCap,
		ApplicationVndIbmRightsManagement,
		ApplicationVndIbmSecureContainer,
		ApplicationVndInformixVisionary,
		ApplicationVndInterconFormnet,
		ApplicationVndIntertrustDigibox,
		ApplicationVndIntertrustNncp,
		ApplicationVndIntuQbo,
		ApplicationVndIntuQfx,
		ApplicationVndIpUnpluggedRcProfile,
		ApplicationVndIRepositoryPackageXml,
		ApplicationVndIsXpr,
		ApplicationVndJapanNetDirectoryService,
		ApplicationVndJapanNetJpnStoreWakeUp,
		ApplicationVndJapanNetPaymentWakeUp,
		ApplicationVndJapanNetRegistration,
		ApplicationVndJapanNetRegistrationWakeUp,
		ApplicationVndJapanNetSetStoreWakeUp,
		ApplicationVndJapanNetVerification,
		ApplicationVndJapanNetVerificationWakeUp,
		ApplicationVndJisp,
		ApplicationVndKdeKarbon,
		ApplicationVndKdeKChart,
		ApplicationVndKdeKFormula,
		ApplicationVndKdeKivio,
		ApplicationVndKdeKontour,
		ApplicationVndKdeKPresenter,
		ApplicationVndKdeKSpread,
		ApplicationVndKdeKWord,
		ApplicationVndKenameApp,
		ApplicationVndKidSpiration,
		ApplicationVndKoan,
		ApplicationVndLibertyRequestXml,
		ApplicationVndLLamaGraphicsLifeBalanceDesktop,
		ApplicationVndLLamaGraphicsLifeBalanceExchangeXml,
		ApplicationVndLotus123,
		ApplicationVndLotusApproach,
		ApplicationVndLotusFreelance,
		ApplicationVndLotusNotes,
		ApplicationVndLotusOrganizer,
		ApplicationVndLotusScreenCam,
		ApplicationVndLotusWordPro,
		ApplicationVndMcd,
		ApplicationVndMediaStationCdKey,
		ApplicationVndMeridianSlingshot,
		ApplicationVndMicroGrafxFlo,
		ApplicationVndMicroGrafxIgx,
		ApplicationVndMif,
		ApplicationVndMinisoftHP3000Save,
		ApplicationVndMitsubishiMistyGuardTrustWeb,
		ApplicationVndMobiusDAF,
		ApplicationVndMobiusDIS,
		ApplicationVndMobiusMBK,
		ApplicationVndMobiusMQY,
		ApplicationVndMobiusMSL,
		ApplicationVndMobiusPLC,
		ApplicationVndMobiusTXF,
		ApplicationVndMophunApplication,
		ApplicationVndMophunCertificate,
		ApplicationVndMotorolaFlexSuite,
		ApplicationVndMotorolaFlexSuiteAdsi,
		ApplicationVndMotorolaFlexSuiteFis,
		ApplicationVndMotorolaFlexSuiteGoTap,
		ApplicationVndMotorolaFlexSuiteKmr,
		ApplicationVndMotorolaFlexSuiteTtc,
		ApplicationVndMotorolaFlexSuiteWem,
		ApplicationVndMozillaXulXml,
		ApplicationVndMsArtGarly,
		ApplicationVndMsAsf,
		ApplicationVndMSeq,
		ApplicationVndMsExcel,
		ApplicationVndMSign,
		ApplicationVndMsLrm,
		ApplicationVndMsPowerpoint,
		ApplicationVndMsProject,
		ApplicationVndMsTnef,
		ApplicationVndMsWorks,
		ApplicationVndMsWpl,
		ApplicationVndMusician,
		ApplicationVndMusicNiff,
		ApplicationVndNervana,
		ApplicationVndNetFpx,
		ApplicationVndNobleNetSealer,
		ApplicationVndNobleNetWeb,
		ApplicationVndNovadigmEDM,
		ApplicationVndNovadigmEDX,
		ApplicationVndNovadigmEXT,
		ApplicationVndObn,
		ApplicationVndOsaNetDeploy,
		ApplicationVndPalm,
		ApplicationVndPaOsXml,
		ApplicationVndPgFormat,
		ApplicationVndPicSel,
		ApplicationVndPgOsasli,
		ApplicationVndPowerBuilder6,
		ApplicationVndPowerBuilder6s,
		ApplicationVndPowerBuilder7,
		ApplicationVndPowerBuilder75,
		ApplicationVndPowerBuilder75s,
		ApplicationVndPowerBuilder7s,
		ApplicationVndPreviewSystemsBox,
		ApplicationVndPublishareDeltaTree,
		ApplicationVndPviPtid1,
		ApplicationVndPwgMultiplexed,
		ApplicationVndPwgXhtmlPrintXml,
		ApplicationVndQuarkQuarkXPress,
		ApplicationVndRapid,
		ApplicationVndS3Sms,
		ApplicationVndSealedDoc,
		ApplicationVndSealedEml,
		ApplicationVndSealedMht,
		ApplicationVndSealedNet,
		ApplicationVndSealedPpt,
		ApplicationVndSealedXls,
		ApplicationVndSealedMediaSoftsealHtml,
		ApplicationVndSealedMediaSoftsealPdf,
		ApplicationVndSeemail,
		ApplicationVndShanaInformedFormData,
		ApplicationVndShanaInformedFormTemplate,
		ApplicationVndShanaInformedInterchange,
		ApplicationVndShanaInformedPackage,
		ApplicationVndSmaf,
		ApplicationVndSssCod,
		ApplicationVndSssDtf,
		ApplicationVndSssNtf,
		ApplicationVndStreetStream,
		ApplicationVndSvd,
		ApplicationVndSwiftViewIcs,
		ApplicationVndTriscapeMxs,
		ApplicationVndTrueApp,
		ApplicationVndTrueDoc,
		ApplicationVndUfdl,
		ApplicationVndUiqTheme,
		ApplicationVndUlplanetAlert,
		ApplicationVndUlplanetAlertWbxml,
		ApplicationVndUlplanetBearerChoice,
		ApplicationVndUlplanetBearerChoiceWbxml,
		ApplicationVndUlplanetCacheOp,
		ApplicationVndUlplanetCacheOpWbxml,
		ApplicationVndUlplanetChannel,
		ApplicationVndUlplanetChannelWbxml,
		ApplicationVndUlplanetList,
		ApplicationVndUlplanetListCmd,
		ApplicationVndUlplanetListCmdWbxml,
		ApplicationVndUlplanetListWbxml,
		ApplicationVndUlplanetSignal,
		ApplicationVndVcx,
		ApplicationVndVectorWorks,
		ApplicationVndVidsoftVidConference,
		ApplicationVndVisio,
		ApplicationVndVisionary,
		ApplicationVndVividenceScriptFile,
		ApplicationVndVsf,
		ApplicationVndWapSic,
		ApplicationVndWapSlc,
		ApplicationVndWapWbxml,
		ApplicationVndWapWmlc,
		ApplicationVndWapWmlScriptC,
		ApplicationVndWebTurbo,
		ApplicationVndWqd,
		ApplicationVndWrqHP3000Labelled,
		ApplicationVndWtStf,
		ApplicationVndWvCspXml,
		ApplicationVndWvCspWbxml,
		ApplicationVndWvSspXml,
		ApplicationVndXara,
		ApplicationVndXfdl,
		ApplicationVndYamahaHvDic,
		ApplicationVndYamahaHvScript,
		ApplicationVndYamahaHvVoice,
		ApplicationVndYamahaSmafAudio,
		ApplicationVndYamahaSmafPhrase,
		ApplicationVndYellowRiverCustomMenu,
		ApplicationWhoisPpQuery,
		ApplicationWhoisPpResponse,
		ApplicationWita,
		ApplicationWordPerfect51,
		ApplicationX400Bp,
		ApplicationXAtomXml,
		ApplicationXhtmlXml,
		ApplicationXml,
		ApplicationXmlDtd,
		ApplicationXmlExternalParsedEntity,
		ApplicationZip,
		Audio32KAdPcm,
		AudioAMR,
		AudioAMRWB,
		AudioBasic,
		AudioCN,
		AudioDAT12,
		AudioDsrEs201108,
		AudioDVI4,
		AudioEVRC,
		AudioEVRC0,
		AudioEVRCQCP,
		AudioG722,
		AudioG7221,
		AudioG723,
		AudioG72616,
		AudioG72624,
		AudioG72632,
		AudioG72640,
		AudioG728,
		AudioG729,
		AudioG729D,
		AudioG729E,
		AudioGSM,
		AudioGSMEFR,
		AudioL8,
		AudioL16,
		AudioL20,
		AudioL24,
		AudioLPC,
		AudioLMPA,
		AudioLMP4ALATM,
		AudioMpaRobust,
		AudioMpeg,
		AudioMpeg4Generic,
		AudioParityFec,
		AudioPCMA,
		AudioPCMU,
		AudioPrsSid,
		AudioQCELP,
		AudioRED,
		AudioSMV,
		AudioSMV0,
		AudioSMVQCP,
		AudioTelephoneEvent,
		AudioTone,
		AudioVDVI,
		AudioVnd3GppIufp,
		AudioVndCiscoNse,
		AudioVndCnsAnp1,
		AudioVndCnsInf1,
		AudioVndDigitalWinds,
		AudioVndEveradPlj,
		AudioVndLucentVoice,
		AudioVndNokiaMobileXmf,
		AudioVndNortelVbk,
		AudioVndNueraEcelp4800,
		AudioVndNueraEcelp7470,
		AudioVndNueraEcelp9600,
		AudioVndOctelSbc,
		AudioVndRhetorex32KAdPcm,
		AudioVndSealedMediaSoftSealMpeg,
		AudioVndVmxCvsd,
		ImageCgm,
		ImageG3Fax,
		ImageGif,
		ImageIef,
		ImageJp2,
		ImageJpeg,
		ImageJpm,
		ImageJpx,
		ImageNaplps,
		ImagePng,
		ImagePrsBTif,
		ImagePrsPti,
		ImageT38,
		ImageTiff,
		ImageTiffFx,
		ImageVndCnsInf2,
		ImageVndDjvu,
		ImageVndDwg,
		ImageVndDxf,
		ImageVndFastBidSheet,
		ImageVndFpx,
		ImageVndFst,
		ImageVndFujiXeroxEdmicsMmr,
		ImageVndFujiXeroxEdmicsRlc,
		ImageVndGlobalGraphicsPgb,
		ImageVndMicrosoftIcon,
		ImageVndMix,
		ImageVndMsModi,
		ImageVndNetFpx,
		ImageVndSealedPng,
		ImageVndSealedMediaSoftSealGif,
		ImageVndSealedMediaSoftSealJpeg,
		ImageVndSvf,
		ImageVndWapWbmp,
		ImageVndXiff,
		MessageDeliveryStatus,
		MessageDispositionNotification,
		MessageExternalBody,
		MessageHttp,
		MessageNews,
		MessagePartial,
		MessageRfc822,
		MessageSHttp,
		MessageSip,
		MessageSipFrag,
		ModelIges,
		ModelMesh,
		ModelVndDwf,
		ModelVndFlatLand3dml,
		ModelVndGdl,
		ModelVndGsGdl,
		ModelVndGtw,
		ModelVndMts,
		ModelVndParasolidTransmitBinary,
		ModelVndParasolidTransmitText,
		ModelVndVtu,
		ModelVrml,
        MultipartAlternative,
		MultipartAppleDouble,
		MultipartByteRanges,
		MultipartDigest,
		MultipartEncrypted,
		MultipartFormData,
		MultipartHeaderSet,
		MultipartMixed,
		MultipartParallel,
		MultipartRelated,
		MultipartReport,
		MultipartSigned,
		MultipartVoiceMessage,
		TextCalendar,
		TextCss,
		TextDirectory,
		TextEnriched,
		TextHtml,
		TextParityFec,
		TextPlain,
		TextPrsFallensteinRst,
		TextPrsLinesTag,
		TextRfc822Headers,
		TextRichText,
		TextRtf,
		TextSgml,
		TextT140,
		TextTabSeparatedValues,
		TextUriList,
		TextVndAbc,
		TextVndCurl,
		TextVndDMClientScript,
		TextVndFly,
		TextVndFmiFlexstor,
		TextVndIn3d3dml,
		TextVndIn3dSopt,
		TextVndLatexZ,
		TextVndMotorolaReflex,
		TextVndMsMediaPackage,
		TextVndNet2PhoneCommCenterCommand,
		TextVndSunJ2meAppDescriptor,
		TextVndWapSi,
		TextVndWapSl,
		TextVndWapWml,
		TextVndWapWmlScript,
		TextXml,
		TextXmlExternalParsedEntity,
		VideoBMPEG,
		VideoBT656,
		VideoCelB,
		VideoDV,
		VideoH261,
		VideoH263,
		VideoH2631998,
		VideoH2632000,
		VideoJPEG,
		VideoMJ2,
		VideoMP1S,
		VideoMP2P,
		VideoMP2T,
		VideoMP4VES,
		VideoMPV,
		VideoMpeg,
		VideoMpeg4Generic,
		VideoNv,
		VideoParityFec,
		VideoPointer,
		VideoQuicktime,
		VideoSMPTE292M,
		VideoVndFvt,
		VideoVndMotorolaVideo,
		VideoVndMotorolaVideoP,
		VideoVndMpegUrl,
		VideoVndNokiaInterleavedMultimedia,
		VideoVndObjectVideo,
		VideoVndSealedMpeg1,
		VideoVndSealedMpeg4,
		VideoVndSealedSwf,
		VideoVndSealedMediaSoftSealMov,
		VideoVndVivo,
		UnknownType,
	}

	#endregion MediaType

	#region Relationship

	/// <summary>
	/// The allowed relationships in link elements.
	/// <seealso cref="AtomLink"/>
	/// </summary>
	public enum Relationship
	{
		/// <summary>
		/// The URI in the href attribute points to an alternate representation of the containing resource.
		/// </summary>
		Alternate,
		/// <summary>
		/// The Atom feed at the URI supplied in the href attribute contains the first feed in a linear sequence of entries.
		/// </summary>
		Start,
		/// <summary>
		/// The Atom feed at the URI supplied in the href attribute contains the next N entries in a linear sequence of entries.
		/// </summary>
		Next,
		/// <summary>
		///  The Atom feed at the URI supplied in the href attribute contains the previous N entries in a linear sequence of entries.
		/// </summary>
		Prev,
		/// <summary>
		/// The URI given in the href attribute is used to edit a representation of the referred resource.
		/// </summary>
		ServiceEdit,
		/// <summary>
		/// The URI in the href attribute is used to create new resources.
		/// </summary>
		ServicePost,
		/// <summary>
		/// The URI given in the href attribute is a starting point for navigating content and services.
		/// </summary>
		ServiceFeed,
	}

	#endregion Relationship

	#region Mode
	
	/// <summary>
	/// The allowed encoding modes.
	/// <seealso cref="Atom.Core.AtomContentConstruct"/>
	/// </summary>
	public enum Mode
	{
		/// <summary>
		/// Indicates that the content is inline xml.
		/// </summary>
		Xml,
		/// <summary>
		/// Indicates that the content is an escaped string.
		/// </summary>
		Escaped,
		/// <summary>
		/// Indicates that the content is base64 encoded.
		/// </summary>
		Base64,
	}

	#endregion Mode
}
