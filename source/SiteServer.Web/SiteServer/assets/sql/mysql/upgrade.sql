CREATE TABLE siteserver_PluginConfig(
    Id             INT                      AUTO_INCREMENT,
    PluginId       NATIONAL VARCHAR(50),
    SiteId         INT,
    ConfigName     NATIONAL VARCHAR(200),
    ConfigValue    LONGTEXT,
    PRIMARY KEY (Id)
)ENGINE=INNODB DEFAULT CHARSET=utf8
GO

ALTER TABLE siteserver_CreateTaskLog ADD
(
        ChannelId              INT,
        ContentId              INT,
        TemplateId             INT
)
GO