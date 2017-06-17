CREATE TABLE wx_Account(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    Token                  VARCHAR(200),
    IsBinding              VARCHAR(18),
    AccountType            VARCHAR(50),
    WeChatId               NATIONAL VARCHAR(255),
    SourceId               VARCHAR(200),
    ThumbUrl               VARCHAR(200),
    AppId                  VARCHAR(200),
    AppSecret              VARCHAR(200),
    IsWelcome              VARCHAR(18),
    WelcomeKeyword         NATIONAL VARCHAR(50),
    IsDefaultReply         VARCHAR(18),
    DefaultReplyKeyword    NATIONAL VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Album(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    PvCount                INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_AlbumContent(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    AlbumId                INT,
    ParentId               INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    LargeImageUrl          VARCHAR(200),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Appointment(
    Id                          INT                      AUTO_INCREMENT,
    PublishmentSystemId         INT,
    KeywordId                   INT,
    UserCount                   INT,
    PvCount                     INT,
    StartDate                   DATETIME,
    EndDate                     DATETIME,
    IsDisabled                  VARCHAR(18),
    Title                       NATIONAL VARCHAR(255),
    ImageUrl                    VARCHAR(200),
    Summary                     NATIONAL VARCHAR(255),
    ContentIsSingle             VARCHAR(18),
    ContentImageUrl             VARCHAR(200),
    ContentDescription          NATIONAL VARCHAR(255),
    ContentResultTopImageUrl    VARCHAR(200),
    EndTitle                    NATIONAL VARCHAR(255),
    EndImageUrl                 VARCHAR(200),
    EndSummary                  NATIONAL VARCHAR(255),
    IsFormRealName              VARCHAR(18),
    FormRealNameTitle           NATIONAL VARCHAR(50),
    IsFormMobile                VARCHAR(18),
    FormMobileTitle             NATIONAL VARCHAR(50),
    IsFormEmail                 VARCHAR(18),
    FormEmailTitle              NATIONAL VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_AppointmentContent(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    AppointmentId          INT,
    AppointmentItemId      INT,
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    RealName               NATIONAL VARCHAR(255),
    Mobile                 VARCHAR(50),
    Email                  VARCHAR(200),
    Status                 VARCHAR(50),
    Message                NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    SettingsXml            LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_AppointmentItem(
    Id                         INT                      AUTO_INCREMENT,
    PublishmentSystemId        INT,
    AppointmentId              INT,
    UserCount                  INT,
    Title                      NATIONAL VARCHAR(255),
    TopImageUrl                VARCHAR(200),
    IsDescription              VARCHAR(18),
    DescriptionTitle           NATIONAL VARCHAR(255),
    Description                NATIONAL VARCHAR(255),
    IsImageUrl                 VARCHAR(18),
    ImageUrlTitle              NATIONAL VARCHAR(255),
    ImageUrl                   VARCHAR(200),
    IsVideoUrl                 VARCHAR(18),
    VideoUrlTitle              NATIONAL VARCHAR(255),
    VideoUrl                   VARCHAR(200),
    IsImageUrlCollection       VARCHAR(18),
    ImageUrlCollectionTitle    NATIONAL VARCHAR(255),
    ImageUrlCollection         LONGTEXT,
    LargeImageUrlCollection    LONGTEXT,
    IsMap                      VARCHAR(18),
    MapTitle                   NATIONAL VARCHAR(255),
    MapAddress                 NATIONAL VARCHAR(255),
    IsTel                      VARCHAR(18),
    TelTitle                   NATIONAL VARCHAR(255),
    Tel                        VARCHAR(20),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Card(
    Id                      INT                      AUTO_INCREMENT,
    PublishmentSystemId     INT,
    KeywordId               INT,
    IsDisabled              VARCHAR(18),
    UserCount               INT,
    PvCount                 INT,
    Title                   NATIONAL VARCHAR(255),
    ImageUrl                VARCHAR(200),
    Summary                 NATIONAL VARCHAR(255),
    CardTitle               NATIONAL VARCHAR(255),
    CardTitleColor          NATIONAL VARCHAR(50),
    CardNoColor             VARCHAR(50),
    ContentFrontImageUrl    VARCHAR(200),
    ContentBackImageUrl     VARCHAR(200),
    ShopName                NATIONAL VARCHAR(255),
    ShopAddress             NATIONAL VARCHAR(255),
    ShopTel                 NATIONAL VARCHAR(255),
    ShopPosition            VARCHAR(200),
    ShopPassword            NATIONAL VARCHAR(200),
    ShopOperatorList        LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CardCashLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    UserName               NATIONAL VARCHAR(255),
    CardId                 INT,
    CardSnId               INT,
    CashType               NATIONAL VARCHAR(50),
    Amount                 DECIMAL(20, 2),
    CurAmount              DECIMAL(20, 2),
    ConsumeType            NATIONAL VARCHAR(50),
    Operator               NATIONAL VARCHAR(255),
    Description            NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CardEntitySn(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CardId                 INT,
    Sn                     VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    Mobile                 VARCHAR(50),
    Amount                 DECIMAL(20, 2),
    Credits                INT,
    Email                  NATIONAL VARCHAR(255),
    Address                NATIONAL VARCHAR(255),
    IsBinding              VARCHAR(18),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CardSignLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    UserName               NATIONAL VARCHAR(255),
    SignDate               DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CardSn(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CardId                 INT,
    Sn                     VARCHAR(200),
    Amount                 DECIMAL(20, 2),
    IsDisabled             VARCHAR(18),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Collect(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    UserCount              INT,
    PvCount                INT,
    StartDate              DATETIME,
    EndDate                DATETIME,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ContentImageUrl        VARCHAR(200),
    ContentDescription     LONGTEXT,
    ContentMaxVote         INT,
    ContentIsCheck         VARCHAR(18),
    EndTitle               NATIONAL VARCHAR(255),
    EndImageUrl            VARCHAR(200),
    EndSummary             NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CollectItem(
    Id                     INT                      AUTO_INCREMENT,
    CollectId              INT,
    PublishmentSystemId    INT,
    Title                  NATIONAL VARCHAR(255),
    SmallUrl               VARCHAR(200),
    LargeUrl               VARCHAR(200),
    Description            NATIONAL VARCHAR(255),
    Mobile                 VARCHAR(200),
    IsChecked              VARCHAR(18),
    VoteNum                INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CollectLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CollectId              INT,
    ItemId                 INT,
    IpAddress              VARCHAR(50),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Conference(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    UserCount              INT,
    PvCount                INT,
    StartDate              DATETIME,
    EndDate                DATETIME,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    BackgroundImageUrl     VARCHAR(200),
    ConferenceName         NATIONAL VARCHAR(255),
    Address                NATIONAL VARCHAR(255),
    Duration               NATIONAL VARCHAR(255),
    Introduction           LONGTEXT,
    IsAgenda               VARCHAR(18),
    AgendaTitle            NATIONAL VARCHAR(255),
    AgendaList             LONGTEXT,
    IsGuest                VARCHAR(18),
    GuestTitle             NATIONAL VARCHAR(255),
    GuestList              LONGTEXT,
    EndTitle               NATIONAL VARCHAR(255),
    EndImageUrl            VARCHAR(200),
    EndSummary             NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_ConferenceContent(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ConferenceId           INT,
    IpAddress              VARCHAR(50),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    RealName               NATIONAL VARCHAR(255),
    Mobile                 VARCHAR(50),
    Email                  VARCHAR(200),
    Company                NATIONAL VARCHAR(255),
    Position               NATIONAL VARCHAR(255),
    Note                   NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_ConfigExtend(
    Id                     INT            AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordType            VARCHAR(50),
    FunctionId             INT,
    AttributeName          VARCHAR(50),
    IsVisible              VARCHAR(18),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Count(
    CountId                INT            AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CountYear              INT,
    CountMonth             INT,
    CountDay               INT,
    CountType              VARCHAR(50),
    Count                  INT,
    PRIMARY KEY (CountId)
)ENGINE=INNODB
GO



CREATE TABLE wx_Coupon(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ActId                  INT,
    Title                  NATIONAL VARCHAR(255),
    TotalNum               INT,
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CouponAct(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    UserCount              INT,
    PvCount                INT,
    StartDate              DATETIME,
    EndDate                DATETIME,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ContentImageUrl        VARCHAR(200),
    ContentUsage           LONGTEXT,
    ContentDescription     LONGTEXT,
    IsFormRealName         VARCHAR(18),
    FormRealNameTitle      NATIONAL VARCHAR(255),
    IsFormMobile           VARCHAR(18),
    FormMobileTitle        NATIONAL VARCHAR(255),
    IsFormEmail            VARCHAR(18),
    FormEmailTitle         NATIONAL VARCHAR(255),
    IsFormAddress          VARCHAR(18),
    FormAddressTitle       NATIONAL VARCHAR(255),
    EndTitle               NATIONAL VARCHAR(255),
    EndImageUrl            VARCHAR(200),
    EndSummary             NATIONAL VARCHAR(255),
    AwardCode              NATIONAL VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_CouponSn(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CouponId               INT,
    Sn                     VARCHAR(200),
    Status                 VARCHAR(50),
    HoldDate               DATETIME,
    HoldRealName           NATIONAL VARCHAR(255),
    HoldMobile             VARCHAR(200),
    HoldEmail              VARCHAR(200),
    HoldAddress            NATIONAL VARCHAR(255),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    CashDate               DATETIME,
    CashUserName           NATIONAL VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Keyword(
    KeywordId              INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    Keywords               NATIONAL VARCHAR(255),
    IsDisabled             VARCHAR(18),
    KeywordType            VARCHAR(50),
    MatchType              VARCHAR(50),
    Reply                  LONGTEXT,
    AddDate                DATETIME,
    Taxis                  INT,
    PRIMARY KEY (KeywordId)
)ENGINE=INNODB
GO



CREATE TABLE wx_KeywordGroup(
    GroupId                INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    GroupName              NATIONAL VARCHAR(255),
    Taxis                  INT,
    PRIMARY KEY (GroupId)
)ENGINE=INNODB
GO



CREATE TABLE wx_KeywordMatch(
    MatchId                INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    Keyword                NATIONAL VARCHAR(255),
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    KeywordType            VARCHAR(50),
    MatchType              VARCHAR(50),
    PRIMARY KEY (MatchId)
)ENGINE=INNODB
GO



CREATE TABLE wx_KeywordResource(
    ResourceId             INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ResourceType           VARCHAR(50),
    IsShowCoverPic         VARCHAR(18),
    Content                LONGTEXT,
    NavigationUrl          VARCHAR(200),
    ChannelId              INT,
    ContentId              INT,
    Taxis                  INT,
    PRIMARY KEY (ResourceId)
)ENGINE=INNODB
GO



CREATE TABLE wx_Lottery(
    Id                      INT                      AUTO_INCREMENT,
    PublishmentSystemId     INT,
    LotteryType             VARCHAR(50),
    KeywordId               INT,
    IsDisabled              VARCHAR(18),
    UserCount               INT,
    PvCount                 INT,
    StartDate               DATETIME,
    EndDate                 DATETIME,
    Title                   NATIONAL VARCHAR(255),
    ImageUrl                VARCHAR(200),
    Summary                 NATIONAL VARCHAR(255),
    ContentImageUrl         VARCHAR(200),
    ContentAwardImageUrl    VARCHAR(200),
    ContentUsage            LONGTEXT,
    AwardImageUrl           VARCHAR(200),
    AwardUsage              LONGTEXT,
    IsAwardTotalNum         VARCHAR(10),
    AwardMaxCount           INT,
    AwardMaxDailyCount      INT,
    AwardCode               NATIONAL VARCHAR(50),
    IsFormRealName          VARCHAR(18),
    FormRealNameTitle       NATIONAL VARCHAR(50),
    IsFormMobile            VARCHAR(18),
    FormMobileTitle         NATIONAL VARCHAR(50),
    IsFormEmail             VARCHAR(18),
    FormEmailTitle          NATIONAL VARCHAR(50),
    IsFormAddress           VARCHAR(18),
    FormAddressTitle        NATIONAL VARCHAR(50),
    EndTitle                NATIONAL VARCHAR(255),
    EndImageUrl             VARCHAR(200),
    EndSummary              NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_LotteryAward(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    LotteryId              INT,
    AwardName              NATIONAL VARCHAR(255),
    Title                  NATIONAL VARCHAR(255),
    TotalNum               INT,
    Probability            DECIMAL(18, 2),
    WonNum                 INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_LotteryLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    LotteryId              INT,
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    LotteryCount           INT,
    LotteryDailyCount      INT,
    LastLotteryDate        DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_LotteryWinner(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    LotteryType            VARCHAR(50),
    LotteryId              INT,
    AwardId                INT,
    Status                 VARCHAR(50),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(50),
    RealName               NATIONAL VARCHAR(255),
    Mobile                 VARCHAR(200),
    Email                  VARCHAR(200),
    Address                NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    CashSn                 VARCHAR(200),
    CashDate               DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Map(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    PvCount                INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    MapWd                  NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Menu(
    MenuId                 INT                     AUTO_INCREMENT,
    PublishmentSystemId    INT,
    MenuName               NATIONAL VARCHAR(50),
    MenuType               VARCHAR(50),
    Keyword                NATIONAL VARCHAR(50),
    Url                    VARCHAR(200),
    ChannelId              INT,
    ContentId              INT,
    ParentId               INT,
    Taxis                  INT,
    PRIMARY KEY (MenuId)
)ENGINE=INNODB
GO



CREATE TABLE wx_Message(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    UserCount              INT,
    PvCount                INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ContentImageUrl        VARCHAR(200),
    ContentDescription     LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_MessageContent(
    Id                        INT                      AUTO_INCREMENT,
    PublishmentSystemId       INT,
    MessageId                 INT,
    IpAddress                 VARCHAR(50),
    CookieSn                  VARCHAR(50),
    WxOpenId                  VARCHAR(200),
    UserName                  NATIONAL VARCHAR(255),
    ReplyCount                INT,
    LikeCount                 INT,
    LikeCookieSnCollection    LONGTEXT,
    IsReply                   VARCHAR(18),
    ReplyId                   INT,
    DisplayName               NATIONAL VARCHAR(50),
    Color                     VARCHAR(50),
    Content                   NATIONAL VARCHAR(255),
    AddDate                   DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Search(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    PvCount                INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ContentImageUrl        VARCHAR(200),
    IsOutsiteSearch        VARCHAR(18),
    IsNavigation           VARCHAR(18),
    NavTitleColor          VARCHAR(50),
    NavImageColor          VARCHAR(50),
    ImageAreaTitle         NATIONAL VARCHAR(50),
    ImageAreaChannelId     INT,
    TextAreaTitle          NATIONAL VARCHAR(50),
    TextAreaChannelId      INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_SearchNavigation(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    SearchId               INT,
    Title                  NATIONAL VARCHAR(255),
    Url                    VARCHAR(200),
    ImageCssClass          VARCHAR(200),
    NavigationType         VARCHAR(50),
    KeywordType            VARCHAR(50),
    FunctionId             INT,
    ChannelId              INT,
    ContentId              INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Store(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    PvCount                INT,
    IsDisabled             VARCHAR(18),
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_StoreCategory(
    Id                     INT                     AUTO_INCREMENT,
    PublishmentSystemId    INT,
    CategoryName           NATIONAL VARCHAR(50),
    ParentId               INT,
    Taxis                  INT,
    ChildCount             INT,
    ParentsCount           INT,
    ParentsPath            VARCHAR(100),
    StoreCount             INT,
    IsLastNode             VARCHAR(18),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_StoreItem(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    StoreId                INT,
    CategoryId             INT,
    StoreName              NATIONAL VARCHAR(255),
    Tel                    VARCHAR(50),
    Mobile                 NATIONAL VARCHAR(11),
    ImageUrl               VARCHAR(200),
    Address                NATIONAL VARCHAR(255),
    Longitude              VARCHAR(100),
    Latitude               VARCHAR(100),
    Summary                LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_View360(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    IsDisabled             VARCHAR(18),
    PvCount                INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    ContentImageUrl1       VARCHAR(200),
    ContentImageUrl2       VARCHAR(200),
    ContentImageUrl3       VARCHAR(200),
    ContentImageUrl4       VARCHAR(200),
    ContentImageUrl5       VARCHAR(200),
    ContentImageUrl6       VARCHAR(200),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Vote(
    Id                      INT                      AUTO_INCREMENT,
    PublishmentSystemId     INT,
    KeywordId               INT,
    IsDisabled              VARCHAR(18),
    UserCount               INT,
    PvCount                 INT,
    StartDate               DATETIME,
    EndDate                 DATETIME,
    Title                   NATIONAL VARCHAR(255),
    ImageUrl                VARCHAR(200),
    Summary                 NATIONAL VARCHAR(255),
    ContentImageUrl         VARCHAR(200),
    ContentDescription      LONGTEXT,
    ContentIsImageOption    VARCHAR(18),
    ContentIsCheckBox       VARCHAR(18),
    ContentResultVisible    VARCHAR(50),
    EndTitle                NATIONAL VARCHAR(255),
    EndImageUrl             VARCHAR(200),
    EndSummary              NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_VoteItem(
    Id                     INT                      AUTO_INCREMENT,
    VoteId                 INT,
    PublishmentSystemId    INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    NavigationUrl          VARCHAR(200),
    VoteNum                INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_VoteLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    VoteId                 INT,
    ItemIdCollection       VARCHAR(200),
    IpAddress              VARCHAR(50),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_WebMenu(
    Id                     INT                     AUTO_INCREMENT,
    PublishmentSystemId    INT,
    MenuName               NATIONAL VARCHAR(50),
    IconUrl                VARCHAR(200),
    IconCssClass           VARCHAR(50),
    NavigationType         VARCHAR(50),
    Url                    VARCHAR(200),
    ChannelId              INT,
    ContentId              INT,
    KeywordType            VARCHAR(50),
    FunctionId             INT,
    ParentId               INT,
    Taxis                  INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_Wifi(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordId              INT,
    PvCount                INT,
    IsDisabled             VARCHAR(18),
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    Summary                NATIONAL VARCHAR(255),
    BusinessId             VARCHAR(100),
    CallBackString         LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO



CREATE TABLE wx_WifiNode(
    Id                     INT             AUTO_INCREMENT,
    PublishmentSystemId    INT,
    BusinessId             VARCHAR(100),
    NodeId                 VARCHAR(100),
    CallBackString         LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO
