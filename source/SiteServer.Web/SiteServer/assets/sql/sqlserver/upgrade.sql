ALTER TABLE bairong_Administrator ADD
    CountOfFailedLogin               int              NULL
GO

ALTER TABLE bairong_Config ADD
    UserConfig              ntext            NULL,
    SystemConfig            ntext            NULL
GO

CREATE TABLE bairong_DbCache(
    ID            int              IDENTITY(1,1),
    CacheKey      varchar(200)     NULL,
    CacheValue    nvarchar(500)    NULL,
    AddDate       datetime         NULL,
    CONSTRAINT PK_bairong_DbCache PRIMARY KEY NONCLUSTERED (ID)
)
GO

CREATE CLUSTERED INDEX IX_bairong_DbCache_Key ON bairong_DbCache(CacheKey)
GO

ALTER TABLE bairong_UserGroup ADD
    IsDefault       varchar(18)      NULL,
    Description     nvarchar(255)    NULL
GO

ALTER TABLE bairong_Users ADD
    LastResetPasswordDate    datetime         NULL,
    CountOfLogin             int              NULL,
    CountOfFailedLogin       int              NULL,
    CountOfWriting           int              NULL,
    AvatarUrl                varchar(200)     NULL,
    ExtendValues             ntext            NULL
GO

CREATE TABLE siteserver_CreateTask(
    ID                     int            IDENTITY(1,1),
    CreateType             varchar(50)    NULL,
    PublishmentSystemID    int            NULL,
    ChannelID              int            NULL,
    ContentID              int            NULL,
    TemplateID             int            NULL,
    CONSTRAINT PK_siteserver_CreateTask PRIMARY KEY NONCLUSTERED (ID)
)
GO

CREATE TABLE siteserver_CreateTaskLog(
    ID                     int              IDENTITY(1,1),
    CreateType             varchar(50)      NULL,
    PublishmentSystemID    int              NULL,
    TaskName               nvarchar(50)     NULL,
    TimeSpan               nvarchar(50)     NULL,
    IsSuccess              varchar(18)      NULL,
    ErrorMessage           nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_siteserver_CreateTaskLog PRIMARY KEY NONCLUSTERED (ID)
)
GO

CREATE TABLE siteserver_Task(
    TaskID                  int              IDENTITY(1,1),
    TaskName                nvarchar(50)     NULL,
    IsSystemTask            varchar(18)      NULL,
    PublishmentSystemID     int              NULL,
    ServiceType             varchar(50)      NULL,
    ServiceParameters       ntext            NULL,
    FrequencyType           varchar(50)      NULL,
    PeriodIntervalMinute    int              NULL,
    StartDay                int              NULL,
    StartWeekday            int              NULL,
    StartHour               int              NULL,
    IsEnabled               varchar(18)      NULL,
    AddDate                 datetime         NULL,
    LastExecuteDate         datetime         NULL,
    Description             nvarchar(255)    NULL,
    OnlyOnceDate            datetime         NULL,
    CONSTRAINT PK_siteserver_Task PRIMARY KEY NONCLUSTERED (TaskID)
)
GO

CREATE TABLE siteserver_TaskLog(
    ID              int              IDENTITY(1,1),
    TaskID          int              NULL,
    IsSuccess       varchar(18)      NULL,
    ErrorMessage    nvarchar(255)    NULL,
    AddDate         datetime         NULL,
    CONSTRAINT PK_siteserver_TaskLog PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT FK_siteserver_Task_Log FOREIGN KEY (TaskID)
    REFERENCES siteserver_Task(TaskID) ON DELETE CASCADE ON UPDATE CASCADE
)
GO

DROP TABLE siteserver_Comment
GO

