CREATE DATABASE IF NOT EXISTS db_todo_list;

USE db_todo_list;

CREATE TABLE IF NOT EXISTS tbl_tag
(
    ID_Tag     SMALLINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Name       VARCHAR(255) NOT NULL,
    Created_At DATETIME     NOT NULL,
    Updated_At DATETIME     NOT NULL
);

CREATE TABLE IF NOT EXISTS tbl_category
(
    ID_Category SMALLINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Name        VARCHAR(255) NOT NULL,
    Created_At  DATETIME     NOT NULL,
    Updated_At  DATETIME     NOT NULL
);

CREATE TABLE IF NOT EXISTS tbl_todo
(
    ID_Todo      INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    Title        VARCHAR(255)      NOT NULL,
    Description  TEXT              NOT NULL,
    Is_Completed BOOLEAN           NOT NULL,
    Created_At   DATETIME          NOT NULL,
    Updated_At   DATETIME          NOT NULL,
    ID_Tag       SMALLINT UNSIGNED NULL, 
    ID_Category  SMALLINT UNSIGNED NULL, 
    CONSTRAINT fk_id_tag FOREIGN KEY (ID_Tag) REFERENCES tbl_tag (ID_Tag)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
    CONSTRAINT fk_id_Category FOREIGN KEY (ID_Category) REFERENCES tbl_category (ID_Category)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
