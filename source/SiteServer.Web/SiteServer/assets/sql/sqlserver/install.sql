/*
 * ER/Studio 8.0 SQL Code Generation
 * Company :      BaiRong Software
 * Project :      BaiRong Software Fundation Tables
 * Author :       BaiRong Software
 *
 * Date Created : Tuesday, March 28, 2017 11:48:51
 * Target DBMS : Microsoft SQL Server 2000
 */

CREATE TABLE bairong_Administrator(
    UserName                         nvarchar(255)    NOT NULL,
    Password                         nvarchar(255)    NULL,
    PasswordFormat                   varchar(50)      NULL,
    PasswordSalt                     nvarchar(128)    NULL,
    CreationDate                     datetime         NULL,
    LastActivityDate                 datetime         NULL,
    CountOfLogin                     int              NULL,
    CountOfFailedLogin               int              NULL,
    CreatorUserName                  nvarchar(255)    NULL,
    IsLockedOut                      varchar(18)      NULL,
    PublishmentSystemIdCollection    varchar(50)      NULL,
    PublishmentSystemId              int              NULL,
    DepartmentId                     int              NULL,
    AreaId                           int              NULL,
    DisplayName                      nvarchar(50)     NULL,
    Email                            nvarchar(50)     NULL,
    Mobile                           varchar(20)      NULL,
    CONSTRAINT PK_bairong_Administrator PRIMARY KEY NONCLUSTERED (UserName)
)
go



CREATE TABLE bairong_AdministratorsInRoles(
    RoleName    nvarchar(255)    NOT NULL,
    UserName    nvarchar(255)    NOT NULL,
    CONSTRAINT PK_bairong_AInR PRIMARY KEY NONCLUSTERED (RoleName, UserName)
)
go



CREATE TABLE bairong_Area(
    AreaId           int              IDENTITY(1,1),
    AreaName         nvarchar(255)    NULL,
    ParentId         int              NULL,
    ParentsPath      nvarchar(255)    NULL,
    ParentsCount     int              NULL,
    ChildrenCount    int              NULL,
    IsLastNode       varchar(18)      NULL,
    Taxis            int              NULL,
    CountOfAdmin     int              NULL,
    CONSTRAINT PK_bairong_Area PRIMARY KEY NONCLUSTERED (AreaId)
)
go



CREATE TABLE bairong_Config(
    IsInitialized           varchar(18)      NULL,
    DatabaseVersion         varchar(50)      NULL,
    RestrictionBlackList    nvarchar(255)    NULL,
    RestrictionWhiteList    nvarchar(255)    NULL,
    UpdateDate              datetime         NULL,
    UserConfig              ntext            NULL,
    SystemConfig            ntext            NULL
)
go



CREATE TABLE bairong_ContentCheck(
    CheckId                int              IDENTITY(1,1),
    TableName              varchar(50)      NULL,
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentId              int              NULL,
    IsAdmin                varchar(18)      NULL,
    UserName               nvarchar(255)    NULL,
    IsChecked              varchar(18)      NULL,
    CheckedLevel           int              NULL,
    CheckDate              datetime         NULL,
    Reasons                nvarchar(255)    NULL,
    CONSTRAINT PK_bairong_ContentCheck PRIMARY KEY NONCLUSTERED (CheckId)
)
go



CREATE TABLE bairong_ContentModel(
    ModelId        varchar(50)      NOT NULL,
    SiteId         int              NOT NULL,
    ModelName      nvarchar(50)     NULL,
    IsSystem       varchar(18)      NULL,
    TableName      varchar(200)     NULL,
    TableType      varchar(50)      NULL,
    IconUrl        varchar(50)      NULL,
    Description    nvarchar(255)    NULL,
    CONSTRAINT PK_bairong_ContentModel PRIMARY KEY NONCLUSTERED (ModelId, SiteId)
)
go



CREATE TABLE bairong_Count(
    CountId             int              IDENTITY(1,1),
    RelatedTableName    nvarchar(255)    NULL,
    RelatedIdentity     nvarchar(255)    NULL,
    CountType           varchar(50)      NULL,
    CountNum            int              NULL,
    CONSTRAINT PK_bairong_Count PRIMARY KEY CLUSTERED (CountId)
)
go



