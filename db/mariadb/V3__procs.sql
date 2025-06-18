DELIMITER $$

DROP PROCEDURE IF EXISTS spCreateUser $$

CREATE PROCEDURE spCreateUser (
    IN username VARCHAR(255),
    IN email VARCHAR(255),
    IN password_hash VARCHAR(4000)
)
BEGIN
    INSERT INTO Users (username, email, password_hash)
    VALUES (username, email, password_hash);
    SELECT LAST_INSERT_ID();
END$$


DROP PROCEDURE IF EXISTS spAddUserToRole $$

CREATE PROCEDURE spAddUserToRole (
    IN user_id INT,
    IN role_id INT
)
BEGIN
    REPLACE INTO UserRoles (user_id, role_id)
    VALUES (user_id, role_id);
END$$


DROP PROCEDURE IF EXISTS spAddCategory $$


CREATE PROCEDURE spAddCategory (
    IN name VARCHAR(255)
)
BEGIN
    INSERT INTO Categories (name)
    VALUES (name);
END$$


DROP PROCEDURE IF EXISTS spAddTaskItem $$


CREATE PROCEDURE spAddTaskItem (
    IN user_id INT,
    IN category_id INT,
    IN title VARCHAR(255),
    IN description VARCHAR(255),
    IN due_date DATE,
    IN priority INT
)
BEGIN
    INSERT INTO TaskItems
    (
        user_id,
        category_id,
        title,
        description,
        due_date,
        priority
    )
    VALUES
    (
        user_id,
        category_id,
        title,
        description,
        due_date,
        priority
    );
    SELECT LAST_INSERT_ID();
END$$


DROP PROCEDURE IF EXISTS spDeleteTaskItem $$


CREATE PROCEDURE spDeleteTaskItem (
    IN p_id INT
)
BEGIN
    DELETE FROM TaskItems WHERE id = p_id;
END$$


DROP PROCEDURE IF EXISTS spUpdateTaskItem $$


CREATE PROCEDURE spUpdateTaskItem (
	IN p_id INT,
	IN p_category_id INT,
	IN p_title VARCHAR(255),
	IN p_description VARCHAR(255),
	IN p_due_date DATE,
	IN p_priority INT
)
BEGIN
    UPDATE TaskItems
    SET
        category_id = p_category_id,
        title = p_title,
        description = p_description,
        due_date = p_due_date,
        priority = p_priority
    WHERE
        id = p_id;
END$$


DROP PROCEDURE IF EXISTS spGetTaskItemById $$


CREATE PROCEDURE spGetTaskItemById (
	IN p_id INT
)
BEGIN
	SELECT
		id,
		user_id AS UserId,
		category_id AS CategoryId,
		title,
		description,
		due_date AS DueDate,
		priority,
		completed,
		created_at AS CreatedAt,
		completed_at AS CompletedAt
	FROM
		TaskItems
	WHERE
		id = p_id;
END$$


DROP PROCEDURE IF EXISTS spGetCategoryById $$


CREATE PROCEDURE spGetCategoryById (
    IN p_id INT
)
BEGIN
    SELECT
        *
    FROM
        Categories
    WHERE
        id = p_id;
END$$


DROP PROCEDURE IF EXISTS spGetCategories $$


CREATE PROCEDURE spGetCategories ()
BEGIN
    SELECT
        *
    FROM
        Categories;
END$$


DROP PROCEDURE IF EXISTS spGetUserByUsername $$


CREATE PROCEDURE spGetUserByUsername (
    IN in_username VARCHAR(255)
)
BEGIN
    SELECT
        id,
        username,
        email,
        password_hash AS passwordhash,
        status
    FROM
        Users
    WHERE
        username = in_username;
END$$


DROP PROCEDURE IF EXISTS spCreateCategory $$


CREATE PROCEDURE spCreateCategory (
    IN p_name VARCHAR(255)
)
BEGIN
	INSERT INTO Categories(name)
	VALUES (p_name);
    SELECT LAST_INSERT_ID();
END$$


DROP PROCEDURE IF EXISTS spGetCategoryByName $$


CREATE PROCEDURE spGetCategoryByName (
    IN p_name VARCHAR(255)
)
BEGIN
	SELECT * FROM Categories WHERE NAME = p_name LIMIT 1;
END$$


DROP PROCEDURE IF EXISTS spGetRolesByUserId $$


CREATE PROCEDURE spGetRolesByUserId (
    IN p_user_id INT
)
BEGIN
	SELECT
		r.*
	FROM
		UserRoles ur
		JOIN Roles r ON ur.role_id = r.id
	WHERE
		ur.user_id = p_user_id;
END$$


DROP PROCEDURE IF EXISTS spGetCategories $$


CREATE PROCEDURE spGetCategories (
)
BEGIN
	SELECT
		*
	FROM
		Categories;
END$$


DROP PROCEDURE IF EXISTS spUpdateCategory $$


