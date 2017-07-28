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