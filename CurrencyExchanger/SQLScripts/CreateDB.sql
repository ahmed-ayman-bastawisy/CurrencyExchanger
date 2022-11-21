﻿USE [master]
GO

/****** Object:  Database [ExchangeRate]    Script Date: 21/11/2022 08:56:48 PM ******/
CREATE DATABASE [ExchangeRate]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ExchangeRate', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\ExchangeRate.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ExchangeRate_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\ExchangeRate_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO