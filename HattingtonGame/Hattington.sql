DROP TABLE IF EXISTS "HatCharacters";
DROP TABLE IF EXISTS "Hats";
DROP TABLE IF EXISTS "HatTiers";
DROP TABLE IF EXISTS "Enemies";
DROP TABLE IF EXISTS "EnemyRanks";


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

INSERT INTO "Enemies" ("Name", "EnemyRankID", "Level", "MaxHealth", "Attack", "Defense", "Magic", "MagicDefense", "NormalAttackChances", "MagicAttackChances")
VALUES('Blue Slime', 1, 1, 20, 3, 3, 3, 5, 1, 0),
('Little Goblin', 1, 1, 22, 4, 3, 2, 1, 1, 1),
('Green Slime', 1, 2, 30, 5, 5, 3, 5, 1, 0),
('Red Slime', 1,  4, 40, 7, 7, 4, 6, 1, 1),
('Infected Slime', 2, 9, 80, 7, 7, 12, 6, 1, 4),
('Cursed Tome', 3, 16, 100, 13, 10, 16, 8, 1, 7),
('Werewolf', 4, 25, 150, 25, 20, 15, 25, 8, 1),
('Unholy Paladin', 5, 36, 225, 40, 30, 25, 35, 5, 2),
('Elven Arch Mage', 6, 44, 200, 50, 40, 80, 35, 1, 3),
('Golden Wyvern', 7, 51, 370, 80, 70, 65, 55, 2, 1),
('Nightmare Dragon', 8, 60, 500, 150, 100, 125, 75, 1, 1);