CREATE TABLE imagePoll_Vote(
    Id                      int              IDENTITY(1,1),
    SiteId     int              NULL,
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
    CONSTRAINT PK_imagePoll_Vote PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE imagePoll_VoteItem(
    Id                     int              IDENTITY(1,1),
    VoteId                 int              NULL,
    SiteId    int              NULL,
    Title                  nvarchar(255)    NULL,
    ImageUrl               varchar(200)     NULL,
    NavigationUrl          varchar(200)     NULL,
    VoteNum                int              NULL,
    CONSTRAINT PK_imagePoll_VoteItem PRIMARY KEY NONCLUSTERED (Id)
)
go



CREATE TABLE imagePoll_VoteLog(
    Id                     int              IDENTITY(1,1),
    SiteId    int              NULL,
    VoteId                 int              NULL,
    ItemIdCollection       varchar(200)     NULL,
    IpAddress              varchar(50)      NULL,
    CookieSn               varchar(50)      NULL,
    WxOpenId               varchar(200)     NULL,
    UserName               nvarchar(255)    NULL,
    AddDate                datetime         NULL,
    CONSTRAINT PK_imagePoll_VoteLog PRIMARY KEY NONCLUSTERED (Id)
)
go



