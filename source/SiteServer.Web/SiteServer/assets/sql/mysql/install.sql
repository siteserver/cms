--
-- ER/Studio 8.0 SQL Code Generation
-- Company :      BaiRong Software
-- Project :      BaiRong Software Fundation Tables
-- Author :       BaiRong Software
--
-- Date Created : Tuesday, March 28, 2017 11:47:29
-- Target DBMS : MySQL 5.x
--

CREATE TABLE bairong_Administrator(
    UserName                         NATIONAL VARCHAR(255)    NOT NULL,
    Password                         NATIONAL VARCHAR(255),
    PasswordFormat                   VARCHAR(50),
    PasswordSalt                     NATIONAL VARCHAR(128),
    CreationDate                     DATETIME,
    LastActivityDate                 DATETIME,
    CountOfLogin                     INT,
    CountOfFailedLogin               INT,
    CreatorUserName                  NATIONAL VARCHAR(255),
    IsLockedOut                      VARCHAR(18),
    PublishmentSystemIdCollection    VARCHAR(50),
    PublishmentSystemId              INT,
    DepartmentId                     INT,
    AreaId                           INT,
    DisplayName                      NATIONAL VARCHAR(50),
    Email                            NATIONAL VARCHAR(50),
    Mobile                           VARCHAR(20),
    PRIMARY KEY (UserName)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_AdministratorsInRoles(
    RoleName    NATIONAL VARCHAR(255)    NOT NULL,
    UserName    NATIONAL VARCHAR(255)    NOT NULL,
    PRIMARY KEY (RoleName, UserName)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Area(
    AreaId           INT                      AUTO_INCREMENT,
    AreaName         NATIONAL VARCHAR(255),
    ParentId         INT,
    ParentsPath      NATIONAL VARCHAR(255),
    ParentsCount     INT,
    ChildrenCount    INT,
    IsLastNode       VARCHAR(18),
    Taxis            INT,
    CountOfAdmin     INT,
    PRIMARY KEY (AreaId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Config(
    IsInitialized           VARCHAR(18),
    DatabaseVersion         VARCHAR(50),
    RestrictionBlackList    NATIONAL VARCHAR(255),
    RestrictionWhiteList    NATIONAL VARCHAR(255),
    UpdateDate              DATETIME,
    UserConfig              LONGTEXT,
    SystemConfig            LONGTEXT
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_ContentCheck(
    CheckId                INT                      AUTO_INCREMENT,
    TableName              VARCHAR(50),
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentId              INT,
    IsAdmin                VARCHAR(18),
    UserName               NATIONAL VARCHAR(255),
    IsChecked              VARCHAR(18),
    CheckedLevel           INT,
    CheckDate              DATETIME,
    Reasons                NATIONAL VARCHAR(255),
    PRIMARY KEY (CheckId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_ContentModel(
    ModelId        VARCHAR(50)              NOT NULL,
    SiteId         INT                      NOT NULL,
    ModelName      NATIONAL VARCHAR(50),
    IsSystem       VARCHAR(18),
    TableName      VARCHAR(200),
    TableType      VARCHAR(50),
    IconUrl        VARCHAR(50),
    Description    NATIONAL VARCHAR(255),
    PRIMARY KEY (ModelId, SiteId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Count(
    CountId             INT                      AUTO_INCREMENT,
    RelatedTableName    NATIONAL VARCHAR(255),
    RelatedIdentity     NATIONAL VARCHAR(255),
    CountType           VARCHAR(50),
    CountNum            INT,
    PRIMARY KEY (CountId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_DbCache(
    Id            INT                      AUTO_INCREMENT,
    CacheKey      VARCHAR(200),
    CacheValue    NATIONAL VARCHAR(500),
    AddDate       DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Department(
    DepartmentId      INT                      AUTO_INCREMENT,
    DepartmentName    NATIONAL VARCHAR(255),
    Code              VARCHAR(50),
    ParentId          INT,
    ParentsPath       NATIONAL VARCHAR(255),
    ParentsCount      INT,
    ChildrenCount     INT,
    IsLastNode        VARCHAR(18),
    Taxis             INT,
    AddDate           DATETIME,
    Summary           NATIONAL VARCHAR(255),
    CountOfAdmin      INT,
    PRIMARY KEY (DepartmentId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Digg(
    DiggId                 INT    AUTO_INCREMENT,
    PublishmentSystemId    INT,
    RelatedIdentity        INT,
    Good                   INT,
    Bad                    INT,
    PRIMARY KEY (DiggId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_ErrorLog(
    Id            INT                      AUTO_INCREMENT,
    AddDate       DATETIME,
    Message       NATIONAL VARCHAR(255),
    Stacktrace    LONGTEXT,
    Summary       LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Log(
    Id           INT                      AUTO_INCREMENT,
    UserName     VARCHAR(50),
    IpAddress    VARCHAR(50),
    AddDate      DATETIME,
    Action       NATIONAL VARCHAR(255),
    Summary      NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_PermissionsInRoles(
    RoleName              NATIONAL VARCHAR(255)    NOT NULL,
    GeneralPermissions    TEXT,
    PRIMARY KEY (RoleName)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Roles(
    RoleName           NATIONAL VARCHAR(255)    NOT NULL,
    CreatorUserName    NATIONAL VARCHAR(255),
    Description        NATIONAL VARCHAR(255),
    PRIMARY KEY (RoleName)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_TableCollection(
    TableEnName                  VARCHAR(50)             NOT NULL,
    TableCnName                  NATIONAL VARCHAR(50),
    AttributeNum                 INT,
    AuxiliaryTableType           VARCHAR(50),
    IsCreatedInDb                VARCHAR(18),
    IsChangedAfterCreatedInDb    VARCHAR(18),
    IsDefault                    VARCHAR(18),
    Description                  LONGTEXT,
    PRIMARY KEY (TableEnName)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_TableMatch(
    TableMatchId               INT             AUTO_INCREMENT,
    ConnectionString           VARCHAR(200),
    TableName                  VARCHAR(200),
    ConnectionStringToMatch    VARCHAR(200),
    TableNameToMatch           VARCHAR(200),
    ColumnsMap                 LONGTEXT,
    PRIMARY KEY (TableMatchId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_TableMetadata(
    TableMetadataId         INT            AUTO_INCREMENT,
    AuxiliaryTableEnName    VARCHAR(50)    NOT NULL,
    AttributeName           VARCHAR(50),
    DataType                VARCHAR(50),
    DataLength              INT,
    Taxis                   INT,
    IsSystem                VARCHAR(18),
    PRIMARY KEY (TableMetadataId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_TableStyle(
    TableStyleId       INT                      AUTO_INCREMENT,
    RelatedIdentity    INT,
    TableName          VARCHAR(50),
    AttributeName      VARCHAR(50),
    Taxis              INT,
    DisplayName        NATIONAL VARCHAR(255),
    HelpText           VARCHAR(255),
    IsVisible          VARCHAR(18),
    IsVisibleInList    VARCHAR(18),
    IsSingleLine       VARCHAR(18),
    InputType          VARCHAR(50),
    IsRequired         VARCHAR(18),
    DefaultValue       VARCHAR(255),
    IsHorizontal       VARCHAR(18),
    ExtendValues       LONGTEXT,
    PRIMARY KEY (TableStyleId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_TableStyleItem(
    TableStyleItemId    INT             AUTO_INCREMENT,
    TableStyleId        INT             NOT NULL,
    ItemTitle           VARCHAR(255),
    ItemValue           VARCHAR(255),
    IsSelected          VARCHAR(18),
    PRIMARY KEY (TableStyleItemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Tags(
    TagId                  INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ContentIdCollection    NATIONAL VARCHAR(255),
    Tag                    NATIONAL VARCHAR(255),
    UseNum                 INT,
    PRIMARY KEY (TagId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_ThirdLogin(
    Id                INT                      AUTO_INCREMENT,
    ThirdLoginType    VARCHAR(50),
    ThirdLoginName    NATIONAL VARCHAR(50),
    IsEnabled         VARCHAR(18),
    Taxis             INT,
    Description       NATIONAL VARCHAR(255),
    SettingsXml       LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_UserBinding(
    UserId              INT                      NOT NULL,
    ThirdLoginType      VARCHAR(50),
    ThirdLoginUserId    NATIONAL VARCHAR(200),
    PRIMARY KEY (UserId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_UserGroup(
    GroupId         INT                      AUTO_INCREMENT,
    GroupName       NATIONAL VARCHAR(50),
    IsDefault       VARCHAR(18),
    Description     NATIONAL VARCHAR(255),
    ExtendValues    LONGTEXT,
    PRIMARY KEY (GroupId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_UserLog(
    Id           INT                      AUTO_INCREMENT,
    UserName     VARCHAR(50),
    IpAddress    VARCHAR(50),
    AddDate      DATETIME,
    Action       NATIONAL VARCHAR(255),
    Summary      NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE bairong_Users(
    UserId                   INT                      AUTO_INCREMENT,
    UserName                 NATIONAL VARCHAR(255),
    Password                 NATIONAL VARCHAR(255),
    PasswordFormat           VARCHAR(50),
    PasswordSalt             NATIONAL VARCHAR(128),
    GroupId                  INT,
    CreateDate               DATETIME,
    LastResetPasswordDate    DATETIME,
    LastActivityDate         DATETIME,
    CountOfLogin             INT,
    CountOfFailedLogin       INT,
    CountOfWriting           INT,
    IsChecked                VARCHAR(18),
    IsLockedOut              VARCHAR(18),
    DisplayName              NATIONAL VARCHAR(255),
    Email                    NATIONAL VARCHAR(255),
    Mobile                   VARCHAR(20),
    AvatarUrl                VARCHAR(200),
    Organization             NATIONAL VARCHAR(255),
    Department               NATIONAL VARCHAR(255),
    Position                 NATIONAL VARCHAR(255),
    Gender                   NATIONAL VARCHAR(50),
    Birthday                 VARCHAR(50),
    Education                NATIONAL VARCHAR(255),
    Graduation               NATIONAL VARCHAR(255),
    Address                  NATIONAL VARCHAR(255),
    WeiXin                   NATIONAL VARCHAR(255),
    Qq                       VARCHAR(50),
    WeiBo                    NATIONAL VARCHAR(255),
    Interests                NATIONAL VARCHAR(255),
    Signature                NATIONAL VARCHAR(255),
    ExtendValues             LONGTEXT,
    PRIMARY KEY (UserId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_AdArea(
    AdAreaId               INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    AdAreaName             NATIONAL VARCHAR(50),
    Width                  INT,
    Height                 INT,
    Summary                NATIONAL VARCHAR(255),
    IsEnabled              VARCHAR(18),
    AddDate                DATETIME,
    PRIMARY KEY (AdAreaId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_AdMaterial(
    AdMaterialId           INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    AdvId                  INT                      NOT NULL,
    AdvertId               INT,
    AdMaterialName         NATIONAL VARCHAR(50),
    AdMaterialType         VARCHAR(50),
    Code                   LONGTEXT,
    TextWord               NATIONAL VARCHAR(255),
    TextLink               VARCHAR(200),
    TextColor              VARCHAR(10),
    TextFontSize           INT,
    ImageUrl               VARCHAR(200),
    ImageLink              VARCHAR(200),
    ImageWidth             INT,
    ImageHeight            INT,
    ImageAlt               NATIONAL VARCHAR(50),
    Weight                 INT,
    IsEnabled              VARCHAR(18),
    PRIMARY KEY (AdMaterialId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Adv(
    AdvId                        INT                       AUTO_INCREMENT,
    PublishmentSystemId          INT,
    AdAreaId                     INT                       NOT NULL,
    AdvName                      NATIONAL VARCHAR(50),
    Summary                      NATIONAL VARCHAR(255),
    IsEnabled                    VARCHAR(18),
    IsDateLimited                VARCHAR(18),
    StartDate                    DATETIME,
    EndDate                      DATETIME,
    LevelType                    NATIONAL VARCHAR(50),
    Level                        INT,
    IsWeight                     VARCHAR(18),
    Weight                       INT,
    RotateType                   NATIONAL VARCHAR(50),
    RotateInterval               INT,
    NodeIdCollectionToChannel    NATIONAL VARCHAR(4000),
    NodeIdCollectionToContent    NATIONAL VARCHAR(4000),
    FileTemplateIdCollection     NATIONAL VARCHAR(4000),
    PRIMARY KEY (AdvId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Advertisement(
    AdvertisementName            VARCHAR(50)              NOT NULL,
    PublishmentSystemId          INT                      NOT NULL,
    AdvertisementType            VARCHAR(50),
    NavigationUrl                VARCHAR(200),
    IsDateLimited                VARCHAR(18),
    StartDate                    DATETIME,
    EndDate                      DATETIME,
    AddDate                      DATETIME,
    NodeIdCollectionToChannel    NATIONAL VARCHAR(255),
    NodeIdCollectionToContent    NATIONAL VARCHAR(255),
    FileTemplateIdCollection     NATIONAL VARCHAR(255),
    Settings                     LONGTEXT,
    PRIMARY KEY (AdvertisementName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_AdvImageClassify(
    ItemId                 INT                     AUTO_INCREMENT,
    ItemName               NATIONAL VARCHAR(50),
    ItemIndexName          NATIONAL VARCHAR(50),
    ParentId               INT,
    ParentsPath            VARCHAR(255),
    ParentsCount           INT,
    ChildrenCount          INT,
    ContentNum             INT,
    PublishmentSystemId    INT,
    Enabled                VARCHAR(18),
    IsLastItem             VARCHAR(18),
    Taxis                  INT,
    AddDate                DATETIME,
    PRIMARY KEY (ItemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_AdvImageContent(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    Taxis                  INT,
    IsChecked              VARCHAR(18),
    AddUser                NATIONAL VARCHAR(50),
    AddDate                DATETIME,
    LaseEditUser           VARCHAR(50),
    LastEditDate           DATETIME,
    SettingsXml            LONGTEXT,
    AdvImageName           NATIONAL VARCHAR(50),
    Description            NATIONAL VARCHAR(255),
    ClassifyId             INT,
    AdvImagePath           NATIONAL VARCHAR(500),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Comment(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentId              INT,
    GoodCount              INT,
    UserName               NATIONAL VARCHAR(50),
    IsChecked              VARCHAR(18),
    AddDate                DATETIME,
    Content                NATIONAL VARCHAR(500),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_ContentGroup(
    ContentGroupName       NATIONAL VARCHAR(255)    NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    Taxis                  INT,
    Description            LONGTEXT,
    PRIMARY KEY (ContentGroupName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_CreateTask(
    Id                     INT            AUTO_INCREMENT,
    CreateType             VARCHAR(50),
    PublishmentSystemId    INT,
    ChannelId              INT,
    ContentId              INT,
    TemplateId             INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_CreateTaskLog(
    Id                     INT                      AUTO_INCREMENT,
    CreateType             VARCHAR(50),
    PublishmentSystemId    INT,
    ChannelId              INT,
    ContentId              INT,
    TemplateId             INT,
    TaskName               NATIONAL VARCHAR(50),
    TimeSpan               NATIONAL VARCHAR(50),
    IsSuccess              VARCHAR(18),
    ErrorMessage           NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_GatherDatabaseRule(
    GatherRuleName         NATIONAL VARCHAR(50)     NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    ConnectionString       VARCHAR(255),
    RelatedTableName       VARCHAR(255),
    RelatedIdentity        VARCHAR(255),
    RelatedOrderBy         VARCHAR(255),
    WhereString            NATIONAL VARCHAR(255),
    TableMatchId           INT,
    NodeId                 INT,
    GatherNum              INT,
    IsChecked              VARCHAR(18),
    IsAutoCreate           VARCHAR(18),
    IsOrderByDesc          VARCHAR(18),
    LastGatherDate         DATETIME,
    PRIMARY KEY (GatherRuleName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_GatherFileRule(
    GatherRuleName                   NATIONAL VARCHAR(50)     NOT NULL,
    PublishmentSystemId              INT                      NOT NULL,
    GatherUrl                        NATIONAL VARCHAR(255),
    Charset                          VARCHAR(50),
    LastGatherDate                   DATETIME,
    IsToFile                         VARCHAR(18),
    FilePath                         NATIONAL VARCHAR(255),
    IsSaveRelatedFiles               VARCHAR(18),
    IsRemoveScripts                  VARCHAR(18),
    StyleDirectoryPath               NATIONAL VARCHAR(255),
    ScriptDirectoryPath              NATIONAL VARCHAR(255),
    ImageDirectoryPath               NATIONAL VARCHAR(255),
    NodeId                           INT,
    IsSaveImage                      VARCHAR(18),
    IsChecked                        VARCHAR(18),
    IsAutoCreate                     VARCHAR(18),
    ContentExclude                   LONGTEXT,
    ContentHtmlClearCollection       NATIONAL VARCHAR(255),
    ContentHtmlClearTagCollection    NATIONAL VARCHAR(255),
    ContentTitleStart                LONGTEXT,
    ContentTitleEnd                  LONGTEXT,
    ContentContentStart              LONGTEXT,
    ContentContentEnd                LONGTEXT,
    ContentAttributes                LONGTEXT,
    ContentAttributesXml             LONGTEXT,
    PRIMARY KEY (GatherRuleName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_GatherRule(
    GatherRuleName                   NATIONAL VARCHAR(50)     NOT NULL,
    PublishmentSystemId              INT                      NOT NULL,
    CookieString                     TEXT,
    GatherUrlIsCollection            VARCHAR(18),
    GatherUrlCollection              TEXT,
    GatherUrlIsSerialize             VARCHAR(18),
    GatherUrlSerialize               VARCHAR(200),
    SerializeFrom                    INT,
    SerializeTo                      INT,
    SerializeInterval                INT,
    SerializeIsOrderByDesc           VARCHAR(18),
    SerializeIsAddZero               VARCHAR(18),
    NodeId                           INT,
    Charset                          VARCHAR(50),
    UrlInclude                       VARCHAR(200),
    TitleInclude                     NATIONAL VARCHAR(255),
    ContentExclude                   LONGTEXT,
    ContentHtmlClearCollection       NATIONAL VARCHAR(255),
    ContentHtmlClearTagCollection    NATIONAL VARCHAR(255),
    LastGatherDate                   DATETIME,
    ListAreaStart                    LONGTEXT,
    ListAreaEnd                      LONGTEXT,
    ContentChannelStart              LONGTEXT,
    ContentChannelEnd                LONGTEXT,
    ContentTitleStart                LONGTEXT,
    ContentTitleEnd                  LONGTEXT,
    ContentContentStart              LONGTEXT,
    ContentContentEnd                LONGTEXT,
    ContentNextPageStart             LONGTEXT,
    ContentNextPageEnd               LONGTEXT,
    ContentAttributes                LONGTEXT,
    ContentAttributesXml             LONGTEXT,
    ExtendValues                     LONGTEXT,
    PRIMARY KEY (GatherRuleName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_InnerLink(
    InnerLinkName          NATIONAL VARCHAR(255)    NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    LinkUrl                VARCHAR(200),
    PRIMARY KEY (InnerLinkName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Input(
    InputId                INT                     AUTO_INCREMENT,
    InputName              NATIONAL VARCHAR(50),
    PublishmentSystemId    INT,
    AddDate                DATETIME,
    IsChecked              VARCHAR(18),
    IsReply                VARCHAR(18),
    Taxis                  INT,
    SettingsXml            LONGTEXT,
    PRIMARY KEY (InputId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_InputContent(
    Id             INT                      AUTO_INCREMENT,
    InputId        INT                      NOT NULL,
    Taxis          INT,
    IsChecked      VARCHAR(18),
    UserName       NATIONAL VARCHAR(255),
    IpAddress      VARCHAR(50),
    AddDate        DATETIME,
    Reply          LONGTEXT,
    SettingsXml    LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Keyword(
    KeywordId      INT                     AUTO_INCREMENT,
    Keyword        NATIONAL VARCHAR(50),
    Alternative    NATIONAL VARCHAR(50),
    Grade          NATIONAL VARCHAR(50),
    PRIMARY KEY (KeywordId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Log(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ChannelId              INT,
    ContentId              INT,
    UserName               VARCHAR(50),
    IpAddress              VARCHAR(50),
    AddDate                DATETIME,
    Action                 NATIONAL VARCHAR(255),
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_MenuDisplay(
    MenuDisplayId          INT             AUTO_INCREMENT,
    PublishmentSystemId    INT,
    MenuDisplayName        VARCHAR(50),
    Vertical               VARCHAR(50),
    FontFamily             VARCHAR(200),
    FontSize               INT,
    FontWeight             VARCHAR(50),
    FontStyle              VARCHAR(50),
    MenuItemHAlign         VARCHAR(50),
    MenuItemVAlign         VARCHAR(50),
    FontColor              VARCHAR(50),
    MenuItemBgColor        VARCHAR(50),
    FontColorHilite        VARCHAR(50),
    MenuHiliteBgColor      VARCHAR(50),
    XPosition              VARCHAR(50),
    YPosition              VARCHAR(50),
    HideOnMouseOut         VARCHAR(50),
    MenuWidth              INT,
    MenuItemHeight         INT,
    MenuItemPadding        INT,
    MenuItemSpacing        INT,
    MenuItemIndent         INT,
    HideTimeout            INT,
    MenuBgOpaque           VARCHAR(50),
    MenuBorder             INT,
    BgColor                VARCHAR(50),
    MenuBorderBgColor      VARCHAR(50),
    MenuLiteBgColor        VARCHAR(50),
    ChildMenuIcon          VARCHAR(200),
    AddDate                DATETIME,
    IsDefault              VARCHAR(18),
    Description            LONGTEXT,
    PRIMARY KEY (MenuDisplayId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Node(
    NodeId                     INT                      AUTO_INCREMENT,
    NodeName                   NATIONAL VARCHAR(255),
    NodeType                   VARCHAR(50),
    PublishmentSystemId        INT,
    ContentModelId             VARCHAR(50),
    ParentId                   INT,
    ParentsPath                NATIONAL VARCHAR(255),
    ParentsCount               INT,
    ChildrenCount              INT,
    IsLastNode                 VARCHAR(18),
    NodeIndexName              NATIONAL VARCHAR(255),
    NodeGroupNameCollection    NATIONAL VARCHAR(255),
    Taxis                      INT,
    AddDate                    DATETIME,
    ImageUrl                   VARCHAR(200),
    Content                    LONGTEXT,
    ContentNum                 INT,
    FilePath                   VARCHAR(200),
    ChannelFilePathRule        VARCHAR(200),
    ContentFilePathRule        VARCHAR(200),
    LinkUrl                    VARCHAR(200),
    LinkType                   VARCHAR(200),
    ChannelTemplateId          INT,
    ContentTemplateId          INT,
    Keywords                   NATIONAL VARCHAR(255),
    Description                NATIONAL VARCHAR(255),
    ExtendValues               LONGTEXT,
    PRIMARY KEY (NodeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_NodeGroup(
    NodeGroupName          NATIONAL VARCHAR(255)    NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    Taxis                  INT,
    Description            LONGTEXT,
    PRIMARY KEY (NodeGroupName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Photo(
    Id                     INT             AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ContentId              INT,
    SmallUrl               VARCHAR(200),
    MiddleUrl              VARCHAR(200),
    LargeUrl               VARCHAR(200),
    Taxis                  INT,
    Description            VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO


CREATE TABLE siteserver_PluginConfig(
    Id             INT                      AUTO_INCREMENT,
    PluginId       NATIONAL VARCHAR(50),
    SiteId         INT,
    ConfigName     NATIONAL VARCHAR(200),
    ConfigValue    LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO


CREATE TABLE siteserver_PublishmentSystem(
    PublishmentSystemId             INT                     NOT NULL,
    PublishmentSystemName           NATIONAL VARCHAR(50),
    PublishmentSystemType           VARCHAR(50),
    AuxiliaryTableForContent        VARCHAR(50),
    AuxiliaryTableForGovPublic      VARCHAR(50),
    AuxiliaryTableForGovInteract    VARCHAR(50),
    AuxiliaryTableForVote           VARCHAR(50),
    AuxiliaryTableForJob            VARCHAR(50),
    IsCheckContentUseLevel          VARCHAR(18),
    CheckContentLevel               INT,
    PublishmentSystemDir            VARCHAR(50),
    PublishmentSystemUrl            VARCHAR(200),
    IsHeadquarters                  VARCHAR(18),
    ParentPublishmentSystemId       INT,
    Taxis                           INT,
    SettingsXml                     LONGTEXT,
    PRIMARY KEY (PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_RelatedField(
    RelatedFieldId         INT                      AUTO_INCREMENT,
    RelatedFieldName       NATIONAL VARCHAR(50),
    PublishmentSystemId    INT,
    TotalLevel             INT,
    Prefixes               NATIONAL VARCHAR(255),
    Suffixes               NATIONAL VARCHAR(255),
    PRIMARY KEY (RelatedFieldId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_RelatedFieldItem(
    Id                INT                      AUTO_INCREMENT,
    RelatedFieldId    INT                      NOT NULL,
    ItemName          NATIONAL VARCHAR(255),
    ItemValue         NATIONAL VARCHAR(255),
    ParentId          INT,
    Taxis             INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_ResumeContent(
    Id                       INT                      AUTO_INCREMENT,
    PublishmentSystemId      INT,
    JobContentId             INT,
    UserName                 NATIONAL VARCHAR(255),
    IsView                   VARCHAR(18),
    AddDate                  DATETIME,
    RealName                 NATIONAL VARCHAR(50),
    Nationality              NATIONAL VARCHAR(50),
    Gender                   NATIONAL VARCHAR(50),
    Email                    VARCHAR(50),
    MobilePhone              VARCHAR(50),
    HomePhone                VARCHAR(50),
    LastSchoolName           NATIONAL VARCHAR(50),
    Education                NATIONAL VARCHAR(50),
    IdCardType               NATIONAL VARCHAR(50),
    IdCardNo                 VARCHAR(50),
    Birthday                 VARCHAR(50),
    Marriage                 NATIONAL VARCHAR(50),
    WorkYear                 NATIONAL VARCHAR(50),
    Profession               NATIONAL VARCHAR(50),
    ExpectSalary             NATIONAL VARCHAR(50),
    AvailabelTime            NATIONAL VARCHAR(50),
    Location                 NATIONAL VARCHAR(50),
    ImageUrl                 VARCHAR(200),
    Summary                  NATIONAL VARCHAR(255),
    Exp_Count                INT,
    Exp_FromYear             NATIONAL VARCHAR(50),
    Exp_FromMonth            NATIONAL VARCHAR(50),
    Exp_ToYear               NATIONAL VARCHAR(50),
    Exp_ToMonth              NATIONAL VARCHAR(50),
    Exp_EmployerName         NATIONAL VARCHAR(255),
    Exp_Department           NATIONAL VARCHAR(255),
    Exp_EmployerPhone        NATIONAL VARCHAR(255),
    Exp_WorkPlace            NATIONAL VARCHAR(255),
    Exp_PositionTitle        NATIONAL VARCHAR(255),
    Exp_Industry             NATIONAL VARCHAR(255),
    Exp_Summary              LONGTEXT,
    Exp_Score                LONGTEXT,
    Pro_Count                INT,
    Pro_FromYear             NATIONAL VARCHAR(50),
    Pro_FromMonth            NATIONAL VARCHAR(50),
    Pro_ToYear               NATIONAL VARCHAR(50),
    Pro_ToMonth              NATIONAL VARCHAR(50),
    Pro_ProjectName          NATIONAL VARCHAR(255),
    Pro_Summary              LONGTEXT,
    Edu_Count                INT,
    Edu_FromYear             NATIONAL VARCHAR(50),
    Edu_FromMonth            NATIONAL VARCHAR(50),
    Edu_ToYear               NATIONAL VARCHAR(50),
    Edu_ToMonth              NATIONAL VARCHAR(50),
    Edu_SchoolName           NATIONAL VARCHAR(255),
    Edu_Education            NATIONAL VARCHAR(255),
    Edu_Profession           NATIONAL VARCHAR(255),
    Edu_Summary              LONGTEXT,
    Tra_Count                INT,
    Tra_FromYear             NATIONAL VARCHAR(50),
    Tra_FromMonth            NATIONAL VARCHAR(50),
    Tra_ToYear               NATIONAL VARCHAR(50),
    Tra_ToMonth              NATIONAL VARCHAR(50),
    Tra_TrainerName          NATIONAL VARCHAR(255),
    Tra_TrainerAddress       NATIONAL VARCHAR(255),
    Tra_Lesson               NATIONAL VARCHAR(255),
    Tra_Centification        NATIONAL VARCHAR(255),
    Tra_Summary              NATIONAL VARCHAR(255),
    Lan_Count                INT,
    Lan_Language             NATIONAL VARCHAR(255),
    Lan_Level                NATIONAL VARCHAR(255),
    Ski_Count                INT,
    Ski_SkillName            NATIONAL VARCHAR(255),
    Ski_UsedTimes            NATIONAL VARCHAR(255),
    Ski_Ability              NATIONAL VARCHAR(255),
    Cer_Count                INT,
    Cer_CertificationName    NATIONAL VARCHAR(255),
    Cer_EffectiveDate        NATIONAL VARCHAR(255),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SeoMeta(
    SeoMetaId              INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    SeoMetaName            VARCHAR(50),
    IsDefault              VARCHAR(18),
    PageTitle              NATIONAL VARCHAR(80),
    Keywords               NATIONAL VARCHAR(100),
    Description            NATIONAL VARCHAR(200),
    Copyright              NATIONAL VARCHAR(255),
    Author                 NATIONAL VARCHAR(50),
    Email                  NATIONAL VARCHAR(50),
    Language               VARCHAR(50),
    Charset                VARCHAR(50),
    Distribution           VARCHAR(50),
    Rating                 VARCHAR(50),
    Robots                 VARCHAR(50),
    RevisitAfter           VARCHAR(50),
    Expires                VARCHAR(50),
    PRIMARY KEY (SeoMetaId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SeoMetasInNodes(
    NodeId                 INT            NOT NULL,
    IsChannel              VARCHAR(18)    NOT NULL,
    SeoMetaId              INT            NOT NULL,
    PublishmentSystemId    INT,
    PRIMARY KEY (NodeId, IsChannel, SeoMetaId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SigninLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ContentId              INT,
    UserName               NATIONAL VARCHAR(255),
    IsSignin               VARCHAR(18),
    SigninDate             DATETIME,
    IpAddress              VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SigninSetting(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentId              INT,
    IsGroup                VARCHAR(18),
    UserGroupCollection    TEXT,
    UserNameCollection     NATIONAL VARCHAR(500),
    Priority               INT,
    EndDate                VARCHAR(50),
    IsSignin               VARCHAR(18),
    SigninDate             DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SigninUserContentId(
    Id                     INT                      AUTO_INCREMENT,
    IsGroup                VARCHAR(18),
    GroupId                INT,
    UserName               NATIONAL VARCHAR(255),
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentIdCollection    TEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Star(
    StarId                 INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ChannelId              INT,
    ContentId              INT,
    UserName               NATIONAL VARCHAR(255),
    Point                  INT,
    Message                NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (StarId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_StarSetting(
    StarSettingId          INT               AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ChannelId              INT,
    ContentId              INT,
    TotalCount             INT,
    PointAverage           DECIMAL(18, 1),
    PRIMARY KEY (StarSettingId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_StlTag(
    TagName                NATIONAL VARCHAR(50)     NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    TagDescription         NATIONAL VARCHAR(255),
    TagContent             LONGTEXT,
    PRIMARY KEY (TagName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_SystemPermissions(
    RoleName               NATIONAL VARCHAR(255)    NOT NULL,
    PublishmentSystemId    INT                      NOT NULL,
    NodeIdCollection       TEXT,
    ChannelPermissions     TEXT,
    WebsitePermissions     TEXT,
    PRIMARY KEY (RoleName, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_TagStyle(
    StyleId                INT                     AUTO_INCREMENT,
    StyleName              NATIONAL VARCHAR(50),
    ElementName            VARCHAR(50),
    PublishmentSystemId    INT,
    IsTemplate             VARCHAR(18),
    StyleTemplate          LONGTEXT,
    ScriptTemplate         LONGTEXT,
    ContentTemplate        LONGTEXT,
    SuccessTemplate        LONGTEXT,
    FailureTemplate        LONGTEXT,
    SettingsXml            LONGTEXT,
    PRIMARY KEY (StyleId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Task(
    TaskId                  INT                      AUTO_INCREMENT,
    TaskName                NATIONAL VARCHAR(50),
    IsSystemTask            VARCHAR(18),
    PublishmentSystemId     INT,
    ServiceType             VARCHAR(50),
    ServiceParameters       LONGTEXT,
    FrequencyType           VARCHAR(50),
    PeriodIntervalMinute    INT,
    StartDay                INT,
    StartWeekday            INT,
    StartHour               INT,
    IsEnabled               VARCHAR(18),
    AddDate                 DATETIME,
    LastExecuteDate         DATETIME,
    Description             NATIONAL VARCHAR(255),
    OnlyOnceDate            DATETIME,
    PRIMARY KEY (TaskId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_TaskLog(
    Id              INT                      AUTO_INCREMENT,
    TaskId          INT                      NOT NULL,
    IsSuccess       VARCHAR(18),
    ErrorMessage    NATIONAL VARCHAR(255),
    AddDate         DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Template(
    TemplateId             INT                     AUTO_INCREMENT,
    PublishmentSystemId    INT,
    TemplateName           NATIONAL VARCHAR(50),
    TemplateType           VARCHAR(50),
    RelatedFileName        NATIONAL VARCHAR(50),
    CreatedFileFullName    NATIONAL VARCHAR(50),
    CreatedFileExtName     VARCHAR(50),
    Charset                VARCHAR(50),
    IsDefault              VARCHAR(18),
    PRIMARY KEY (TemplateId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_TemplateLog(
    Id                     INT                      AUTO_INCREMENT,
    TemplateId             INT,
    PublishmentSystemId    INT,
    AddDate                DATETIME,
    AddUserName            NATIONAL VARCHAR(255),
    ContentLength          INT,
    TemplateContent        LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_TemplateMatch(
    NodeId                 INT             NOT NULL,
    PublishmentSystemId    INT,
    ChannelTemplateId      INT,
    ContentTemplateId      INT,
    FilePath               VARCHAR(200),
    ChannelFilePathRule    VARCHAR(200),
    ContentFilePathRule    VARCHAR(200),
    PRIMARY KEY (NodeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_Tracking(
    TrackingId             INT             AUTO_INCREMENT,
    PublishmentSystemId    INT,
    TrackerType            VARCHAR(50),
    LastAccessDateTime     DATETIME,
    PageUrl                VARCHAR(200),
    PageNodeId             INT,
    PageContentId          INT,
    Referrer               VARCHAR(200),
    IpAddress              VARCHAR(200),
    OperatingSystem        VARCHAR(200),
    Browser                VARCHAR(200),
    AccessDateTime         DATETIME,
    PRIMARY KEY (TrackingId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_VoteOperation(
    OperationId            INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentId              INT,
    IpAddress              VARCHAR(50),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (OperationId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE siteserver_VoteOption(
    OptionId               INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT,
    ContentId              INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    NavigationUrl          VARCHAR(200),
    VoteNum                INT,
    PRIMARY KEY (OptionId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractChannel(
    NodeId                    INT                      NOT NULL,
    PublishmentSystemId       INT,
    ApplyStyleId              INT,
    QueryStyleId              INT,
    DepartmentIdCollection    NATIONAL VARCHAR(255),
    Summary                   NATIONAL VARCHAR(255),
    PRIMARY KEY (NodeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractLog(
    LogId                  INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT                      NOT NULL,
    ContentId              INT,
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    LogType                VARCHAR(50),
    IpAddress              VARCHAR(50),
    AddDate                DATETIME,
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (LogId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractPermissions(
    UserName       NATIONAL VARCHAR(50)     NOT NULL,
    NodeId         INT                      NOT NULL,
    Permissions    NATIONAL VARCHAR(255),
    PRIMARY KEY (UserName, NodeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractRemark(
    RemarkId               INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT                      NOT NULL,
    ContentId              INT,
    RemarkType             VARCHAR(50),
    Remark                 NATIONAL VARCHAR(255),
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (RemarkId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractReply(
    ReplyId                INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT                      NOT NULL,
    ContentId              INT,
    Reply                  LONGTEXT,
    FileUrl                NATIONAL VARCHAR(255),
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (ReplyId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovInteractType(
    TypeId                 INT                     AUTO_INCREMENT,
    TypeName               NATIONAL VARCHAR(50),
    NodeId                 INT                     NOT NULL,
    PublishmentSystemId    INT,
    Taxis                  INT,
    PRIMARY KEY (TypeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicApply(
    Id                     INT                      AUTO_INCREMENT,
    StyleId                INT,
    PublishmentSystemId    INT,
    IsOrganization         VARCHAR(18),
    CivicName              NATIONAL VARCHAR(255),
    CivicOrganization      NATIONAL VARCHAR(255),
    CivicCardType          NATIONAL VARCHAR(255),
    CivicCardNo            NATIONAL VARCHAR(255),
    CivicPhone             VARCHAR(50),
    CivicPostCode          VARCHAR(50),
    CivicAddress           NATIONAL VARCHAR(255),
    CivicEmail             NATIONAL VARCHAR(255),
    CivicFax               VARCHAR(50),
    OrgName                NATIONAL VARCHAR(255),
    OrgUnitCode            NATIONAL VARCHAR(255),
    OrgLegalPerson         NATIONAL VARCHAR(255),
    OrgLinkName            NATIONAL VARCHAR(255),
    OrgPhone               VARCHAR(50),
    OrgPostCode            VARCHAR(50),
    OrgAddress             NATIONAL VARCHAR(255),
    OrgEmail               NATIONAL VARCHAR(255),
    OrgFax                 VARCHAR(50),
    Title                  NATIONAL VARCHAR(255),
    Content                LONGTEXT,
    Purpose                NATIONAL VARCHAR(255),
    IsApplyFree            VARCHAR(18),
    ProvideType            VARCHAR(50),
    ObtainType             VARCHAR(50),
    DepartmentName         NATIONAL VARCHAR(255),
    DepartmentId           INT,
    AddDate                DATETIME,
    QueryCode              NATIONAL VARCHAR(255),
    State                  VARCHAR(50),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicApplyLog(
    LogId                  INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ApplyId                INT,
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    LogType                VARCHAR(50),
    IpAddress              VARCHAR(50),
    AddDate                DATETIME,
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (LogId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicApplyRemark(
    RemarkId               INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ApplyId                INT,
    RemarkType             VARCHAR(50),
    Remark                 NATIONAL VARCHAR(255),
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (RemarkId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicApplyReply(
    ReplyId                INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ApplyId                INT,
    Reply                  LONGTEXT,
    FileUrl                NATIONAL VARCHAR(255),
    DepartmentId           INT,
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (ReplyId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicCategory(
    CategoryId             INT                      AUTO_INCREMENT,
    ClassCode              NATIONAL VARCHAR(50),
    PublishmentSystemId    INT,
    CategoryName           NATIONAL VARCHAR(255),
    CategoryCode           VARCHAR(50),
    ParentId               INT,
    ParentsPath            NATIONAL VARCHAR(255),
    ParentsCount           INT,
    ChildrenCount          INT,
    IsLastNode             VARCHAR(18),
    Taxis                  INT,
    AddDate                DATETIME,
    Summary                NATIONAL VARCHAR(255),
    ContentNum             INT,
    PRIMARY KEY (CategoryId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicCategoryClass(
    ClassCode               NATIONAL VARCHAR(50)     NOT NULL,
    PublishmentSystemId     INT                      NOT NULL,
    ClassName               NATIONAL VARCHAR(255),
    IsSystem                VARCHAR(18),
    IsEnabled               VARCHAR(18),
    ContentAttributeName    VARCHAR(50),
    Taxis                   INT,
    Description             NATIONAL VARCHAR(255),
    PRIMARY KEY (ClassCode, PublishmentSystemId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicChannel(
    NodeId                 INT                      NOT NULL,
    PublishmentSystemId    INT,
    Code                   NATIONAL VARCHAR(50),
    Summary                NATIONAL VARCHAR(255),
    PRIMARY KEY (NodeId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicIdentifierRule(
    RuleId                 INT                      AUTO_INCREMENT,
    RuleName               NATIONAL VARCHAR(255),
    PublishmentSystemId    INT,
    IdentifierType         VARCHAR(50),
    MinLength              INT,
    Suffix                 VARCHAR(50),
    FormatString           VARCHAR(50),
    AttributeName          VARCHAR(50),
    Sequence               INT,
    Taxis                  INT,
    SettingsXml            LONGTEXT,
    PRIMARY KEY (RuleId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wcm_GovPublicIdentifierSeq(
    SeqId                  INT    AUTO_INCREMENT,
    PublishmentSystemId    INT,
    NodeId                 INT,
    DepartmentId           INT,
    AddYear                INT,
    Sequence               INT,
    PRIMARY KEY (SeqId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO

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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wx_CardSignLog(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    UserName               NATIONAL VARCHAR(255),
    SignDate               DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wx_ConfigExtend(
    Id                     INT            AUTO_INCREMENT,
    PublishmentSystemId    INT,
    KeywordType            VARCHAR(50),
    FunctionId             INT,
    AttributeName          VARCHAR(50),
    IsVisible              VARCHAR(18),
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wx_Coupon(
    Id                     INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    ActId                  INT,
    Title                  NATIONAL VARCHAR(255),
    TotalNum               INT,
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wx_KeywordGroup(
    GroupId                INT                      AUTO_INCREMENT,
    PublishmentSystemId    INT,
    GroupName              NATIONAL VARCHAR(255),
    Taxis                  INT,
    PRIMARY KEY (GroupId)
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
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
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO



CREATE TABLE wx_WifiNode(
    Id                     INT             AUTO_INCREMENT,
    PublishmentSystemId    INT,
    BusinessId             VARCHAR(100),
    NodeId                 VARCHAR(100),
    CallBackString         LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO


CREATE INDEX IX_bairong_DbCache_Key ON bairong_DbCache(CacheKey)
GO
CREATE INDEX IX_bairong_TM_ATE ON bairong_TableMetadata(AuxiliaryTableEnName)
GO
CREATE INDEX IX_bairong_TSI_TSI ON bairong_TableStyleItem(TableStyleId)
GO
CREATE INDEX IX_birong_UserLog_UserName ON bairong_UserLog(UserName)
GO
CREATE UNIQUE INDEX UK_bairong_Users_UserName ON bairong_Users(UserName)
GO
CREATE INDEX IX_siteserver_Comment_ContentID ON siteserver_Comment(ContentId)
GO
CREATE INDEX IX_siteserver_Comment_PSID ON siteserver_Comment(PublishmentSystemId)
GO
CREATE INDEX IK_siteserver_Node_Taxis ON siteserver_Node(Taxis)
GO
CREATE INDEX IK_siteserver_Node_PSID ON siteserver_Node(PublishmentSystemId)
GO
CREATE INDEX IX_siteserver_PS_Taxis ON siteserver_PublishmentSystem(Taxis)
GO
ALTER TABLE bairong_AdministratorsInRoles ADD CONSTRAINT FK_bairong_AInR_A 
    FOREIGN KEY (UserName)
    REFERENCES bairong_Administrator(UserName) ON DELETE CASCADE ON UPDATE CASCADE
GO

ALTER TABLE bairong_AdministratorsInRoles ADD CONSTRAINT FK_bairong_AInR_R 
    FOREIGN KEY (RoleName)
    REFERENCES bairong_Roles(RoleName) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE bairong_TableMetadata ADD CONSTRAINT FK_bairong_ATM_AT 
    FOREIGN KEY (AuxiliaryTableEnName)
    REFERENCES bairong_TableCollection(TableEnName) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE bairong_TableStyleItem ADD CONSTRAINT FK_bairong_ATSI_ATS 
    FOREIGN KEY (TableStyleId)
    REFERENCES bairong_TableStyle(TableStyleId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE siteserver_AdMaterial ADD CONSTRAINT FK_siteserver_Adv_AdMaterial 
    FOREIGN KEY (AdvId)
    REFERENCES siteserver_Adv(AdvId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE siteserver_Adv ADD CONSTRAINT FK_siteserver_AdArea_Adv 
    FOREIGN KEY (AdAreaId)
    REFERENCES siteserver_AdArea(AdAreaId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE siteserver_InputContent ADD CONSTRAINT FK_siteserver_IC_I 
    FOREIGN KEY (InputId)
    REFERENCES siteserver_Input(InputId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE siteserver_RelatedFieldItem ADD CONSTRAINT FK_siteserver_RFI_RF 
    FOREIGN KEY (RelatedFieldId)
    REFERENCES siteserver_RelatedField(RelatedFieldId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE siteserver_TaskLog ADD CONSTRAINT FK_siteserver_Task_Log 
    FOREIGN KEY (TaskId)
    REFERENCES siteserver_Task(TaskId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovInteractLog ADD CONSTRAINT FK_RIC_RIL 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovInteractPermissions ADD CONSTRAINT FK_GIC_GIA 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovInteractRemark ADD CONSTRAINT FK_GIC_GIRe 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovInteractReply ADD CONSTRAINT FK_GIC_GIR 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovInteractType ADD CONSTRAINT FK_GIC_GIT 
    FOREIGN KEY (NodeId)
    REFERENCES wcm_GovInteractChannel(NodeId) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovPublicApplyLog ADD CONSTRAINT FK_wcm_GPA_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovPublicApplyRemark ADD CONSTRAINT FK_wcm_GPARemark_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovPublicApplyReply ADD CONSTRAINT FK_wcm_GPAReply_GPAL 
    FOREIGN KEY (ApplyId)
    REFERENCES wcm_GovPublicApply(Id) ON DELETE CASCADE ON UPDATE CASCADE
GO


ALTER TABLE wcm_GovPublicCategory ADD CONSTRAINT FK_wcm_GPC_GPCC 
    FOREIGN KEY (ClassCode, PublishmentSystemId)
    REFERENCES wcm_GovPublicCategoryClass(ClassCode, PublishmentSystemId) ON DELETE CASCADE ON UPDATE CASCADE
GO


