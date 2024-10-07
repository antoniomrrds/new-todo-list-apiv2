CREATE DATABASE IF NOT EXISTS db_todo_list;

USE db_todo_list;

CREATE TABLE IF NOT EXISTS tbl_tag
(
    ID     SMALLINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Name       VARCHAR(255) NOT NULL,
    Slug      VARCHAR(255) NOT NULL,
    Description   VARCHAR(500) DEFAULT NULL,
    Status TINYINT(1) DEFAULT 1, -- Status da tag (ativa/inativa),
    Color         VARCHAR(7) DEFAULT '#FFFFFF',
    Created_At DATETIME     NOT NULL,
    Updated_At DATETIME     NOT NULL
);

CREATE TABLE IF NOT EXISTS tbl_category
(
    ID SMALLINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Name        VARCHAR(255) NOT NULL,
    Created_At  DATETIME     NOT NULL,
    Updated_At  DATETIME     NOT NULL
);

CREATE TABLE IF NOT EXISTS tbl_todo
(
    ID      INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Title        VARCHAR(255)      NOT NULL,
    Description  TEXT              NOT NULL,
    Is_Completed BOOLEAN           NOT NULL,
    Created_At   DATETIME          NOT NULL,
    Status TINYINT(1) DEFAULT 1, -- Status da tag (ativa/inativa),
    Updated_At   DATETIME          NOT NULL,
    ID_Tag       SMALLINT UNSIGNED NULL, 
    ID_Category  SMALLINT UNSIGNED NULL, 
    CONSTRAINT fk_id_tag FOREIGN KEY (ID_Tag) REFERENCES tbl_tag (ID)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
    CONSTRAINT fk_id_Category FOREIGN KEY (ID_Category) REFERENCES tbl_category (ID)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
