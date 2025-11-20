-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: localhost    Database: bazahci
-- ------------------------------------------------------
-- Server version	8.0.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `kategorija`
--

DROP TABLE IF EXISTS `kategorija`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `kategorija` (
  `IdKategorije` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) NOT NULL,
  PRIMARY KEY (`IdKategorije`),
  UNIQUE KEY `Naziv_UNIQUE` (`Naziv`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kategorija`
--

LOCK TABLES `kategorija` WRITE;
/*!40000 ALTER TABLE `kategorija` DISABLE KEYS */;
INSERT INTO `kategorija` VALUES (9,'Mineralna voda'),(4,'Pivo'),(5,'Rakija'),(3,'Sokovi'),(2,'Topli napici'),(7,'Vino'),(6,'Žestoka pića');
/*!40000 ALTER TABLE `kategorija` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `narudžba`
--

DROP TABLE IF EXISTS `narudžba`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `narudžba` (
  `IdNarudžbe` int NOT NULL AUTO_INCREMENT,
  `VrijemeNarudžbe` datetime NOT NULL,
  `Zaposleni_IdZaposleni` int NOT NULL,
  `Sto_IdSto` int NOT NULL,
  `Račun_IdRačuna` int DEFAULT NULL,
  PRIMARY KEY (`IdNarudžbe`),
  KEY `fk_Narudžba_Zaposleni1_idx` (`Zaposleni_IdZaposleni`),
  KEY `fk_Narudžba_Sto1_idx` (`Sto_IdSto`),
  KEY `fk_Narudžba_Račun1_idx` (`Račun_IdRačuna`),
  CONSTRAINT `fk_Narudžba_Račun1` FOREIGN KEY (`Račun_IdRačuna`) REFERENCES `račun` (`IdRačuna`),
  CONSTRAINT `fk_Narudžba_Sto1` FOREIGN KEY (`Sto_IdSto`) REFERENCES `sto` (`IdSto`),
  CONSTRAINT `fk_Narudžba_Zaposleni1` FOREIGN KEY (`Zaposleni_IdZaposleni`) REFERENCES `zaposleni` (`IdZaposleni`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `narudžba`
--

LOCK TABLES `narudžba` WRITE;
/*!40000 ALTER TABLE `narudžba` DISABLE KEYS */;
/*!40000 ALTER TABLE `narudžba` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `narudžba_has_proizvod`
--

DROP TABLE IF EXISTS `narudžba_has_proizvod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `narudžba_has_proizvod` (
  `Narudžba_IdNarudžbe` int NOT NULL,
  `Proizvod_IdProizvoda` int NOT NULL,
  `Kolicina` int NOT NULL,
  PRIMARY KEY (`Narudžba_IdNarudžbe`,`Proizvod_IdProizvoda`),
  KEY `fk_Narudžba_has_Proizvod_Proizvod1_idx` (`Proizvod_IdProizvoda`),
  KEY `fk_Narudžba_has_Proizvod_Narudžba1_idx` (`Narudžba_IdNarudžbe`),
  CONSTRAINT `fk_Narudžba_has_Proizvod_Narudžba1` FOREIGN KEY (`Narudžba_IdNarudžbe`) REFERENCES `narudžba` (`IdNarudžbe`),
  CONSTRAINT `fk_Narudžba_has_Proizvod_Proizvod1` FOREIGN KEY (`Proizvod_IdProizvoda`) REFERENCES `proizvod` (`IdProizvoda`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `narudžba_has_proizvod`
--

LOCK TABLES `narudžba_has_proizvod` WRITE;
/*!40000 ALTER TABLE `narudžba_has_proizvod` DISABLE KEYS */;
/*!40000 ALTER TABLE `narudžba_has_proizvod` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proizvod`
--

DROP TABLE IF EXISTS `proizvod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proizvod` (
  `IdProizvoda` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(45) NOT NULL,
  `Cijena` decimal(10,2) NOT NULL,
  `Količina` int NOT NULL,
  `Kategorija_IdKategorije` int NOT NULL,
  PRIMARY KEY (`IdProizvoda`),
  UNIQUE KEY `Naziv_UNIQUE` (`Naziv`),
  KEY `fk_Proizvod_Kategorija1_idx` (`Kategorija_IdKategorije`),
  CONSTRAINT `fk_Proizvod_Kategorija1` FOREIGN KEY (`Kategorija_IdKategorije`) REFERENCES `kategorija` (`IdKategorije`)
) ENGINE=InnoDB AUTO_INCREMENT=53 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proizvod`
--

LOCK TABLES `proizvod` WRITE;
/*!40000 ALTER TABLE `proizvod` DISABLE KEYS */;
INSERT INTO `proizvod` VALUES (5,'Domaća kafa',2.50,98,2),(6,'Espresso',2.00,100,2),(7,'Espresso sa mlijekom',2.20,98,2),(8,'Cappuccino',3.00,100,2),(9,'Caffe Latte',3.00,100,2),(10,'Nes caffe',2.80,100,2),(11,'Čaj menta',2.00,99,2),(12,'Čaj kamilica',2.00,100,2),(13,'Čaj voćni',2.00,100,2),(14,'Coca cola',3.00,99,3),(15,'Fanta',3.00,99,3),(16,'Sprite',3.00,99,3),(17,'Schweppes Tonic',3.00,99,3),(18,'Schweppes Bitter lemon',3.00,99,3),(19,'Jagoda',3.00,100,3),(20,'Jabuka',3.00,100,3),(21,'Ledeni čaj',3.00,100,3),(22,'Vivia negazirana',2.00,97,9),(23,'Vivia gazirana',2.00,100,9),(24,'Rosa negazirana',2.00,100,9),(25,'Rosa gazirana',2.00,98,9),(26,'Lav premium 0,33',3.50,100,4),(27,'Lav premium 0,5',3.00,100,4),(28,'Jelen 0,33',3.50,98,4),(29,'Jelen 0,5',3.00,100,4),(30,'Nektar 0,33',3.50,100,4),(31,'Nektar 0,5',3.00,100,4),(32,'Tuborg 0,33',3.50,100,4),(33,'Tuborg 0,5',3.00,100,4),(34,'Heineken',4.00,100,4),(35,'Dunja rakija',3.50,99,5),(36,'Šljiva rakija',3.50,100,5),(37,'Kruška rakija',3.50,100,5),(38,'Kajsija rakija',3.50,100,5),(39,'Meduška rakija',3.50,100,5),(40,'Gorki list',3.00,100,6),(41,'Jeger',3.00,100,6),(42,'Stock',3.00,100,6),(43,'Jack Daniels',4.00,100,6),(44,'Johnnie Walker',3.50,100,6),(45,'Vodka',3.00,100,6),(46,'Džin',3.00,98,6),(47,'Tekila',3.50,100,6),(48,'Merlot',3.50,100,7),(49,'Chardonnay',3.50,98,7),(50,'Vranac',3.50,100,7),(51,'Tamjanika',3.50,100,7),(52,'Malvazija',3.50,100,7);
/*!40000 ALTER TABLE `proizvod` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `račun`
--

DROP TABLE IF EXISTS `račun`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `račun` (
  `IdRačuna` int NOT NULL AUTO_INCREMENT,
  `Iznos` decimal(10,2) NOT NULL,
  `VrijemeIzdavanja` datetime NOT NULL,
  `Zaposleni_IdZaposleni` int NOT NULL,
  `Status` enum('Otvoren','Zatvoren') DEFAULT 'Otvoren',
  `Sto_IdStola` int NOT NULL,
  PRIMARY KEY (`IdRačuna`),
  KEY `fk_Račun_Zaposleni_idx` (`Zaposleni_IdZaposleni`),
  KEY `FK_Račun_Sto` (`Sto_IdStola`),
  CONSTRAINT `FK_Račun_Sto` FOREIGN KEY (`Sto_IdStola`) REFERENCES `sto` (`IdSto`),
  CONSTRAINT `fk_Račun_Zaposleni` FOREIGN KEY (`Zaposleni_IdZaposleni`) REFERENCES `zaposleni` (`IdZaposleni`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `račun`
--

LOCK TABLES `račun` WRITE;
/*!40000 ALTER TABLE `račun` DISABLE KEYS */;
INSERT INTO `račun` VALUES (29,17.50,'2025-11-20 09:02:45',12,'Otvoren',1),(30,15.40,'2025-11-20 09:04:03',12,'Zatvoren',3),(31,11.00,'2025-11-20 09:06:02',10,'Zatvoren',3),(32,16.00,'2025-11-20 09:06:43',10,'Otvoren',4);
/*!40000 ALTER TABLE `račun` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stavkaračuna`
--

DROP TABLE IF EXISTS `stavkaračuna`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stavkaračuna` (
  `Količina` int NOT NULL,
  `Cijena` decimal(10,2) NOT NULL,
  `Ukupno` decimal(10,2) NOT NULL,
  `Račun_IdRačuna` int NOT NULL,
  `Proizvod_IdProizvoda` int NOT NULL,
  PRIMARY KEY (`Račun_IdRačuna`,`Proizvod_IdProizvoda`),
  KEY `fk_StavkaRačuna_Proizvod1_idx` (`Proizvod_IdProizvoda`),
  CONSTRAINT `fk_StavkaRačuna_Proizvod1` FOREIGN KEY (`Proizvod_IdProizvoda`) REFERENCES `proizvod` (`IdProizvoda`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_StavkaRačuna_Račun1` FOREIGN KEY (`Račun_IdRačuna`) REFERENCES `račun` (`IdRačuna`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stavkaračuna`
--

LOCK TABLES `stavkaračuna` WRITE;
/*!40000 ALTER TABLE `stavkaračuna` DISABLE KEYS */;
INSERT INTO `stavkaračuna` VALUES (2,2.50,5.00,29,5),(1,2.00,2.00,29,22),(2,3.50,7.00,29,28),(1,3.50,3.50,29,35),(2,2.20,4.40,30,7),(1,2.00,2.00,30,11),(1,3.00,3.00,30,14),(1,3.00,3.00,30,15),(1,3.00,3.00,30,16),(2,2.00,4.00,31,25),(2,3.50,7.00,31,49),(1,3.00,3.00,32,17),(1,3.00,3.00,32,18),(2,2.00,4.00,32,22),(2,3.00,6.00,32,46);
/*!40000 ALTER TABLE `stavkaračuna` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sto`
--

DROP TABLE IF EXISTS `sto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sto` (
  `IdSto` int NOT NULL AUTO_INCREMENT,
  `Kapacitet` int NOT NULL,
  `Status` varchar(20) NOT NULL,
  PRIMARY KEY (`IdSto`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sto`
--

LOCK TABLES `sto` WRITE;
/*!40000 ALTER TABLE `sto` DISABLE KEYS */;
INSERT INTO `sto` VALUES (1,5,'Zauzet'),(2,4,'Slobodan'),(3,6,'Slobodan'),(4,2,'Zauzet'),(6,4,'Slobodan');
/*!40000 ALTER TABLE `sto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `zaposleni`
--

DROP TABLE IF EXISTS `zaposleni`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zaposleni` (
  `IdZaposleni` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(45) NOT NULL,
  `Prezime` varchar(45) NOT NULL,
  `KorisničkoIme` varchar(45) NOT NULL,
  `Lozinka` varchar(256) NOT NULL,
  `Uloga` varchar(45) NOT NULL,
  `BrojTelefona` varchar(20) NOT NULL,
  `Plata` decimal(6,2) NOT NULL,
  `Tema` varchar(20) DEFAULT 'OrangeTheme',
  `Jezik` varchar(20) DEFAULT 'Srpski',
  PRIMARY KEY (`IdZaposleni`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zaposleni`
--

LOCK TABLES `zaposleni` WRITE;
/*!40000 ALTER TABLE `zaposleni` DISABLE KEYS */;
INSERT INTO `zaposleni` VALUES (7,'Marko','Markovic','marko123','marko123','Admin','065065065',2000.00,'OrangeTheme','Srpski'),(10,'David','Davidovic','david123','david123','Radnik','066066066',1500.00,'OrangeTheme','Engleski'),(11,'Ivan','Ivanovic','ivan123','ivan123','Admin','065066067',2000.00,'LightTheme','Srpski'),(12,'Milos','Milosevic','milos123','milos123','Radnik','065066068',1800.00,'OrangeTheme','Engleski');
/*!40000 ALTER TABLE `zaposleni` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-20  9:53:12