CREATE PROCEDURE spUpdateCategory (
	IN p_id INT,
	IN p_name VARCHAR(255)
)
BEGIN
	UPDATE Categories SET name = p_name WHERE id = p_id;
END$$


DROP PROCEDURE IF EXISTS spDeleteCategory $$


CREATE PROCEDURE spDeleteCategory (
    IN p_id INT
)
BEGIN
    DELETE
    FROM
        Categories
    WHERE
        id = p_id;
END$$


DROP PROCEDURE IF EXISTS spCreateTaskItem $$


CREATE PROCEDURE spCreateTaskItem (
	IN p_user_id INT, 
	IN p_category_id INT,
	IN p_title VARCHAR(255),
	IN p_description VARCHAR(255),
	IN p_due_date DATE,
	IN p_priority INT
)
BEGIN
	INSERT INTO TaskItems(user_id, category_id, title, description, due_date, priority)
	VALUES (p_user_id, p_category_id, p_title, p_description, p_due_date, p_priority);
	SELECT LAST_INSERT_ID();
END$$


DROP PROCEDURE IF EXISTS spGetTaskItemsByUserId $$


CREATE PROCEDURE spGetTaskItemsByUserId (
	IN p_user_id INT,
	IN p_offset INT,
	IN p_fetch_rows INT
)
BEGIN
	SELECT
		id,
		user_id AS UserId,
		category_id AS CategoryId,
		title,
		description,
		due_date AS DueDate,
		priority,
		completed,
		created_at AS CreatedAt,
		completed_at AS CompletedAt
	FROM
		TaskItems
	WHERE
		user_id = p_user_id
	ORDER BY
		id DESC
	OFFSET p_offset ROWS FETCH NEXT p_fetch_rows ROWS ONLY;
END$$


DROP PROCEDURE IF EXISTS spTaskItemCountByUserId $$


CREATE PROCEDURE spTaskItemCountByUserId (
	IN p_user_id INT
)
BEGIN
	SELECT
		COUNT(*)
	FROM
		TaskItems
	WHERE
		user_id = p_user_id;
END$$


DROP PROCEDURE IF EXISTS spCompleteTaskItem $$


CREATE PROCEDURE spCompleteTaskItem (
	IN p_id INT,
	IN p_completed_at DATETIME
)
BEGIN
	UPDATE
		TaskItems
	SET
		completed = 1,
		completed_at = p_completed_at
	WHERE
		id = p_id
		AND completed = 0;
END$$


DROP PROCEDURE IF EXISTS spGetUserById $$

CREATE PROCEDURE spGetUserById (
    IN p_id INT
)
BEGIN
    SELECT
        id,
        username,
        email,
        password_hash AS passwordhash,
        status
    FROM
        Users
    WHERE
        id = p_id;
END$$


DROP PROCEDURE IF EXISTS spUpdateUserPassword $$


CREATE PROCEDURE spUpdateUserPassword (
    IN p_id INT,
    IN p_password VARCHAR(255)
)
BEGIN    
	UPDATE
		Users
	SET
		password_hash = p_password
	WHERE
        id = p_id;
END$$


DROP PROCEDURE IF EXISTS spDeleteUser $$


CREATE PROCEDURE spDeleteUser (
	IN p_id INT
)
BEGIN
	DELETE FROM Users WHERE id = p_id;
END$$


DROP PROCEDURE IF EXISTS spDeleteUserRoles $$


CREATE PROCEDURE spDeleteUserRoles (
    IN p_id INT
)
BEGIN
    DELETE FROM UserRoles WHERE user_id = p_id;
END$$


DROP PROCEDURE IF EXISTS spGetRoleByName $$


CREATE PROCEDURE spGetRoleByName (
    IN p_name VARCHAR(255)
)
BEGIN
    SELECT * FROM Roles WHERE name = p_name LIMIT 1;
END$$


DROP PROCEDURE IF EXISTS spAddUserRole $$


CREATE PROCEDURE spAddUserRole (
    IN p_user_id INT,
    IN p_role_id INT
)
BEGIN    
	INSERT INTO UserRoles
		(user_id, role_id)
	VALUES
		(p_user_id, p_role_id);
END$$


DROP PROCEDURE IF EXISTS spUpdateUser $$


CREATE PROCEDURE spUpdateUser (
    IN p_id INT,
    IN p_email VARCHAR(255)
)
BEGIN    
	UPDATE Users SET email = p_email WHERE id = p_id;
END$$


DROP PROCEDURE IF EXISTS spUsersCount $$


CREATE PROCEDURE spUsersCount ()
BEGIN
	SELECT
		COUNT(*)
	FROM
		Users;
END$$


DROP PROCEDURE IF EXISTS spGetUsers $$


CREATE PROCEDURE spGetUsers (
	IN p_offset INT,
	IN p_fetch_rows INT
)
BEGIN
	SELECT
		id,
		email,
		username,
		status
	FROM
		Users
	ORDER BY
		id DESC
	OFFSET p_offset ROWS FETCH NEXT p_fetch_rows ROWS ONLY;
END$$

DELIMITER ;