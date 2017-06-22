CREATE TABLE imagePoll_Vote(
    Id                      INT                      AUTO_INCREMENT,
    SiteId     INT,
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
;

CREATE TABLE imagePoll_VoteItem(
    Id                     INT                      AUTO_INCREMENT,
    VoteId                 INT,
    SiteId    INT,
    Title                  NATIONAL VARCHAR(255),
    ImageUrl               VARCHAR(200),
    NavigationUrl          VARCHAR(200),
    VoteNum                INT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
;

CREATE TABLE imagePoll_VoteLog(
    Id                     INT                      AUTO_INCREMENT,
    SiteId    INT,
    VoteId                 INT,
    ItemIdCollection       VARCHAR(200),
    IpAddress              VARCHAR(50),
    CookieSn               VARCHAR(50),
    WxOpenId               VARCHAR(200),
    UserName               NATIONAL VARCHAR(255),
    AddDate                DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
;
