/*Provision*/
INSERT INTO Users(username, email, password_hash)
SELECT 'admin','admin@mail.com','AQAAAAIAAYagAAAAEChgfEhZp+9Y0Qu3NsP860vRv5DAtumDA6lFmhYO5AGJVva++0q4ySrCTTBomGvOlw=='
WHERE NOT EXISTS
(
    SELECT 1 FROM Users WHERE username = 'admin'
);

INSERT INTO Roles(name)
SELECT 'admin'
WHERE NOT EXISTS
(
    SELECT 1 FROM Roles WHERE name = 'admin'
);

INSERT INTO Roles(name)
SELECT 'taskuser'
WHERE NOT EXISTS
(
    SELECT 1 FROM Roles WHERE name = 'taskuser'
);

INSERT INTO UserRoles(user_id, role_id)
VALUES (1,1);