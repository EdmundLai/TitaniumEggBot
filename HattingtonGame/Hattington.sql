DROP TABLE IF EXISTS "HatCharacters";
DROP TABLE IF EXISTS "Hats";
DROP TABLE IF EXISTS "HatTiers";
DROP TABLE IF EXISTS "Enemies";
DROP TABLE IF EXISTS "EnemyRanks";
DROP TABLE IF EXISTS "FoodItems";
DROP TABLE IF EXISTS "ItemTypes";
DROP TABLE IF EXISTS "CharacterInventories";

CREATE TABLE "HatTiers" (
    "HatTierID" INTEGER PRIMARY KEY,
    "MinLevel" INTEGER NOT NULL,
    "MaxLevel" INTEGER NOT NULL
);

CREATE TABLE "Hats" (
    "HatID" INTEGER PRIMARY KEY,
    "HatName" nvarchar (30) NOT NULL,
    "HatTierID" INTEGER NOT NULL,
    CONSTRAINT "FK_Hats_HatTiers" FOREIGN KEY (
        "HatTierID"
    ) REFERENCES "HatTiers"(
        "HatTierID"
    )
);

CREATE TABLE "HatCharacters" (
    "HatCharacterID" INTEGER PRIMARY KEY,
    "CharacterName" nvarchar (20) NOT NULL,
    "DiscordUser" nvarchar (50) NOT NULL,
    "HatID" INTEGER NOT NULL,
    "Level" INTEGER NOT NULL,
    "Experience" INTEGER NOT NULL,
    "Health" INTEGER NOT NULL,
    "MaxHealth" INTEGER NOT NULL,
    "Attack" INTEGER NOT NULL,
    "Defense" INTEGER NOT NULL,
    "Magic" INTEGER NOT NULL,
    "MagicDefense" INTEGER NOT NULL,
    "Stamina" INTEGER NOT NULL,
    "MaxStamina" INTEGER NOT NULL,
    "Fullness" INTEGER NOT NULL,
    "MaxFullness" INTEGER NOT NULL,
    CONSTRAINT "FK_HatCharacters_Hats" FOREIGN KEY(
        "HatID"
    ) REFERENCES "Hats"(
        "HatID"
    )
);

/* Can add additional columns for varying level scaling (MinLevel, MaxLevel, etc) */
CREATE TABLE "Enemies" (
    "EnemyID" INTEGER PRIMARY KEY,
    "Name" nvarchar (20) NOT NULL,
    "EnemyRankID" INTEGER NOT NULL,
    "Level" INTEGER NOT NULL,
    "MaxHealth" INTEGER NOT NULL,
    "Attack" INTEGER NOT NULL,
    "Defense" INTEGER NOT NULL,
    "Magic" INTEGER NOT NULL,
    "MagicDefense" INTEGER NOT NULL,
    "NormalAttackChances" INTEGER NOT NULL,
    "MagicAttackChances" INTEGER NOT NULL,
    "ExpGain" INTEGER NOT NULL,
    CONSTRAINT "FK_Enemies_EnemyRanks" FOREIGN KEY(
        "EnemyRankID"
    ) REFERENCES "EnemyRanks"(
        "EnemyRankID"
    )
);

CREATE TABLE "EnemyRanks" (
    "EnemyRankID" INTEGER PRIMARY KEY,
    "MinLevel" INTEGER NOT NULL,
    "MaxLevel" INTEGER NOT NULL
);

CREATE TABLE "CharacterInventories" (
    "InventoryID" INTEGER PRIMARY KEY,
    "DiscordUser" nvarchar(50) NOT NULL,
    "FoodItemsString" nvarchar(200) NOT NULL
);

CREATE TABLE "ItemTypes" (
    "ItemTypeID" INTEGER PRIMARY KEY,
    "TypeName" nvarchar(20) NOT NULL
);

CREATE TABLE "FoodItems" (
    "ItemID" INTEGER PRIMARY KEY,
    "Name" nvarchar(20) NOT NULL,
    "ItemTypeID" INTEGER NOT NULL,
    "Energy" INTEGER NOT NULL,
    "AppearanceChance" INTEGER NOT NULL,
    CONSTRAINT "FK_FoodItems_ItemTypes" FOREIGN KEY(
        "ItemTypeID"
    ) REFERENCES "ItemTypes"(
        "ItemTypeID"
    )
);

INSERT INTO "HatTiers" ("MinLevel", "MaxLevel")
VALUES(1, 9),
(10, 29),
(30, 49),
(50, 60);

INSERT INTO "EnemyRanks" ("MinLevel", "MaxLevel")
VALUES(1, 5),
(6, 9),
(10, 20),
(21, 29),
(30, 37),
(38, 47),
(46, 54),
(55, 60);

INSERT INTO "Hats" ("HatName", "HatTierID")
VALUES('Paper Bag', 1),
('Dirty Rag', 1),
('Broken Tennis Racket', 1),
('Garbage Can', 1),
('Traffic Cone', 1),
('Bowler Hat', 2),
('Cowboy Hat', 2),
('Beanie', 2),
('Sombrero', 2),
('Crown', 3),
('Sorting Hat', 3),
('Santa Hat', 4),
('Angel Halo', 4);

INSERT INTO "Enemies" ("Name", "EnemyRankID", "Level", "MaxHealth", "Attack", "Defense", "Magic", "MagicDefense", "NormalAttackChances", "MagicAttackChances", "ExpGain")
VALUES('Blue Slime', 1, 1, 20, 7, 6, 9, 5, 1, 2, 5),
('Little Goblin', 1, 1, 22, 9, 6, 5, 5, 1, 1, 6),
('Green Slime', 1, 2, 30, 13, 10, 9, 7, 1, 1, 13),
('Red Slime', 1,  4, 40, 16, 11, 12, 10, 1, 1, 25),
('Infected Slime', 2, 9, 80, 20, 18, 17, 14, 1, 4, 35),
('Cursed Tome', 3, 16, 100, 27, 22, 30, 19, 1, 7, 50),
('Werewolf', 4, 25, 150, 46, 40, 33, 30, 8, 1, 70),
('Unholy Paladin', 5, 36, 225, 60, 59, 48, 45, 5, 2, 100),
('Elven Arch Mage', 6, 44, 200, 70, 60, 100, 95, 1, 3, 135),
('Golden Wyvern', 7, 51, 370, 110, 90, 115, 95, 2, 1, 180),
('Nightmare Dragon', 8, 60, 500, 200, 150, 175, 175, 1, 1, 400);

INSERT INTO "ItemTypes" ("TypeName")
VALUES('Foraged'),
('Hunted');

INSERT INTO "FoodItems" ("Name", "ItemTypeID", "Energy", "AppearanceChance")
VALUES('Banana', 1, 3, 50),
('Potato', 1, 2, 50),
('Blueberry', 1, 5, 50),
('Apple', 1, 3, 70),
('Raspberry', 1, 5, 50),
('Peach', 1, 6, 40),
('Pineapple', 1, 15, 10),
('Dragonfruit', 1, 20, 10),
('Dreamerfruit', 1, 200, 2),
('Rat', 2, 10, 60),
('Fox', 2, 20, 50),
('Rabbit', 2, 20, 50),
('Deer', 2, 40, 20),
('Bison', 2, 80, 10),
('Rhino', 2, 170, 5),
('Tiger', 2, 250, 3);