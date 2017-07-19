CREATE TABLE siteserver_PluginConfig(
    Id             INT                      AUTO_INCREMENT,
    PluginId       NATIONAL VARCHAR(50),
    SiteId         INT,
    ConfigName     NATIONAL VARCHAR(200),
    ConfigValue    LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB
GO

ALTER TABLE siteserver_CreateTask ADD
    Guid           VARCHAR(50)
GO