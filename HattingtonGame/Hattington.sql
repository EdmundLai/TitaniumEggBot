DROP TABLE IF EXISTS "HatCharacters";
DROP TABLE IF EXISTS "Hats";
DROP TABLE IF EXISTS "HatTiers";

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

INSERT INTO "HatTiers" ("MinLevel", "MaxLevel")
VALUES(1, 9),
(10, 29),
(30, 49),
(50, 60);


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

INSERT INTO "HatCharacters" ("CharacterName", "HatID", "Level", "Experience", "Health", "MaxHealth", "Attack", "Defense", "Magic", "MagicDefense", "Stamina", "MaxStamina", "Fullness", "MaxFullness")
VALUES('EggJunior', 2, 1, 0, 100, 100, 5, 5, 10, 5, 100, 100, 100, 100),
('Delpathos', 7, 10, 0, 200, 200, 15, 15, 20, 15, 100, 100, 100, 100);