CREATE TABLE bairong_DbCache(
    Id            int              IDENTITY(1,1),
    CacheKey      varchar(200)     NULL,
    CacheValue    nvarchar(500)    NULL,
    AddDate       datetime         NULL,
    CONSTRAINT PK_bairong_DbCache PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE bairong_Department(
    DepartmentId      int              IDENTITY(1,1),
    DepartmentName    nvarchar(255)    NULL,
    Code              varchar(50)      NULL,
    ParentId          int              NULL,
    ParentsPath       nvarchar(255)    NULL,
    ParentsCount      int              NULL,
    ChildrenCount     int              NULL,
    IsLastNode        varchar(18)      NULL,
    Taxis             int              NULL,
    AddDate           datetime         NULL,
    Summary           nvarchar(255)    NULL,
    CountOfAdmin      int              NULL,
    CONSTRAINT PK_bairong_Department PRIMARY KEY NONCLUSTERED (DepartmentId)
)
go



CREATE TABLE bairong_Digg(
    DiggId                 int    IDENTITY(1,1),
    PublishmentSystemId    int    NULL,
    RelatedIdentity        int    NULL,
    Good                   int    NULL,
    Bad                    int    NULL,
    CONSTRAINT PK_bairong_Digg PRIMARY KEY NONCLUSTERED (DiggId)
)
go



CREATE TABLE bairong_ErrorLog(
    Id            int              IDENTITY(1,1),
    AddDate       datetime         NULL,
    Message       nvarchar(255)    NULL,
    Stacktrace    ntext            NULL,
    Summary       ntext            NULL,
    CONSTRAINT PK_bairong_ErrorLog PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE bairong_Log(
    Id           int              IDENTITY(1,1),
    UserName     varchar(50)      NULL,
    IpAddress    varchar(50)      NULL,
    AddDate      datetime         NULL,
    Action       nvarchar(255)    NULL,
    Summary      nvarchar(255)    NULL,
    CONSTRAINT PK_bairong_Log PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE bairong_PermissionsInRoles(
    RoleName              nvarchar(255)    NOT NULL,
    GeneralPermissions    text             NULL,
    CONSTRAINT PK_bairong_GPInR PRIMARY KEY CLUSTERED (RoleName)
)
go



CREATE TABLE bairong_Roles(
    RoleName           nvarchar(255)    NOT NULL,
    CreatorUserName    nvarchar(255)    NULL,
    Description        nvarchar(255)    NULL,
    CONSTRAINT PK_bairong_Roles PRIMARY KEY NONCLUSTERED (RoleName)
)
go



CREATE TABLE bairong_TableCollection(
    TableEnName                  varchar(50)     NOT NULL,
    TableCnName                  nvarchar(50)    NULL,
    AttributeNum                 int             NULL,
    AuxiliaryTableType           varchar(50)     NULL,
    IsCreatedInDb                varchar(18)     NULL,
    IsChangedAfterCreatedInDb    varchar(18)     NULL,
    IsDefault                    varchar(18)     NULL,
    Description                  ntext           NULL,
    CONSTRAINT PK_bairong_AT PRIMARY KEY CLUSTERED (TableEnName)
)
go



CREATE TABLE bairong_TableMatch(
    TableMatchId               int             IDENTITY(1,1),
    ConnectionString           varchar(200)    NULL,
    TableName                  varchar(200)    NULL,
    ConnectionStringToMatch    varchar(200)    NULL,
    TableNameToMatch           varchar(200)    NULL,
    ColumnsMap                 ntext           NULL,
    CONSTRAINT PK_bairong_TableMatch PRIMARY KEY CLUSTERED (TableMatchId)
)
go



CREATE TABLE bairong_TableMetadata(
    TableMetadataId         int            IDENTITY(1,1),
    AuxiliaryTableEnName    varchar(50)    NOT NULL,
    AttributeName           varchar(50)    NULL,
    DataType                varchar(50)    NULL,
    DataLength              int            NULL,
    Taxis                   int            NULL,
    IsSystem                varchar(18)    NULL,
    CONSTRAINT PK_bairong_ATM PRIMARY KEY CLUSTERED (TableMetadataId)
)
go



CREATE TABLE bairong_TableStyle(
    TableStyleId       int              IDENTITY(1,1),
    RelatedIdentity    int              NULL,
    TableName          varchar(50)      NULL,
    AttributeName      varchar(50)      NULL,
    Taxis              int              NULL,
    DisplayName        nvarchar(255)    NULL,
    HelpText           varchar(255)     NULL,
    IsVisible          varchar(18)      NULL,
    IsVisibleInList    varchar(18)      NULL,
    IsSingleLine       varchar(18)      NULL,
    InputType          varchar(50)      NULL,
    IsRequired         varchar(18)      NULL,
    DefaultValue       varchar(255)     NULL,
    IsHorizontal       varchar(18)      NULL,
    ExtendValues       ntext            NULL,
    CONSTRAINT PK_bairong_ATS PRIMARY KEY NONCLUSTERED (TableStyleId)
)
go



CREATE TABLE bairong_TableStyleItem(
    TableStyleItemId    int             IDENTITY(1,1),
    TableStyleId        int             NOT NULL,
    ItemTitle           varchar(255)    NULL,
    ItemValue           varchar(255)    NULL,
    IsSelected          varchar(18)     NULL,
    CONSTRAINT PK_bairong_STSI PRIMARY KEY NONCLUSTERED (TableStyleItemId)
)
go



CREATE TABLE bairong_Tags(
    TagId                  int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ContentIdCollection    nvarchar(255)    NULL,
    Tag                    nvarchar(255)    NULL,
    UseNum                 int              NULL,
    CONSTRAINT PK_bairong_Tags PRIMARY KEY NONCLUSTERED (TagId)
)
go



CREATE TABLE bairong_ThirdLogin(
    Id                int              IDENTITY(1,1),
    ThirdLoginType    varchar(50)      NULL,
    ThirdLoginName    nvarchar(50)     NULL,
    IsEnabled         varchar(18)      NULL,
    Taxis             int              NULL,
    Description       nvarchar(255)    NULL,
    SettingsXml       ntext            NULL,
    CONSTRAINT PK_bairong_ThirdLogin PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE bairong_UserBinding(
    UserId              int              NOT NULL,
    ThirdLoginType      varchar(50)      NULL,
    ThirdLoginUserId    nvarchar(200)    NULL,
    CONSTRAINT PK_bairong_UserBinding PRIMARY KEY NONCLUSTERED (UserId)
)
go



CREATE TABLE bairong_UserGroup(
    GroupId         int              IDENTITY(1,1),
    GroupName       nvarchar(50)     NULL,
    IsDefault       varchar(18)      NULL,
    Description     nvarchar(255)    NULL,
    ExtendValues    ntext            NULL,
    CONSTRAINT PK_bairong_UserGroup PRIMARY KEY NONCLUSTERED (GroupId)
)
go



CREATE TABLE bairong_UserLog(
    Id           int              IDENTITY(1,1),
    UserName     varchar(50)      NULL,
    IpAddress    varchar(50)      NULL,
    AddDate      datetime         NULL,
    Action       nvarchar(255)    NULL,
    Summary      nvarchar(255)    NULL,
    CONSTRAINT PK_bairong_UserLog PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE bairong_Users(
    UserId                   int              IDENTITY(1,1),
    UserName                 nvarchar(255)    NULL,
    Password                 nvarchar(255)    NULL,
    PasswordFormat           varchar(50)      NULL,
    PasswordSalt             nvarchar(128)    NULL,
    GroupId                  int              NULL,
    CreateDate               datetime         NULL,
    LastResetPasswordDate    datetime         NULL,
    LastActivityDate         datetime         NULL,
    CountOfLogin             int              NULL,
    CountOfFailedLogin       int              NULL,
    CountOfWriting           int              NULL,
    IsChecked                varchar(18)      NULL,
    IsLockedOut              varchar(18)      NULL,
    DisplayName              nvarchar(255)    NULL,
    Email                    nvarchar(255)    NULL,
    Mobile                   varchar(20)      NULL,
    AvatarUrl                varchar(200)     NULL,
    Organization             nvarchar(255)    NULL,
    Department               nvarchar(255)    NULL,
    Position                 nvarchar(255)    NULL,
    Gender                   nvarchar(50)     NULL,
    Birthday                 varchar(50)      NULL,
    Education                nvarchar(255)    NULL,
    Graduation               nvarchar(255)    NULL,
    Address                  nvarchar(255)    NULL,
    WeiXin                   nvarchar(255)    NULL,
    Qq                       varchar(50)      NULL,
    WeiBo                    nvarchar(255)    NULL,
    Interests                nvarchar(255)    NULL,
    Signature                nvarchar(255)    NULL,
    ExtendValues             ntext            NULL,
    CONSTRAINT PK_bairong_Users PRIMARY KEY NONCLUSTERED (UserId),
    UNIQUE (UserName)
)
go



CREATE TABLE siteserver_AdArea(
    AdAreaId               int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    AdAreaName             nvarchar(50)     NULL,
    Width                  int              NULL,
    Height                 int              NULL,
    Summary                nvarchar(255)    NULL,
    IsEnabled              varchar(18)      NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_siteserver_AdArea PRIMARY KEY NONCLUSTERED (AdAreaId)
)
go



CREATE TABLE siteserver_AdMaterial(
    AdMaterialId           int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    AdvId                  int              NOT NULL,
    AdvertId               int              NULL,
    AdMaterialName         nvarchar(50)     NULL,
    AdMaterialType         varchar(50)      NULL,
    Code                   ntext            NULL,
    TextWord               nvarchar(255)    NULL,
    TextLink               varchar(200)     NULL,
    TextColor              varchar(10)      NULL,
    TextFontSize           int              NULL,
    ImageUrl               varchar(200)     NULL,
    ImageLink              varchar(200)     NULL,
    ImageWidth             int              NULL,
    ImageHeight            int              NULL,
    ImageAlt               nvarchar(50)     NULL,
    Weight                 int              NULL,
    IsEnabled              varchar(18)      NULL,
    CONSTRAINT PK_siteserver_AdMaterial PRIMARY KEY NONCLUSTERED (AdMaterialId)
)
go



CREATE TABLE siteserver_Adv(
    AdvId                        int               IDENTITY(1,1),
    PublishmentSystemId          int               NULL,
    AdAreaId                     int               NOT NULL,
    AdvName                      nvarchar(50)      NULL,
    Summary                      nvarchar(255)     NULL,
    IsEnabled                    varchar(18)       NULL,
    IsDateLimited                varchar(18)       NULL,
    StartDate                    datetime          NULL,
    EndDate                      datetime          NULL,
    LevelType                    nvarchar(50)      NULL,
    Level                        int               NULL,
    IsWeight                     varchar(18)       NULL,
    Weight                       int               NULL,
    RotateType                   nvarchar(50)      NULL,
    RotateInterval               int               NULL,
    NodeIdCollectionToChannel    nvarchar(4000)    NULL,
    NodeIdCollectionToContent    nvarchar(4000)    NULL,
    FileTemplateIdCollection     nvarchar(4000)    NULL,
    CONSTRAINT PK_siteserver_Adv PRIMARY KEY NONCLUSTERED (AdvId)
)
go



CREATE TABLE siteserver_Advertisement(
    AdvertisementName            varchar(50)      NOT NULL,
    PublishmentSystemId          int              NOT NULL,
    AdvertisementType            varchar(50)      NULL,
    NavigationUrl                varchar(200)     NULL,
    IsDateLimited                varchar(18)      NULL,
    StartDate                    datetime         NULL,
    EndDate                      datetime         NULL,
    AddDate                      datetime         NULL,
    NodeIdCollectionToChannel    nvarchar(255)    NULL,
    NodeIdCollectionToContent    nvarchar(255)    NULL,
    FileTemplateIdCollection     nvarchar(255)    NULL,
    Settings                     ntext            NULL,
    CONSTRAINT PK_siteserver_Advertisement PRIMARY KEY CLUSTERED (AdvertisementName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_AdvImageClassify(
    ItemId                 int             IDENTITY(1,1),
    ItemName               nvarchar(50)    NULL,
    ItemIndexName          nvarchar(50)    NULL,
    ParentId               int             NULL,
    ParentsPath            varchar(255)    NULL,
    ParentsCount           int             NULL,
    ChildrenCount          int             NULL,
    ContentNum             int             NULL,
    PublishmentSystemId    int             NULL,
    Enabled                varchar(18)     NULL,
    IsLastItem             varchar(18)     NULL,
    Taxis                  int             NULL,
    AddDate                datetime        NULL,
    CONSTRAINT PK_siteserver_AdvImageClassify PRIMARY KEY NONCLUSTERED (ItemId)
)
go



CREATE TABLE siteserver_AdvImageContent(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    Taxis                  int              NULL,
    IsChecked              varchar(18)      NULL,
    AddUser                nvarchar(50)     NULL,
    AddDate                datetime         NULL,
    LaseEditUser           varchar(50)      NULL,
    LastEditDate           datetime         NULL,
    SettingsXml            ntext            NULL,
    AdvImageName           nvarchar(50)     NULL,
    Description            nvarchar(255)    NULL,
    ClassifyId             int              NULL,
    AdvImagePath           nvarchar(500)    NULL,
    CONSTRAINT PK_siteserver_AdvImageContent PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_Comment(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentId              int              NULL,
    GoodCount              int              NULL,
    UserName               nvarchar(50)     NULL,
    IsChecked              varchar(18)      NULL,
    AddDate                datetime         NULL,
    Content                nvarchar(500)    NULL,
    CONSTRAINT PK_siteserver_Comment PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_ContentGroup(
    ContentGroupName       nvarchar(255)    NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    Taxis                  int              NULL,
    Description            ntext            NULL,
    CONSTRAINT PK_siteserver_ContentGroup PRIMARY KEY CLUSTERED (ContentGroupName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_CreateTask(
    Id                     int            IDENTITY(1,1),
    CreateType             varchar(50)    NULL,
    PublishmentSystemId    int            NULL,
    ChannelId              int            NULL,
    ContentId              int            NULL,
    TemplateId             int            NULL,
    CONSTRAINT PK_siteserver_CreateTask PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_CreateTaskLog(
    Id                     int              IDENTITY(1,1),
    CreateType             varchar(50)      NULL,
    PublishmentSystemId    int              NULL,
    TaskName               nvarchar(50)     NULL,
    TimeSpan               nvarchar(50)     NULL,
    IsSuccess              varchar(18)      NULL,
    ErrorMessage           nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_siteserver_CreateTaskLog PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_GatherDatabaseRule(
    GatherRuleName         nvarchar(50)     NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    ConnectionString       varchar(255)     NULL,
    RelatedTableName       varchar(255)     NULL,
    RelatedIdentity        varchar(255)     NULL,
    RelatedOrderBy         varchar(255)     NULL,
    WhereString            nvarchar(255)    NULL,
    TableMatchId           int              NULL,
    NodeId                 int              NULL,
    GatherNum              int              NULL,
    IsChecked              varchar(18)      NULL,
    IsAutoCreate           varchar(18)      NULL,
    IsOrderByDesc          varchar(18)      NULL,
    LastGatherDate         datetime         NULL,
    CONSTRAINT PK_siteserver_GatherDR PRIMARY KEY CLUSTERED (GatherRuleName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_GatherFileRule(
    GatherRuleName                   nvarchar(50)     NOT NULL,
    PublishmentSystemId              int              NOT NULL,
    GatherUrl                        nvarchar(255)    NULL,
    Charset                          varchar(50)      NULL,
    LastGatherDate                   datetime         NULL,
    IsToFile                         varchar(18)      NULL,
    FilePath                         nvarchar(255)    NULL,
    IsSaveRelatedFiles               varchar(18)      NULL,
    IsRemoveScripts                  varchar(18)      NULL,
    StyleDirectoryPath               nvarchar(255)    NULL,
    ScriptDirectoryPath              nvarchar(255)    NULL,
    ImageDirectoryPath               nvarchar(255)    NULL,
    NodeId                           int              NULL,
    IsSaveImage                      varchar(18)      NULL,
    IsChecked                        varchar(18)      NULL,
    IsAutoCreate                     varchar(18)      NULL,
    ContentExclude                   ntext            NULL,
    ContentHtmlClearCollection       nvarchar(255)    NULL,
    ContentHtmlClearTagCollection    nvarchar(255)    NULL,
    ContentTitleStart                ntext            NULL,
    ContentTitleEnd                  ntext            NULL,
    ContentContentStart              ntext            NULL,
    ContentContentEnd                ntext            NULL,
    ContentAttributes                ntext            NULL,
    ContentAttributesXml             ntext            NULL,
    CONSTRAINT PK_siteserver_GatherFileRule PRIMARY KEY NONCLUSTERED (GatherRuleName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_GatherRule(
    GatherRuleName                   nvarchar(50)     NOT NULL,
    PublishmentSystemId              int              NOT NULL,
    CookieString                     text             NULL,
    GatherUrlIsCollection            varchar(18)      NULL,
    GatherUrlCollection              text             NULL,
    GatherUrlIsSerialize             varchar(18)      NULL,
    GatherUrlSerialize               varchar(200)     NULL,
    SerializeFrom                    int              NULL,
    SerializeTo                      int              NULL,
    SerializeInterval                int              NULL,
    SerializeIsOrderByDesc           varchar(18)      NULL,
    SerializeIsAddZero               varchar(18)      NULL,
    NodeId                           int              NULL,
    Charset                          varchar(50)      NULL,
    UrlInclude                       varchar(200)     NULL,
    TitleInclude                     nvarchar(255)    NULL,
    ContentExclude                   ntext            NULL,
    ContentHtmlClearCollection       nvarchar(255)    NULL,
    ContentHtmlClearTagCollection    nvarchar(255)    NULL,
    LastGatherDate                   datetime         NULL,
    ListAreaStart                    ntext            NULL,
    ListAreaEnd                      ntext            NULL,
    ContentChannelStart              ntext            NULL,
    ContentChannelEnd                ntext            NULL,
    ContentTitleStart                ntext            NULL,
    ContentTitleEnd                  ntext            NULL,
    ContentContentStart              ntext            NULL,
    ContentContentEnd                ntext            NULL,
    ContentNextPageStart             ntext            NULL,
    ContentNextPageEnd               ntext            NULL,
    ContentAttributes                ntext            NULL,
    ContentAttributesXml             ntext            NULL,
    ExtendValues                     ntext            NULL,
    CONSTRAINT PK_siteserver_GatherRule PRIMARY KEY CLUSTERED (GatherRuleName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_InnerLink(
    InnerLinkName          nvarchar(255)    NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    LinkUrl                varchar(200)     NULL,
    CONSTRAINT PK_siteserver_InnerLink PRIMARY KEY NONCLUSTERED (InnerLinkName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_Input(
    InputId                int             IDENTITY(1,1),
    InputName              nvarchar(50)    NULL,
    PublishmentSystemId    int             NULL,
    AddDate                datetime        NULL,
    IsChecked              varchar(18)     NULL,
    IsReply                varchar(18)     NULL,
    Taxis                  int             NULL,
    SettingsXml            ntext           NULL,
    CONSTRAINT PK_siteserver_Input PRIMARY KEY NONCLUSTERED (InputId)
)
go



CREATE TABLE siteserver_InputContent(
    Id             int              IDENTITY(1,1),
    InputId        int              NOT NULL,
    Taxis          int              NULL,
    IsChecked      varchar(18)      NULL,
    UserName       nvarchar(255)    NULL,
    IpAddress      varchar(50)      NULL,
    AddDate        datetime         NULL,
    Reply          ntext            NULL,
    SettingsXml    ntext            NULL,
    CONSTRAINT PK_siteserver_InputContent PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_Keyword(
    KeywordId      int             IDENTITY(1,1),
    Keyword        nvarchar(50)    NULL,
    Alternative    nvarchar(50)    NULL,
    Grade          nvarchar(50)    NULL,
    CONSTRAINT PK_siteserver_Keyword PRIMARY KEY NONCLUSTERED (KeywordId)
)
go



CREATE TABLE siteserver_Log(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ChannelId              int              NULL,
    ContentId              int              NULL,
    UserName               varchar(50)      NULL,
    IpAddress              varchar(50)      NULL,
    AddDate                datetime         NULL,
    Action                 nvarchar(255)    NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_siteserver_Log PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_MenuDisplay(
    MenuDisplayId          int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    MenuDisplayName        varchar(50)     NULL,
    Vertical               varchar(50)     NULL,
    FontFamily             varchar(200)    NULL,
    FontSize               int             NULL,
    FontWeight             varchar(50)     NULL,
    FontStyle              varchar(50)     NULL,
    MenuItemHAlign         varchar(50)     NULL,
    MenuItemVAlign         varchar(50)     NULL,
    FontColor              varchar(50)     NULL,
    MenuItemBgColor        varchar(50)     NULL,
    FontColorHilite        varchar(50)     NULL,
    MenuHiliteBgColor      varchar(50)     NULL,
    XPosition              varchar(50)     NULL,
    YPosition              varchar(50)     NULL,
    HideOnMouseOut         varchar(50)     NULL,
    MenuWidth              int             NULL,
    MenuItemHeight         int             NULL,
    MenuItemPadding        int             NULL,
    MenuItemSpacing        int             NULL,
    MenuItemIndent         int             NULL,
    HideTimeout            int             NULL,
    MenuBgOpaque           varchar(50)     NULL,
    MenuBorder             int             NULL,
    BgColor                varchar(50)     NULL,
    MenuBorderBgColor      varchar(50)     NULL,
    MenuLiteBgColor        varchar(50)     NULL,
    ChildMenuIcon          varchar(200)    NULL,
    AddDate                datetime        NULL,
    IsDefault              varchar(18)     NULL,
    Description            ntext           NULL,
    CONSTRAINT PK_siteserver_MenuDisplay PRIMARY KEY CLUSTERED (MenuDisplayId)
)
go



CREATE TABLE siteserver_Node(
    NodeId                     int              IDENTITY(1,1),
    NodeName                   nvarchar(255)    NULL,
    NodeType                   varchar(50)      NULL,
    PublishmentSystemId        int              NULL,
    ContentModelId             varchar(50)      NULL,
    ParentId                   int              NULL,
    ParentsPath                nvarchar(255)    NULL,
    ParentsCount               int              NULL,
    ChildrenCount              int              NULL,
    IsLastNode                 varchar(18)      NULL,
    NodeIndexName              nvarchar(255)    NULL,
    NodeGroupNameCollection    nvarchar(255)    NULL,
    Taxis                      int              NULL,
    AddDate                    datetime         NULL,
    ImageUrl                   varchar(200)     NULL,
    Content                    ntext            NULL,
    ContentNum                 int              NULL,
    FilePath                   varchar(200)     NULL,
    ChannelFilePathRule        varchar(200)     NULL,
    ContentFilePathRule        varchar(200)     NULL,
    LinkUrl                    varchar(200)     NULL,
    LinkType                   varchar(200)     NULL,
    ChannelTemplateId          int              NULL,
    ContentTemplateId          int              NULL,
    Keywords                   nvarchar(255)    NULL,
    Description                nvarchar(255)    NULL,
    ExtendValues               ntext            NULL,
    CONSTRAINT PK_siteserver_Node PRIMARY KEY NONCLUSTERED (NodeId)
)
go



CREATE TABLE siteserver_NodeGroup(
    NodeGroupName          nvarchar(255)    NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    Taxis                  int              NULL,
    Description            ntext            NULL,
    CONSTRAINT PK_siteserver_NodeGroup PRIMARY KEY CLUSTERED (NodeGroupName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_Photo(
    Id                     int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    ContentId              int             NULL,
    SmallUrl               varchar(200)    NULL,
    MiddleUrl              varchar(200)    NULL,
    LargeUrl               varchar(200)    NULL,
    Taxis                  int             NULL,
    Description            varchar(255)    NULL,
    CONSTRAINT PK_siteserver_Photo PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_PublishmentSystem(
    PublishmentSystemId             int             NOT NULL,
    PublishmentSystemName           nvarchar(50)    NULL,
    PublishmentSystemType           varchar(50)     NULL,
    AuxiliaryTableForContent        varchar(50)     NULL,
    AuxiliaryTableForGovPublic      varchar(50)     NULL,
    AuxiliaryTableForGovInteract    varchar(50)     NULL,
    AuxiliaryTableForVote           varchar(50)     NULL,
    AuxiliaryTableForJob            varchar(50)     NULL,
    IsCheckContentUseLevel          varchar(18)     NULL,
    CheckContentLevel               int             NULL,
    PublishmentSystemDir            varchar(50)     NULL,
    PublishmentSystemUrl            varchar(200)    NULL,
    IsHeadquarters                  varchar(18)     NULL,
    ParentPublishmentSystemId       int             NULL,
    Taxis                           int             NULL,
    SettingsXml                     ntext           NULL,
    CONSTRAINT PK_siteserver_PS PRIMARY KEY CLUSTERED (PublishmentSystemId)
)
go



CREATE TABLE siteserver_RelatedField(
    RelatedFieldId         int              IDENTITY(1,1),
    RelatedFieldName       nvarchar(50)     NULL,
    PublishmentSystemId    int              NULL,
    TotalLevel             int              NULL,
    Prefixes               nvarchar(255)    NULL,
    Suffixes               nvarchar(255)    NULL,
    CONSTRAINT PK_siteserver_RelatedField PRIMARY KEY NONCLUSTERED (RelatedFieldId)
)
go



CREATE TABLE siteserver_RelatedFieldItem(
    Id                int              IDENTITY(1,1),
    RelatedFieldId    int              NOT NULL,
    ItemName          nvarchar(255)    NULL,
    ItemValue         nvarchar(255)    NULL,
    ParentId          int              NULL,
    Taxis             int              NULL,
    CONSTRAINT PK_siteserver_RelatedFieldItem PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_ResumeContent(
    Id                       int              IDENTITY(1,1),
    PublishmentSystemId      int              NULL,
    JobContentId             int              NULL,
    UserName                 nvarchar(255)    NULL,
    IsView                   varchar(18)      NULL,
    AddDate                  datetime         NULL,
    RealName                 nvarchar(50)     NULL,
    Nationality              nvarchar(50)     NULL,
    Gender                   nvarchar(50)     NULL,
    Email                    varchar(50)      NULL,
    MobilePhone              varchar(50)      NULL,
    HomePhone                varchar(50)      NULL,
    LastSchoolName           nvarchar(50)     NULL,
    Education                nvarchar(50)     NULL,
    IdCardType               nvarchar(50)     NULL,
    IdCardNo                 varchar(50)      NULL,
    Birthday                 varchar(50)      NULL,
    Marriage                 nvarchar(50)     NULL,
    WorkYear                 nvarchar(50)     NULL,
    Profession               nvarchar(50)     NULL,
    ExpectSalary             nvarchar(50)     NULL,
    AvailabelTime            nvarchar(50)     NULL,
    Location                 nvarchar(50)     NULL,
    ImageUrl                 varchar(200)     NULL,
    Summary                  nvarchar(255)    NULL,
    Exp_Count                int              NULL,
    Exp_FromYear             nvarchar(50)     NULL,
    Exp_FromMonth            nvarchar(50)     NULL,
    Exp_ToYear               nvarchar(50)     NULL,
    Exp_ToMonth              nvarchar(50)     NULL,
    Exp_EmployerName         nvarchar(255)    NULL,
    Exp_Department           nvarchar(255)    NULL,
    Exp_EmployerPhone        nvarchar(255)    NULL,
    Exp_WorkPlace            nvarchar(255)    NULL,
    Exp_PositionTitle        nvarchar(255)    NULL,
    Exp_Industry             nvarchar(255)    NULL,
    Exp_Summary              ntext            NULL,
    Exp_Score                ntext            NULL,
    Pro_Count                int              NULL,
    Pro_FromYear             nvarchar(50)     NULL,
    Pro_FromMonth            nvarchar(50)     NULL,
    Pro_ToYear               nvarchar(50)     NULL,
    Pro_ToMonth              nvarchar(50)     NULL,
    Pro_ProjectName          nvarchar(255)    NULL,
    Pro_Summary              ntext            NULL,
    Edu_Count                int              NULL,
    Edu_FromYear             nvarchar(50)     NULL,
    Edu_FromMonth            nvarchar(50)     NULL,
    Edu_ToYear               nvarchar(50)     NULL,
    Edu_ToMonth              nvarchar(50)     NULL,
    Edu_SchoolName           nvarchar(255)    NULL,
    Edu_Education            nvarchar(255)    NULL,
    Edu_Profession           nvarchar(255)    NULL,
    Edu_Summary              ntext            NULL,
    Tra_Count                int              NULL,
    Tra_FromYear             nvarchar(50)     NULL,
    Tra_FromMonth            nvarchar(50)     NULL,
    Tra_ToYear               nvarchar(50)     NULL,
    Tra_ToMonth              nvarchar(50)     NULL,
    Tra_TrainerName          nvarchar(255)    NULL,
    Tra_TrainerAddress       nvarchar(255)    NULL,
    Tra_Lesson               nvarchar(255)    NULL,
    Tra_Centification        nvarchar(255)    NULL,
    Tra_Summary              nvarchar(255)    NULL,
    Lan_Count                int              NULL,
    Lan_Language             nvarchar(255)    NULL,
    Lan_Level                nvarchar(255)    NULL,
    Ski_Count                int              NULL,
    Ski_SkillName            nvarchar(255)    NULL,
    Ski_UsedTimes            nvarchar(255)    NULL,
    Ski_Ability              nvarchar(255)    NULL,
    Cer_Count                int              NULL,
    Cer_CertificationName    nvarchar(255)    NULL,
    Cer_EffectiveDate        nvarchar(255)    NULL,
    CONSTRAINT PK_siteserver_ResumeContent PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_SeoMeta(
    SeoMetaId              int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    SeoMetaName            varchar(50)      NULL,
    IsDefault              varchar(18)      NULL,
    PageTitle              nvarchar(80)     NULL,
    Keywords               nvarchar(100)    NULL,
    Description            nvarchar(200)    NULL,
    Copyright              nvarchar(255)    NULL,
    Author                 nvarchar(50)     NULL,
    Email                  nvarchar(50)     NULL,
    Language               varchar(50)      NULL,
    Charset                varchar(50)      NULL,
    Distribution           varchar(50)      NULL,
    Rating                 varchar(50)      NULL,
    Robots                 varchar(50)      NULL,
    RevisitAfter           varchar(50)      NULL,
    Expires                varchar(50)      NULL,
    CONSTRAINT PK_siteserver_SeoMeta PRIMARY KEY CLUSTERED (SeoMetaId)
)
go



CREATE TABLE siteserver_SeoMetasInNodes(
    NodeId                 int            NOT NULL,
    IsChannel              varchar(18)    NOT NULL,
    SeoMetaId              int            NOT NULL,
    PublishmentSystemId    int            NULL,
    CONSTRAINT PK_siteserver_SeoMetasInNodes PRIMARY KEY CLUSTERED (NodeId, IsChannel, SeoMetaId)
)
go



CREATE TABLE siteserver_SigninLog(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ContentId              int              NULL,
    UserName               nvarchar(255)    NULL,
    IsSignin               varchar(18)      NULL,
    SigninDate             datetime         NULL,
    IpAddress              varchar(50)      NULL,
    CONSTRAINT PK_siteserver_SigninLog PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE siteserver_SigninSetting(
    Id                     int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentId              int              NULL,
    IsGroup                varchar(18)      NULL,
    UserGroupCollection    varchar(500)     NULL,
    UserNameCollection     nvarchar(500)    NULL,
    Priority               int              NULL,
    EndDate                varchar(50)      NULL,
    IsSignin               varchar(18)      NULL,
    SigninDate             datetime         NULL,
    CONSTRAINT PK_siteserver_SigninSetting PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE siteserver_SigninUserContentId(
    Id                     int              IDENTITY(1,1),
    IsGroup                varchar(18)      NULL,
    GroupId                int              NULL,
    UserName               nvarchar(255)    NULL,
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentIdCollection    varchar(500)     NULL,
    CONSTRAINT PK_siteserver_SigninUserContentID PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_Star(
    StarId                 int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ChannelId              int              NULL,
    ContentId              int              NULL,
    UserName               nvarchar(255)    NULL,
    Point                  int              NULL,
    Message                nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_siteserver_Star PRIMARY KEY NONCLUSTERED (StarId)
)
go



CREATE TABLE siteserver_StarSetting(
    StarSettingId          int               IDENTITY(1,1),
    PublishmentSystemId    int               NULL,
    ChannelId              int               NULL,
    ContentId              int               NULL,
    TotalCount             int               NULL,
    PointAverage           decimal(18, 1)    NULL,
    CONSTRAINT PK_siteserver_StarSetting PRIMARY KEY NONCLUSTERED (StarSettingId)
)
go



CREATE TABLE siteserver_StlTag(
    TagName                nvarchar(50)     NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    TagDescription         nvarchar(255)    NULL,
    TagContent             ntext            NULL,
    CONSTRAINT PK_siteserver_StlTag PRIMARY KEY NONCLUSTERED (TagName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_SystemPermissions(
    RoleName               nvarchar(255)    NOT NULL,
    PublishmentSystemId    int              NOT NULL,
    NodeIdCollection       text             NULL,
    ChannelPermissions     text             NULL,
    WebsitePermissions     text             NULL,
    CONSTRAINT PK_siteserver_SP PRIMARY KEY CLUSTERED (RoleName, PublishmentSystemId)
)
go



CREATE TABLE siteserver_TagStyle(
    StyleId                int             IDENTITY(1,1),
    StyleName              nvarchar(50)    NULL,
    ElementName            varchar(50)     NULL,
    PublishmentSystemId    int             NULL,
    IsTemplate             varchar(18)     NULL,
    StyleTemplate          ntext           NULL,
    ScriptTemplate         ntext           NULL,
    ContentTemplate        ntext           NULL,
    SuccessTemplate        ntext           NULL,
    FailureTemplate        ntext           NULL,
    SettingsXml            ntext           NULL,
    CONSTRAINT PK_siteserver_TagStyle PRIMARY KEY CLUSTERED (StyleId)
)
go



CREATE TABLE siteserver_Task(
    TaskId                  int              IDENTITY(1,1),
    TaskName                nvarchar(50)     NULL,
    IsSystemTask            varchar(18)      NULL,
    PublishmentSystemId     int              NULL,
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
    CONSTRAINT PK_siteserver_Task PRIMARY KEY NONCLUSTERED (TaskId)
)
go



CREATE TABLE siteserver_TaskLog(
    Id              int              IDENTITY(1,1),
    TaskId          int              NOT NULL,
    IsSuccess       varchar(18)      NULL,
    ErrorMessage    nvarchar(255)    NULL,
    AddDate         datetime         NULL,
    CONSTRAINT PK_siteserver_TaskLog PRIMARY KEY CLUSTERED (Id)
)
go



CREATE TABLE siteserver_Template(
    TemplateId             int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    TemplateName           nvarchar(50)    NULL,
    TemplateType           varchar(50)     NULL,
    RelatedFileName        nvarchar(50)    NULL,
    CreatedFileFullName    nvarchar(50)    NULL,
    CreatedFileExtName     varchar(50)     NULL,
    Charset                varchar(50)     NULL,
    IsDefault              varchar(18)     NULL,
    CONSTRAINT PK_siteserver_Template PRIMARY KEY CLUSTERED (TemplateId)
)
go



CREATE TABLE siteserver_TemplateLog(
    Id                     int              IDENTITY(1,1),
    TemplateId             int              NULL,
    PublishmentSystemId    int              NULL,
    AddDate                datetime         NULL,
    AddUserName            nvarchar(255)    NULL,
    ContentLength          int              NULL,
    TemplateContent        ntext            NULL,
    CONSTRAINT PK_siteserver_TemplateLog PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE siteserver_TemplateMatch(
    NodeId                 int             NOT NULL,
    PublishmentSystemId    int             NULL,
    ChannelTemplateId      int             NULL,
    ContentTemplateId      int             NULL,
    FilePath               varchar(200)    NULL,
    ChannelFilePathRule    varchar(200)    NULL,
    ContentFilePathRule    varchar(200)    NULL,
    CONSTRAINT PK_siteserver_TemplateMatch PRIMARY KEY NONCLUSTERED (NodeId)
)
go



CREATE TABLE siteserver_Tracking(
    TrackingId             int             IDENTITY(1,1),
    PublishmentSystemId    int             NULL,
    TrackerType            varchar(50)     NULL,
    LastAccessDateTime     datetime        NULL,
    PageUrl                varchar(200)    NULL,
    PageNodeId             int             NULL,
    PageContentId          int             NULL,
    Referrer               varchar(200)    NULL,
    IpAddress              varchar(200)    NULL,
    OperatingSystem        varchar(200)    NULL,
    Browser                varchar(200)    NULL,
    AccessDateTime         datetime        NULL,
    CONSTRAINT PK_siteserver_Tracking PRIMARY KEY CLUSTERED (TrackingId)
)
go



CREATE TABLE siteserver_VoteOperation(
    OperationId            int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentId              int              NULL,
    IpAddress              varchar(50)      NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_siteserver_VoteOperation PRIMARY KEY NONCLUSTERED (OperationId)
)
go



CREATE TABLE siteserver_VoteOption(
    OptionId               int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NULL,
    ContentId              int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    NavigationUrl          varchar(200)     NULL,
    VoteNum                int              NULL,
    CONSTRAINT PK_siteserver_VoteOption PRIMARY KEY NONCLUSTERED (OptionId)
)
go



CREATE TABLE wcm_GovInteractChannel(
    NodeId                    int              NOT NULL,
    PublishmentSystemId       int              NULL,
    ApplyStyleId              int              NULL,
    QueryStyleId              int              NULL,
    DepartmentIdCollection    nvarchar(255)    NULL,
    Summary                   nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovInteractChannel PRIMARY KEY NONCLUSTERED (NodeId)
)
go



CREATE TABLE wcm_GovInteractLog(
    LogId                  int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NOT NULL,
    ContentId              int              NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    LogType                varchar(50)      NULL,
    IpAddress              varchar(50)      NULL,
    AddDate                datetime         NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovInteractLog PRIMARY KEY NONCLUSTERED (LogId)
)
go



CREATE TABLE wcm_GovInteractPermissions(
    UserName       nvarchar(50)     NOT NULL,
    NodeId         int              NOT NULL,
    Permissions    nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovInteractAdministrator PRIMARY KEY NONCLUSTERED (UserName, NodeId)
)
go



CREATE TABLE wcm_GovInteractRemark(
    RemarkId               int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NOT NULL,
    ContentId              int              NULL,
    RemarkType             varchar(50)      NULL,
    Remark                 nvarchar(255)    NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wcm_GovInteractRemark PRIMARY KEY NONCLUSTERED (RemarkId)
)
go



CREATE TABLE wcm_GovInteractReply(
    ReplyId                int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    NodeId                 int              NOT NULL,
    ContentId              int              NULL,
    Reply                  ntext            NULL,
    FileUrl                nvarchar(255)    NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wcm_GovInteractReply PRIMARY KEY NONCLUSTERED (ReplyId)
)
go



CREATE TABLE wcm_GovInteractType(
    TypeId                 int             IDENTITY(1,1),
    TypeName               nvarchar(50)    NULL,
    NodeId                 int             NOT NULL,
    PublishmentSystemId    int             NULL,
    Taxis                  int             NULL,
    CONSTRAINT PK_wcm_GovInteractType PRIMARY KEY NONCLUSTERED (TypeId)
)
go



CREATE TABLE wcm_GovPublicApply(
    Id                     int              IDENTITY(1,1),
    StyleId                int              NULL,
    PublishmentSystemId    int              NULL,
    IsOrganization         varchar(18)      NULL,
    CivicName              nvarchar(255)    NULL,
    CivicOrganization      nvarchar(255)    NULL,
    CivicCardType          nvarchar(255)    NULL,
    CivicCardNo            nvarchar(255)    NULL,
    CivicPhone             varchar(50)      NULL,
    CivicPostCode          varchar(50)      NULL,
    CivicAddress           nvarchar(255)    NULL,
    CivicEmail             nvarchar(255)    NULL,
    CivicFax               varchar(50)      NULL,
    OrgName                nvarchar(255)    NULL,
    OrgUnitCode            nvarchar(255)    NULL,
    OrgLegalPerson         nvarchar(255)    NULL,
    OrgLinkName            nvarchar(255)    NULL,
    OrgPhone               varchar(50)      NULL,
    OrgPostCode            varchar(50)      NULL,
    OrgAddress             nvarchar(255)    NULL,
    OrgEmail               nvarchar(255)    NULL,
    OrgFax                 varchar(50)      NULL,
    Title                  nvarchar(255)    NULL,
    Content                ntext            NULL,
    Purpose                nvarchar(255)    NULL,
    IsApplyFree            varchar(18)      NULL,
    ProvideType            varchar(50)      NULL,
    ObtainType             varchar(50)      NULL,
    DepartmentName         nvarchar(255)    NULL,
    DepartmentId           int              NULL,
    AddDate                datetime         NULL,
    QueryCode              nvarchar(255)    NULL,
    State                  varchar(50)      NULL,
    CONSTRAINT PK_wcm_GovPublicApply PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE wcm_GovPublicApplyLog(
    LogId                  int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ApplyId                int              NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    LogType                varchar(50)      NULL,
    IpAddress              varchar(50)      NULL,
    AddDate                datetime         NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovPublicApplyLog PRIMARY KEY NONCLUSTERED (LogId)
)
go



CREATE TABLE wcm_GovPublicApplyRemark(
    RemarkId               int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ApplyId                int              NULL,
    RemarkType             varchar(50)      NULL,
    Remark                 nvarchar(255)    NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wcm_GovPublicApplyRemark PRIMARY KEY NONCLUSTERED (RemarkId)
)
go



CREATE TABLE wcm_GovPublicApplyReply(
    ReplyId                int              IDENTITY(1,1),
    PublishmentSystemId    int              NULL,
    ApplyId                int              NULL,
    Reply                  ntext            NULL,
    FileUrl                nvarchar(255)    NULL,
    DepartmentId           int              NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_wcm_GovPublicApplyReply PRIMARY KEY NONCLUSTERED (ReplyId)
)
go



CREATE TABLE wcm_GovPublicCategory(
    CategoryId             int              IDENTITY(1,1),
    ClassCode              nvarchar(50)     NULL,
    PublishmentSystemId    int              NULL,
    CategoryName           nvarchar(255)    NULL,
    CategoryCode           varchar(50)      NULL,
    ParentId               int              NULL,
    ParentsPath            nvarchar(255)    NULL,
    ParentsCount           int              NULL,
    ChildrenCount          int              NULL,
    IsLastNode             varchar(18)      NULL,
    Taxis                  int              NULL,
    AddDate                datetime         NULL,
    Summary                nvarchar(255)    NULL,
    ContentNum             int              NULL,
    CONSTRAINT PK_wcm_GovPublicCategory PRIMARY KEY NONCLUSTERED (CategoryId)
)
go



CREATE TABLE wcm_GovPublicCategoryClass(
    ClassCode               nvarchar(50)     NOT NULL,
    PublishmentSystemId     int              NOT NULL,
    ClassName               nvarchar(255)    NULL,
    IsSystem                varchar(18)      NULL,
    IsEnabled               varchar(18)      NULL,
    ContentAttributeName    varchar(50)      NULL,
    Taxis                   int              NULL,
    Description             nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovPublicCategoryClass PRIMARY KEY NONCLUSTERED (ClassCode, PublishmentSystemId)
)
go



CREATE TABLE wcm_GovPublicChannel(
    NodeId                 int              NOT NULL,
    PublishmentSystemId    int              NULL,
    Code                   nvarchar(50)     NULL,
    Summary                nvarchar(255)    NULL,
    CONSTRAINT PK_wcm_GovPublicChannel PRIMARY KEY NONCLUSTERED (NodeId)
)
go



CREATE TABLE wcm_GovPublicIdentifierRule(
    RuleId                 int              IDENTITY(1,1),
    RuleName               nvarchar(255)    NULL,
    PublishmentSystemId    int              NULL,
    IdentifierType         varchar(50)      NULL,
    MinLength              int              NULL,
    Suffix                 varchar(50)      NULL,
    FormatString           varchar(50)      NULL,
    AttributeName          varchar(50)      NULL,
    Sequence               int              NULL,
    Taxis                  int              NULL,
    SettingsXml            ntext            NULL,
    CONSTRAINT PK_wcm_GovPublicIdentifierRule PRIMARY KEY NONCLUSTERED (RuleId)
)
go



CREATE TABLE wcm_GovPublicIdentifierSeq(
    SeqId                  int    IDENTITY(1,1),
    PublishmentSystemId    int    NULL,
    NodeId                 int    NULL,
    DepartmentId           int    NULL,
    AddYear                int    NULL,
    Sequence               int    NULL,
    CONSTRAINT PK_wcm_GovPublicIdentifierSeq PRIMARY KEY NONCLUSTERED (SeqId)
)
go



CREATE CLUSTERED INDEX IX_bairong_DbCache_Key ON bairong_DbCache(CacheKey)
go
CREATE INDEX IX_bairong_TM_ATE ON bairong_TableMetadata(AuxiliaryTableEnName)
go
CREATE CLUSTERED INDEX IX_bairong_TSI_TSI ON bairong_TableStyleItem(TableStyleId)
go
CREATE INDEX IX_birong_UserLog_UserName ON bairong_UserLog(UserName)
go
CREATE CLUSTERED INDEX IX_siteserver_Comment_PSID ON siteserver_Comment(PublishmentSystemId)
go
CREATE INDEX IX_siteserver_Comment_ContentID ON siteserver_Comment(ContentId)
go
CREATE CLUSTERED INDEX IK_siteserver_Node_PSID ON siteserver_Node(PublishmentSystemId)
go
CREATE INDEX IK_siteserver_Node_Taxis ON siteserver_Node(Taxis)
go
CREATE INDEX IX_siteserver_PS_Taxis ON siteserver_PublishmentSystem(Taxis)
go
ALTER TABLE bairong_AdministratorsInRoles ADD CONSTRAINT FK_bairong_AInR_A 
    FOREIGN KEY (UserName)
    REFERENCES bairong_Administrator(UserName) ON DELETE CASCADE ON UPDATE CASCADE
go

ALTER TABLE bairong_AdministratorsInRoles ADD CONSTRAINT FK_bairong_AInR_R 
    FOREIGN KEY (RoleName)
    REFERENCES bairong_Roles(RoleName) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE bairong_TableMetadata ADD CONSTRAINT FK_bairong_ATM_AT 
    FOREIGN KEY (AuxiliaryTableEnName)
    REFERENCES bairong_TableCollection(TableEnName) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE bairong_TableStyleItem ADD CONSTRAINT FK_bairong_ATSI_ATS 
    FOREIGN KEY (TableStyleId)
    REFERENCES bairong_TableStyle(TableStyleId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE siteserver_AdMaterial ADD CONSTRAINT FK_siteserver_Adv_AdMaterial 
    FOREIGN KEY (AdvId)
    REFERENCES siteserver_Adv(AdvId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE siteserver_Adv ADD CONSTRAINT FK_siteserver_AdArea_Adv 
    FOREIGN KEY (AdAreaId)
    REFERENCES siteserver_AdArea(AdAreaId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE siteserver_InputContent ADD CONSTRAINT FK_siteserver_IC_I 
    FOREIGN KEY (InputId)
    REFERENCES siteserver_Input(InputId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE siteserver_RelatedFieldItem ADD CONSTRAINT FK_siteserver_RFI_RF 
    FOREIGN KEY (RelatedFieldId)
    REFERENCES siteserver_RelatedField(RelatedFieldId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE siteserver_TaskLog ADD CONSTRAINT FK_siteserver_Task_Log 
    FOREIGN KEY (TaskId)
    REFERENCES siteserver_Task(TaskId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovInteractLog ADD CONSTRAINT FK_RIC_RIL 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovInteractPermissions ADD CONSTRAINT FK_GIC_GIA 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovInteractRemark ADD CONSTRAINT FK_GIC_GIRe 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovInteractReply ADD CONSTRAINT FK_GIC_GIR 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovInteractType ADD CONSTRAINT FK_GIC_GIT 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovPublicApplyLog ADD CONSTRAINT FK_wcm_GPA_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovPublicApplyRemark ADD CONSTRAINT FK_wcm_GPARemark_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovPublicApplyReply ADD CONSTRAINT FK_wcm_GPAReply_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
go


ALTER TABLE wcm_GovPublicCategory ADD CONSTRAINT FK_wcm_GPC_GPCC 
    FOREIGN KEY (ClassCode, PublishmentSystemId)
    REFERENCES wcm_GovPublicCategoryClass(ClassCode, PublishmentSystemId) ON DELETE CASCADE ON UPDATE CASCADE
go