CREATE TABLE siteserver_Comment(
    ID                     int              IDENTITY(1,1),
    PublishmentSystemID    int              NULL,
    NodeID                 int              NULL,
    ContentID              int              NULL,
    GoodCount              int              NULL,
    UserName               nvarchar(50)     NULL,
    IsChecked              varchar(18)      NULL,
    AddDate                datetime         NULL,
    Content                nvarchar(500)    NULL,
    CONSTRAINT PK_siteserver_Comment PRIMARY KEY NONCLUSTERED (ID)
)
GO

CREATE CLUSTERED INDEX IX_siteserver_Comment_PSID ON siteserver_Comment(PublishmentSystemID)
GO

CREATE INDEX IX_siteserver_Comment_ContentID ON siteserver_Comment(ContentID)
GO

ALTER TABLE siteserver_PublishmentSystem ADD
    PublishmentSystemType               varchar(50)              NULL
GO

CREATE TABLE wx_Account(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    Token                  varchar(200)     NULL,
    IsBinding              varchar(18)      NULL,
    AccountType            varchar(50)      NULL,
    WeChatId               nvarchar(255)    NULL,
    SourceId               varchar(200)     NULL,
    ThumbUrl               varchar(200)     NULL,
    AppId                  varchar(200)     NULL,
    AppSecret              varchar(200)     NULL,
    IsWelcome              varchar(18)      NULL,
    WelcomeKeyword         nvarchar(50)     NULL,
    IsDefaultReply         varchar(18)      NULL,
    DefaultReplyKeyword    nvarchar(50)     NULL,
    CONSTRAINT PK_wx_Account PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Album(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    PvCount                int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Album PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_AlbumContent(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    AlbumId                int              NULL,
    ParentId               int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    LargeImageUrl          varchar(200)     NULL,
    CONSTRAINT PK_wx_AlbumContent PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Appointment(
    Id                          int              IDENTITY(1,1),
    PublishmentSystemId         int              NULL,
    KeywordId                   int              NULL,
    UserCount                   int              NULL,
    PvCount                     int              NULL,
    StartDate                   datetime         NULL,
    EndDate                     datetime         NULL,
    IsDisabled                  varchar(18)      NULL,
    Title                       nvarchar(255)    NULL,
    ImageUrl                    varchar(200)     NULL,
    Summary                     nvarchar(255)    NULL,
    ContentIsSingle             varchar(18)      NULL,
    ContentImageUrl             varchar(200)     NULL,
    ContentDescription          nvarchar(255)    NULL,
    ContentResultTopImageUrl    varchar(200)     NULL,
    EndTitle                    nvarchar(255)    NULL,
    EndImageUrl                 varchar(200)     NULL,
    EndSummary                  nvarchar(255)    NULL,
    IsFormRealName              varchar(18)      NULL,
    FormRealNameTitle           nvarchar(50)     NULL,
    IsFormMobile                varchar(18)      NULL,
    FormMobileTitle             nvarchar(50)     NULL,
    IsFormEmail                 varchar(18)      NULL,
    FormEmailTitle              nvarchar(50)     NULL,
    CONSTRAINT PK_wx_Appointment PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_AppointmentContent(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    AppointmentId          int              NULL,
    AppointmentItemId      int              NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    RealName               nvarchar(255)    NULL,
    Mobile                 varchar(50)      NULL,
    Email                  varchar(200)     NULL,
    Status                 varchar(50)      NULL,
    Message                nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    SettingsXml            ntext            NULL,
    CONSTRAINT PK_wx_AppointmentContent PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_AppointmentItem(
    Id                         int              IDENTITY(1,1),
    PublishmentSystemId        int              NULL,
    AppointmentId              int              NULL,
    UserCount                  int              NULL,
    Title                      nvarchar(255)    NULL,
    TopImageUrl                varchar(200)     NULL,
    IsDescription              varchar(18)      NULL,
    DescriptionTitle           nvarchar(255)    NULL,
    Description                nvarchar(255)    NULL,
    IsImageUrl                 varchar(18)      NULL,
    ImageUrlTitle              nvarchar(255)    NULL,
    ImageUrl                   varchar(200)     NULL,
    IsVideoUrl                 varchar(18)      NULL,
    VideoUrlTitle              nvarchar(255)    NULL,
    VideoUrl                   varchar(200)     NULL,
    IsImageUrlCollection       varchar(18)      NULL,
    ImageUrlCollectionTitle    nvarchar(255)    NULL,
    ImageUrlCollection         ntext            NULL,
    LargeImageUrlCollection    ntext            NULL,
    IsMap                      varchar(18)      NULL,
    MapTitle                   nvarchar(255)    NULL,
    MapAddress                 nvarchar(255)    NULL,
    IsTel                      varchar(18)      NULL,
    TelTitle                   nvarchar(255)    NULL,
    Tel                        varchar(20)      NULL,
    CONSTRAINT PK_wx_AppointmentItem PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Card(
    Id                      int              IDENTITY(1,1),
    PublishmentSystemId     int              NULL,
    KeywordId               int              NULL,
    IsDisabled              varchar(18)      NULL,
    UserCount               int              NULL,
    PvCount                 int              NULL,
    Title                   nvarchar(255)    NULL,
    ImageUrl                varchar(200)     NULL,
    Summary                 nvarchar(255)    NULL,
    CardTitle               nvarchar(255)    NULL,
    CardTitleColor          nvarchar(50)     NULL,
    CardNoColor             varchar(50)      NULL,
    ContentFrontImageUrl    varchar(200)     NULL,
    ContentBackImageUrl     varchar(200)     NULL,
    ShopName                nvarchar(255)    NULL,
    ShopAddress             nvarchar(255)    NULL,
    ShopTel                 nvarchar(255)    NULL,
    ShopPosition            varchar(200)     NULL,
    ShopPassword            nvarchar(200)    NULL,
    ShopOperatorList        ntext            NULL,
    CONSTRAINT PK_wx_Card PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CardCashLog(
    Id                     int               IDENTITY(1,1),
    PublishmentSystemId    int               NULL,
    UserName               nvarchar(255)     NULL,
    CardId                 int               NULL,
    CardSnId               int               NULL,
    CashType               nvarchar(50)      NULL,
    Amount                 decimal(20, 2)    NULL,
    CurAmount              decimal(20, 2)    NULL,
    ConsumeType            nvarchar(50)      NULL,
    Operator               nvarchar(255)     NULL,
    Description            nvarchar(255)     NULL,
    AddDate                datetime          NULL,
    CONSTRAINT PK_wx_CardCashLog PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CardEntitySn(
    Id                     int               IDENTITY(1,1),
    PublishmentSystemId    int               NULL,
    CardId                 int               NULL,
    Sn                     varchar(200)      NULL,
    UserName               nvarchar(255)     NULL,
    Mobile                 varchar(50)       NULL,
    Amount                 decimal(20, 2)    NULL,
    Credits                int               NULL,
    Email                  nvarchar(255)     NULL,
    Address                nvarchar(255)     NULL,
    IsBinding              varchar(18)       NULL,
    AddDate                datetime          NULL,
    CONSTRAINT PK_wx_CardEntitySn PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CardSignLog(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    UserName               nvarchar(255)    NULL,
    SignDate               datetime         NULL,
    CONSTRAINT PK_wx_CardSignLog PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CardSn(
    Id                     int               IDENTITY(1,1),
    PublishmentSystemId    int               NULL,
    CardId                 int               NULL,
    Sn                     varchar(200)      NULL,
    Amount                 decimal(20, 2)    NULL,
    IsDisabled             varchar(18)       NULL,
    UserName               nvarchar(255)     NULL,
    AddDate                datetime          NULL,
    CONSTRAINT PK_wx_CardSn PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Collect(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    UserCount              int              NULL,
    PvCount                int              NULL,
    StartDate              datetime         NULL,
    EndDate                datetime         NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ContentImageUrl        varchar(200)     NULL,
    ContentDescription     ntext            NULL,
    ContentMaxVote         int              NULL,
    ContentIsCheck         varchar(18)      NULL,
    EndTitle               nvarchar(255)    NULL,
    EndImageUrl            varchar(200)     NULL,
    EndSummary             nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Collect PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CollectItem(
    Id                     int              IDENTITY(1,1),
    CollectId              int              NULL,
    PublishmentSystemId    int              NULL,
    Title                  nvarchar(255)    NULL,
    SmallUrl               varchar(200)     NULL,
    LargeUrl               varchar(200)     NULL,
    Description            nvarchar(255)    NULL,
    Mobile                 varchar(200)     NULL,
    IsChecked              varchar(18)      NULL,
    VoteNum                int              NULL,
    CONSTRAINT PK_wx_CollectItem PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CollectLog(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    CollectId              int              NULL,
    ItemId                 int              NULL,
    IpAddress              varchar(50)      NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wx_CollectLog PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Conference(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    UserCount              int              NULL,
    PvCount                int              NULL,
    StartDate              datetime         NULL,
    EndDate                datetime         NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    BackgroundImageUrl     varchar(200)     NULL,
    ConferenceName         nvarchar(255)    NULL,
    Address                nvarchar(255)    NULL,
    Duration               nvarchar(255)    NULL,
    Introduction           ntext            NULL,
    IsAgenda               varchar(18)      NULL,
    AgendaTitle            nvarchar(255)    NULL,
    AgendaList             ntext            NULL,
    IsGuest                varchar(18)      NULL,
    GuestTitle             nvarchar(255)    NULL,
    GuestList              ntext            NULL,
    EndTitle               nvarchar(255)    NULL,
    EndImageUrl            varchar(200)     NULL,
    EndSummary             nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Conference PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_ConferenceContent(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ConferenceId           int              NULL,
    IpAddress              varchar(50)      NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    RealName               nvarchar(255)    NULL,
    Mobile                 varchar(50)      NULL,
    Email                  varchar(200)     NULL,
    Company                nvarchar(255)    NULL,
    Position               nvarchar(255)    NULL,
    Note                   nvarchar(255)    NULL,
    CONSTRAINT PK_wx_ConferenceContent PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_ConfigExtend(
    Id                     int            IDENTITY(1,1),
    PublishmentSystemId    int            NULL,
    KeywordType            varchar(50)    NULL,
    FunctionId             int            NULL,
    AttributeName          varchar(50)    NULL,
    IsVisible              varchar(18)    NULL,
    CONSTRAINT PK_wx_ConfigExtend PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Count(
    CountId                int            IDENTITY(1,1),
    PublishmentSystemId    int            NULL,
    CountYear              int            NULL,
    CountMonth             int            NULL,
    CountDay               int            NULL,
    CountType              varchar(50)    NULL,
    Count                  int            NULL,
    CONSTRAINT PK_wx_Count PRIMARY KEY NONCLUSTERED (CountId)
)
GO



CREATE TABLE wx_Coupon(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ActId                  int              NULL,
    Title                  nvarchar(255)    NULL,
    TotalNum               int              NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wx_Coupon PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CouponAct(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    UserCount              int              NULL,
    PvCount                int              NULL,
    StartDate              datetime         NULL,
    EndDate                datetime         NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ContentImageUrl        varchar(200)     NULL,
    ContentUsage           ntext            NULL,
    ContentDescription     ntext            NULL,
    IsFormRealName         varchar(18)      NULL,
    FormRealNameTitle      nvarchar(255)    NULL,
    IsFormMobile           varchar(18)      NULL,
    FormMobileTitle        nvarchar(255)    NULL,
    IsFormEmail            varchar(18)      NULL,
    FormEmailTitle         nvarchar(255)    NULL,
    IsFormAddress          varchar(18)      NULL,
    FormAddressTitle       nvarchar(255)    NULL,
    EndTitle               nvarchar(255)    NULL,
    EndImageUrl            varchar(200)     NULL,
    EndSummary             nvarchar(255)    NULL,
    AwardCode              nvarchar(50)     NULL,
    CONSTRAINT PK_wx_CouponAct PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_CouponSn(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    CouponId               int              NULL,
    Sn                     varchar(200)     NULL,
    Status                 varchar(50)      NULL,
    HoldDate               datetime         NULL,
    HoldRealName           nvarchar(255)    NULL,
    HoldMobile             varchar(200)     NULL,
    HoldEmail              varchar(200)     NULL,
    HoldAddress            nvarchar(255)    NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    CashDate               datetime         NULL,
    CashUserName           nvarchar(50)     NULL,
    CONSTRAINT PK_wx_CouponSn PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Keyword(
    KeywordId              int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    Keywords               nvarchar(255)    NULL,
    IsDisabled             varchar(18)      NULL,
    KeywordType            varchar(50)      NULL,
    MatchType              varchar(50)      NULL,
    Reply                  ntext            NULL,
    AddDate                datetime         NULL,
    Taxis                  int              NULL,
    CONSTRAINT PK_wx_Keyword PRIMARY KEY NONCLUSTERED (KeywordId)
)
GO



CREATE TABLE wx_KeywordGroup(
    GroupId                int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    GroupName              nvarchar(255)    NULL,
    Taxis                  int              NULL,
    CONSTRAINT PK_wx_KeywordGroup PRIMARY KEY NONCLUSTERED (GroupId)
)
GO



CREATE TABLE wx_KeywordMatch(
    MatchId                int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    Keyword                nvarchar(255)    NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    KeywordType            varchar(50)      NULL,
    MatchType              varchar(50)      NULL,
    CONSTRAINT PK_wx_KeywordMatch PRIMARY KEY NONCLUSTERED (MatchId)
)
GO



CREATE TABLE wx_KeywordResource(
    ResourceId             int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ResourceType           varchar(50)      NULL,
    IsShowCoverPic         varchar(18)      NULL,
    Content                ntext            NULL,
    NavigationUrl          varchar(200)     NULL,
    ChannelId              int              NULL,
    ContentId              int              NULL,
    Taxis                  int              NULL,
    CONSTRAINT PK_wx_KeywordResource PRIMARY KEY NONCLUSTERED (ResourceId)
)
GO



CREATE TABLE wx_Lottery(
    Id                      int              IDENTITY(1,1),
    PublishmentSystemId     int              NULL,
    LotteryType             varchar(50)      NULL,
    KeywordId               int              NULL,
    IsDisabled              varchar(18)      NULL,
    UserCount               int              NULL,
    PvCount                 int              NULL,
    StartDate               datetime         NULL,
    EndDate                 datetime         NULL,
    Title                   nvarchar(255)    NULL,
    ImageUrl                varchar(200)     NULL,
    Summary                 nvarchar(255)    NULL,
    ContentImageUrl         varchar(200)     NULL,
    ContentAwardImageUrl    varchar(200)     NULL,
    ContentUsage            ntext            NULL,
    AwardImageUrl           varchar(200)     NULL,
    AwardUsage              ntext            NULL,
    IsAwardTotalNum         varchar(10)      NULL,
    AwardMaxCount           int              NULL,
    AwardMaxDailyCount      int              NULL,
    AwardCode               nvarchar(50)     NULL,
    IsFormRealName          varchar(18)      NULL,
    FormRealNameTitle       nvarchar(50)     NULL,
    IsFormMobile            varchar(18)      NULL,
    FormMobileTitle         nvarchar(50)     NULL,
    IsFormEmail             varchar(18)      NULL,
    FormEmailTitle          nvarchar(50)     NULL,
    IsFormAddress           varchar(18)      NULL,
    FormAddressTitle        nvarchar(50)     NULL,
    EndTitle                nvarchar(255)    NULL,
    EndImageUrl             varchar(200)     NULL,
    EndSummary              nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Lottery PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_LotteryAward(
    Id                     int               IDENTITY(1,1),
    PublishmentSystemId    int               NULL,
    LotteryId              int               NULL,
    AwardName              nvarchar(255)     NULL,
    Title                  nvarchar(255)     NULL,
    TotalNum               int               NULL,
    Probability            decimal(18, 2)    NULL,
    WonNum                 int               NULL,
    CONSTRAINT PK_wx_LotteryAward PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_LotteryLog(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    LotteryId              int              NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    LotteryCount           int              NULL,
    LotteryDailyCount      int              NULL,
    LastLotteryDate        datetime         NULL,
    CONSTRAINT PK_wx_LotteryLog PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_LotteryWinner(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    LotteryType            varchar(50)      NULL,
    LotteryId              int              NULL,
    AwardId                int              NULL,
    Status                 varchar(50)      NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(50)     NULL,
    RealName               nvarchar(255)    NULL,
    Mobile                 varchar(200)     NULL,
    Email                  varchar(200)     NULL,
    Address                nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CashSn                 varchar(200)     NULL,
    CashDate               datetime         NULL,
    CONSTRAINT PK_wx_LotteryWinner PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Map(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    PvCount                int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    MapWd                  nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Map PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Menu(
    MenuId                 int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    MenuName               nvarchar(50)    NULL,
    MenuType               varchar(50)     NULL,
    Keyword                nvarchar(50)    NULL,
    Url                    varchar(200)    NULL,
    ChannelId              int             NULL,
    ContentId              int             NULL,
    ParentId               int             NULL,
    Taxis                  int             NULL,
    CONSTRAINT PK_wx_Menu PRIMARY KEY NONCLUSTERED (MenuId)
)
GO



CREATE TABLE wx_Message(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    UserCount              int              NULL,
    PvCount                int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ContentImageUrl        varchar(200)     NULL,
    ContentDescription     ntext            NULL,
    CONSTRAINT PK_wx_Message PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_MessageContent(
    Id                        int              IDENTITY(1,1),
    PublishmentSystemId       int              NULL,
    MessageId                 int              NULL,
    IpAddress                 varchar(50)      NULL,
    CookieSn                  varchar(50)      NULL,
    WxOpenId                  varchar(200)     NULL,
    UserName                  nvarchar(255)    NULL,
    ReplyCount                int              NULL,
    LikeCount                 int              NULL,
    LikeCookieSnCollection    ntext            NULL,
    IsReply                   varchar(18)      NULL,
    ReplyId                   int              NULL,
    DisplayName               nvarchar(50)     NULL,
    Color                     varchar(50)      NULL,
    Content                   nvarchar(255)    NULL,
    AddDate                   datetime         NULL,
    CONSTRAINT PK_wx_MessageContent PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Search(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    PvCount                int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ContentImageUrl        varchar(200)     NULL,
    IsOutsiteSearch        varchar(18)      NULL,
    IsNavigation           varchar(18)      NULL,
    NavTitleColor          varchar(50)      NULL,
    NavImageColor          varchar(50)      NULL,
    ImageAreaTitle         nvarchar(50)     NULL,
    ImageAreaChannelId     int              NULL,
    TextAreaTitle          nvarchar(50)     NULL,
    TextAreaChannelId      int              NULL,
    CONSTRAINT PK_wx_Search PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_SearchNavigation(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    SearchId               int              NULL,
    Title                  nvarchar(255)    NULL,
    Url                    varchar(200)     NULL,
    ImageCssClass          varchar(200)     NULL,
    NavigationType         varchar(50)      NULL,
    KeywordType            varchar(50)      NULL,
    FunctionId             int              NULL,
    ChannelId              int              NULL,
    ContentId              int              NULL,
    CONSTRAINT PK_wx_SearchNavigation PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Store(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    PvCount                int              NULL,
    IsDisabled             varchar(18)      NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Store PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_StoreCategory(
    Id                     int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    CategoryName           nvarchar(50)    NULL,
    ParentId               int             NULL,
    Taxis                  int             NULL,
    ChildCount             int             NULL,
    ParentsCount           int             NULL,
    ParentsPath            varchar(100)    NULL,
    StoreCount             int             NULL,
    IsLastNode             varchar(18)     NULL,
    CONSTRAINT PK_wx_StoreCategory PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_StoreItem(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    StoreId                int              NULL,
    CategoryId             int              NULL,
    StoreName              nvarchar(255)    NULL,
    Tel                    varchar(50)      NULL,
    Mobile                 nvarchar(11)     NULL,
    ImageUrl               varchar(200)     NULL,
    Address                nvarchar(255)    NULL,
    Longitude              varchar(100)     NULL,
    Latitude               varchar(100)     NULL,
    Summary                ntext            NULL,
    CONSTRAINT PK_wx_StoreItem PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_View360(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    IsDisabled             varchar(18)      NULL,
    PvCount                int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    ContentImageUrl1       varchar(200)     NULL,
    ContentImageUrl2       varchar(200)     NULL,
    ContentImageUrl3       varchar(200)     NULL,
    ContentImageUrl4       varchar(200)     NULL,
    ContentImageUrl5       varchar(200)     NULL,
    ContentImageUrl6       varchar(200)     NULL,
    CONSTRAINT PK_wx_View360 PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Vote(
    Id                      int              IDENTITY(1,1),
    PublishmentSystemId     int              NULL,
    KeywordId               int              NULL,
    IsDisabled              varchar(18)      NULL,
    UserCount               int              NULL,
    PvCount                 int              NULL,
    StartDate               datetime         NULL,
    EndDate                 datetime         NULL,
    Title                   nvarchar(255)    NULL,
    ImageUrl                varchar(200)     NULL,
    Summary                 nvarchar(255)    NULL,
    ContentImageUrl         varchar(200)     NULL,
    ContentDescription      ntext            NULL,
    ContentIsImageOption    varchar(18)      NULL,
    ContentIsCheckBox       varchar(18)      NULL,
    ContentResultVisible    varchar(50)      NULL,
    EndTitle                nvarchar(255)    NULL,
    EndImageUrl             varchar(200)     NULL,
    EndSummary              nvarchar(255)    NULL,
    CONSTRAINT PK_wx_Vote PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_VoteItem(
    Id                     int              IDENTITY(1,1),
    VoteId                 int              NULL,
    PublishmentSystemId    int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    NavigationUrl          varchar(200)     NULL,
    VoteNum                int              NULL,
    CONSTRAINT PK_wx_VoteItem PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_VoteLog(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    VoteId                 int              NULL,
    ItemIdCollection       varchar(200)     NULL,
    IpAddress              varchar(50)      NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wx_VoteLog PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_WebMenu(
    Id                     int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    MenuName               nvarchar(50)    NULL,
    IconUrl                varchar(200)    NULL,
    IconCssClass           varchar(50)     NULL,
    NavigationType         varchar(50)     NULL,
    Url                    varchar(200)    NULL,
    ChannelId              int             NULL,
    ContentId              int             NULL,
    KeywordType            varchar(50)     NULL,
    FunctionId             int             NULL,
    ParentId               int             NULL,
    Taxis                  int             NULL,
    CONSTRAINT PK_wx_WebMenu PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_Wifi(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    KeywordId              int              NULL,
    PvCount                int              NULL,
    IsDisabled             varchar(18)      NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    Summary                nvarchar(255)    NULL,
    BusinessId             varchar(100)     NULL,
    CallBackString         ntext            NULL,
    CONSTRAINT PK_wx_Wifi PRIMARY KEY NONCLUSTERED (Id)
)
GO



CREATE TABLE wx_WifiNode(
    Id                     int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    BusinessId             varchar(100)    NULL,
    NodeId                 varchar(100)    NULL,
    CallBackString         ntext           NULL,
    CONSTRAINT PK_wx_WifiNode PRIMARY KEY NONCLUSTERED (Id)
)
GO
